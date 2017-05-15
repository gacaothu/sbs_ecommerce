using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string AddressType { get; set; }
        [Required(ErrorMessage = "The Address is required")]
        public string Address { get; set; }
        [DisplayName("Ward")]
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        [Required(ErrorMessage = "The Country is required")]
        public string Country { get; set; }
        [DisplayName("State")]
        public string State { get; set; }
        [Required(ErrorMessage = "The ZipCode is required")]
        public Nullable<int> ZipCode { get; set; }
        [DisplayName("Receive at weekend")]
        public string ReceiveAtWeekend { get; set; }
        [Required(ErrorMessage = "The Phone is required")]
        public string Phone { get; set; }
        [DisplayName("Receiver FirstName")]
        [Required(ErrorMessage = "The Receiver first name is required")]
        public string ReceiverFirstName { get; set; }
        [DisplayName("Receiver LastName")]
        [Required(ErrorMessage = "The receiver last name is required")]
        public string ReceiverLastName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    }
}