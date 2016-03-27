using System.Collections.Generic;
using System.Windows;

namespace VirtualDesktop
{
    public class FrameworkElementReferenceManager
    {
        private static SortedDictionary<string , FrameworkElement> dictUIControl
            = new SortedDictionary<string , FrameworkElement>();

        private FrameworkElementReferenceManager() { }

        public static void AddNewFrameworkElement(FrameworkElement frameworkElement)
        {
            if (!dictUIControl.ContainsKey(frameworkElement.Name))
                dictUIControl.Add(frameworkElement.Name , frameworkElement);
        }

        public static bool ExistFrameworkElement(string elementName)
        {
            if (dictUIControl.ContainsKey(elementName))
                return true;
            return false;
        }

        public static List<FreeSpaceCanvas> GetAllFreeSpaceCanvas()
        {
            List<FreeSpaceCanvas> list = new List<FreeSpaceCanvas>();
            foreach (KeyValuePair<string, FrameworkElement> dictEntry in dictUIControl)
            {
                if (dictEntry.Key.StartsWith("FreeSpaceCanvas_"))
                {
                    list.Add(dictEntry.Value as FreeSpaceCanvas);
                }
            }
            return list;
        }

        public static FrameworkElement GetFrameworkElement(string elementName)
        {
            FrameworkElement element = null;
            dictUIControl.TryGetValue(elementName , out element);
            return element;
        }

        public static void RemoveFrameworkElement(FrameworkElement element)
        {
            string elementName = element.Name;
            if (dictUIControl.ContainsKey(elementName))
                dictUIControl.Remove(elementName);
        }
    }
}
