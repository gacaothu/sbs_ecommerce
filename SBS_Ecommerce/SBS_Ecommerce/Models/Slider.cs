//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SBS_Ecommerce.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Slider
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }
        public int ThemeID { get; set; }
    
        public virtual Theme Theme { get; set; }
    }
}
