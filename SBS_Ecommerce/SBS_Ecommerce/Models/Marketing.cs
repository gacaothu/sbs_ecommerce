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
    
    public partial class Marketing
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Marketing()
        {
            this.ScheduleEmails = new HashSet<ScheduleEmail>();
        }
    
        public int Id { get; set; }
        public string NameCampain { get; set; }
        public string ListEmail { get; set; }
        public string Schedule { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScheduleEmail> ScheduleEmails { get; set; }
    }
}
