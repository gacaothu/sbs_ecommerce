using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class EmailNotificationDTO
    {
        public List<OrderDetailDTO> ListOrderEmail { get; set; }
        public User User { get; set; }
        public Order Order { get; set; }
        public string OrderStatus { get; set; }
        public Company Company { get; set; }
    }
}