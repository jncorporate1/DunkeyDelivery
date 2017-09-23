using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class ShopCategoriesViewModel 
    {
        public List<Category> Categories { get; set; }

        //public ShopCategoriesViewModel(Category model)
        //{
        //    Name = model.Name;
           
        //    Status = model.Status;
        //    Description = model.Description;

        //}
        //public string Name { get; set; }
        
        //public short Status { get; set; }

        //public string Description { get; set; }

        //public void Dispose()
        //{
            
        //}
    }
}