using DunkeyDelivery;
using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var response = await ApiCall<BlogViewModel>.CallApi("api/Blog/GetBlogPosts", null, false);
            var responseResult = response.GetValue("Result").ToObject<BlogViewModel>();
            returnResponse = responseResult;
            returnResponse.SetSharedData(User);

            return View("~/Areas/User/Views/Footer/Blog.cshtml", returnResponse);
        }

        public async Task<ActionResult> RedeemReward(int RewardID)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            var responseRedeem = await ApiCall<RewardsViewModel>.CallApi("api/Reward/RedeemPrize?RewardID=" + RewardID + "&UserID=" + userId, null, false);
            string RewardMessage = string.Empty;

            var Message = responseRedeem.GetValue("Message").ToString();
            if (Message == "Success")
            {
                RewardMessage = "Congratulations! You have redeemed your reward, Which will be applied on your next shopping cart.";
            }

            var responseRewards = await ApiCall<RewardsViewModel>.CallApi("api/Reward/GetRewardPrizes?UserID=" + userId, null, false);
            var objRewardsViewModel = responseRewards.GetValue("Result").ToObject<RewardsViewModel>();
            var listRewards = objRewardsViewModel.Rewards;
            var objUserPoints = objRewardsViewModel.UserPoints;

            ViewBag.BannerImage = "rewards-banner.jpg";
            ViewBag.Title = "Get Rewards";
            ViewBag.BannerTitle = "Get Rewards";
            ViewBag.Path = "Home > Get Rewards";
            ViewBag.RewardMessage = RewardMessage;
            ViewBag.RewardPoints = objUserPoints.RewardPoints;
            ViewBag.listRewards = listRewards;
            Global.sharedDataModel.SetSharedData(User);
            return View("GetRewards", Global.sharedDataModel);
        }

        public async Task<ActionResult> GetRewards()
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            var responseRewards = await ApiCall<RewardsViewModel>.CallApi("api/Reward/GetRewardPrizes?UserID=" + userId, null, false);
            var objRewardsViewModel = responseRewards.GetValue("Result").ToObject<RewardsViewModel>();
            var listRewards = objRewardsViewModel.Rewards;
            var objUserPoints = objRewardsViewModel.UserPoints;

            ViewBag.RewardMessage = string.Empty;
            ViewBag.BannerImage = "rewards-banner.jpg";
            ViewBag.Title = "Get Rewards";
            ViewBag.BannerTitle = "Get Rewards";
            ViewBag.Path = "Home > Get Rewards";
            ViewBag.RewardPoints = objUserPoints.RewardPoints;
            ViewBag.listRewards = listRewards;
            Global.sharedDataModel.SetSharedData(User);
            return View("GetRewards", Global.sharedDataModel);

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
            return View("FAQ", Global.sharedDataModel);
        }





    }
}