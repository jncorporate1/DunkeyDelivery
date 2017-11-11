using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using DunkeyAPI.ViewModels;
using static DunkeyAPI.Utility.Global;
using DunkeyAPI.Utility;
using System.Data.Entity;

namespace DunkeyAPI.ExtensionMethods
{
    public static class ExtensionMethods
    {
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
                throw;
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
                throw;
            }
        }

        public static void SetOrderItem(this Order_Items orderItem, CartItemViewModel model, DunkeyContext ctx)
        {
            try
            {
                model.ItemType = 0;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        public static void SetOrderDetails(this Order order, OrderViewModel model)
        {
            try
            {
                order.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
                order.OrderDateTime = DateTime.Now;
                order.Status = (int)OrderStatuses.Initiated;
                order.DeliveryTime_From = model.DeliveryDateTime_From;
                order.DeliveryTime_To = model.DeliveryDateTime_To;
                order.PaymentMethod = model.PaymentMethodType;
                order.User_ID = model.UserId;

                //Set Delivery Details
                order.DeliveryDetails_FirstName = model.DeliveryDetails.FirstName;
                order.DeliveryDetails_LastName = model.DeliveryDetails.LastName;
                order.DeliveryDetails_Phone = model.DeliveryDetails.Phone;
                order.DeliveryDetails_ZipCode = model.DeliveryDetails.ZipCode;
                order.DeliveryDetails_Email = model.DeliveryDetails.Email;
                order.DeliveryDetails_City = model.DeliveryDetails.City;
                order.DeliveryDetails_Address = model.DeliveryDetails.Address;
                order.DeliveryDetails_AddtionalNote = model.DeliveryDetails.AdditionalNote;

                order.TipAmount = model.TipAmount;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void MakeOrder(this Order order, OrderViewModel model, DunkeyContext ctx)
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

                order.CalculateSubTotal();
                order.CalculateTotal();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CalculateSubTotal(this StoreOrder storeOrder)
        {
            storeOrder.Subtotal = storeOrder.Order_Items.Sum(x => x.Price);
        }

        public static void CalculateSubTotal(this Order order)
        {
            foreach (var storeOrder in order.StoreOrders)
            {
                storeOrder.CalculateSubTotal();
            }
            order.Subtotal = order.StoreOrders.Sum(x => x.Subtotal);
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
            catch (Exception)
            {
                throw;
            }
        }

    }
}