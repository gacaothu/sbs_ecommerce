using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ConfigPaypalDTO
    {
        public int Id { get; set; }
        [Required]
        public string Mode { get; set; }
        [Required]
        public Nullable<int> ConnectionTimeout { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}