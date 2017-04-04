using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Base
{
    public class Page
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool UsingLayout { get; set; }
    }
}