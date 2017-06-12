using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SBS_Ecommerce.Framework;

namespace SBS_Ecommerce.Models
{
    public partial class SBS_Entities : DbContext
    {
        private int CompanyId  = SBSCommon.Instance.GetCompany().Company_ID;

        public IQueryable<Block> GetBlocks { get { return Blocks.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Blog> GetBlogs { get { return Blogs.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<BlogComment> GetBlogComments { get { return BlogComments.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigChatting> GetConfigChattings { get { return ConfigChattings.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigPaypal> GetConfigPaypals { get { return ConfigPaypals.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigSystem> GetConfigSystems { get { return ConfigSystems.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Coupon> GetCoupons { get { return Coupons.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<EmailAccount> GetEmailAccounts { get { return EmailAccounts.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Marketing> GetMarketings { get { return Marketings.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Order> GetOrders { get { return Orders.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<OrderDetail> GetOrderDetails { get { return OrderDetails.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Payment> GetPayments { get { return Payments.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<QueuedEmail> GetQueuedEmails { get { return QueuedEmails.Where(m => m.CompanyId == CompanyId); }  }
        public IQueryable<SBSLog> GetSBSLogs { get { return SBSLogs.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ScheduleEmail> GetScheduleEmails { get { return ScheduleEmails.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigShipping> GetConfigShippings { get { return ConfigShippings.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<User> GetUsers { get { return Users; } }
        public IQueryable<UserAddress> GetUserAddresses { get { return UserAddresses.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Wishlist> GetWishlists { get { return Wishlists.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigDeliveryDay> GetConfigDeliveryDays { get { return ConfigDeliveryDays.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigHoliday> GetConfigHolidays { get { return ConfigHolidays.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<WeightBased> GetWeightBaseds { get { return WeightBaseds.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<LocalPickup> GetLocalPickups { get { return LocalPickups.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<DeliveryCompany> GetDeliveryCompanies { get { return DeliveryCompanies.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<DeliveryScheduler> GetDeliverySchedulers { get { return DeliverySchedulers.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<SEO> GetSEOs { get { return SEOs.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigMenu> GetConfigMenus { get { return ConfigMenus.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigChildMenu> GetConfigChildMenus { get { return ConfigChildMenus.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Page> GetPages { get { return Pages.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<Theme> GetThemes { get { return Themes.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigLayout> GetConfigLayouts { get { return ConfigLayouts.Where(m => m.CompanyId == CompanyId); } }
        public IQueryable<ConfigSlider> GetConfigSliders { get { return ConfigSliders.Where(m => m.CompanyId == CompanyId); } }

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
                        entity.CurrentValues["CompanyId"] = CompanyId;
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
                        entity.CurrentValues["CompanyId"] = CompanyId;
                    }

                }

            }
            return await base.SaveChangesAsync();
        }
    }
}