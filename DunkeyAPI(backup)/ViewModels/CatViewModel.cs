using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    //public class CatViewModel
    //{
    //    public List<CategoryViewModel> Categories { get; set; }
    //    public int TotalCategories { get; set; }


    //}

    //public class CategoryViewModel
    //{
    //    public CategoryViewModel()
    //    {
    //        SubCategories = new List<CategoryViewModel>();
    //    }

    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public int Store_Id { get; set; }
    //    public int ParentCategoryId { get; set; }
    //    public string parentName { get; set; } = "";
    //    public short Status { get; set; }
    //    public int? parentId { get; set; }
    //    public List<CategoryViewModel> SubCategories { get; set; }
    //}

    public class CategoriesViewModel
    {
        public CategoriesViewModel()
        {
            ParentCategories = new List<ParentCategory>();
            
    }
        public List<ParentCategory> ParentCategories { get; set; }
        public int TotalRecords { get; set; }

    }
    public class ParentCategory
    {
        public ParentCategory()
        {
            SubCategories = new List<SubCategories>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Store_Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string parentName { get; set; } = "";
        public short Status { get; set; }
        public int? parentId { get; set; }

        public List<SubCategories> SubCategories { get; set; }
    }
    public class SubCategories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Store_Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string parentName { get; set; } = "";
        public short Status { get; set; }
        public int? parentId { get; set; }
    }
}