using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class LaundryController : Controller
    {
        // GET: User/Laundry
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LaundryDetails()
        {

            return View();
        }
    }
}