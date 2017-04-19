using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ConfigPaypalDTO
    {
        public int Id { get; set; }
        public string Mode { get; set; }
        public Nullable<int> ConnectionTimeout { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}