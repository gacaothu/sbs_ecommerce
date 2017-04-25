using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class TaxProductDTO
    {
        public TaxProduct Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }

    public class TaxProduct
    {
        public int Tax_ID { get; set; }
        public int Tax_Percen { get; set; }
        public string Tax_Type { get; set; }
    }
}