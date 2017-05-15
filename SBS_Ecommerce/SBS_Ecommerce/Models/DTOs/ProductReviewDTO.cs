using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class LstProductReviewDTO
    {
        public List<ProductReview> Items { get; set; }
        public int? Return_Code { get; set; }
        public int? Rows_Count { get; set; }
        public string Msg { get; set; }
    }
    public class ProductReview
    {
        [DisplayName("Product Review ID")]
        public int Product_Review_ID { get; set; }
        [DisplayName("Product ID")]
        public int Product_ID { get; set; }
        [DisplayName("Commentator ID")]
        public int? Commentator_ID { get; set; }
        [DisplayName("Title comment")]
        public string Title { get; set; }
        [DisplayName("Content comment")]
        public string Comment { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Rate")]
        public int? Rate { get; set; }
        [DisplayName("Status")]
        public string Record_Status { get; set; }

    }
}