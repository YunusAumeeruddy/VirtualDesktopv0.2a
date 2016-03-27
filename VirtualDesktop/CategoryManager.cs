using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace VirtualDesktop
{
    public class CategoryManager
    {
        // Key = Category name , Value = Category index
        private static SortedDictionary<string , int> dictCategoryNameToIndex
            = new SortedDictionary<string , int>();

        // Key = Category index , Value = Category object
        private static SortedDictionary<int , Category> dictCategory
            = new SortedDictionary<int , Category>();

        public static int LastCategoryIndex { get; set; } = -1;
        public static int NextCategoryIndex { get; set; } = 0;
        public static int SelectedCategoryIndex { get; set; } = 0;

        public static Category LatestCategoryAdded { get; set; } = null;

        public static Category MainTypeCategory { get; set; } = null;

        public static void AddCategory(string categoryName, string imagePath
                                     , bool mainType, bool mixedType , XElement categoryElement)
        {
            Canvas shortcutIconGridCanvas = new Canvas();
            shortcutIconGridCanvas.Background = Brushes.Transparent;

            MainWindow mainWindow = App.MainAppWindow;
            mainWindow.ResizeShortcutIconGridCanvas(shortcutIconGridCanvas);

            shortcutIconGridCanvas.AddHandler(Canvas.MouseLeftButtonDownEvent
                        , new MouseButtonEventHandler(mainWindow.FreeSpace_PreviewMouseLeftButtonDown));
            shortcutIconGridCanvas.AddHandler(Canvas.MouseLeftButtonUpEvent
                        , new MouseButtonEventHandler(mainWindow.All_Controls_PreviewMouseLeftButtonUp));
            shortcutIconGridCanvas.AddHandler(Canvas.MouseMoveEvent
                        , new MouseEventHandler(mainWindow.FreeSpace_MouseMove));

            Category category = new Category(categoryElement);

            ShortcutIconGrid shortcutIconGrid = new ShortcutIconGrid(category , shortcutIconGridCanvas);

            category.Index = NextCategoryIndex;
            category.Name = categoryName;
            category.ImagePath = imagePath;
            category.ShortcutIconGrid = shortcutIconGrid;
            category.IsMainType = mainType;
            category.IsMixedType = mixedType;
            category.CategoryXElement = categoryElement;
            category.CreateCategoryXElement();
            if (mainType)
            {
                if (MainTypeCategory != null)
                {
                    MainTypeCategory.UnmarkAsMainType();
                }
                MainTypeCategory = category;
            }

            dictCategoryNameToIndex.Add(categoryName , NextCategoryIndex);
            dictCategory.Add(NextCategoryIndex , category);
            if (LatestCategoryAdded != null)
            {
                LatestCategoryAdded.NextCategory = category;
                category.PreviousCategory = LatestCategoryAdded;
            }
            LatestCategoryAdded = category;
            NextCategoryIndex++;
            LastCategoryIndex++;

            mainWindow.SliderWindow.AddCategoryMenuItem(category);

            if (dictCategory.Count == 1)
            {
                mainWindow.ShowSelectedShortcutIconGridCanvas();
                category.RemoveContextMenuItem.IsEnabled = false;
            }
            else
            {
                GetSelectedCategory().RemoveContextMenuItem.IsEnabled = true;
                mainWindow.SliderWindow.btnRemove_ContextMenuItem_Category.IsEnabled = true;
            }

            mainWindow.SliderWindow.RefreshBtnMoveAndCopyContextMenuItems();
        }

        public static void RestoreCategory(Category category)
        {
            int categoryIndex = category.Index;
            CategoryBorder categoryBorder = category.CategoryBorder;
            SliderWindow sliderWin = App.MainAppWindow.SliderWindow;
            sliderWin.CategoryMenuPanel.Children.Insert(categoryIndex , categoryBorder);

            dictCategory.Add(categoryIndex , category);
            dictCategoryNameToIndex.Add(category.Name , categoryIndex);

            Category prevCategory = category.PreviousCategory;
            Category nextCategory = category.NextCategory;

            bool IsConfigFileReadyToBeUpdated = false;

            if (prevCategory != null)
            {
                prevCategory.NextCategory = category;
                prevCategory.CategoryXElement.AddAfterSelf(category.CategoryXElement);
                IsConfigFileReadyToBeUpdated = true;
            }
            if(nextCategory != null)
            {
                nextCategory.PreviousCategory = category;
                if (!IsConfigFileReadyToBeUpdated)
                {
                    nextCategory.CategoryXElement.AddBeforeSelf(category.CategoryXElement);
                    IsConfigFileReadyToBeUpdated = true;
                }
            }

            if (!IsConfigFileReadyToBeUpdated)
            {
                ConfigManager.RootElement.Add(category.CategoryXElement);
            }
            ConfigManager.SaveConfigFile();
        }

        public static int CategoryCount
        {
            get
            {
                return dictCategory.Count;
            }
        }

        public static List<Category> CategoryListSortedByIndex
        {
            get
            {
                List<Category> categoryList = new List<Category>();
                foreach (KeyValuePair<int , Category> dictEntry in dictCategory)
                {
                    categoryList.Add(dictEntry.Value);
                }
                return categoryList;
            }
        }

        public static List<Category> CategoryListSortedByName
        {
            get
            {
                return CategoryListSortedByIndex.OrderBy(o => o.Name).ToList();
            }
        }

        public static bool HasExistingCategoryName(string categoryName)
        {
            return dictCategoryNameToIndex.ContainsKey(categoryName);
        }

        public static Category GetCategory(int categoryIndex)
        {
            Category category = null;
            dictCategory.TryGetValue(categoryIndex , out category);
            return category;
        }

        public static int GetCategoryIndex(string categoryName)
        {
            int categoryIndex = -1;
            foreach (KeyValuePair<string , int> dictEntry in dictCategoryNameToIndex)
            {
                if (dictEntry.Key.ToLower().Equals(categoryName.ToLower()))
                {
                    categoryIndex = dictEntry.Value;
                    break;
                }
            }
            return categoryIndex;
        }

        public static string GetCategoryName(int categoryIndex)
        {
            string categoryName = null;
            if (dictCategoryNameToIndex.ContainsValue(categoryIndex))
            {
                foreach(KeyValuePair<string , int> entry in dictCategoryNameToIndex)
                {
                    if (entry.Value == categoryIndex)
                    {
                        categoryName = entry.Key;
                        break;
                    }
                }
            }
            return categoryName;
        }

        public static Category GetFirstCategory()
        {
            Category category = null;
            foreach(KeyValuePair<int , Category> dictEntry in dictCategory)
            {
                category = dictEntry.Value;
                break;
            }
            return category;
        }

        public static Category GetFirstMixedTypeCategory()
        {
            Category category = null;
            foreach (KeyValuePair<int, Category> dictEntry in dictCategory)
            {
                if (dictEntry.Value.IsMixedType)
                {
                    category = dictEntry.Value;
                    break;
                }
            }
            return category;
        }

        public static Category GetSelectedCategory()
        {
            return GetCategory(SelectedCategoryIndex);
        }

        public static ShortcutIconGrid GetSelectedShortcutIconGrid()
        {
            return GetSelectedCategory().ShortcutIconGrid;
        }

        public static ShortcutIconGrid GetShortcutIconGrid(int categoryIndex)
        {
            Category category = GetCategory(categoryIndex);
            return category.ShortcutIconGrid;
        }

        public static void RemoveCategory(Category category)
        {
            if (dictCategory.ContainsKey(category.Index))
            {
                App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Remove(category.CategoryBorder);
                category.RemoveCategoryXElementFromConfigFile();
                dictCategory.Remove(category.Index);
                dictCategoryNameToIndex.Remove(category.Name);
                Category prevCategory = category.PreviousCategory;
                Category nextCategory = category.NextCategory;
                Category nextCategoryToBeSelected = null;
                if (nextCategory != null)
                {
                    nextCategory.PreviousCategory = prevCategory;
                    nextCategoryToBeSelected = nextCategory;
                }
                if (prevCategory != null)
                {
                    prevCategory.NextCategory = nextCategory;
                    if(nextCategoryToBeSelected == null)
                    {
                        nextCategoryToBeSelected = prevCategory;
                    }
                }

                if (category.IsSelected && nextCategoryToBeSelected != null)
                {
                    nextCategoryToBeSelected.CategoryBorder.Select();
                }
            }

            App.MainAppWindow.SliderWindow.RefreshBtnMoveAndCopyContextMenuItems();
        }

        public static void SortShortcutIconLists()
        {
            foreach (KeyValuePair<int , Category> entry in dictCategory)
            {
                Category category = null;
                dictCategory.TryGetValue(entry.Key , out category);
                ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;
                List<ShortcutIcon> shortcutIconList = shortcutIconGrid.ShortcutIconList;
                shortcutIconList = shortcutIconList.OrderBy(o => o.LabelText).ToList();
            }
        }

        public static void SwapCategory(Category cat1 , Category cat2)
        {
            dictCategory.Remove(cat1.Index);
            dictCategory.Remove(cat2.Index);
            dictCategoryNameToIndex.Remove(cat1.Name);
            dictCategoryNameToIndex.Remove(cat2.Name);

            CategoryBorder categoryBorder1 = cat1.CategoryBorder;
            CategoryBorder categoryBorder2 = cat2.CategoryBorder;

            if (cat1.IsSelected)
            {
                SelectedCategoryIndex = cat2.Index;
                categoryBorder2.BorderBrush = Brushes.Transparent;
            }
            else
            {
                SelectedCategoryIndex = cat1.Index;
                categoryBorder1.BorderBrush = Brushes.Transparent;
            }

            App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Remove(categoryBorder1);
            App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Remove(categoryBorder2);

            int cat1Index = cat1.Index;
            cat1.Index = cat2.Index;
            cat2.Index = cat1Index;

            if(cat1.Index < cat2.Index)
            {
                App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Insert(cat1.Index, categoryBorder1);
                App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Insert(cat2.Index, categoryBorder2);
            }
            else
            {
                App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Insert(cat2.Index , categoryBorder2);
                App.MainAppWindow.SliderWindow.CategoryMenuPanel.Children.Insert(cat1.Index , categoryBorder1);
            }

            dictCategoryNameToIndex.Add(cat1.Name, cat1.Index);
            dictCategoryNameToIndex.Add(cat2.Name, cat2.Index);
            dictCategory.Add(cat1.Index, cat1);
            dictCategory.Add(cat2.Index, cat2);

            XElement newCat1Element = new XElement(cat1.CategoryXElement);
            XElement newCat2Element = new XElement(cat2.CategoryXElement);

            cat1.CategoryXElement.AddBeforeSelf(newCat2Element);
            cat2.CategoryXElement.AddBeforeSelf(newCat1Element);
            cat1.CategoryXElement.Remove();
            cat2.CategoryXElement.Remove();

            cat1.CategoryXElement = newCat1Element;
            cat2.CategoryXElement = newCat2Element;

            cat1.FetchRowXElement();
            cat2.FetchRowXElement();

            ConfigManager.SaveConfigFile();

            if (cat1 == LatestCategoryAdded)
            {
                LatestCategoryAdded = cat2;
            }
            else if(cat2 == LatestCategoryAdded)
            {
                LatestCategoryAdded = cat1;
            }

            Category cat1PreviousCategory = cat1.PreviousCategory;
            Category cat1NextCategory = cat1.NextCategory;
            cat1.PreviousCategory = cat2.PreviousCategory;
            cat1.NextCategory = cat2.NextCategory;
            cat2.PreviousCategory = cat1PreviousCategory;
            cat2.NextCategory = cat1NextCategory;
        }
    }
}
