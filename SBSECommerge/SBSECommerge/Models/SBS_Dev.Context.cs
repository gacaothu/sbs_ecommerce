﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SBSECommerge.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SBS_DevEntities : DbContext
    {
        public SBS_DevEntities()
            : base("name=SBS_DevEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogComment> BlogComments { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<ProductComment> ProductComments { get; set; }
        public virtual DbSet<ProductRating> ProductRatings { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<ScheduledDelivery> ScheduledDeliveries { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
