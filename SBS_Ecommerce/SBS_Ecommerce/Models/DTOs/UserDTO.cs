using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public Nullable<int> PaymentId { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The Email is required")]
        public string Email { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "The FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The LastName is required")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string FacebookId { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
    }
}