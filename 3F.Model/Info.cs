using System;
using System.IO;
using System.Reflection;

namespace _3F.Model
{
    public class Info
    {
        public static DateTime CentralEuropeNow
        {
            get
            {
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central Europe Standard Time");
            }
        }

        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        public static string VersionDate
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileInfo fileInfo = new FileInfo(assembly.Location);
                return fileInfo.LastWriteTime.ToShortDateString();
            }
        }

        public static string VersionTime
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileInfo fileInfo = new FileInfo(assembly.Location);
                return fileInfo.LastWriteTime.ToLongTimeString();
            }
        }

        public static string ConnectionString { get; set; }
    }
}
