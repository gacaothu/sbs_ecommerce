using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ForgotPasswordDTO
    {
        public string Msg { get; set; }
        public int Return_Code { get; set; }
        public string Field { get; set; }
    }
}