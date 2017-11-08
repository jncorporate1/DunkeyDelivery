using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace DunkeyAPI.ViewModels
{
    public class ContentListViewModel
    {
      public ContentListViewModel()
        {
            Contributors = new List<Contributors>();
        }
        public List<Contributors> Contributors { get; set; }
        
    }
}