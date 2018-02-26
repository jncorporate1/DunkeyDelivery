using BasketApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Utility
{
    public static class Global
    {
        public static PushNotifications objPushNotifications = new PushNotifications(false);
        public class Constants
        {
            public const string Food = "Food";
            public const string Grocery = "Grocery";
            public const string Laundry = "Laundry";
            public const string Alcohol = "Alcohol";
            public const string Pharmacy = "Pharmacy";
            public const string Retail = "Retail";
            
        }

        public enum NotificationTargetAudienceTypes
        {
            UserAndDeliverer = 1,
            User = 2,
            Deliverer = 3,
            SubAdmins = 4
        }


        public enum OrderFrequency
        {
            OneTime=0,
            Weekly=1,
            Monthly=2
        }

        public enum CartItemTypes
        {
            Product,
            Package,
            Offer_Product,
            Offer_Package
        }

        public enum ClothRequestTypes
        {
            Pending=0,
            Accepted=1,
            Rejected=2

        }

        public enum OrderStatuses
        {
            Initiated,
            InProgress,
            ReadyForDelivery,
            AssignedToDeliverer, //equivalent to deliverer initiated 3
            DelivererInProgress,
            Dispatched,
            Completed
        }
        public enum PushNotificationType
        {
            Announcement = 1,
            OrderAccepted = 2,
            OrderRejected = 3,
            OrderAssignedToDeliverer = 4,
            OrderDispatched = 5,
            OrderCompleted = 6,

        }

        public class ResponseMessages
        {
            public const string Success = "Success";
            public const string NotFound = "NotFound";
            public const string BadRequest = "BadRequest";
            public const string Conflict = "Conflict";

            public static string CannotBeEmpty(params string[] args)
            {
                try
                {
                    string returnString = "";
                    for (int i = 0; i < args.Length; i++)
                        returnString += args[i] + ", ";
                    returnString = returnString.Remove(returnString.LastIndexOf(','), 1);
                    return returnString + "cannot be empty";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateInvalid(params string[] args)
            {
                try
                {
                    string returnString = "";
                    for (int i = 0; i < args.Length; i++)
                        returnString += args[i] + ", ";
                    returnString = returnString.Remove(returnString.LastIndexOf(','), 1);
                    return "Invalid " + returnString;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateAlreadyExists(string arg)
            {
                try
                {
                    return arg + " already exists";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateNotFound(string arg)
            {
                try
                {
                    return arg + " not found";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }




    }
}