using DunkeyDelivery.Areas.Dashboard.Models;
using DunkeyDelivery.BindingModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
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
    public class UserController : Controller
    {
        // GET: Dashboard/User
        public async Task<ActionResult> Index()
        {
            var model = new UserViewModel1();
            model.SetSharedData(User);

            return View(model);
        }
        public ActionResult AddUser()
        {
            UserViewModel1 userModel = new UserViewModel1();
            var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall<ShopViewModel>.CallApi("api/Shop/GetAllStores",null,false));

            //Providing Stores

            if (responseStores == null || responseStores is Error)
            {

            }
            else
            {
                var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();
                IEnumerable<SelectListItem> selectList = from store in Stores
                                                         select new SelectListItem
                                                         {
                                                             Selected = false,
                                                             Text = store.BusinessName,
                                                             Value = store.Id.ToString()
                                                         };
                userModel.StoreOptions = new SelectList(selectList);
            }

            //Providing Roles
            userModel.RoleOptions = new SelectList(
                new List<SelectListItem> {
                    new SelectListItem { Text = Utility.RoleTypes.SubAdmin.ToString(), Value = Utility.RoleTypes.SubAdmin.ToString("D") },
                    new SelectListItem { Text = Utility.RoleTypes.SuperAdmin.ToString(), Value = Utility.RoleTypes.SuperAdmin.ToString("D") }}
                );

            userModel.SetSharedData(User);
            return PartialView("_AddUser", userModel);
        }


        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Create()
        {

            return PartialView("_AddUser", new UserViewModel1());
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
                           
        public async Task<ActionResult> Create(UserViewModel1 model, string returnUrl)
        {
            model.FullName = model.FullName ?? model.FirstName + " " + model.LastName;
            model.Role = 2;
           

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var response = await ApiCall<UserViewModel1>.CallApi("api/User/Register", model);

            
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }


            else
            {
                //return RedirectToAction("Index");
                return RedirectToAction("AddUser");
            }
            




        }




    }
}