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
    
    public partial class ConfigHoliday
    {
        public int Id { get; set; }
        public string HolidayName { get; set; }
        public Nullable<System.DateTime> HolidayDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string Description { get; set; }
    }
}
