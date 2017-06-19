using SBS_Ecommerce.Models.Extension;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SBS_Ecommerce.Models.ViewModels
{
    public class DeliveryCompanyViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Country { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public partial class ConfigHolidayViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Holiday Name")]
        public string HolidayName { get; set; }
        [Required]
        [DisplayName("Holiday Date")]
        public DateTime? HolidayDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
    }

    public partial class DeliverySchedulerViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string TimeSlot { get; set; }
        [Required]
        public string FromHour { get; set; }
        [Required]
        public string ToHour { get; set; }
        [Required]
        public double Rate { get; set; }
        public int? PerSlot { get; set; }
        public bool IsWeekday { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public partial class WeightBasedViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public double Min { get; set; }
        [Required]
        //[CustomCompare(nameof(Min), ErrorMessage = "Max value must be larger than Min value")]
        public double Max { get; set; }
        [Required]
        public double Rate { get; set; }
        [Required]
        public string UnitOfMass { get; set; }
        [Required]
        public string DeliveryCompany { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }  
}