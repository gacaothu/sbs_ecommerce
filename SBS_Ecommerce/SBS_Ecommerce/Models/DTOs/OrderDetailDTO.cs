using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int ProId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public string OrderType { get; set; }
        public int ShippingStatus { get; set; }
    }
}