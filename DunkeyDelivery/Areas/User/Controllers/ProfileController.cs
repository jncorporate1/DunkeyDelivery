using DunkeyDelivery.Areas.User.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class ProfileController : Controller
    {
        // GET: User/Profile
        public ActionResult Index()
        {
            ViewBag.BannerImage = "adrees-banner.jpg";
            ViewBag.Title = " Profile";
            ViewBag.BannerTitle = "PROFILE";
            ViewBag.Path = "Home > Profile";
            Global.sharedDataModel.SetSharedData(User);
            return View("Profile", Global.sharedDataModel);
        }

        public ActionResult PageView(int id)
        {
            if (id == 0)
            {
                // for Order History 

                return PartialView("_OrderHistory");
                
            }
            else if (id == 1)
            {
                // for recurring orders

                return PartialView("_RecurringOrders");

            }
            else if (id == 2)
            {
                // for account ( Profile View )
                ProfileViewModel model = new ProfileViewModel();
                model.SetSharedData(User);
                //Global.sharedDataModel.SetSharedData(User);
                return PartialView("_Account",model);

            }
            else if (id == 3)
            {

                // for addresses
                //var claimIdentity = ((ClaimsIdentity)User.Identity);
                //var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
                //var response = await ApiCall<>.CallApi("api/User/GetAddresses?User_id=" + userId, null, false);

                return PartialView("_Addresses");

            }
            else if (id == 4)
            {
                // for credit cards
                return PartialView("_CreditCards");

            }
            Global.sharedDataModel.SetSharedData(User);
            return PartialView("_Account",Global.sharedDataModel);
        }

      
    }
}