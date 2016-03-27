using System.Collections;
using System.Linq;
using System.Reflection;

namespace VirtualDesktop
{
    public class InternalResourceManager
    {
        public static string[] ResourceNames { get; } = GetResourceNames();

        public static bool HasResource(string resourcePath)
        {
            return ResourceNames.Contains(resourcePath.Replace("pack://application:,,,/", "").ToLower());
        }

        public static string[] GetResourceNames()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            {
                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>().Select(entry =>
                             (string)entry.Key).ToArray();
                }
            }
        }
    }
}
