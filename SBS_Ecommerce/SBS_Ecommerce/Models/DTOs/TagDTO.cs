using System.Collections.Generic;

namespace SBS_Ecommerce.Models.DTOs
{
    public class TagDTO
    {
        public List<Tag> Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }

    public class Tag
    {
        public int Tag_ID { get; set; }
        public string Tag_Name { get; set; }
    }
}