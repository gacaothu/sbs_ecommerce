using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class PaymentModel
    {
        public int PaymentMethod { get; set; }
        public string CreditCardType { get; set; }
        public string CardholderName { get; set; }
        public string CardNumber { get; set; }
        public int ExpireMonth { get; set; }
        public int ExpireYear { get; set; }
        public string CardCode { get; set; }
    }
}