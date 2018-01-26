using DAL;
using DunkeyAPI.Models;
using DunkeyAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Reward")]
    public class RewardsController : ApiController
    {
        [HttpGet]
        [Route("GetRewardPrizes")]
        public IHttpActionResult GetRewardPrizes(int UserID)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    RewardViewModel resp = new RewardViewModel();
                    var RewardMilestone = ctx.RewardMilestones.Include(x=>x.RewardPrize).ToList();
                    //var RewardMilestone1 = ctx.RewardMilestones.Include(x => x.RewardPrize);
                    resp.Rewards = RewardMilestone;
                    var CurrentUser = ctx.Users.Where(x => x.Id == UserID).FirstOrDefault();
                    resp.UserPoints.RewardPoints =CurrentUser.RewardPoints;
                    CustomResponse<RewardViewModel> response = new CustomResponse<RewardViewModel>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = resp
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("RedeemPrize")]
        public IHttpActionResult RedeemPrize(int UserID,int RewardID)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var Reward = new RewardMilestones();
                    var CurrentUser = ctx.Users.Where(x => x.Id == UserID).FirstOrDefault();

                    Reward = ctx.RewardMilestones.Where(x => x.Id == RewardID).FirstOrDefault();
                    
                    if(CurrentUser.RewardPoints>=Reward.PointsRequired)
                    {
                       var reward= ctx.UserRewards.Add(new UserRewards {
                            Created_Date = DateTime.Now,
                            is_deleted = 0,
                            User_Id=CurrentUser.Id,
                           RewardMilestones_Id=Reward.Id 
                        });
                        ctx.SaveChanges();

                        CurrentUser.RewardPoints = CurrentUser.RewardPoints - Reward.PointsRequired;
                        ctx.SaveChanges();


                        CustomResponse<UserRewards> response = new CustomResponse<UserRewards>
                        {
                            Message = "Success",
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = reward
                        };
                        return Ok(response);
                    }
                    
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Insufficient Points." }
                        });
                    
                    

                   
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [HttpGet]
        [Route("GetRedeemedPrize")]
        public IHttpActionResult GetRedeemedPrize(int UserID)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    double TotalPrize=0;
                    UserReward resp = new UserReward();
                    var UserRewards = ctx.UserRewards.Include("RewardMilestones").Include("User").Where(x => x.User_Id == UserID && x.is_deleted == 0).ToList();
                    if (UserRewards == null)
                    {
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "No redeemed prize found." } });

                    }

                    foreach (var Reward in UserRewards)
                    {
                        if (Reward.RewardMilestones.RewardPrize_Id.HasValue)
                        {
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Prize cannot be redeemed." } });

                        }else
                        {
                            TotalPrize = +Reward.RewardMilestones.AmountAward;
                            
                        }
                    }
                        resp.RewardAmount = TotalPrize;
                        CustomResponse<UserReward> response = new CustomResponse<UserReward>
                        {
                            Message = "Success",
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = resp
                        };
                        return Ok(response);

                    //CustomResponse<RewardViewModel> response = new CustomResponse<RewardViewModel>
                    //{
                    //    Message = "Success",
                    //    StatusCode = (int)HttpStatusCode.OK,
                    //    Result = resp
                    //};
                    //return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetPrizes")]
        public IHttpActionResult GetPrizes(int? Page=0,int? Items=0)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    double TotalPrize = 0;
                    Rewards resp = new Rewards();
                    var Rewards = ctx.RewardMilestones.Include(x=>x.RewardPrize).ToList(); // bug asi 
                    if (Rewards == null)                    {
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Reward list not found." } });

                    }
                    resp.RewardsList = Rewards;
                    CustomResponse<Rewards> response = new CustomResponse<Rewards>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = resp
                    };
                    return Ok(response);
                    
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


    }
}
