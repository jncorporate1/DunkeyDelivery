using DAL;
using DunkeyAPI.Models;
using DunkeyAPI.Utility;
using DunkeyDelivery.CustomAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using static DunkeyDelivery.Utility;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Notification")]
    public class NotificationsController : ApiController
    {
        #region Commented
        ///// <summary>
        ///// Get All Notifications
        ///// </summary>
        ///// <param name="userId">User unique identifier</param>
        ///// <param name="SignInType">0 for User, 1 for deliverer</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetNotifications")]
        //public async Task<IHttpActionResult> GetNotifications(int UserId, int SignInType)
        //{
        //    try
        //    {
        //        using (DunkeyContext ctx = new DunkeyContext())
        //        {
        //            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();

        //            if (SignInType == (int)RoleTypes.User)
        //                notificationsViewModel.Notifications = ctx.Notifications.Where(x => x.User_ID.HasValue && x.User_ID.Value == UserId && x.Status == 0).OrderByDescending(x => x.Id).ToList();
        //            //else if (SignInType == (int)RoleTypes.Deliverer)
        //            //    notificationsViewModel.Notifications = ctx.Notifications.Where(x => x.DeliveryMan_ID.HasValue && x.DeliveryMan_ID.Value == UserId && x.Status == 0).OrderByDescending(x => x.Id).ToList();

        //            CustomResponse<NotificationsViewModel> response = new CustomResponse<NotificationsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = notificationsViewModel };

        //            return Ok(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// Mark notification as read.
        ///// </summary>
        ///// <param name="notificationId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("MarkNotificationAsRead")]
        //public async Task<IHttpActionResult> MarkNotificationAsRead(int NotificationId)
        //{
        //    try
        //    {
        //        using (DunkeyContext ctx = new DunkeyContext())
        //        {
        //            var notification = ctx.Notifications.FirstOrDefault(x => x.Id == NotificationId);

        //            if (notification != null)
        //            {
        //                notification.Status = (int)Global.NotificationStatus.Read;
        //                ctx.SaveChanges();
        //                CustomResponse<string> response = new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK };
        //                return Ok(response);
        //            }
        //            else
        //            {
        //                CustomResponse<Error> response = new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid notificationid" } };
        //                return Ok(response);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[HttpGet]
        //[Route("MarkAllNotificationAsRead")]
        //public async Task<IHttpActionResult> MarkAllNotificationAsRead(int User_Id)
        //{
        //    try
        //    {
        //        using (DunkeyContext ctx = new DunkeyContext())
        //        {
        //            var notification = ctx.Notifications.Where(x => x.User_ID == User_Id).ToList();

        //            if (notification != null)
        //            {
        //                foreach (var notify in notification)
        //                {
        //                    notify.Status = (int)BasketApi.Utility.NotificationStatus.Read;
        //                }
        //                ctx.SaveChanges();
        //                CustomResponse<string> response = new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK };
        //                return Ok(response);
        //            }
        //            else
        //            {
        //                CustomResponse<Error> response = new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid notificationid" } };
        //                return Ok(response);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}


        ///// <summary>
        ///// Turn user notifications on or off
        ///// </summary>
        ///// <param name="UserId"></param>
        ///// <param name="On"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("UserNoticationsOnOff")]
        //public async Task<IHttpActionResult> UserNoticationsOnOff(int UserId, int SignInType, bool On)
        //{
        //    try
        //    {
        //        using (DunkeyContext ctx = new DunkeyContext())
        //        {
        //            if (SignInType == (int)RoleTypes.User)
        //            {

        //                var user = ctx.Users.FirstOrDefault(x => x.Id == UserId);
        //                if (user != null)
        //                {
        //                    user.IsNotificationsOn = On;
        //                    ctx.SaveChanges();
        //                }
        //            }
        //            else
        //            {
        //                var deliveryMan = ctx.DeliveryMen.FirstOrDefault(x => x.Id == UserId);
        //                if (deliveryMan != null)
        //                {
        //                    deliveryMan.IsNotificationsOn = On;
        //                    ctx.SaveChanges();
        //                }
        //            }

        //        }
        //        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //} 
        #endregion

        [HttpGet]
        [Route("SendPushNotificationTemp")]
        public async Task<IHttpActionResult> SendPushNotification(int User_Id,string Text)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    Global.objPushNotifications.SendIOSPushNotification(ctx.UserDevice.Where(x1 => x1.Platform == false && x1.User_Id== User_Id).ToList(), null, new Notification { Title = "Swagger", Text = Text });
                    Global.objPushNotifications.SendAndroidPushNotification(ctx.UserDevice.Where(x1 => x1.Platform == true && x1.User_Id == User_Id).ToList(), null, new Notification { Title = "Swagger", Text = Text });

                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        //[BasketApi.Authorize]
        [HttpPost]
        [Route("RegisterPushNotification")]
        public async Task<IHttpActionResult> RegisterPushNotification(RegisterPushNotificationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                   
                        var user = ctx.Users.Include(x => x.UserDevice).FirstOrDefault(x => x.Id == model.User_Id);
                        if (user != null)
                        {
                            var existingUserDevice = user.UserDevice.FirstOrDefault(x => x.UDID.Equals(model.UDID));
                            if (existingUserDevice == null)
                            {
                                //foreach (var userDevice in user.UserDevices)
                                //    userDevice.IsActive = false;

                                var userDeviceModel = new UserDevice
                                {
                                    Platform = model.IsAndroidPlatform,
                                    ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
                                    EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
                                    UDID = model.UDID,
                                    AuthToken = model.AuthToken,
                                    IsActive = true
                                };

                                PushNotificationsUtil.ConfigurePushNotifications(userDeviceModel);

                                user.UserDevice.Add(userDeviceModel);
                                ctx.SaveChanges();
                                return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userDeviceModel });
                            }
                            else
                            {
                                //foreach (var userDevice in user.UserDevices)
                                //    userDevice.IsActive = false;

                                existingUserDevice.Platform = model.IsAndroidPlatform;
                                existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
                                existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
                                existingUserDevice.UDID = model.UDID;
                                existingUserDevice.AuthToken = model.AuthToken;
                                existingUserDevice.IsActive = true;
                                existingUserDevice.User_Id = user.Id;
                                ctx.SaveChanges();
                                PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
                                return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUserDevice });
                            }
                        }

                        else
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("SignInType") } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Mark notification as read.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserNotifications")]
        public async Task<IHttpActionResult> GetUserNotifications(int User_Id, int Page, int Items)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    NotificationsViewModel returnModel = new NotificationsViewModel();
                    var notification = ctx.Notifications.Where(x => x.User_ID == User_Id).OrderBy(x => x.TimeOfSending).Take(Items).Skip(Page * Items).ToList();
                    if (notification != null)
                    {
                        returnModel.Notifications = notification;
                        returnModel.TotalNotifications = ctx.Notifications.Count(x => x.User_ID == User_Id);
                        CustomResponse<NotificationsViewModel> response = new CustomResponse<NotificationsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel };
                        return Ok(response);
                    }
                    else
                    {
                        CustomResponse<Error> response = new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid notificationid" } };
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetUserNotificationsCount")]
        public async Task<IHttpActionResult> GetUserNotificationsCount(int User_Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    NotificationVM returnModel = new NotificationVM();
                    returnModel.NotificationCount = ctx.Notifications.Count(x => x.User_ID == User_Id && x.Status == (int)NotificationStatus.Unread);

                    //returnModel.NotificationCount = notificationCount;
                    CustomResponse<NotificationVM> response = new CustomResponse<NotificationVM> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel };
                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("MarkAllNotificationAsUnRead")]
        public async Task<IHttpActionResult> MarkAllNotificationAsUnRead(int User_Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var notification = ctx.Notifications.Where(x => x.User_ID == User_Id).ToList();

                    if (notification != null)
                    {
                        foreach (var notify in notification)
                        {
                            notify.Status = (int)NotificationStatus.Unread;
                        }
                        ctx.SaveChanges();
                        CustomResponse<string> response = new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK };
                        return Ok(response);
                    }
                    else
                    {
                        CustomResponse<Error> response = new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid notificationid" } };
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

    }
}
