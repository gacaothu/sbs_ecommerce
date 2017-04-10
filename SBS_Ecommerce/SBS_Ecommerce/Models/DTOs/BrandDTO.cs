using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class BrandDTO
    {
            public List<Brand>  Items { get; set; }
            public int Return_Code { get; set; }
            public int Rows_Count { get; set; }
            public string Msg { get; set; }

    }
    public class Brand
    {
        public int Brand_ID { get; set; }
        public string Brand_Name { get; set; }
    }
}