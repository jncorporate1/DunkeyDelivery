namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public partial  class BlogPosts
    {

        public BlogPosts()
        {
            BlogComments = new HashSet<BlogComments>();
        }
        public int Id { get; set; }


        [Required]
        public string Title { get; set; }

        public string CategoryType { get; set; }

        public string ImageUrl { get; set; }

        public int Admin_ID { get; set; }

        [Required]
        public string Description { get; set; }
        
        public DateTime DateOfPosting { get; set; }

        public short is_popular { get; set; }

        public short? is_deleted { get; set; } = 0;

        //public virtual User User { get; set; }

        [NotMapped]
        public int TotalComments { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlogComments> BlogComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual Admin Admin { get; set; }




    }
}
