using System.Collections.Generic;

namespace SBS_Ecommerce.Models.DTOs
{
    public class PriceRangeDTO
    {
        public List<PriceRange> Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }

    public class PriceRange
    {
        public int Range_ID { get; set; }
        public string Range_Name { get; set; }
        public object Range_Type { get; set; }
        public float Start_From { get; set; }
        public float End_To { get; set; }
    }
}