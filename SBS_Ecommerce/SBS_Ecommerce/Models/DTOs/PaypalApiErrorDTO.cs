using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class PaypalApiErrorDTO
    {
        public string name { get; set; }
        public string message { get; set; }
        public List<Dictionary<string, string>> details { get; set; }
        public string debug_id { get; set; }
        public string information_link { get; set; }
    }
}