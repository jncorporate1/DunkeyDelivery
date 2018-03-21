using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using DunkeyAPI.ViewModels;
using static DunkeyAPI.Utility.Global;
using DunkeyAPI.Utility;
using System.Data.Entity;
using System.Net;
using DunkeyAPI.Models;

namespace DunkeyAPI.ExtensionMethods
{
    public static class ExtensionMethods
    {

        public static void AddDeliveryTypesToList(this List<DAL.StoreDeliveryTypes> DeliveryTypes, int Store_Id,List<int> DeliveryTypes_Ids)
        {
            try
            {
                if (DeliveryTypes_Ids.Count > 0)
                {
                    foreach (var item in DeliveryTypes_Ids)
                    {
                        DeliveryTypes.Add(new StoreDeliveryTypes { Store_Id = Store_Id, Type_Id = item, Type_Name = DunkeyDelivery.Utility.StoreDeliveryTypes(item) });
                    }
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }
        
        public static void SetAverageRating(this Store store)
        {
            try
            {
                if (store.StoreRatings.Count > 0)
                {
                    store.AverageRating = store.StoreRatings.Average(x => x.Rating);
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void CalculateAverageRating(this Store store)
        {
            try
            {
                if (store.StoreRatings.Count > 0)
                {
                    store.AverageRating = store.StoreRatings.Average(x => x.Rating);
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void CalculateCustomStoreAverageRating(this FilterStores store)
        {
            try
            {
                if (store.StoreRatings.Count > 0)
                {
                    store.AverageRating = store.StoreRatings.Average(x => x.Rating);
                }else
                {
                    store.AverageRating = 0;
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void CalculateAllTypesAverageRating(this Store store)
        {
            try
            {
                foreach (var rate in store.StoreRatings)
                {
                    if (rate.Rating == 5)
                    {
                        store.RatingType.FiveStar++;

                    }
                    else if (rate.Rating == 4)
                    {
                        store.RatingType.FourStar++;
                    }
                    else if (rate.Rating == 3)
                    {
                        store.RatingType.ThreeStar++;
                    }
                    else if (rate.Rating == 2)
                    {
                        store.RatingType.TwoStar++;
                    }
                    else if (rate.Rating == 1)
                    {
                        store.RatingType.OneStar++;
                    }
                }

                store.RatingType.TotalRatings = store.RatingType.OneStar + store.RatingType.TwoStar + store.RatingType.ThreeStar + store.RatingType.FourStar + store.RatingType.FiveStar;


            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void SetOrderItem(this Order_Items orderItem, CartItemViewModel model, DunkeyContext ctx)
        {
            try
            {
                switch (model.ItemType)
                {
                    case (int)CartItemTypes.Product:
                        orderItem.Product_Id = model.ItemId;
                        var product = ctx.Products.FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                        orderItem.Name = product.Name;
                        orderItem.Price = Convert.ToDouble(product.Price);
                        orderItem.Description = product.Description;
                        break;
                    case (int)CartItemTypes.Package:
                        orderItem.Package_Id = model.ItemId;
                        var package = ctx.Packages.FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                        orderItem.Name = package.Name;
                        orderItem.Price = Convert.ToDouble(package.Price);
                        orderItem.Description = package.Description;
                        break;
                    case (int)CartItemTypes.Offer_Product:
                        orderItem.Offer_Product_Id = model.ItemId;
                        var offerProduct = ctx.Offer_Products.Include(x => x.Product).FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                        orderItem.Name = offerProduct.Product.Name;
                        orderItem.Price = offerProduct.DiscountedPrice * model.Qty;
                        orderItem.Description = offerProduct.Description;
                        break;
                    case (int)CartItemTypes.Offer_Package:
                        orderItem.Offer_Package_Id = model.ItemId;
                        var offerPackage = ctx.Offer_Packages.Include(x => x.Package).FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                        orderItem.Name = offerPackage.Package.Name;
                        orderItem.Price = offerPackage.DiscountedPrice * model.Qty;
                        orderItem.Description = offerPackage.Description;
                        break;
                    default:
                        throw new Exception("Invalid CartItemType");
                }


                orderItem.Qty = model.Qty;
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void AddNewStoreOrder(this Order order, CartItemViewModel model, DunkeyContext ctx)
        {
            try
            {
                StoreOrder storeOrder = new StoreOrder();
                storeOrder.Store_Id = model.StoreId;
                storeOrder.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
                storeOrder.AddNewOrderItem(model, ctx);
                var businessTypeTax = ctx.BusinessTypeTax.FirstOrDefault(x => x.BusinessType == ctx.Stores.FirstOrDefault(x1 => x1.Id == model.StoreId).BusinessType);
                if (businessTypeTax != null)
                {
                    storeOrder.BusinessTypeTax = businessTypeTax.Tax;
                    storeOrder.BusinessType = businessTypeTax.BusinessType;
                }
                order.StoreOrders.Add(storeOrder);
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void AddNewOrderItem(this StoreOrder storeOrder, CartItemViewModel model, DunkeyContext ctx)
        {
            try
            {
                Order_Items orderItem = new Order_Items();
                orderItem.SetOrderItem(model, ctx);
                storeOrder.Order_Items.Add(orderItem);
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void SetOrderDetails(this Order order, OrderViewModel model)
        {
            try
            {
                order.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
                order.OrderDateTime = DateTime.UtcNow;
                order.Status = (int)OrderStatuses.Initiated;
                order.DeliveryTime_From = model.DeliveryDateTime_From;
                order.DeliveryTime_To = model.DeliveryDateTime_To;
                order.PaymentMethod = model.PaymentMethodType;
                order.User_ID = model.UserId;
                order.DeliveryDetails_AddtionalNote = model.AdditionalNote;
                
                if (string.IsNullOrEmpty(order.DeliveryDetails_Address) && string.IsNullOrEmpty(order.DeliveryDetails_Phone))
                {
                    var UserAddress = new UserAddress();
                    using (DunkeyContext ctx = new DunkeyContext())
                    {
                        //Set Delivery Details


                        UserAddress = ctx.UserAddresses.Include(x => x.User).FirstOrDefault(x => x.User_ID == order.User_ID && x.IsPrimary == true && x.IsDeleted == false);
                        if (UserAddress == null)
                        {
                            UserAddress = ctx.UserAddresses.Include(x => x.User).Where(x => x.User_ID == order.User_ID && x.IsDeleted == false).OrderByDescending(x => x.Id).FirstOrDefault();

                        }


                    }


                    order.DeliveryDetails_FirstName = UserAddress.User.FirstName;
                    order.DeliveryDetails_LastName = UserAddress.User.LastName;
                    order.DeliveryDetails_Phone = UserAddress.Telephone;
                    order.DeliveryDetails_ZipCode = UserAddress.PostalCode;
                    order.DeliveryDetails_Email = UserAddress.User.Email;
                    order.DeliveryDetails_City = UserAddress.City;
                    order.DeliveryDetails_Address = UserAddress.FullAddress;
                    order.DeliveryDetails_AddtionalNote = model.DeliveryDetails.AdditionalNote;
                }
                else
                {
                    order.DeliveryDetails_FirstName = model.DeliveryDetails.FirstName;
                    order.DeliveryDetails_LastName = model.DeliveryDetails.LastName;
                    order.DeliveryDetails_Phone = model.DeliveryDetails.Phone;
                    order.DeliveryDetails_ZipCode = model.DeliveryDetails.ZipCode;
                    order.DeliveryDetails_Email = model.DeliveryDetails.Email;
                    order.DeliveryDetails_City = model.DeliveryDetails.City;
                    order.DeliveryDetails_Address = model.DeliveryDetails.Address;
                    order.DeliveryDetails_AddtionalNote = model.DeliveryDetails.AdditionalNote;

                }

                order.TipAmount = model.TipAmount;
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void SetOrderDeliveryDetails(this OrderViewModel model,Order order)
        {
            using (DunkeyContext ctx=new DunkeyContext())
            {
                foreach (var storeOrder in order.StoreOrders)
                {
                    var SingleOrder=ctx.StoreOrders.FirstOrDefault(x => x.OrderNo == storeOrder.OrderNo);
                    var StoreDeliveryDetail = model.StoreDeliverytype.FirstOrDefault(x=>x.Store_Id==SingleOrder.Store_Id);
                    if (!SingleOrder.OrderDeliveryTime.HasValue)
                    {
                        SingleOrder.OrderDeliveryTime = StoreDeliveryDetail.OrderDateTime.Value;
                        SingleOrder.Type_Id = StoreDeliveryDetail.Type_Id;
                        ctx.SaveChanges();
                    }
                }
                //var StoreOrder=ctx.StoreOrders.Where()
            }
        }

        public static void MakeOrder(this Order order, OrderViewModel model, DunkeyContext ctx, int? Device = 0)
        {
            try
            {
                order.User_ID = model.UserId;
                foreach (var cartItem in model.Cart.CartItems)
                {
                    if (order.StoreOrders.Count == 0)
                    {
                        order.AddNewStoreOrder(cartItem, ctx);
                    }
                    else
                    {
                        var existingStoreOrder = order.StoreOrders.FirstOrDefault(x => x.Store_Id == cartItem.StoreId);

                        if (existingStoreOrder == null)
                        {
                            order.AddNewStoreOrder(cartItem, ctx);
                        }
                        else
                        {
                            existingStoreOrder.AddNewOrderItem(cartItem, ctx);
                        }
                    }
                }

         
                order.SetOrderDetails(model);
                if (Device.Value != 0)
                {
                    // for mobile
                    order.CalculateStoreOrderSubTotal();
                    order.CalculateStoreTotal();
                    order.CalculateDeliveryDetails();
                    order.CalculateStoreSubTotal();
                    order.CalculateOrderSettings();
                }
                else
                {
                    // for website
                    order.CalculateSubTotal();
                    order.CalculateTotal();
                }



            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }



        // store related totals



        // for mobile

            // store sub total
        public static void CalculateStoreOrderSubTotal(this Order order)
        {
            // store sub total
            foreach (var storeOrder in order.StoreOrders)
            {
                foreach (var orderItem in storeOrder.Order_Items)
                {
                    storeOrder.Subtotal = storeOrder.Subtotal + orderItem.Price * orderItem.Qty;
                }

            }
        }


        // store  total
        public static void CalculateStoreTotal(this Order StoreOrder)
        {
            try
            {// check if min delivery time is in store object or noot
                using (DunkeyContext ctx=new DunkeyContext())
                {
                    foreach (var storeOrder in StoreOrder.StoreOrders)
                    {
                        storeOrder.Total = storeOrder.Subtotal + Convert.ToDouble(ctx.Stores.FirstOrDefault(x=>x.Id==storeOrder.Store_Id).MinDeliveryCharges);
                    } 
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        // order sub total
        public static void CalculateStoreSubTotal(this Order order)
        {
            order.Subtotal = order.StoreOrders.Sum(x => x.Total);
        }

        // order total and other parameters
        public static void CalculateOrderSettings(this Order order)
        {
            try
            {
                order.TipAmount = (order.StoreOrders.Sum(x => x.Subtotal) / 100) * 12;

                DunkeySettings.LoadSettings();
                order.DeliveryFee = DunkeySettings.DeliveryFee;
                order.TotalTaxDeducted = order.StoreOrders.Distinct(new StoreOrder.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);
                order.Total = order.ServiceFee + order.DeliveryFee + order.StoreOrders.Sum(x => x.Subtotal) + order.TipAmount + order.TotalTaxDeducted;
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }


        //public static void CalculateMobileOrderSettings(this MobileOrderViewModel MobileModel)
        //{
        //    try
        //    {
        //        DunkeySettings.LoadSettings();
        //        MobileModel.OrderSummary.Tax = MobileModel.Store.Distinct(new StoreOrder.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);
                
        //    }
        //    catch (Exception ex)
        //    {
        //        DunkeyDelivery.Utility.LogError(ex);
        //    }
        //}




        // order total and sub total
        public static void CalculateSubTotal(this Order order)
        {
            foreach (var storeOrder in order.StoreOrders)
            {
                storeOrder.CalculateSubTotal();
            }
            order.Subtotal = order.StoreOrders.Sum(x => x.Subtotal);
            order.TipAmount = (order.Subtotal) / 100 * 12;
        }

        public static void CalculateSubTotal(this StoreOrder storeOrder)
        {
            storeOrder.Subtotal = storeOrder.Order_Items.Sum(x => x.Price);
        }

        public static void CalculateTax(this Order order)
        {
            var groupData = order.StoreOrders.Distinct();
            foreach (var storeOrder in order.StoreOrders)
            {

            }
        }

        public static void CalculateTotal(this Order order)
        {
            try
            {

                order.CalculateSubTotal();
                order.ServiceFee = 0;
                order.DeliveryFee = DunkeySettings.DeliveryFee;
                order.TotalTaxDeducted = order.StoreOrders.Distinct(new StoreOrder.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);
                order.Total = order.ServiceFee + order.DeliveryFee + order.Subtotal + order.TipAmount + order.TotalTaxDeducted;
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

        public static void CalculateDeliveryDetails(this Order StoreOrder)
        {
            try
            {// check if min delivery time is in store object or noot
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    //var Store = StoreOrder.StoreOrders.FirstOrDefault().Store_Id;
                    var storeId = StoreOrder.StoreOrders.FirstOrDefault().Store_Id;
                    var Storee = ctx.Stores.FirstOrDefault(x => x.Id == storeId);
                    StoreOrder.MaxDeliveryTime = Storee.MinDeliveryTime;
                    // adding delivery date time to every store order time
                    //foreach (var Order in StoreOrder.StoreOrders)
                    //{
                    //    var time = ctx.Stores.FirstOrDefault(x => x.Id == Order.Store_Id).MinDeliveryTime;
                    //    if(time != 0)
                    //    {
                    //        Order.OrderDeliveryTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(time)).TimeOfDay;
                    //    }
                    //}




                    //foreach (var storeOrder in StoreOrder.StoreOrders)
                    //{
                    //    storeOrder.Total = storeOrder.Subtotal + Convert.ToDouble(ctx.Stores.FirstOrDefault(x => x.Id == storeOrder.Store_Id).MinDeliveryCharges);
                    //}
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }
    }
}