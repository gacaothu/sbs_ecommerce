using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Base
{
    public class Theme
    {
       public int ID { get; set; }
       public string Name { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
    }
}