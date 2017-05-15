using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class EmailAccountDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        public string Host { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        [Display(Name = "Username Email")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Password Email")]
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        [Display(Name = "Use default credentials")]
        public bool UseDefaultCredentials { get; set; }
    }
}