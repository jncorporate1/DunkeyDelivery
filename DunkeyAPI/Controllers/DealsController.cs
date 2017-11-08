using DAL;
using DunkeyAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using DunkeyAPI.Models.Admin;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Deals")]
    public class DealsController : ApiController
    {

        [HttpGet]
        [Route("GetOfferPackage")]
        public IHttpActionResult GetOfferPackage()
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                var res = ctx.Offer_Packages.Include("Offer_Product").ToList();

                OfferViewModel model = new OfferViewModel { Offer_Packages = ctx.Offer_Packages.Include(x => x.Package).Include(x => x.Offer.Store).ToList(), Offer_Products = ctx.Offer_Products.Include(x => x.Product).Include(x => x.Product.Store).ToList() };

                CustomResponse<OfferViewModel> response = new CustomResponse<OfferViewModel>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = model

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [HttpGet]
        [Route("GetOfferProductsAndPackages")]
        public IHttpActionResult GetOfferProductsAndPackages(int OfferId, int StoreId)
        {
            try
            {
                #region InlineQueries

                var productsQuery = @"SELECT
  COALESCE(t2.Product_Id, t1.Product_Id) AS Id,
  COALESCE(t2.Product_Id, t1.Product_Id) AS Product_Id,
  COALESCE(t2.Name, t1.Name) AS Name,
  COALESCE(t2.Price, t1.Price) AS Price,
  COALESCE(t2.StoreName, t1.StoreName) AS StoreName,
  COALESCE(t2.CategoryName, t1.CategoryName) AS CategoryName,
  COALESCE(t2.ImageUrl, t1.ImageUrl) AS ImageUrl,
  COALESCE(t2.OfferProductId, 0) AS OfferProductId,
  COALESCE(t2.Offer_Id, 0) AS Offer_Id,
  COALESCE(t2.IsChecked, CAST(0 AS bit)) AS IsChecked
FROM (SELECT
  Products.Id AS Product_Id,
  Products.Name AS Name,
  Products.Price AS Price,
  Stores.BusinessName AS StoreName,
  Categories.Name AS CategoryName,
  Products.Image AS ImageUrl,
  1 AS Qty,
  0 AS OfferProductId,
  0 AS Package_Id,
  CAST(1 AS bit) AS IsChecked
FROM Products
JOIN Categories
  ON Products.Category_Id = Categories.Id
JOIN Stores
  ON Products.Store_Id = Stores.Id
WHERE 
Stores.Id = " + StoreId + @" 
AND Products.IsDeleted = 0
AND Categories.IsDeleted = 0
AND Stores.IsDeleted = 0) t1
LEFT JOIN (SELECT
  Products.Id AS Product_Id,
  Products.Name AS Name,
  Offer_Products.DiscountedPrice AS Price,
  Stores.BusinessName AS StoreName,
  Categories.Name AS CategoryName,
  Products.Image AS ImageUrl,
  Offer_Products.Id AS OfferProductId,
  Offer_Products.Offer_Id,
  CAST(1 AS bit) AS IsChecked
FROM Offers
JOIN Offer_Products
  ON Offer_Products.Offer_Id = Offers.Id
JOIN Products
  ON Products.Id = Offer_Products.Product_Id
JOIN Stores
  ON Stores.Id = Products.Store_Id
JOIN Categories
  ON Categories.Id = Products.Category_Id
WHERE Offers.Id = " + OfferId + @" 
AND Offers.IsDeleted = 0
And Offer_Products.IsDeleted = 0
AND Products.IsDeleted = 0
AND Categories.IsDeleted = 0) t2
  ON t1.Product_Id = t2.Product_Id
order by IsChecked desc

";

                var packageQuery = @"SELECT
  COALESCE(t2.Package_Id, t1.Package_Id) AS Id,
  COALESCE(t2.Package_Id, t1.Package_Id) AS Package_Id,
  COALESCE(t2.Name, t1.Name) AS Name,
  COALESCE(t2.Price, t1.Price) AS Price,
  COALESCE(t2.StoreName, t1.StoreName) AS StoreName,
  COALESCE(t2.ImageUrl, t1.ImageUrl) AS ImageUrl,
  COALESCE(t2.OfferPackageId, 0) AS OfferPackageId,
  COALESCE(t2.Offer_Id, 0) AS Offer_Id,
  COALESCE(t2.IsChecked, CAST(0 AS bit)) AS IsChecked
FROM (SELECT
  Packages.Id AS Package_Id,
  Packages.Name AS Name,
  Packages.Price AS Price,
  Stores.BusinessName AS StoreName,
  Packages.ImageUrl AS ImageUrl,
  0 AS OfferPackageId,
  CAST(1 AS bit) AS IsChecked
FROM Packages
JOIN Stores
  ON Packages.Store_Id = Stores.Id
WHERE Stores.Id = " + StoreId + @"
AND Stores.IsDeleted = 0) t1
LEFT JOIN (SELECT
  Packages.Id AS Package_Id,
  Packages.Name AS Name,
  Offer_Packages.DiscountedPrice AS Price,
  Stores.BusinessName AS StoreName,
  Packages.ImageUrl AS ImageUrl,
  Offer_Packages.Id AS OfferPackageId,
  Offer_Packages.Offer_Id,
  CAST(1 AS bit) AS IsChecked
FROM Offers
JOIN Offer_Packages
  ON Offer_Packages.Offer_Id = Offers.Id
JOIN Packages
  ON Packages.id = Offer_Packages.Package_Id
JOIN Stores
  ON Stores.Id = Offers.Store_Id
WHERE Offers.Id = " + OfferId + @"
AND Offers.IsDeleted = 0
And Offer_Packages.IsDeleted = 0
) t2
  ON t1.Package_Id = t2.Package_Id
ORDER BY IsChecked DESC
";

                #endregion



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    OfferVM model = new OfferVM
                    {
                        Packages = ctx.Database.SqlQuery<SearchPackageViewModel>(packageQuery).ToList(),
                        Products = ctx.Database.SqlQuery<SearchProductViewModel>(productsQuery).ToList()
                    };

                    CustomResponse<OfferVM> response = new CustomResponse<OfferVM>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
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
