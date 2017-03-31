using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ShippingAddressDTO
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string AddressType { get; set; }
        [Required(ErrorMessage = "The Address is required")]
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        [Required(ErrorMessage = "The City is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "The Country is required")]
        public string Country { get; set; }
        public string State { get; set; }
        public Nullable<int> ZipCode { get; set; }
        public string ReceiveAtWeekend { get; set; }
        [Required(ErrorMessage = "The Phone is required")]
        public string Phone { get; set; }
        [DisplayName("FirstName")]
        [Required(ErrorMessage = "The FirstName is required")]
        public string ReceiverFirstName { get; set; }
        [DisplayName("LastName")]
        [Required(ErrorMessage = "The LastName is required")]
        public string ReceiverLastName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    }
}