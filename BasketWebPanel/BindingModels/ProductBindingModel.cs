using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasketWebPanel.BindingModels
{
    public class ProductBindingModel
    {
        public ProductBindingModel()
        {
            ProductSizes = new List<ProductSizeBindingModel>();
        }
       
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //[Required(ErrorMessage = "This field is required")]
        //[Range(1, 10000, ErrorMessage = "Please enter a valid price")]
        //[RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        public string Weight { get; set; }

        public double? WeightInGrams { get; set; }

        public double? WeightInKiloGrams { get; set; }

        public int WeightUnit { get; set; }

        public string Image { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select category")]
        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public int TypeOfProduct { get; set; } = 2;// 0 for wine liquor 1 for beer and 2 for nothing

        public List<ProductSizeBindingModel> ProductSizes { get; set; }

        public virtual CategoryBindingModel Category { get; set; }

        public virtual StoreBindingModel Store { get; set; }


    }

    public class ProductSizeBindingModel
    {

        public string Unit { get; set; }


        [Required(ErrorMessage = "This field is required")]
        public string Size { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public double Price { get; set; }

        public string NetWeight { get; set; }

        public int? SizeUnit_Id { get; set; }

        public int Product_Id { get; set; }

        public int TypeID { get; set; }

        public SizeUnitViewModel SizesUnits { get; set; }

    }
    public class SizeUnitViewModel
    {
        public int Id { get; set; }

        public string Unit { get; set; }

        public bool IsDeleted { get; set; }
    }




    public class ProductSizeViewModel
    {
        public int Id { get; set; }

        public string Unit { get; set; }

        public double Weight { get; set; }

        public string Size { get; set; }

        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public int Product_Id { get; set; }
    }
}