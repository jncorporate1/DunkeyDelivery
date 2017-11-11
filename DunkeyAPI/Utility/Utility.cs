using DAL;
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
        private static HttpClient client = new HttpClient();

        public static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public static string GuestEmail = "Guest@gmail.com";

        public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static async Task GenerateToken(this User user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
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
        public static HttpStatusCode SendEmail(string From,string To, string Subject, string Body)
        {

            using (var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("data.expert8@gmail.com", "ASHFAQahmed123"),
                EnableSsl = true
            })
            {
                client.Send(From,To,Subject,Body);
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
            Offer
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
            All=10

        }
        
    }

    //public class EmailUtil
    //{
    //    public static string FromEmail = ConfigurationManager.AppSettings["FromMailAddress"];
    //    public static string FromName = ConfigurationManager.AppSettings["OffCampusParking"];
    //    public static string FromPassword = ConfigurationManager.AppSettings["FromPassword"];
    //    public static MailAddress FromMailAddress = new MailAddress(FromEmail, FromName);
    //}

}