using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class DeliveryController : Controller
    {
        // GET: Dashboard/Delivery
        public ActionResult Index()
        {
            try
            {
                SearchOrdersListViewModel model = new SearchOrdersListViewModel();

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetReadyForDeliveryOrders", User, GetRequest: true));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    model = apiResponse.GetValue("Result").ToObject<SearchOrdersListViewModel>();

                    foreach (var order in model.Orders)
                    {
                        order.DeliveryMen.Insert(0, new DelivererOptionsViewModel { Id = 0, Name = "None" });
                        IEnumerable<SelectListItem> selectList = from deliverer in order.DeliveryMen
                                                                 select new SelectListItem
                                                                 {
                                                                     Selected = (deliverer.Id == order.DeliveryManId),
                                                                     Text = deliverer.Name,
                                                                     Value = deliverer.Id.ToString()
                                                                 };
                        order.DeliveryMenOptions = new SelectList(selectList, order.DeliveryManId);
                        //order.DeliveryMenOptions.SelectedValue = order.DeliveryManId;
                    }
                }
                model.Orders = model.Orders.Distinct(new SearchOrdersViewModel.Comparer()).ToList();
                model.SetSharedData(User);
                return View(model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        [HttpPost]
        public ActionResult Index(SearchOrdersListViewModel model)
        {
            try
            {
                if (model.Orders.Any(x => x.IsChecked))
                {
                    SearchOrdersListViewModel postModel = new SearchOrdersListViewModel();
                    postModel.Orders = model.Orders.Where(x => x.IsChecked && x.DeliveryManId > 0).ToList();

                    var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/AssignOrdersToDeliverer", User, postModel));

                    if (apiResponse == null || apiResponse is Error)
                        return new HttpStatusCodeResult(500, "Internal Server Error");
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select an order to save");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }
    }
}