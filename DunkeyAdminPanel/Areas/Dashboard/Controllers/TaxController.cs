using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class TaxController : Controller
    {
        // GET: Dashboard/Tax
        public ActionResult Index()
        {
            TaxListViewModel model = new TaxListViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetTaxes", User, null, true, false, null));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model = response.GetValue("Result").ToObject<TaxListViewModel>();
            }


            model.SetSharedData(User);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(TaxListViewModel model)
        {


            model.SetSharedData(User);

            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/UpdateTaxes", User, model));

            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
                return Json("Success");
        }


    }
}