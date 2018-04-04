using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class SizeController : Controller
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AddUnitViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/AddUnit", User, model.Size));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, (apiResponse as Error).ErrorMessage);
                else
                {
                    
                    TempData["SuccessMessage"] = "The weight unit added successfully.";
                    model.SetSharedData(User);
                    return Json("Success");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex));
            }
        }
        public JsonResult DeleteUnit(int UnitId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Unit, "Id=" + UnitId));
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
        public ActionResult Index(int? SizeId)
        {

            AddUnitViewModel model = new AddUnitViewModel();
            model.SetSharedData(User);
            model.UnitOptions = Utility.GetUnits(User);

            return View("Index", model);
        }
        
        public ActionResult SizeListResults()
        {
            SizeListViewModel returnModel = new SizeListViewModel();
            var responseUnits = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Size/GetAllUnits", User, GetRequest: true));
            if (responseUnits == null || responseUnits is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (responseUnits as Error).ErrorMessage);
            }
            else
            {
                var returnResponse = responseUnits.GetValue("Result").ToObject<List<SizeBindingModel>>();
                returnModel.SizeList = returnResponse;
                return PartialView("_SizeList", returnModel);
            }
        }
    }
}