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
    }
}