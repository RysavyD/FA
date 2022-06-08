using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace _3F.Model.Extensions
{
    public static class StringExtensions
    {
        public static string TakeSafetely(this string source, int count)
        {
            return (source.Length <= count) ? source : source.Substring(0, count - 3) + "...";
        }

        public static string Decode(this string source)
        {
            return WebUtility.HtmlDecode(source);
        }

        public static string StringToHtmlLink(this string text)
        {
            string result = text.RemoveDiakriticsExtended();
            result = WebUtility.UrlEncode(result);
            result = result.RemoveUrlEncodeCharacters();
            return result;
        }

        public static string RemoveUrlEncodeCharacters(this string text)
        {
            var regex = new Regex("%[A-Z0-9][A-Z0-9]");
            return regex.Replace(text, "-");
        }

        public static string RemoveDiakriticsExtended(this string text)
        {
            string result = text.RemoveDiakritics();
            result = result.Replace(" ", "-").Replace(".", "").Replace("&", "-").Replace("/", "-").Replace("\\", "-").Replace("+", "-").Replace(":", "").Replace("?", "");
            result = result.Replace("\"", "").Replace("*", "-").Replace("%", "").Replace("#", "-").Replace("(", "").Replace(")", "").Replace(",", "").Replace(";", "-");
            result = result.Replace("@", "-").Replace("{", "-").Replace("}", "-");
            return result;
        }

        public static string RemoveDiakritics(this string text)
        {
            string stringFormD = text.Normalize(NormalizationForm.FormD);
            StringBuilder retVal = new StringBuilder();
            for (int index = 0; index < stringFormD.Length; index++)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != UnicodeCategory.NonSpacingMark)
                    retVal.Append(stringFormD[index]);
            }

            return retVal.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string AtFilter(this string source)
        {
            return source.Replace("@", "(at)");
        }

        public static string ToHtml(this string message)
        {
            string result = message.AtFilter();

            result = result.Replace("\n", " @ "); //nahradit odradkovani tagem
            var words = result.Split(new string[] { " " }, StringSplitOptions.None);
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = ((words[i].StartsWith("http://") || (words[i].StartsWith("https://")))
                    ? string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", words[i]) 
                    : words[i]);
            }
            result = string.Join(" ", words);

            result = result.Replace(" @ ", "<br />"); //nahradit odradkovani tagem

            return result;
        }

        public static string FormatWith(this string text, params object[] objects)
        {
            return string.Format(text, objects);
        }

        public static string NullToEmpty(this string source)
        {
            return string.IsNullOrEmpty(source) ? string.Empty : source;
        }

        public static string NullToEmpty(this int? source)
        {
            return source.HasValue ? source.ToString() : string.Empty;
        }
    }
}
