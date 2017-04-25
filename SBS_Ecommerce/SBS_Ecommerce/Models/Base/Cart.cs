using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Base
{
    public class Order
    {
        public Product Product { get; set; }
        public int Count { get; set; }
    }

    public class Cart
    {
        public List<Order> LstOrder { get; set; }
        public double ShippingFee { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
    }
}