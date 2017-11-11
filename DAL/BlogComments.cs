using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        public User User { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual User User { get; set; }


    }
}
