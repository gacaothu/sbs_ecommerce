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
    
    public partial class ScheduleEmail
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> Schedule { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public int MarketingID { get; set; }
        public Nullable<bool> Status { get; set; }
    
        public virtual Marketing Marketing { get; set; }
    }
}
