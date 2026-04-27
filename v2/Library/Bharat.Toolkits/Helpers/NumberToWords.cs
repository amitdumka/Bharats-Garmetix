using System.Globalization;

namespace Bharat.ToolKits.Helpers;

public static class NumberToWords
{
    public static string ConvertAmount(double amount)
    {
        try
        {
            long number = (long)amount;
            long dec = (long)Math.Round((amount - (double)number) * 100);

            if (dec == 0)
            {
                return ToWords(number) + " Only";
            }
            else
            {
                return ToWords(number) + " and " + ToWords(dec) + " Paise Only";
            }
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static string ConvertMonthYearToString(int monthYear)
    {


        // Extract the year from the last four digits
        int year = monthYear % 10000;
        // Extract the month from the remaining digits
        int month = monthYear / 10000;

        // Ensure that the month is valid (1-12)
        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException("month", "Month must be between 1 and 12.");
        }
        string monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
        // Create a DateTime object so we can get the full month name using the "MMMM" format specifier.
        //DateTime date = new DateTime(year, month, 1);
        //string monthName = date.ToString("MMMM");
        // Return the formatted string
        return $"{monthName} {year}";

    }

    private static string ToWords(long number)
    {
        if (number == 0)
        {
            return "Zero";
        }

        if (number < 0)
        {
            return "Minus " + ToWords(Math.Abs(number));
        }

        string words = "";

        if ((number / 10000000) > 0)
        {
            words += ToWords(number / 10000000) + " Crore ";
            number %= 10000000;
        }

        if ((number / 100000) > 0)
        {
            words += ToWords(number / 100000) + " Lakh ";
            number %= 100000;
        }

        if ((number / 1000) > 0)
        {
            words += ToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += ToWords(number / 100) + " Hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
            {
                words += "and ";
            }

            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
            {
                words += unitsMap[number];
            }
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                {
                    words += "-" + unitsMap[number % 10];
                }
            }
        }
        return words.Trim();
    }
}


