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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public string OrderId { get; set; }
        public int CompanyId { get; set; }
        public int PaymentId { get; set; }
        public Nullable<int> UId { get; set; }
        public string Coupon { get; set; }
        public Nullable<int> ShippingStatus { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public Nullable<int> ShippingAddressId { get; set; }
        public Nullable<int> BillingAddressId { get; set; }
        public Nullable<double> ShippingFee { get; set; }
        public string Payslip { get; set; }
        public Nullable<double> Tax { get; set; }
        public Nullable<double> Discount { get; set; }
        public string ShippingProvider { get; set; }
        public string TimeShipping { get; set; }
        public string AccountCode { get; set; }
        public Nullable<double> MoneyTransfer { get; set; }
        public string AccountName { get; set; }
        public Nullable<int> CountProduct { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public string Currency { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    
        public virtual Payment Payment { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
