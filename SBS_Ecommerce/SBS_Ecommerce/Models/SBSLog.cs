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
    
    public partial class SBSLog
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string ErrorMessage { get; set; }
        public string FromClass { get; set; }
        public string FromMethod { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
