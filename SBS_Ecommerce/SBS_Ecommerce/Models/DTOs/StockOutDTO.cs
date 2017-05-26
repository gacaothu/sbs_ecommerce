using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class StockOutDTO
    {
        public int Product_ID { get; set; }
        public int Product_Detail_ID { get; set; }
        public int Quantity { get; set; }
    }
    public class ListStockOutDTO
    {
        public ListStockOutDTO()
        {
            stks = new List<StockOutDTO>();
        }
        public List<StockOutDTO> stks { get; set; }
    }
    public class OutputStockOut
    {
        public int Return_Code { get; set; }
        public string Msg { get; set; }
    }
}