using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleMF.Helper
{
    public static class DateTimeHelper
    {
        public static List<DateTime> GetDates(this DateTime dt)
        {
            int y = dt.Year;
            int m = dt.Month;
            return Enumerable.Range(1, DateTime.DaysInMonth(y, m))// Days: 1, 2 ... 31 etc.
                    .Select(day => new DateTime(y, m, day)) // Map each day to a date
                    .ToList();
        }
        public static List<DateTime> GetDatesExceptWeekends(this DateTime dt)
        {
            //* Get all dates of a month Except weekends *//
            int y = dt.Year;
            int m = dt.Month;
            var days = Enumerable.Range(1, DateTime.DaysInMonth(y, m))
                     .Select(day => new DateTime(y, m, day));
            return days.Where(d => (d.DayOfWeek != DayOfWeek.Saturday) && (d.DayOfWeek != DayOfWeek.Sunday)).ToList();
        }
        public static List<DateTime> GetDatesExceptWeekends(this DateTime dt, int startDay = 0)
        {
            //* Get all dates of a month Except weekends *//
            int y = dt.Year;
            int m = dt.Month;

            var days = Enumerable.Range(1, DateTime.DaysInMonth(y, m))
                     .Select(day => new DateTime(y, m, day));
            return days.Where(d => (d.DayOfWeek != DayOfWeek.Saturday) && (d.DayOfWeek != DayOfWeek.Sunday) && (d.Day >= (startDay <= 0 ? 1 : startDay))).ToList();
        }
        public static DateTime UtcToIst(this DateTime dt)
        {
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dt, istZone);
            //DateTimeOffset timeIST = TimeZoneInfo.ConvertTimeFromUtc(dt, istZone);
            //Console.WriteLine($"UTC: {theDate} IST: {timeIST}");
        }
    }
}
