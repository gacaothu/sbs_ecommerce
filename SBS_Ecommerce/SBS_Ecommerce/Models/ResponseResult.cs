using SBS_Ecommerce.Framework.Configurations;

namespace SBS_Ecommerce.Models
{
    public class ResponseResult
    {
        public int Status { get; set; } = SBSConstants.Success;
        public string Message { get; set; }
        public string Html { get; set; }
    }
}