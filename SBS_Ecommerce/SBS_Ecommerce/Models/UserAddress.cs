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
    
    public partial class UserAddress
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string AddressType { get; set; }
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<int> ZipCode { get; set; }
        public string ReceiveAtWeekend { get; set; }
        public string Phone { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    }
}
