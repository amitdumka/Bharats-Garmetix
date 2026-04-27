namespace Bharat.ToolKits.Helpers;

public static class AmountInWordsHelper
{
    private static readonly string[] Ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
    private static readonly string[] Teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
    private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

    public static string ConvertToWords(long number)
    {
        if (number == 0)
            return "Zero";

        if (number < 0)
            return "Minus " + ConvertToWords(Math.Abs(number));

        string words = "";

        words += NumberToWords(number / 10000000, "Crore ");
        words += NumberToWords(number / 100000 % 100, "Lakh ");
        words += NumberToWords(number / 1000 % 100, "Thousand ");
        words += NumberToWords(number / 100 % 10, "Hundred ");

        if (number > 100 && number % 100 > 0)
            words += "and ";

        words += NumberToWords(number % 100, "");

        return words.Trim();
    }

    private static string NumberToWords(long number, string suffix)
    {
        if (number == 0)
            return "";

        if (number < 10)
            return Ones[number] + " " + suffix;
        if (number < 20)
            return Teens[number - 10] + " " + suffix;
        if (number < 100)
            return Tens[number / 10] + " " + Ones[number % 10] + " " + suffix;

        return NumberToWords(number, "");
    }
}


