using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VirtualDesktop
{
    public class FileExtCategory
    {
        private static SortedDictionary<string, string> dictFileExtToCategory = null;

        public FileExtCategory()
        {
            LoadFileExtCategory();
        }

        public bool ContainsFileExtension(string fileExt)
        {
            foreach (KeyValuePair<string , string> dictEntry in dictFileExtToCategory)
            {
                if (dictEntry.Key.ToLower().Equals(fileExt.ToLower()))
                {
                    return true;
                }
            }  
            return false;
        }

        public string GetFileExtCategoryName(string fileExt)
        {
            string categoryName = null;
            dictFileExtToCategory.TryGetValue(fileExt, out categoryName);
            int categoryIndex = CategoryManager.GetCategoryIndex(categoryName);
            if (categoryIndex == -1)
                return null;

            Category category = CategoryManager.GetCategory(categoryIndex);
            return category.Name;
        }

        public List<string> GetFileExtListForCategory(string categoryName)
        {
            List<string> list = dictFileExtToCategory.Where(kvp => kvp.Value.ToLower().Equals(categoryName.ToLower()))
                                .Select(kvp => kvp.Key)
                                .ToList();
            list.Sort();
            return list;
        }

        private void LoadFileExtCategory()
        {
            dictFileExtToCategory = new SortedDictionary<string , string>();
            var txtFiles = Directory.EnumerateFiles(Path.GetFullPath("FileExt"), "*.txt");
            foreach (string currentFilePath in txtFiles)
            {
                string[] currentFilePathSplit = currentFilePath.Split('\\');
                string catNameInclExt = currentFilePathSplit[currentFilePathSplit.Length - 1];
                string catNameExclExt = catNameInclExt.Replace(".txt", "");
                string line;
                StreamReader file = new StreamReader(@currentFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    dictFileExtToCategory.Add(line.ToLower() , catNameExclExt);
                }
            }
        }

        public void ModifyCategoryFileExtensions(string categoryName , List<string> listFileExts)
        {
            RemoveAllEntriesForCategory(categoryName);
            string saveFilePath = @"FileExt\" + categoryName + ".txt";
            using (StreamWriter file = new StreamWriter(saveFilePath))
            {
                foreach (string fileExt in listFileExts)
                {
                    dictFileExtToCategory.Add(fileExt, categoryName);
                    file.WriteLine(fileExt);
                }
            }
        }

        public void RemoveAllEntriesForCategory(string categoryName)
        {
            List<string> listFileExtToRemove = GetFileExtListForCategory(categoryName);
            foreach (string fileExt in listFileExtToRemove)
            {
                dictFileExtToCategory.Remove(fileExt);
            }
            string saveFilePath = @"FileExt\" + categoryName + ".txt";
            if (File.Exists(saveFilePath))
            {
                File.WriteAllText(saveFilePath , string.Empty);
            }
            else
            {
                File.Create(saveFilePath).Dispose();
            }
        }
    }
}
