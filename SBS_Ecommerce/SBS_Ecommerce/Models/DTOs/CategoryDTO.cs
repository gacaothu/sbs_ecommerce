using System.Collections.Generic;

namespace SBS_Ecommerce.Models.DTOs
{
    public class CategoryDTO
    {
        public List<Category> Items { get; set; }
        public string Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }
    public class Category
    {
        public int Category_ID { get; set; }
        public string Category_Name { get; set; }
        public string Img { get; set; }
        public List<Category> Items { get; set; }
    }
}