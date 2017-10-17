using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models.Admin
{
    public class StoresViewModel
    {
        public int Count { get; set; }
        public IEnumerable<Store> Stores { get; set; }
    }
}