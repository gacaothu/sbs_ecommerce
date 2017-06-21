using AutoMapper;
using SBS_Ecommerce.Models.DTOs;
using SBS_Ecommerce.Models.ViewModels;
using System;

namespace SBS_Ecommerce.Models.Extension
{
    public class AutoMapperConfiguration
    {
        #region Singleton

        private readonly static Lazy<AutoMapperConfiguration> LazyInstance = new Lazy<AutoMapperConfiguration>(() => new AutoMapperConfiguration());

        public static AutoMapperConfiguration Instance
        {
            get { return LazyInstance.Value; }
        }

        #endregion

        public void Configure()
        {
            ConfigureShippingAddress();
            ConfigureUser();
            //ConfigureProductReview();
            ConfigureOrder();
            ConfigureOrderDetail();
            ConfigurePayPal();
            ConfigureEmail();
            ConfigureHoliday();
            ConfigDeliveryScheduler();
            ConfigWeightBased();
            ConfigDeliveryCompany();
            ConfigPage();
        }

        private void ConfigPage()
        {
            Mapper.CreateMap<Page, PageViewModel>();
            Mapper.CreateMap<PageViewModel, Page>();
        }

        private void ConfigDeliveryCompany()
        {
            Mapper.CreateMap<DeliveryCompany, DeliveryCompanyViewModel>();
            Mapper.CreateMap<DeliveryCompanyViewModel, DeliveryCompany>();
        }

        private void ConfigWeightBased()
        {
            Mapper.CreateMap<WeightBased, WeightBasedViewModel>();
            Mapper.CreateMap<WeightBasedViewModel, WeightBased>();
        }

        private void ConfigDeliveryScheduler()
        {
            Mapper.CreateMap<DeliveryScheduler, DeliverySchedulerViewModel>();
            Mapper.CreateMap<DeliverySchedulerViewModel, DeliveryScheduler>();
        }

        private void ConfigureShippingAddress()
        {
            Mapper.CreateMap<UserAddress, AddressDTO>();
            Mapper.CreateMap<AddressDTO, UserAddress>();
        }
        private void ConfigureUser()
        {
            Mapper.CreateMap<User, UserDTO>();
            Mapper.CreateMap<UserDTO, User>();
        }
        //private void ConfigureProductReview()
        //{
        //    Mapper.CreateMap<ProductReview, ProductReviewDTO>();
        //    Mapper.CreateMap<ProductReviewDTO, ProductReview>();
        //}
        private void ConfigureOrder()
        {
            Mapper.CreateMap<Order, OrderDTO>();
            Mapper.CreateMap<OrderDTO, Order>();
        }
        private void ConfigureOrderDetail()
        {
            Mapper.CreateMap<OrderDetail, OrderDetailDTO>();
            Mapper.CreateMap<OrderDetailDTO, OrderDetail>();
        }
        private void ConfigureHoliday()
        {
            Mapper.CreateMap<ConfigHoliday, ConfigHolidayViewModel>();
            Mapper.CreateMap<ConfigHolidayViewModel, ConfigHoliday>();
        }
        private void ConfigurePayPal()
        {
            Mapper.CreateMap<ConfigPaypal, ConfigPaypalDTO>();
            Mapper.CreateMap<ConfigPaypalDTO, ConfigPaypal>();
        }
        private void ConfigureEmail()
        {
            Mapper.CreateMap<EmailAccount, EmailAccountDTO>();
            Mapper.CreateMap<EmailAccountDTO, EmailAccount>();
        }

    }
}