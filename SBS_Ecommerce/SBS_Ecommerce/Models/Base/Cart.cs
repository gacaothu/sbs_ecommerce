﻿using SBS_Ecommerce.Models.DTOs;
using System.Collections.Generic;

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
        public double Discount { get; set; }
        public string Counpon { get; set; }
        public string ShippingProvider { get; set; }
        public string DateTimeShipping { get; set; }
        public int? shippingAddressId { get; set; }
        public int? billingAddressId { get; set; }
    }
}