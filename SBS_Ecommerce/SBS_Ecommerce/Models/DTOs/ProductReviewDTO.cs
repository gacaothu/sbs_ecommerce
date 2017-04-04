using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }
        public int UId { get; set; }
        [DisplayName("Product ID")]
        public int ProId { get; set; }
        [DisplayName("Title comment")]
        public string Title { get; set; }
        [DisplayName("Content comment")]
        public string Content { get; set; }
        public Nullable<int> Rating { get; set; }
        [DisplayName("Create date")]
        public System.DateTime CreatedAt { get; set; }
    }
}