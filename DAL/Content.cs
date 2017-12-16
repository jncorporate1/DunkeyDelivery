using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Content
    {
        public int Id { get; set; }
        
        public string Heading { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public string ImageUrl { get; set; }

        public int Type { get; set; }

    }
}
