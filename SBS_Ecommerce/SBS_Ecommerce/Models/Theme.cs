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
    
    public partial class Theme
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string PathView { get; set; }
        public string PathContent { get; set; }
        public bool Active { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public string Thumb { get; set; }
    }
}
