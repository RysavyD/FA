using System;

namespace _3F.Model.Extensions
{
    public static class DateExtension
    {
        public static long ToUnixTimestamp(this DateTime target)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            var unixTimestamp = System.Convert.ToInt64((target - date).TotalSeconds);

            return unixTimestamp;
        }

        public static string ToDayDateTimeString(this DateTime source)
        {
            return source.ToString("ddd dd.MM.yyyy HH:mm");
        }

        public static string ToOnlyDateTimeString(this DateTime source)
        {
            return source.ToString("dd.MM.yyyy");
        }
    }
}
