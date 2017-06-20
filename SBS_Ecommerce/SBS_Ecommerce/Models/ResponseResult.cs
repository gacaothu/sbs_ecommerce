using SBS_Ecommerce.Framework.Configurations;
using System.Collections.Generic;

namespace SBS_Ecommerce.Models
{
    public class ResponseResult
    {
        public int Status { get; set; } = SBSConstants.Success;
        public string Message { get; set; }
        public string Html { get; set; }
        public List<ErrorState> ErrorStates { get; set; }

        public class ErrorState
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}