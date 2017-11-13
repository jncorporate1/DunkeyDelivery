using System.ComponentModel.DataAnnotations;

namespace BasketWebPanel.BindingModels
{
    public class ProductBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        public string Weight { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid weight")]
        public double? WeightInGrams { get; set; }

        public double? WeightInKiloGrams { get; set; }

        public int WeightUnit { get; set; }

        [Required]
        public string Image { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select category")]
        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public virtual CategoryBindingModel Category { get; set; }

        public virtual StoreBindingModel Store { get; set; }


    }
}