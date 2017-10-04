using DunkeyDelivery;
using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class FooterController : Controller
    {
        // GET: Footer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Investors()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Investors";
            ViewBag.BannerTitle = "Our Valuable Investors";
            ViewBag.Path = "Home > Investors";
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }
        public async Task<ActionResult> Blog()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Blog";
            ViewBag.BannerTitle = "Blog";
            ViewBag.Path = "Home > Blog";
            BlogViewModel returnResponse = new BlogViewModel();
            var response = await ApiCall<List<PostsViewModel>>.CallApi("api/Blog/GetBlogPosts", null, false);
            var responseResult = response.GetValue("Result").ToObject<List<PostsViewModel>>();
            returnResponse.postsViewModel = responseResult;
            returnResponse.SetSharedData(User);

            return View("Blog", returnResponse);
        }
        public ActionResult GetRewards()
        {
            ViewBag.BannerImage = "rewards-banner.jpg";
            ViewBag.Title = "Get Rewards";
            ViewBag.BannerTitle = "Get Rewards";
            ViewBag.Path = "Home > Get Rewards";
            Global.sharedDataModel.SetSharedData(User);
            return View("GetRewards",Global.sharedDataModel);
           
        }
        public ActionResult Career()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Careers";
            ViewBag.BannerTitle = "Careers";
            ViewBag.Path = "Home > Careers";
            Global.sharedDataModel.SetSharedData(User);
            return View("Career",Global.sharedDataModel);
        }

        public ActionResult CareerApply()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Career Apply";
            ViewBag.BannerTitle = "Career Apply";
            ViewBag.Path = "Home > Career";
            return View("CareerApply");
        }



        public ActionResult Press()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Press";
            ViewBag.BannerTitle = "Press";
            ViewBag.Path = "Home > Press";
            Global.sharedDataModel.SetSharedData(User);
            return View("Press",Global.sharedDataModel);
        }
       
        public ActionResult Gift()
        {
            GiftViewModel model = new GiftViewModel();
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Gift Cards";
            ViewBag.BannerTitle = "Gifts Cards";
            ViewBag.Path = "Home > Gift Cards";
            model.SetSharedData(User);
            return View("Gift",model);
        }
        public ActionResult Partners()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Partners";
            ViewBag.BannerTitle = "Partners";
            ViewBag.Path = "Home > Partners";
            Global.sharedDataModel.SetSharedData(User);
            return View("Partners",Global.sharedDataModel);
        }
        public ActionResult FAQs()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "FAQs";
            ViewBag.BannerTitle = "Frequently Asked Question";
            ViewBag.Path = "Home > FAQs";
            Global.sharedDataModel.SetSharedData(User);
            return View("FAQ",Global.sharedDataModel);
        }
        public ActionResult RewardPoints()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "FAQs";
            ViewBag.BannerTitle = "Frequently Asked Question";
            ViewBag.Path = "Home > FAQs";
            Global.sharedDataModel.SetSharedData(User);
            return View("FAQ",Global.sharedDataModel);
        }

        



    }
}