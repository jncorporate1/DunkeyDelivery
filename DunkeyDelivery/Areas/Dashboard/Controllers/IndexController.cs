using DunkeyDelivery.Areas.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.Dashboard.Controllers
{
    public class IndexController : Controller
    {
        //// GET: Dashboard/Home
        //[Authorize(Roles = "SuperAdmin")]
        //public ActionResult Index()
        //{
        //    return View();
        //}
        // GET: Dashboard/Home
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            //var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            //var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");

            WebDashboardStatsViewModel model = new WebDashboardStatsViewModel();
            var response = await ApiCall<WebDashboardStatsViewModel>.CallApi("api/Admin/GetDashboardStats", null,false);

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model = response.GetValue("Result").ToObject<WebDashboardStatsViewModel>();
            }


            model.SetSharedData(User);

            return View(model);
        }
    }
}