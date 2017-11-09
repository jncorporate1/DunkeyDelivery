﻿using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class CategoryProductViewModel
    {
        public CategoryProductViewModel()
        {
            productslist = new List<productslist>();
        }
       public List<productslist> productslist { get; set; }
       public int TotalRecords { get; set; }
    }
    public class productslist
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public int? Category_id { get; set; }

        public int Store_id { get; set; }

        public string Size { get; set; }

        public void Dispose()
        {

        }
    }


    public class MedicationNames
    {
   
        public int Id { get; set; }

        public string Name { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

    }

    public class Medications
    {
        public Medications()
        {
            medications = new List<MedicationNames>();
        }
        public List<MedicationNames> medications;
    }



    //public class ProductsInCategory
    //{
    //    public ProductsInCategory()
    //    {
    //        categoryViewModel = new List<categoryViewModel>();
    //    }
    //    public List<categoryViewModel> categoryViewModel { get; set; }
        
    //}

    public class categoryViewModel
    {
        public categoryViewModel()
        {
            Products = new List<Product>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }



        public int? ParentCategoryId { get; set; }
        public List<Product> Products { get; set; }
    }


    

}