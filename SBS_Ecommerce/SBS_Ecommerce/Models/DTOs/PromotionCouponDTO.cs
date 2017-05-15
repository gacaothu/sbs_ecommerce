using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class PromotionCouponDTO
    {
        public List<PromotionCoupon> Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }

    public class PromotionCoupon
    {
        public int Usage_Count_Limit { get; set; }
        public int Product_ID { get; set; }
        public int Promotion_ID { get; set; }
        public string Promotion_Name { get; set; }
        public double Promotion_Price { get; set; }
        public string Promotion_Type { get; set; }
        public string Coupon_Code { get; set; }
        public int Priority { get; set; }
    }
}