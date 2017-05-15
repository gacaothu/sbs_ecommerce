namespace SBS_Ecommerce.Models.DTOs
{
    public class WishlistDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UId { get; set; }
        public int ProId { get; set; }
        public string Status { get; set; }
        public Product Product { get; set; }
    }
}