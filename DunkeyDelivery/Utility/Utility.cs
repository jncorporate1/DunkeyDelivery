using DunkeyDelivery.BindingModels;
using DunkeyDelivery.Models;
using DunkeyDelivery.ViewModels;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace DunkeyDelivery 
{
    public class ApiCall<T>
    {
        static HttpClient client;
        public static string BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        //public static string BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];      //"http://10.100.28.44/";

        static ApiCall()
        {
            client = new HttpClient();
             
            //client.BaseAddress = new Uri("http://10.100.28.47/");

            client.BaseAddress = new Uri("http://10.100.28.38/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public static async Task<JObject> CallApi(string Uri, T model,bool method=true)
        {
            HttpResponseMessage response;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (method) // Post Method
                    {
                       response = await client.PostAsJsonAsync(Utility.BaseUrl + Uri, model);
                    }
                    else{  // Get Method

                       response = await client.GetAsync(Utility.BaseUrl + Uri);
                    }
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        if (responseJson.GetValue("StatusCode").ToObject<int>() == (int)HttpStatusCode.OK)
                        {
                            return responseJson;
                        }
                        else
                        {
                            var error = responseJson.GetValue("Result").ToObject<Error>();
                            return error;
                        }

                    }
                    else
                        return null;
                    //var result = await response.Content.ReadAsAsync<CustomResponse<UserViewModel>>();
                    //return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task<JObject> CallApiAsMultipart(string Uri, MultipartFormDataContent content)
        {
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsync(Utility.BaseUrl + Uri, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    if (responseJson.GetValue("StatusCode").ToObject<int>() == (int)HttpStatusCode.OK)
                    {
                        return responseJson;
                    }
                    else
                    {
                        var error = responseJson.GetValue("Result").ToObject<Error>();
                        return error;
                    }

                }
                else
                    return null;

            }
        }



    }
    public static class ExtensionMethods
    {
        public static void ReloadStripeSettings(this StripeSettings Me)
        {
            Me.PublishableKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            Me.SecretKey = ConfigurationManager.AppSettings["StripeSecretKey"];
        }

        public static string GetFormattedBreadCrumb(this CategoryBindingModel category, IEnumerable<CategoryBindingModel> categoryService, string separator = ">>")
        {
            string result = string.Empty;
            if (category.ParentCategoryId == null || category.ParentCategoryId == 0)
            {
                return category.Name;
            }
            var breadcrumb = categoryService.Where(x => x.Id == category.ParentCategoryId).FirstOrDefault();

            if (breadcrumb == null)
            {
                return category.Name;
            }
            var categoryName = breadcrumb.Name;
            result = string.Format("{0} {1} {2}", categoryName, separator, category.Name);

            //}

            return result;
        }

        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }



    }

    public class Content
    {

        public string GetContentTypeText(int Type)
        {
            var TypeStr = "";
            switch (Type)
            {
                case 0:
                    TypeStr = "About Us";
                    break;
                case 1:
                    TypeStr = "Contact Us";
                    break;

                default:
                    break;

            }
            return TypeStr;


        }

        public enum Types
        {
            AboutUs = 0,
            ContactUs = 1
        }

    }


    public class Utility
    {
        public enum CartItemType
        {
            Normal,
            OfferProduct,
            OfferPackage
        }
        public static string BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];      //"http://10.100.28.44/";
        public static string WebBaseUrl = ConfigurationManager.AppSettings["WebBaseUrl"];
        public static string EmailBaseUrl = ConfigurationManager.AppSettings["EmailBaseUrl"];

        public enum RoleTypes
        {
            User = 0,
            Deliverer = 1,
            SubAdmin = 2,
            SuperAdmin = 3,
            ApplicationAdmin = 4,
            Facebook=5,
            Gmail = 6
        }
        public enum DeliveryTypes
        {
            ASAP= 0,
            Today= 1,
            Later = 2
           
        }
        public static HttpStatusCode LogError(Exception ex)
        {

            try
            {
                using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
                {
                    if (ex.Message != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Message" + ex.Message);
                        sw.WriteLine(Environment.NewLine + "StackTrace" + ex.StackTrace);
                    }
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine(Environment.NewLine + "InnerExceptionStackTrace : " + ex.InnerException.StackTrace);
                    }
                    sw.WriteLine("------******------");
                }
                return HttpStatusCode.InternalServerError;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public static StripeCharge GetStripeChargeInfo(string stripeEmail, string stripeToken, int amount)
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
                Amount = amount * 100, //charge in cents
                Description = "Dunkey Delivery",
                Currency = "usd",
                CustomerId = customer.Id,
            });

            return charge;
        }
    }

    public static class DefaultImages
    {
        //public static string UserDefaultImage()
        //{
        //    var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        //    //var DefaultUserImage = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["DefaultImageFolderPath"] + "DefaultUserImage.jpg");
        //    var DefaultUserImage= baseUrl + ConfigurationManager.AppSettings["ProductImageFolderPath"] + "DefaultUserImage.jpg";
        //    return DefaultUserImage;
        //}
        public static string StoreDefaultImage()
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            //var DefaultStoreImage = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["DefaultImageFolderPath"] + "DefaultStoreImage.png");
            var DefaultStoreImage = "/"+ConfigurationManager.AppSettings["DefaultImageFolderPath"] + "DefaultStoreImage.jpg";
            return DefaultStoreImage;
        }

    }

    //public class EmailUtil
    //{
    //    public static string FromEmail = ConfigurationManager.AppSettings["FromMailAddress"];
    //    public static string FromName = ConfigurationManager.AppSettings["OffCampusParking"];
    //    public static string FromPassword = ConfigurationManager.AppSettings["FromPassword"];
    //    public static MailAddress FromMailAddress = new MailAddress(FromEmail, FromName);
    //}
    public class EmailUtil
    {
        public static string FromEmail = ConfigurationManager.AppSettings["FromMailAddress"];
        public static string FromName = ConfigurationManager.AppSettings["FromMailName"];
        public static string FromPassword = ConfigurationManager.AppSettings["FromPassword"];
        public static MailAddress FromMailAddress = new MailAddress(FromEmail, FromName);
    }

    class Global
    {

        public static SharedViewModel sharedDataModel = new SharedViewModel();

    }


}