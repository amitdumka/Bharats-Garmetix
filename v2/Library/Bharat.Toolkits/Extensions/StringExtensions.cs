using System.Text;
using System.Text.RegularExpressions;

namespace Bharat.ToolKits.Extensions
{
    public static class StringExtensions
    {
        //        using Humanizer;
        //var pretty = "LedgerGroup".Humanize(LetterCasing.Title);  // "Ledger Group"
        public static string SplitPascalCase_Simple(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            // Insert a space before every uppercase letter that isn't at the start
            return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        }

        public static string SplitPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (i > 0 && char.IsUpper(c))
                {
                    char prev = input[i - 1];
                    bool prevIsLower = char.IsLower(prev);
                    bool nextIsLower = i + 1 < input.Length && char.IsLower(input[i + 1]);

                    // Only split if we’re transitioning from lower→upper
                    // or if the next letter is lower (start of a normal word)
                    if (prevIsLower || nextIsLower)
                    {
                        sb.Append(' ');
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        // Usage:
        //var fancy1 = SplitPascalCase("LedgerGroup");    // "Ledger Group"
        //var fancy2 = SplitPascalCase("HTTPResponse");   // "HTTP Response/*"*/
    }
}