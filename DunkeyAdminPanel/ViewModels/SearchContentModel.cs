using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchContentModel
    {
        public string Name { get; set; }

        public short? is_investor { get; set; } = 0;

        public short? is_partner { get; set; } = 0;

        public short? is_press { get; set; } = 0;


    }
}