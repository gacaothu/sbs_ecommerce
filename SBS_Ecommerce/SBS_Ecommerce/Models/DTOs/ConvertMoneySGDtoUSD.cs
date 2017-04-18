using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ConvertMoneySGDtoUSD
    {
        public rates rates { get; set; }
    }
    public class rates
    {
        public double SGD { get; set; }
        public double USD { get; set; }
    }
}