using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyAPI.Controllers
{
    public class ResetPasswordController : Controller
    {
        // GET: ResetPassword
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();

            using (DunkeyContext ctx = new DunkeyContext())
            {
                var token = ctx.ForgotPasswordTokens.FirstOrDefault(x => x.Code == code && x.IsDeleted == false && (DateTime.Now.Hour - x.CreatedAt.Hour) < 4);

                if (token != null)
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Id == token.User_Id);
                    model.UserId = user.Id;
                    model.Email = user.Email;
                    return View(model);
                }
                else
                    return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DunkeyContext ctx = new DunkeyContext())
            {
                var tokens = ctx.ForgotPasswordTokens.Where(x => x.User_Id == model.UserId && x.IsDeleted == false);

                if (tokens.Count() > 0)
                {
                    foreach (var token in tokens)
                        token.IsDeleted = true;

                    ctx.Users.FirstOrDefault(x => x.Id == model.UserId).Password = model.Password;

                    ctx.SaveChanges();

                }
            }

            return View("PasswordResetSuccess");
        }
    }

    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
