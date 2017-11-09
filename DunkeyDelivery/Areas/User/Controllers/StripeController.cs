using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    public class StripeController : Controller
    {
        // GET: User/Stripe
        public ActionResult Index()
        {
            var stripePublishKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;

            return View();
        }

        public ActionResult Charge(string stripeEmail, string stripeToken)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = 500,//charge in cents
                Description = "Sample Charge",
                Currency = "usd",
                CustomerId = customer.Id
            });

            // further application specific code goes here

            return View();
        }
    }


}