using AutoMapper;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        }

        private void ConfigureShippingAddress()
        {
            Mapper.CreateMap<UserAddress, ShippingAddressDTO>();
            Mapper.CreateMap<ShippingAddressDTO, UserAddress>();
        }
    } 
}