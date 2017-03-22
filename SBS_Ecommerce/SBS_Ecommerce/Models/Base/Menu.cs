using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Base
{
    public class Menu
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public List<ChildMenu> LstChildMenu { get; set; }
    }
    public class ChildMenu
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
    }
}