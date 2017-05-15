using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ConfigShippingDTO
    {
        public List<ConfigShipping> ListConfigShipping { get; set; }
        public List<WeightBased> ListWeightBased { get; set; }
    }
}