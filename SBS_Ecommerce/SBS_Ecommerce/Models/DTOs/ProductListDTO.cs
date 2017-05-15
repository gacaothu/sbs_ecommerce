using System;
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
        public string Brand_Name { get; set; }
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
        public int Star_Rating { get; set; }
        public float? Weight { get; set; }
        public float? Length { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public string Weight_UOM { get; set; }
        public bool Allowable_PreOrder { get; set; }
        public string Delivery_Noted { get; set; }        
        public int Product_Related_Count { get; set; }
        public int Product_Same_Cate_Count { get; set; }
        public int Products_Variant_Count { get; set; }
        public int Products_Attr_Count { get; set; }
        public int Products_Tag_Count { get; set; }
        public int Priority { get; set; }
        public List<Img> Imgs { get; set; }
        public List<Product> Products_Related { get; set; }
        public List<Product> Products_Same_Cate { get; set; }
        public List<ProductsVariant> Products_Variant { get; set; }
        public List<object> Products_Attr { get; set; }
        public List<object> Products_Tag { get; set; }
        public bool IsApplyCoupon { get; set; }
    }

    public class Img
    {
        public string Original_Img { get; set; }
        public string Small_Img { get; set; }
    }

    public class ProductsVariant
    {
        public int Product_ID { get; set; }
        public int Variant_ID { get; set; }
        public string Variant_Code { get; set; }
        public string Variant_Name { get; set; }
        public float Selling_Price { get; set; }
        public float Stocked_Quantity { get; set; }
        public List<object> Values_ID { get; set; }
    }
}