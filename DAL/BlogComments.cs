using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class BlogComments
    {

        public int Id { get; set; }

        public string Comment { get; set; }

        public virtual User User { get; set; }


    }
}
