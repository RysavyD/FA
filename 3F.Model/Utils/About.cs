using System.IO;
using System.Reflection;

namespace _3F.Model.Utils
{
    public class About
    {
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

        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }
    }
}
