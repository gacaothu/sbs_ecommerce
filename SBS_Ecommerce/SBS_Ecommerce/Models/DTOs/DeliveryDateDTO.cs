﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.DTOs
{
    public class DeliveryDateDTO
    {
        public string DateTime { get; set; }
        public List<TimeSlot> TimeSlot { get; set; }
    }
    public class TimeSlot
    {
        public string NameTimeSlot { get; set; }
        public string Money { get; set; }
    }

    public class TabWeekDTO
    {
        public int CurrentWeek { get; set; }
        public string TabLabel { get; set; }
        public List<DeliveryDateDTO> DateRows { get; set; }
    }
}