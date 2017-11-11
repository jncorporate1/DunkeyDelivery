using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    public class BlogController : Controller
    {
        // GET: User/Blog
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BlogDetails(int Id)
        {
            BlogDetailsViewModel model = new BlogDetailsViewModel();
            var response = await ApiCall<BlogDetailsViewModel>.CallApi("api/Blog/GetBlogPostsById?Id="+Id, null, false);
            var responseResult = response.GetValue("Result").ToObject<BlogDetailsViewModel>();
            model = responseResult;
            model.SetSharedData(User);
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Blog Detail";
            ViewBag.BannerTitle = "Blog Detail";
            ViewBag.Path = "Home > Blog > Blog Detail ";
            return View("~/Areas/User/Views/Blog/BlogDetails.cshtml", model);
        }

        public async Task<ActionResult> Comment(int PostId,string Message)
        {
            Global.sharedDataModel.SetSharedData(User);
            var User_id=Convert.ToInt32(Global.sharedDataModel.Id);
            if (User_id == null)
            {
                User_id = 0;
            }
            var response = await ApiCall<CommentViewModel>.CallApi("api/Blog/InsertComments?Message="+Message+"&Post_id="+PostId+"&UserId="+ User_id, null, false);
            if (response == null)
            {
                return Json("Failed");
            }
            else
            {
                var responseResult = response.GetValue("Result").ToObject<CommentViewModel>();

                return Json("Success");
            }
            
        }


    }
}