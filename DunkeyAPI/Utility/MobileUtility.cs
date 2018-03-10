using DAL;
using DunkeyAPI.Models;
using DunkeyAPI.ViewModels;
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

namespace DunkeyAPI
{
    public class MobileUtility
    {
        public static CreditCard GetUserCreditCard(int User_Id)
        {
            try
            {
                using (DunkeyContext ctx=new DunkeyContext())
                {
                    var UserCreditCard = ctx.CreditCards.FirstOrDefault(x => x.User_ID == User_Id && x.Is_Primary == 1 && x.is_delete == false);
                    if (UserCreditCard == null)
                    {
                        UserCreditCard = ctx.CreditCards.FirstOrDefault(x => x.User_ID == User_Id && x.is_delete == false);
                        if (UserCreditCard == null)
                        {
                            return null;
                        }
                    }
                    return UserCreditCard;
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
                return null;
            }
        }
        
        public static UserAddress GetUserUserAddress(int User_Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var UserAddress = ctx.UserAddresses.FirstOrDefault(x => x.User_ID == User_Id && x.IsPrimary == true && x.IsDeleted == false);
                    if (UserAddress == null)
                    {
                        UserAddress = ctx.UserAddresses.FirstOrDefault(x => x.User_ID == User_Id && x.IsDeleted == false);
                        if (UserAddress == null)
                        {
                            return null;
                        }
                    }
                    return UserAddress;
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
                return null;
            }
        }
        
    }
}