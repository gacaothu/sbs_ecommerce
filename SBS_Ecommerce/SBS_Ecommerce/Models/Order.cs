//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SBS_Ecommerce.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public string OderId { get; set; }
        public int PaymentId { get; set; }
        public Nullable<int> CouponId { get; set; }
        public string DeliveryStatus { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public Nullable<int> UId { get; set; }
        public string OrderType { get; set; }
    }
}
