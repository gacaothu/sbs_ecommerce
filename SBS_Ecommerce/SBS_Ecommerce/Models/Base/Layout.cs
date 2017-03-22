using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Base
{
    public class Layout
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public bool CanEdit { get; set; }
    }
}