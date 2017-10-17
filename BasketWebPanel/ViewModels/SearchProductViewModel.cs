using Newtonsoft.Json;

namespace BasketWebPanel.ViewModels
{
    public class SearchProductViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public double Price { get; set; }
        
        public string Description { get; set; }

        public string Weight { get; set; }

       
        public string Image { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public string StoreName { get; set; }

        public string CategoryName { get; set; }

        //Used in Add Package
        public int Qty { get; set; } = 1;

        public bool IsChecked { get; set; }

    }
}