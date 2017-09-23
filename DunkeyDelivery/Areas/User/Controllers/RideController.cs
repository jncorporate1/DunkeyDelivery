using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class RideController : Controller
    {
        // GET: User/Ride
        public ActionResult Index()
        {
            RideRegisterViewModel model = new RideRegisterViewModel();
            ViewBag.BannerImage = "ridebanner.jpg";
            ViewBag.Title = "Ride";
            ViewBag.BannerTitle = "Ride";
            ViewBag.Path = "Home > Ride";
            model.SetSharedData(User);
            return View("RideIndex", model);
        }
        public async Task<ActionResult> Register(RideRegisterViewModel model)
        {
        
            model.Status = 0;


            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            var response = await ApiCall<RideRegisterViewModel>.CallApi("api/Ride/Register", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }

            return View("RideIndex");
        }
       

    }
}