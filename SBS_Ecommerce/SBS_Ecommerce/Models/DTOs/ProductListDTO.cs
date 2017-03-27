using System.Collections.Generic;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ProductListDTO
    {
        public List<Product> Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }

    public class Product
    {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; }
        public string Product_Code { get; set; }
        public string Description { get; set; }
        public int? Company_ID { get; set; }
        public int? Category_ID { get; set; }
        public int? Brand_ID { get; set; }
        public int? Branch_ID { get; set; }
        public int? Promotion_ID { get; set; }
        public string Promotion_Name { get; set; }
        public float? Promotion_Price { get; set; }
        public bool? Has_Variants { get; set; }
        public bool? Has_Image { get; set; }
        public bool? Has_Kitset { get; set; }
        public float Selling_Price { get; set; }
        public float? Stocked_Quantity { get; set; }
        public bool? Track_Stock { get; set; }
        public string Original_Img { get; set; }
        public string Small_Img { get; set; }
        public List<Product> Products_Related { get; set; }
    }
}