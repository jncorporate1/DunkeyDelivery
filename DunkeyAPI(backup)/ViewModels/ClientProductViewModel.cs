using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class ClientProductViewModel
    {
        //public ClientProductViewModel(Product model)
        //{
        //    Id = model.Id;
        //    Name = model.Name;
        //    Price = model.Price;
        //    Description = model.Description;
        //    Image = model.Image;
        //    Status = model.Status;
        //    Category_Id = model.Category_Id;
        //    Store_Id = model.Store_Id;
        //}

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public short Status { get; set; }

        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual Store Store { get; set; }
    }
}