using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class ForgetPasswordTokens
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }
    }
    public partial class UserForgetToken
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }
    }
}
