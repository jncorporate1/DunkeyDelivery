using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Dashboard/Settings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GeneralSettings()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }
        public ActionResult AboutSettings()
        {
            

            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }
    }
}