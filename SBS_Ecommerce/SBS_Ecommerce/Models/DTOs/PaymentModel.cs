//using SBS_Ecommerce.Models.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class PaymentModel
    {
        [Required]
        public int PaymentMethod { get; set; }
        [Required]
        public string CreditCardType { get; set; }
        [Required]
        public string CardholderName { get; set; }
        [CreditCard]
        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
        [Required]
        public int ExpireMonth { get; set; }
        [Required]
        public int ExpireYear { get; set; }
        [Required]
        public string CardCode { get; set; }
    }
}