using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class BankDTO
    {
            public List<Bank>  Items { get; set; }
            public int Return_Code { get; set; }
            public int Rows_Count { get; set; }
            public string Msg { get; set; }

    }
    public class Bank
    {
        public int Bank_ID { get; set; }
        public string Bank_Code { get; set; }
        public string Bank_Name { get; set; }
        public string Bank_Description { get; set; }
    }
}