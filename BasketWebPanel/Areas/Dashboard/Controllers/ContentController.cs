using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        // GET: Dashboard/Content


        public ActionResult FAQ(int? Id)
        {
            FAQBindingModel model = new FAQBindingModel();
            if (Id.HasValue)
            {
                var responseAdmin = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetFAQsById", User, null, true, false, null,"Id=" + Id.Value));
                if (responseAdmin == null || responseAdmin is Error)
                    ;
                else
                {
                    model = responseAdmin.GetValue("Result").ToObject<FAQBindingModel>();
                }
            }

            model.SetSharedData(User);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FAQ(FAQBindingModel model)
        {
            model.SetSharedData(User);

            if (model.Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
            }

            var response = await ApiCall.CallApi("api/Content/FAQ", User, model);
            if (response.ToString().Contains("UnAuthorized"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
            }

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model = response.GetValue("Result").ToObject<FAQBindingModel>();
                //var claimIdentity = ((ClaimsIdentity)User.Identity);

                //if (model.Role == "SuperAdmin")
                //{
               
                   TempData["SuccessMessage"] = "Data saved successfully.";
             
                      
                //}

                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
        }
        

        public ActionResult SearchFAQ()
        {
            FAQBindingModel returnModel = new FAQBindingModel();
            return PartialView("_SearchFAQ", returnModel);
        }
        public ActionResult ManageFAQ()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchFAQsResults(SearchFAQModel model)
        {
            SearchFAQViewModel returnModel = new SearchFAQViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetFAQs", User, null, true, false, null, "Type=" +model.Type));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchFAQViewModel>();
            }
         
            returnModel.SetSharedData(User);
            return PartialView("_SearchFAQsResults", returnModel);
        }
        
        public JsonResult DeleteFAQ(int Id)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Content/DeleteFAQ", User, null, true, false, null, "Id=" + Id));
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

        public ActionResult AboutUs(ContentManagementViewModel returnModel)
        {
            ContentManagementViewModel resp = new ContentManagementViewModel();
            resp.SetSharedData(User);
            returnModel.Type = (int)BasketWebPanel.Content.Types.AboutUs;
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Content/GetContentEntities", User, returnModel));
            if (response == null || response is Error)
            {

            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<ContentManagementViewModel>();
                returnModel.SetSharedData(User);
                return View("AboutUs", returnModel);

            }
            return View("AboutUs", resp);

        }

        public ActionResult Investors(int? Id)
        {
            AddContentViewModel model = new AddContentViewModel();
            model.SetSharedData(User);
            return View(model);
        }

        public ActionResult ManageContent()
        {
            AddContentViewModel model = new AddContentViewModel();
            model.SetSharedData(User);
            return View(model);
        }


        [HttpPost]
        public JsonResult DeleteImageOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            if (Request.Files.Count > 0)
            {
                var ImageFileList = (List<HttpFileCollectionBase>)Request.RequestContext.HttpContext.Session["ImageFiles"];

                if (ImageFileList == null)
                {
                    // Adding first file
                    List<HttpFileCollectionBase> files = new List<HttpFileCollectionBase>();
                    files.Add(Request.Files);

                    Request.RequestContext.HttpContext.Session.Add("ImageFiles", files);
                }
                else
                {
                    // Adding files to existing list
                    ImageFileList.Add(Request.Files);
                    Request.RequestContext.HttpContext.Session.Remove("ImageFiles");
                    Request.RequestContext.HttpContext.Session.Add("ImageFiles", ImageFileList);
                }

                //Request.RequestContext.HttpContext.Session.Remove("AddContentImage");
                //Request.RequestContext.HttpContext.Session.Add("AddContentImage", Request.Files);

                //Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                //Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("ImageFiles");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public ActionResult SearchContent(SearchContentVM returnModel)
        {
            //SearchBlogModel returnModel = new SearchBlogModel();
            //if (!string.IsNullOrEmpty(returnModel.Name))
            //{
            //    var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Content/GetAllContents", User, GetRequest: true));

            //    if (responseStores == null || responseStores is Error)
            //    {

            //    }
            //    else
            //    {
            //        var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();



            //    }

            //}

            return PartialView("_SearchContent", returnModel);
        }

        public ActionResult SearchContentResults(SearchContentVM model)
        {

            ListContentViewModel returnModel = new ListContentViewModel();


            switch (model.SearchType)
            {
                case 0:
                    model.is_invester = 1;
                    break;
                case 1:
                    model.is_partner = 1;
                    break;
                case 2:
                    model.is_press = 1;
                    break;

                default:
                    model.is_invester = 0;
                    model.is_partner = 0;
                    model.is_press = 0;
                    break;
            }



            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Content/SearchContent", User, null, true, false, null, "Name=" + model.Name + "", "is_investor=" + model.is_invester + "", "is_partner=" + model.is_partner + "", "is_press=" + model.is_press + "", "SearchType=" + model.SearchType + ""));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<ListContentViewModel>();
            }


            returnModel.SetSharedData(User);
            return PartialView("_SearchContentResults", returnModel);
        }

        public JsonResult DeleteContent(int contentId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Content/DeleteEntity", User, null, true, false, null, "Id=" + contentId));
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