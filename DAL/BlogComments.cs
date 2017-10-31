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

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime PostedDate { get; set; }
        
        public int Post_Id { get; set; }

        public int User_Id { get; set; }
        [JsonIgnore]
        public virtual BlogPosts Post { get; set; }
        
        public virtual User User { get; set; }


    }
}
