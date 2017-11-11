using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        public ActionResult ManageOrders()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchOrders()
        {
            SearchOrderModel returnModel = new SearchOrderModel();

            returnModel.StoreOptions = Utility.GetStoresOptions(User, "All");

            returnModel.PaymentMethodOptions = Utility.GetPaymentMethodOptions("All");

            returnModel.PaymentStatusOptions = Utility.GetPaymentStatusOptions("All");

            returnModel.OrderStatusOptions = Utility.GetOrderStatusOptions("All");

            returnModel.SetSharedData(User);

            if (returnModel.Role == RoleTypes.SubAdmin)
            {
                returnModel.StoreId = (returnModel as BaseViewModel).StoreId;
            }

            return PartialView("_SearchOrders", returnModel);
        }

        public ActionResult SearchOrderResults(SearchOrderModel model)
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi
            ("api/Admin/SearchOrders", User, null, true, false, null,
            "OrderStatusId=" + (model.OrderStatusId == 0 ? null : model.OrderStatusId - 1),
            "PaymentStatusId=" + (model.PaymentStatusId == 0 ? null : model.PaymentStatusId - 1),
            "PaymentMethodId=" + (model.PaymentMethodId == 0 ? null : model.PaymentMethodId - 1),
            "StoreId=" + (model.StoreId == 0 ? null : model.StoreId),
            "StartDate=" + (model.StartDate == DateTime.MinValue ? DateTime.Now : model.StartDate),
            "EndDate=" + (model.EndDate == DateTime.MinValue ? DateTime.Now : model.EndDate)));

            SearchOrdersListViewModel returnModel = new SearchOrdersListViewModel();

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchOrdersListViewModel>();
            }

            returnModel.OrderStatusOptions = Utility.GetOrderStatusOptions();

            foreach (var order in returnModel.Orders)
            {
                order.OrderStatus = order.OrderStatus + 1;
            }
            returnModel.SetSharedData(User);
            return PartialView("_SearchOrderResults", returnModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrderStatus(List<ChangeOrderStatusModel> selectedOrders)
        {
            try
            {
                if (selectedOrders == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select an order to save");
                }

                foreach (var order in selectedOrders)
                {
                    order.Status = order.Status - 1;
                }

                ChangeOrderStatusListModel postModel = new ChangeOrderStatusListModel();
                postModel.Orders = selectedOrders;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeOrderStatus", User, postModel));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    SearchOrdersListViewModel model = new SearchOrdersListViewModel();
                    model.OrderStatusOptions = Utility.GetOrderStatusOptions();
                    foreach (var order in model.Orders)
                    {
                        order.OrderStatus = order.OrderStatus + 1;
                    }
                    model.SetSharedData(User);
                    return PartialView("_SearchOrderResults", model);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SaveOrderStatuses(SearchOrdersListViewModel model)
        //{
        //    try
        //    {
        //        var selectedOrders = model.Orders.Where(x => x.IsChecked).ToList();

        //        if (selectedOrders == null || selectedOrders.Count > 0)
        //        {
        //            ChangeOrderStatusListModel postModel = new ChangeOrderStatusListModel();

        //            foreach (var order in selectedOrders)
        //            {
        //                postModel.Orders.Add(new ChangeOrderStatusModel { OrderId = order.Id, StoreOrder_Id = order.StoreOrder_Id, Status = (order.OrderStatus - 1) });
        //            }

        //            var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeOrderStatus", User, postModel));

        //            if (apiResponse == null || apiResponse is Error)
        //                return new HttpStatusCodeResult(500, "Internal Server Error");
        //            else
        //            {
        //                model.OrderStatusOptions = Utility.GetOrderStatusOptions();
        //                foreach (var order in model.Orders)
        //                {
        //                    order.OrderStatus = order.OrderStatus + 1;
        //                }
        //                model.SetSharedData(User);
        //                return PartialView("_SearchOrderResults", model);
        //            }
        //        }
        //        else
        //            return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select an order to save");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        public ActionResult EditOrders()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }
        public ActionResult Edit()
        {
            return View();
        }
    }
}