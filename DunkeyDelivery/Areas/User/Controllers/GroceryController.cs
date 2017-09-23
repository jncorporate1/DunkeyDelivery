using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class GroceryController : Controller
    {
        // GET: User/Grocery
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GroceryDetails()
        {

            return View();
        }

        
        
    }
}