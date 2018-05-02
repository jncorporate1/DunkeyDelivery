using BasketWebPanel.DomainModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel
{
    public class ApiCall
    {
        static HttpClient client;
        public static string BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

        //public static async Task<JObject> CallPostApi(string Uri, Object model, ClaimsIdentity claimsIdentity)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var access_token = claimsIdentity.FindFirst("access_token");
        //            if (access_token != null)
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(claimsIdentity.FindFirst("token_type").Value, access_token.Value);
        //            }

        //            var response = await client.PostAsJsonAsync(BaseUrl + Uri, model);

        //            if (response.StatusCode == HttpStatusCode.OK)
        //            {
        //                var responseJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);
        //                if (responseJson.GetValue("StatusCode").ToObject<int>() == (int)HttpStatusCode.OK)
        //                {
        //                    return responseJson;
        //                }
        //                else
        //                {
        //                    var error = responseJson.GetValue("Result").ToObject<Error>();
        //                    return error;
        //                }

        //            }
        //            else
        //                return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public static async Task<JObject> CallApi(string Uri, IPrincipal User, object model = null, bool GetRequest = false, bool isMultipart = false, MultipartFormDataContent multipartContent = null, params string[] parameters)
        {
            try
            {
                HttpResponseMessage response;
                string paramString = parameters.Count() > 0 ? "?" : String.Empty;
                bool RefreshTokenAttempted = false;
                using (client = new HttpClient())
                {
                    foreach (var param in parameters)
                    {
                        paramString += param + "&";
                    }
                    paramString = paramString.TrimEnd('&');

                    callAgain: var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var access_token = claimsIdentity.FindFirst("access_token");

                    if (access_token != null)
                    {
                        if (isMultipart)
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(claimsIdentity.FindFirst("token_type").Value, access_token.Value);
                        }
                        else
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(claimsIdentity.FindFirst("token_type").Value, access_token.Value);
                    }
                    if (isMultipart)
                    {
                        response = await client.PostAsync(BaseUrl + Uri, multipartContent);
                    }
                    else
                    {
                        if (GetRequest)
                            response = await client.GetAsync(BaseUrl + Uri + paramString);
                        else
                            response = await client.PostAsJsonAsync(BaseUrl + Uri, model);
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
                    else if (response.StatusCode == HttpStatusCode.Unauthorized && RefreshTokenAttempted == false)
                    {
                        RefreshTokenAttempted = true;
                        var refreshResponse = await RefreshAccessToken(claimsIdentity.FindFirst("refresh_token").Value);
                        var tokenModel = refreshResponse.ToObject<Token>();
                        if (tokenModel != null)
                        {

                            User.AddUpdateClaim("access_token", tokenModel.access_token);
                            User.AddUpdateClaim("token_type", tokenModel.token_type);
                            User.AddUpdateClaim("expires_in", tokenModel.expires_in);
                            User.AddUpdateClaim("refresh_token", tokenModel.refresh_token);
                            if (isMultipart)
                            {
                                return JObject.Parse("{\"message\":\"UnAuthorized\"}");
                            }
                            else
                                goto callAgain;
                        }
                        else
                            return null;

                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return null;
                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<JObject> RefreshAccessToken(string refreshToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpContent content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("refresh_token", refreshToken),
                        new KeyValuePair<string, string>("grant_type", "refresh_token")
                    });

                    var response = await client.PostAsync(BaseUrl + "/token", content);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public static async Task<JObject> CallApiAsMultipart(string Uri, MultipartFormDataContent content)
        //{
        //    using (client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        HttpResponseMessage response = await client.PostAsync(BaseUrl + Uri, content);

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            var responseJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);
        //            if (responseJson.GetValue("StatusCode").ToObject<int>() == (int)HttpStatusCode.OK)
        //            {
        //                return responseJson;
        //            }
        //            else
        //            {
        //                var error = responseJson.GetValue("Result").ToObject<Error>();
        //                return error;
        //            }

        //        }
        //        else
        //            return null;

        //    }
        //}

    }
}