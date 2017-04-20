using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
namespace SBS_Ecommerce.Models.Extension
{
    public partial class SBS_Entities : DbContext
    {

        public IQueryable<Blog> Blogs
        {
            get
            {
                return Blogs.Where(b => b.CompanyId == 1);
            }
        }
        public IQueryable<AspNetRole> AspNetRoles
        {
            get
            {
                return AspNetRoles.Where(b => b.CompanyId == 1);
            }
        }
        public IQueryable<AspNetUserClaim> AspNetUserClaims
        {
            get
            {
                return AspNetUserClaims.Where(b => b.CompanyId == 1);
            }
        }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<BlogComment> BlogComments { get; set; }
        public virtual DbSet<ConfigChatting> ConfigChattings { get; set; }
        public virtual DbSet<ConfigPaypal> ConfigPaypals { get; set; }
        public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<EmailAccount> EmailAccounts { get; set; }
        public virtual DbSet<Marketing> Marketings { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<QueuedEmail> QueuedEmails { get; set; }
        public virtual DbSet<SBSLog> SBSLogs { get; set; }
        public virtual DbSet<ScheduledDelivery> ScheduledDeliveries { get; set; }
        public virtual DbSet<ScheduleEmail> ScheduleEmails { get; set; }
        public virtual DbSet<ShippingFee> ShippingFees { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }

        /// <summary>  
        /// Overriding Save Changes  
        /// </summary>  
        /// <returns></returns>  
        public override int SaveChanges()
        {
            var selectedEntityList = ChangeTracker.Entries()
                                    .Where(x =>
                                    (x.State == EntityState.Added || x.State == EntityState.Modified));


            if (selectedEntityList.Where(a => a.CurrentValues["CompanyId"] != null).Any())
            {
                foreach (var entity in selectedEntityList)
                {
                    //if (entity.Entity is AspNetRole)
                    if (entity.CurrentValues["CompanyId"] != null)
                    {
                        entity.CurrentValues["CompanyId"] = 1;
                    }

                }

            }

            return base.SaveChanges();
        }



        /// <summary>  
        /// Overriding Save Changes  
        /// </summary>  
        /// <returns></returns>  
        public override async Task<int> SaveChangesAsync()
        {
            var selectedEntityList = ChangeTracker.Entries()
                                    .Where(x =>
                                    (x.State == EntityState.Added || x.State == EntityState.Modified));
            if (selectedEntityList.Where(a => a.CurrentValues["CompanyId"] != null).Any())
            {
                foreach (var entity in selectedEntityList)
                {
                    if (entity.CurrentValues["CompanyId"] != null)
                    {
                        entity.CurrentValues["CompanyId"] = 1;
                    }

                }

            }
            return await base.SaveChangesAsync();
        }
    }
}