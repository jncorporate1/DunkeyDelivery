using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PharmacyRequest_Products
    {
        public int Id { get; set; }
        public int Product_Id { get; set; }
        public int PharmacyRequest_Id { get; set; }
        public Product Product { get; set; }
        public PharmacyRequest PharmacyRequest { get; set; }
    }
}
