namespace Bharat.ToolKits.Extensions
{
    public class DateHelper
    {
        /// <summary>
        /// Count  no of Days in a month( Like Sunday or Monday or any) between two dates.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = end.DayOfWeek - day;   // Number of days since last [day]
            if (sinceLastDay < 0)
            {
                sinceLastDay += 7;         // Adjust for negative days since last [day]
            }

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay)
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Count  no of Days in a month( Like Sunday or Monday or any) for Given month and year.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="curMnt"></param>
        /// <returns></returns>
        public static int CountDays(DayOfWeek day, DateTime curMnt)
        {
            DateTime start = new(curMnt.Year, curMnt.Month, 1);
            DateTime end = new(curMnt.Year, curMnt.Month, DateTime.DaysInMonth(curMnt.Year, curMnt.Month));
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = end.DayOfWeek - day;   // Number of days since last [day]
            if (sinceLastDay < 0)
            {
                sinceLastDay += 7;         // Adjust for negative days since last [day]
            }

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay)
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// List All Sunday's of the year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<DateTime> AllSundayOfTheYear(int year)
        {
            return AllSunday(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        /// <summary>
        /// List of All Sunday between two Dates.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<DateTime> AllSunday(DateTime startDate, DateTime endDate)
        {
            List<DateTime> days_list = [];
            //Searching First Sunday.
            for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = d;
                    break;
                }
            }

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(7))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    days_list.Add(date);
                }
            }

            return days_list;
        }

        /// <summary>
        /// List of All Sunday on Current Month.
        /// </summary>
        /// <param name="currMnt"></param>
        /// <returns></returns>
        public static List<DateTime> AllSunday(DateTime currMnt)
        {
            DateTime startDate = new(currMnt.Year, currMnt.Month, 1);
            DateTime endDate = new(currMnt.Year, currMnt.Month, DateTime.DaysInMonth(currMnt.Year, currMnt.Month));
            List<DateTime> days_list = [];
            //Searching First Sunday.
            for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = d;
                    break;
                }
            }

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(7))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    days_list.Add(date);
                }
            }

            return days_list;
        }

        /// <summary>
        /// Count no of working days in a month
        /// It remove the Sunday
        /// </summary>
        /// <param name="currMnt"></param>
        /// <returns></returns>
        public static int NoOfWorkingDays(DateTime currMnt)
        {
            DateTime startDate = new(currMnt.Year, currMnt.Month, 1);
            DateTime endDate = new(currMnt.Year, currMnt.Month, DateTime.DaysInMonth(currMnt.Year, currMnt.Month));
            return Enumerable.Range(0, (int)(endDate - startDate).TotalDays + 1)
                             .Select(n => startDate.AddDays(n))
                             .Where(x => x.DayOfWeek != DayOfWeek.Sunday)
                             .Count();
        }
        public static int NoOfWorkingDaysWithoutSaturday(DateTime currMnt)
        {
            DateTime startDate = new(currMnt.Year, currMnt.Month, 1);
            DateTime endDate = new(currMnt.Year, currMnt.Month, DateTime.DaysInMonth(currMnt.Year, currMnt.Month));
            return Enumerable.Range(0, (int)(endDate - startDate).TotalDays + 1)
                             .Select(n => startDate.AddDays(n))
                             .Where(x => x.DayOfWeek != DayOfWeek.Sunday && x.DayOfWeek != DayOfWeek.Saturday)
                             .Count();
        }

        /// <summary>
        /// Generate Weekend Dates between two Dates
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <returns></returns>
        public static List<DateTime> GetWeekendDates(DateTime start_date, DateTime end_date)
        {
            return Enumerable.Range(0, (int)(end_date - start_date).TotalDays + 1)
                             .Select(n => start_date.AddDays(n))
                             .Where(x => x.DayOfWeek == DayOfWeek.Saturday
                                    || x.DayOfWeek == DayOfWeek.Sunday)
                             .ToList();
        }
    }
}