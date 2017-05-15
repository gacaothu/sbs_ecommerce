using System;
using System.Collections.Generic;

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
        public string Currency { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public Nullable<int> OrderStatus { get; set; }
    }

    public class OrderAdmin
    {
        public string OrderId { get; set; }
        public Payment Payment { get; set; }
        public string Coupon { get; set; }
        public string DeliveryStatus { get; set; }
        public double TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; }
        public string Currency { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public string OrderStatus { get; set; }
    }
}