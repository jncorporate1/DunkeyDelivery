using DAL;
using DunkeyAPI.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static DunkeyAPI.Utility.Global;

namespace DunkeyDelivery
{
    public static class Utility
    {

        public static string StoreDeliveryTypes(int Type_Id)
        {

            var Type = "";

            switch (Type_Id)
            {
                case 0:
                    Type = "ASAP";
                    break;
                case 1:
                    Type = "Today";
                    break;
                case 2:
                    Type = "Later";
                    break;
                default:
                    break;

            }
            return Type;


        }

        private static HttpClient client = new HttpClient();

        public static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public static string GuestEmail = "Guest@gmail.com";

        public static void ReloadStripeSettings(this StripeSettings Me)
        {
            Me.PublishableKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            Me.SecretKey = ConfigurationManager.AppSettings["StripeSecretKey"];
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                destination.Add(item);
            }
        }

        public static async Task GenerateToken(this User user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "role", Convert.ToString(user.Role)},
                            { "grant_type", "password" }
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();


                //var TokenResponse = await response.Content.ReadAsStringAsync();
                //var accessToken = TokenResponse.Substring(TokenResponse.IndexOf(":") + 2, TokenResponse.IndexOf("token_type") - "token_type".Length - 10);
                //return accessToken;
                //return TokenResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task GenerateToken(this Admin user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", user.Role.ToString() }
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public static HttpStatusCode SendEmail(string From, string To, string Subject, string Body)
        {

            using (var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("data.expert8@gmail.com", "ASHFAQahmed123"),
                EnableSsl = true
            })
            {
                client.Send(From, To, Subject, Body);
            }

            return HttpStatusCode.OK;




            //    MailMessage mail = new MailMessage("you@yourcompany.com", "user@hotmail.com");
            //    SmtpClient client = new SmtpClient();
            //    client.Port = 25;
            //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    client.UseDefaultCredentials = false;
            //    client.Host = "smtp.google.com";
            //    mail.Subject = "this is a test email.";
            //    mail.Body = "this is my test email body";
            //    client.Send(mail);

            //    return Content(HttpStatusCode.OK, new CustomResponse<Error>
            //    {
            //        Message = "Forbidden",
            //        StatusCode = (int)HttpStatusCode.Forbidden,
            //        Result = new Error { ErrorMessage = "Invalid UserName or Password" }
            //    });
        }

        public static DbGeography CreatePoint(double lat, double lon, int srid = 4326)
        {
            string wkt = String.Format("POINT({0} {1})", lon, lat);

            return DbGeography.PointFromText(wkt, srid);
        }

        public enum DunkeyEntityTypes
        {
            Product,
            Category,
            Store,
            Package,
            Admin,
            Offer,
            Unit
        }
        public static string GetOrderStatusName(int orderStatus)
        {
            try
            {
                return ((OrderStatuses)orderStatus).ToString();
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }

        public static string GetOrderStatusNameString(int orderStatus)
        {
            try
            {

                switch (orderStatus)
                {
                    case 0:
                        return "Initiated";
                        break;
                    case 1:
                        return "Accepted";
                        break;
                    case 2:
                        return "Rejected";
                        break;
                    case 3:
                        return "In Progress";
                        break;
                    case 4:
                        return "Ready For Delivery";
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
                return ((OrderStatuses)orderStatus).ToString();
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }

        public static StripeCharge GetStripeChargeInfo(string stripeEmail, string stripeToken, int amount)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();
            var stripeSettings = new StripeSettings();
            stripeSettings.ReloadStripeSettings();
            StripeConfiguration.SetApiKey(stripeSettings.SecretKey);

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

        public enum NotificationStatus
        {
            Unread = 0,
            Read = 1
        }

    }



    public class Global
    {
        public static int MaximumImageSize = 1024 * 1024 * 10; // 10 Mb
        public static int MinDeliveryTime = 30;
        public static string ImageSize = "10 MB";
        public static int PointsToReward = 20;
        public const string PushFrom = "Dunkey Delivery";
        public static int VerificationCodeTimeOut = 4;
        private static int searchStoreRadius = Convert.ToInt32(ConfigurationManager.AppSettings["NearByStoreRadius"]);
        public static double NearbyStoreRadius = searchStoreRadius * 1609.344;
        public enum StatusCode
        {
            NotVerified = 1,
            Verified = 2
        }

        public const string SuccessMessage = "Success";
    }
    public class Stores
    {

        public string Categories_enum(int Category_id)
        {

            var Type = "";

            switch (Category_id)
            {
                case 0:
                    Type = "Food";
                    break;
                case 1:
                    Type = "Alcohol";
                    break;
                case 2:
                    Type = "Grocery";
                    break;
                case 3:
                    Type = "Laundry";
                    break;
                case 4:
                    Type = "Pharmacy";
                    break;
                case 5:
                    Type = "Retail";
                    break;
                case 10:
                    Type = "All";
                    break;
                default:
                    break;

            }
            return Type;


        }

        public enum Categories
        {
            Food = 0,
            Alcohol = 1,
            Grocery = 2,
            Laundry = 3,
            Pharmacy = 4,
            Retail = 5,
            All = 10

        }

        public enum AlcoholCategoryTypes
        {
            Wine = 1,
            Liquor = 2,
            Beer = 3
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
    public class EmailUtil
    {
        public static string FromEmail = ConfigurationManager.AppSettings["FromMailAddress"];
        public static string FromName = ConfigurationManager.AppSettings["FromMailName"];
        public static string FromPassword = ConfigurationManager.AppSettings["FromPassword"];
        public static MailAddress FromMailAddress = new MailAddress(FromEmail, FromName);
    }

    public enum FilterSortBy
    {
        Distance = 0,
        Rating = 1,
        DeliveryTime = 2,
        Price = 3,
        MinDelivery = 4,
        AtoZ = 5,
        Relevance = 6

    }

    public enum FilterAlcoholSortBy
    {
        BestSelling = 0,
        Low2High = 1,
        A2Z = 2,
        High2Low = 3,
        Z2A = 4
    }
}

