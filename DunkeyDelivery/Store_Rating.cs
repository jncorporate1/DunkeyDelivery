//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DunkeyDelivery
{
    using System;
    using System.Collections.Generic;
    
    public partial class Store_Rating
    {
        public int Id { get; set; }
        public short Rating { get; set; }
        public string Feedback { get; set; }
    
        public virtual Store Store { get; set; }
    }
}