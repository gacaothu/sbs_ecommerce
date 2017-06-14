using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class DeliveryDateUtil
    {
        public DeliveryDateUtil()
        {

        }

        public List<TabWeekDTO> GetTabWeek(int currentPage)
        {
            int currentYear = DateTime.Today.Year;
            CultureInfo ci = CultureInfo.CurrentCulture;
            int currentWeek = GetNumOfWeek(DateTime.Now);
            if (currentPage > 1)
            {
                currentWeek += (currentPage - 1) * 3;
            }
            List<TabWeekDTO> weekDTO = new List<TabWeekDTO>();
            for (int i = 0; i < 3; i++)
            {
                DateTime startDOW = FirstDateOfWeek(currentYear, currentWeek + i, ci);
                DateTime endDOW = startDOW.AddDays(6);
                var item = new TabWeekDTO();
                if (endDOW.Month > startDOW.Month)
                {
                    item.TabLabel = startDOW.ToShortMonthName() + " " + startDOW.Day + " - " + endDOW.ToShortMonthName() + " " + endDOW.Day;
                    item.CurrentWeek = currentWeek + i;
                }
                else
                {
                    item.TabLabel = startDOW.ToShortMonthName() + " " + startDOW.Day + " - " + endDOW.Day;
                    item.CurrentWeek = currentWeek + i;
                }
                weekDTO.Add(item);
            }

            return weekDTO;
        }

        private int GetNumOfWeek()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.CurrentInfo;
            DateTime date = new DateTime(DateTime.Today.Year, 12, 31);
            Calendar cal = info.Calendar;
            return cal.GetWeekOfYear(date, info.CalendarWeekRule, info.FirstDayOfWeek);
        }

        private int GetNumOfWeek(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Saturday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public DateTime FirstDateOfWeek(int year, int weekOfYear, CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= GetNumOfWeek()) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }
    }
}