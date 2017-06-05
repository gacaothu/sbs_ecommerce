﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SBS_Entities : DbContext
    {
        public SBS_Entities()
            : base("name=SBS_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogComment> BlogComments { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<ConfigChatting> ConfigChattings { get; set; }
        public virtual DbSet<ConfigHoliday> ConfigHolidays { get; set; }
        public virtual DbSet<ConfigMailChimp> ConfigMailChimps { get; set; }
        public virtual DbSet<ConfigPaypal> ConfigPaypals { get; set; }
        public virtual DbSet<ConfigShipping> ConfigShippings { get; set; }
        public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<DeliveryCompany> DeliveryCompanies { get; set; }
        public virtual DbSet<DeliveryScheduler> DeliverySchedulers { get; set; }
        public virtual DbSet<EmailAccount> EmailAccounts { get; set; }
        public virtual DbSet<LocalPickup> LocalPickups { get; set; }
        public virtual DbSet<Marketing> Marketings { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<QueuedEmail> QueuedEmails { get; set; }
        public virtual DbSet<SBSLog> SBSLogs { get; set; }
        public virtual DbSet<ScheduleEmail> ScheduleEmails { get; set; }
        public virtual DbSet<SEO> SEOs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Theme> Themes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<WeightBased> WeightBaseds { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<ConfigDeliveryDay> ConfigDeliveryDays { get; set; }
        public virtual DbSet<ConfigChildMenu> ConfigChildMenus { get; set; }
        public virtual DbSet<ConfigMenu> ConfigMenus { get; set; }
    }
}
