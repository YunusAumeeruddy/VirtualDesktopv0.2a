using System.IO;

namespace VirtualDesktop
{
    public class ExternalResourceDirManager
    {
        public static string GetAbsolutePathAsOfAppDirectory(string fileName)
        {
            return Path.GetFullPath(fileName);
        }
    }
}
