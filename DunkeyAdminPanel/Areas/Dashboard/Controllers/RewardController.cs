using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class RewardController : Controller
    {
        public ActionResult Index(int? RewardId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddAdminImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            AddRewardViewModel model = new AddRewardViewModel();

            model.SetSharedData(User);

            if (RewardId.HasValue)
            {
                var responseAdmin = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetEntityById", User, null, true, false, null,parameters: "EntityType=" + (int)BasketEntityTypes.Reward+"&Id="+RewardId));
                if (responseAdmin == null || responseAdmin is Error)
                    ;
                else
                {
                    model.Rewards = responseAdmin.GetValue("Result").ToObject<RewardViewModel>();
                }
            }
            return View(model);
        }


        [HttpPost]
        public JsonResult DeleteImageOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddGiftImage");
                Request.RequestContext.HttpContext.Session.Add("AddGiftImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddGiftImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddRewardViewModel model)
        {
            model.SetSharedData(User);

            if (model.Rewards.Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
            }


            MultipartFormDataContent content;

            bool FileAttached = (Request.RequestContext.HttpContext.Session["AddGiftImage"] != null);
            bool ImageDeletedOnEdit = false;
            var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
            if (imgDeleteSessionValue != null)
            {
                ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
            }
            byte[] fileData = null;
            var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddGiftImage"];
            if (FileAttached)
            {
                using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                {

                    fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                }
            }

            ByteArrayContent fileContent;
            JObject response;

            bool firstCall = true;
            callAgain: content = new MultipartFormDataContent();
            if (FileAttached)
            {
                fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                content.Add(fileContent);
            }

            if (model.Rewards.Id > 0)
                content.Add(new StringContent(model.Rewards.Id.ToString()), "Id");
            //AddGiftImage

            if (model.Rewards.RewardPrize_Id > 0)
                content.Add(new StringContent(model.Rewards.RewardPrize_Id.ToString()), "RewardPrize_Id");
            
            if (model.Rewards.PointsRequired > 0)
                content.Add(new StringContent(Convert.ToString(model.Rewards.PointsRequired)), "RequiredPoints");

            if (model.Rewards.AmountAward > 0)
                content.Add(new StringContent(Convert.ToString(model.Rewards.AmountAward)), "AmountReward");

            if (!string.IsNullOrEmpty(model.Rewards.RewardPrizes.Name))
                content.Add(new StringContent(model.Rewards.RewardPrizes.Name), "Name");
            if (!string.IsNullOrEmpty(model.Rewards.Description))
                content.Add(new StringContent(model.Rewards.Description), "Description");


            content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
            response = await ApiCall.CallApi("api/Admin/AddReward", User, isMultipart: true, multipartContent: content);

            if (firstCall && response.ToString().Contains("UnAuthorized"))
            {
                firstCall = false;
                goto callAgain;
            }
            else if (response.ToString().Contains("UnAuthorized"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
            }

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                //model.Admin = response.GetValue("Result").ToObject<AdminViewModel>();
                model.SetSharedData(User);

                if (model.Role == RoleTypes.SuperAdmin)
                {
                    if (model.Rewards.Id > 0)
                        TempData["SuccessMessage"] = "The reward has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The reward has been added successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Your reward has been updated successfully.";
                }

                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
        }

        public ActionResult ManageReward()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        //public ActionResult SearchAdmin()
        //{
        //    SearchAdminModel returnModel = new SearchAdminModel();

        //    var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
        //    if (responseStores == null || responseStores is Error)
        //    {
        //    }
        //    else
        //    {
        //        var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();
        //        IEnumerable<SelectListItem> selectList = from store in Stores
        //                                                 select new SelectListItem
        //                                                 {
        //                                                     Selected = false,
        //                                                     Text = store.BusinessName,
        //                                                     Value = store.Id.ToString()
        //                                                 };
        //        Stores.Insert(0, new StoreBindingModel { Id = 0, BusinessName = "All" });

        //        returnModel.StoreOptions = new SelectList(selectList);
        //    }
        //    return PartialView("_SearchAdmin", returnModel);
        //}

        public ActionResult SearchRewardResults()
        {
            RewardListViewModel returnModel = new RewardListViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchRewards", User, null, true, false, null));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<RewardListViewModel>();
            }
            
            returnModel.SetSharedData(User);
            return PartialView("_SearchRewardResults", returnModel);
        }

        public JsonResult DeleteReward(int RewardId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Reward, "Id=" + RewardId));
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                    return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}