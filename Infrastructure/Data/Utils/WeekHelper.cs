using System;
using System.Globalization;
namespace MediHub.Infrastructure.Data.Utils
{
    public static class WeekHelper
    {
        /// <summary>
        /// Get the Monday and Sunday dates of a specific ISO 8601 week number in a given year.
        /// </summary>

        public static (DateTime Monday, DateTime Sunday) GetWeekDates(int year, int weekNumber)
        {
            // ISO 8601 standard: weeks start on Monday
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Get first Thursday of the year
            var firstThursday = jan1.AddDays(daysOffset);

            // ISO 8601 week 1 is the week with the first Thursday
            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Calculate the start of the requested week
            var weekNum = weekNumber;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7 - 3); // Monday of the requested week
            var monday = result;
            var sunday = monday.AddDays(6);

            return (monday.Date, sunday.Date);
        }

        /// <summary>
        /// Get the ISO 8601 week number for today in the specified year.
        /// </summary>
        public static int GetCurrentWeekOfYear(int year)
        {
            var today = DateTime.UtcNow;

            // If today is in the requested year, return its ISO week
            if (today.Year == year)
            {
                return GetWeekOfYear(today);
            }

            // Otherwise, default to week 1
            return 1;
        }

        /// <summary>
        /// Get the ISO 8601 week number for any date.
        /// </summary>
        public static int GetWeekOfYear(DateTime date)
        {
            var cal = CultureInfo.CurrentCulture.Calendar;
            var weekNumber = cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Adjust for ISO 8601 edge cases
            if (date.Month == 1 && weekNumber >= 52)
                weekNumber = 1;

            return weekNumber;
        }
    }

}