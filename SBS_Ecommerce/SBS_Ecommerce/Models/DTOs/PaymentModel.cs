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

        [Required]
        [Display(Name = "Bank")]
        public string Bank { get; set; }

        public string BankName { get; set; }

        [Required]
        [Display(Name = "Bank account")]
        public string BankAccount { get; set; }

        public string BankAccountName { get; set; }

        [Required]
        [Display(Name = "Payslip")]
        public string PaySlip { get; set; }

        [Required]
        [Display(Name = "Date Time Transfer")]
        public DateTime DateTimeTransfer { get; set; }

        public HttpPostedFileBase File { get; set; }

        public string CurrencyCode { get; set; }
        public string CountryCode { get; internal set; }
        [Required]
        public float MoneyTranster { get; set; }
    }
}