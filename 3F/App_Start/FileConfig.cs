using System.IO;
using _3F.Web.Models;

namespace _3F.Web
{
    public class FileConfig
    {
        public static void CheckDirectory()
        {
            CreateDirectoryIfNeeded("Chat");
            CreateDirectoryIfNeeded("Emails");
        }

        private static void CreateDirectoryIfNeeded(string directoryName)
        {
            string path = Path.Combine(Values.Instance.AppDataPath, directoryName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}