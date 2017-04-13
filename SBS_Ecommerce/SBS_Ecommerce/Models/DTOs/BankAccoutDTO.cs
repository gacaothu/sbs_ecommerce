using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class BankAcountDTO
    {
        public List<BankAcount> Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }
    public class BankAcount
    {
        public int Bank_Account_ID { get; set; }
        public int Bank_ID { get; set; }
        public string Account_Code { get; set; }
        public string Account_Name { get; set; }
        public string Description { get; set; }
        public string Branch { get; set; }
        public string Company_ID { get; set; }
    }
}