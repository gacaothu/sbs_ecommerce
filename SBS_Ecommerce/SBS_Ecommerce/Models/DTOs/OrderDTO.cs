using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class OrderDTO
    {
        public string OrderId { get; set; }
        public int PaymentId { get; set; }
        public Nullable<int> CouponId { get; set; }
        public string DeliveryStatus { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public Nullable<int> UId { get; set; }
        public string PaymentName { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}