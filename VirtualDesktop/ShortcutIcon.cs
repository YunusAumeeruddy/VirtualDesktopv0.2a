using IWshRuntimeLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace VirtualDesktop
{
    public class ShortcutIcon
    {
        public Category Category { get; set; }
        public string LabelText { get; set; }
        public Image IconImage { get; set; }
        public FileSystemInfo TargetFilePathInfo { get; set; }
        public FileSystemInfo ShortcutFilePathInfo { get; set; }
        public ShortcutIconBorder ShortcutIconBorder { get; set; }
        public ShortcutIconGridPosition GridPosition { get; set; }
        public bool IsFile { get; set; } = true;
        public bool IsSelected { get; set; } = false;

        public XElement ShortcutIconXElement { get; set; }
        public XElement PositionXElement { get; set; }

        public ShortcutIcon(Category category, string labelText
                            , Image iconImage
                            , FileSystemInfo targetFilePathInfo
                            , FileSystemInfo shortcutFilePathInfo)
        {
            Category = category;
            LabelText = labelText;
            IconImage = iconImage;
            TargetFilePathInfo = targetFilePathInfo;
            ShortcutFilePathInfo = shortcutFilePathInfo;
        }

        public void AddToGrid()
        {

        }

        public void CreatePositionXElement()
        {
            if (PositionXElement == null)
            {
                PositionXElement = new XElement("Position");
                List<XAttribute> listPositionAttributes = new List<XAttribute>();
                listPositionAttributes.Add(new XAttribute("ScreenProfileId", ConfigManager.MainScreenProfileId));
                listPositionAttributes.Add(new XAttribute("GridLeftPos", GridPosition.LeftPos));
                listPositionAttributes.Add(new XAttribute("GridTopPos", GridPosition.TopPos));
                listPositionAttributes.Add(new XAttribute("GridRightPos", GridPosition.RightPos));
                listPositionAttributes.Add(new XAttribute("GridBottomPos", GridPosition.BottomPos));
                foreach (XAttribute attribute in listPositionAttributes)
                {
                    PositionXElement.Add(attribute);
                }
                ShortcutIconXElement.Add(PositionXElement);
                ConfigManager.SaveConfigFile();
            }
        }

        public void CreateShortcutIconXElement()
        {
            if (ShortcutIconXElement == null)
            {
                ShortcutIconXElement = new XElement("ShortcutIcon");
                List<XAttribute> listShortcutIconAttributes = new List<XAttribute>();
                listShortcutIconAttributes.Add(new XAttribute("LabelText", LabelText));
                listShortcutIconAttributes.Add(new XAttribute("ShortcutPath", ShortcutFilePathInfo.FullName));

                foreach (XAttribute attribute in listShortcutIconAttributes)
                {
                    ShortcutIconXElement.Add(attribute);
                }
                XElement categoryElement = Category.CategoryXElement;
                categoryElement.Add(ShortcutIconXElement);
                ConfigManager.SaveConfigFile();
            }
        }

        public static ShortcutIcon GenerateShortcutIconFromFile(string filePath , Category category)
        {
            FileSystemInfo shortcutFilePathInfo = null;
            FileSystemInfo targetFilePathInfo = null;
            bool IsFile = true;
            bool IsFolderTarget = false;
            string fileExt = null;
            StringBuilder fileName = new StringBuilder();

            if (System.IO.File.Exists(filePath))
            {
                shortcutFilePathInfo = new FileInfo(filePath);
                targetFilePathInfo = new FileInfo(filePath);

                fileExt = targetFilePathInfo.Extension.Substring(1).ToLower();
                string[] fileNameSplit = shortcutFilePathInfo.Name.Split('.');
                for (int i = 0; i < fileNameSplit.Length - 1; i++)
                {
                    fileName.Append(fileNameSplit[i]);
                    if (i < fileNameSplit.Length - 2)
                    {
                        fileName.Append(".");
                    }
                }

                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(filePath);
                if (System.IO.File.Exists(link.TargetPath))
                {
                    targetFilePathInfo = new FileInfo(link.TargetPath);
                    fileExt = targetFilePathInfo.Extension.Substring(1).ToLower();
                }
                else if (Directory.Exists(link.TargetPath))
                {
                    targetFilePathInfo = new DirectoryInfo(link.TargetPath);
                    IsFolderTarget = true;
                }
            }
            else
            {
                shortcutFilePathInfo = new DirectoryInfo(filePath);
                targetFilePathInfo = shortcutFilePathInfo;
                IsFile = false;
                IsFolderTarget = true;
                fileName.Append(shortcutFilePathInfo.Name);
            }

            if(category == null)
            { 
                int categoryIndex = CategoryManager.SelectedCategoryIndex;
                if (ConfigManager.AutoCategorisedMode && IsFile && !IsFolderTarget)
                {
                    string categoryName = App.MainFileExtCategory.GetFileExtCategoryName(fileExt);

                    if (categoryName == null)
                    {
                        if (!CategoryManager.GetSelectedCategory().IsMixedType)
                        {
                            categoryIndex = CategoryManager.MainTypeCategory.Index;
                        }
                    }
                    else
                    {
                        categoryIndex = CategoryManager.GetCategoryIndex(categoryName);
                    }
                }
                category = CategoryManager.GetCategory(categoryIndex);
            }

            Image iconImg = new Image();
            Image iconImgCopy = new Image();
            if (IsFolderTarget)
            {
                string folderImagePath = "pack://application:,,,/Resources/Images/folder.png";
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(folderImagePath);
                imageSource.EndInit();
                iconImg.Source = imageSource;
            }
            else
            {
                iconImg.Source =
                    IconExtract.GetBmpSrcLargeIcon(targetFilePathInfo.FullName, false, false);
            }
            ShortcutIcon shortcutIcon = new ShortcutIcon(category
                                                        , fileName.ToString()
                                                        , iconImg
                                                        , targetFilePathInfo
                                                        , shortcutFilePathInfo);
            shortcutIcon.IsFile = IsFile;
            return shortcutIcon;
        }
        
        public string GetShortcutFileDirectoryPath()
        {
            if (IsFile)
            {
                return (ShortcutFilePathInfo as FileInfo).Directory.FullName;
            }
            return new DirectoryInfo(ShortcutFilePathInfo.FullName).Parent.FullName;
        }

        public XElement GetPositionXElementForCurrentScreenProfile()
        {
            XElement positionElement = null;
            IEnumerable<XElement> positionElements_IEnum = ShortcutIconXElement.Elements("Position");
            foreach (XElement element in positionElements_IEnum)
            {
                XAttribute screenProfileIdAttr = element.Attribute("ScreenProfileId");
                if (screenProfileIdAttr == null)
                    continue;

                int screenProfileId = 0;
                int.TryParse(screenProfileIdAttr.Value , out screenProfileId);
                if(screenProfileId == ConfigManager.MainScreenProfileId)
                {
                    positionElement = element;
                    break;
                }
            }
            return positionElement;
        }

        public static ShortcutIcon GetShortcutIconCopy(ShortcutIcon shortcutIcon)
        {
            if (shortcutIcon == null)
                return null;

            Category category = shortcutIcon.Category;
            string labelText = shortcutIcon.LabelText;
            Image image = shortcutIcon.IconImage;
            FileSystemInfo shortcutFilePathInfo = null;
            FileSystemInfo targetFilePathInfo = null;
            
            if (shortcutIcon.ShortcutFilePathInfo is FileInfo)
            {
                shortcutFilePathInfo = new FileInfo(shortcutIcon.ShortcutFilePathInfo.FullName);
                if (shortcutIcon.TargetFilePathInfo is FileInfo)
                    targetFilePathInfo = new FileInfo(shortcutIcon.TargetFilePathInfo.FullName);
                else
                    targetFilePathInfo = new DirectoryInfo(shortcutIcon.TargetFilePathInfo.FullName);
            }
            else
            {
                shortcutFilePathInfo = new DirectoryInfo(shortcutIcon.ShortcutFilePathInfo.FullName);
                targetFilePathInfo = shortcutFilePathInfo;
            }

            ShortcutIcon newShortcutIcon = new ShortcutIcon(category, labelText, image,
                                                    targetFilePathInfo, shortcutFilePathInfo);
            newShortcutIcon.IsFile = shortcutIcon.IsFile;
            newShortcutIcon.GridPosition = null;
            newShortcutIcon.IsSelected = false;

            return newShortcutIcon;
        }

        public void ModifyGridPosXAttrValues()
        {
            GridPosition.CalculateBottomPos();
            int leftPos = 0;
            int topPos = 0;
            int rightPos = 0;
            int bottomPos = 0;

            XAttribute leftPosAttr = PositionXElement.Attribute("GridLeftPos");
            XAttribute topPosAttr = PositionXElement.Attribute("GridTopPos");
            XAttribute rightPosAttr = PositionXElement.Attribute("GridRightPos");
            XAttribute bottomPosAttr = PositionXElement.Attribute("GridBottomPos");

            int.TryParse(leftPosAttr.Value, out leftPos);
            int.TryParse(topPosAttr.Value, out topPos);
            int.TryParse(rightPosAttr.Value, out rightPos);
            int.TryParse(bottomPosAttr.Value, out bottomPos);

            if (leftPos != GridPosition.LeftPos
                || topPos != GridPosition.TopPos
                || rightPos != GridPosition.RightPos
                || bottomPos != GridPosition.BottomPos)
            {
                leftPosAttr.SetValue(GridPosition.LeftPos);
                topPosAttr.SetValue(GridPosition.TopPos);
                rightPosAttr.SetValue(GridPosition.RightPos);
                bottomPosAttr.SetValue(GridPosition.BottomPos);
                ConfigManager.SaveConfigFile();
            }
        }

        public void ModifyLabelTextXAttrValue()
        {
            ShortcutIconXElement.Attribute("LabelText").SetValue(LabelText);
            ConfigManager.SaveConfigFile();
        }

        public void ModifyShortcutPathXAttrValue()
        {
            ShortcutIconXElement.Attribute("ShortcutPath").SetValue(ShortcutFilePathInfo.FullName);
            ConfigManager.SaveConfigFile();
        }

        public void Relocate()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.NavigateToShortcut = false;
            if (IsFile)
            {
                openFileDialog.IsFolderPicker = false;
            }
            else
            {
                openFileDialog.IsFolderPicker = true;
            }

            CommonFileDialogResult result = openFileDialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileExt = null;
                StringBuilder fileName = new StringBuilder();

                if (IsFile)
                {
                    ShortcutFilePathInfo = new FileInfo(selectedFilePath);
                    TargetFilePathInfo = new FileInfo(selectedFilePath);

                    fileExt = TargetFilePathInfo.Extension.Substring(1).ToLower();
                    string[] fileNameSplit = ShortcutFilePathInfo.Name.Split('.');
                    for (int i = 0; i < fileNameSplit.Length - 1; i++)
                    {
                        fileName.Append(fileNameSplit[i]);
                        if (i < fileNameSplit.Length - 2)
                        {
                            fileName.Append(".");
                        }
                    }

                    if (fileExt.Equals("lnk"))
                    {
                        WshShell shell = new WshShell();
                        IWshShortcut link = (IWshShortcut)shell.CreateShortcut(selectedFilePath);
                        if (System.IO.File.Exists(link.TargetPath))
                        {
                            TargetFilePathInfo = new FileInfo(link.TargetPath);
                            fileExt = TargetFilePathInfo.Extension.Substring(1).ToLower();
                        }
                        else if (Directory.Exists(link.TargetPath))
                        {
                            TargetFilePathInfo = new DirectoryInfo(link.TargetPath);
                        }
                    }
                }
                else
                {
                    ShortcutFilePathInfo = new DirectoryInfo(selectedFilePath);
                    TargetFilePathInfo = ShortcutFilePathInfo;
                }

                ShortcutIconBorder.TextBlock.Text = fileName.ToString();
                LabelText = fileName.ToString();
                IconImage.Source =
                    IconExtract.GetBmpSrcLargeIcon(TargetFilePathInfo.FullName, false, false);

                ModifyLabelTextXAttrValue();
                ModifyShortcutPathXAttrValue();
            }
        }

        public void Remove()
        {
            ShortcutIconGrid shortcutIconGrid = Category.ShortcutIconGrid;
            shortcutIconGrid.Canvas.Children.Remove(ShortcutIconBorder);
            FrameworkElementReferenceManager.RemoveFrameworkElement(ShortcutIconBorder);
            ShortcutIconXElement.Remove();
            ConfigManager.SaveConfigFile();
            shortcutIconGrid.RemoveShortcutIcon(this);
            shortcutIconGrid.AddShortcutIconFreePosition(GridPosition);
            shortcutIconGrid.RemoveUnusedLastRowsIfGreaterThanMin();
            App.MainAppWindow.SliderWindow.RefreshBottomDockPanelButtons();
            App.MainAppWindow.SliderWindow.RefreshTopMenuShowPanelLabels();
        }

        public void RenameFile(string newName)
        {
            if (!StringUtils.EqualsIgnoreCase(ShortcutFilePathInfo.Name, newName))
            {
                string oldFilePath = null;
                string newFilePath = null;
                string fileLocation = "";

                oldFilePath = ShortcutFilePathInfo.FullName;
                newFilePath = GetShortcutFileDirectoryPath() + @"\" + newName;
                if (IsFile)
                {
                    newFilePath += ShortcutFilePathInfo.Extension;
                }

                try
                {
                    if (IsFile)
                    {
                        fileLocation = (ShortcutFilePathInfo as FileInfo).Directory.FullName;
                        System.IO.File.Move(@oldFilePath, @newFilePath);
                        ShortcutFilePathInfo = new FileInfo(newFilePath);
                    }
                    else
                    {
                        Directory.Move(@oldFilePath, @newFilePath);
                        ShortcutFilePathInfo = new DirectoryInfo(newFilePath);
                    }
                    ModifyShortcutPathXAttrValue();
                }
                catch (Exception ex)
                {
                    string msg = "The application was unable to rename the file/folder \""
                        + ShortcutFilePathInfo.Name + "\" to \""
                        + newName + "\"" + (IsFile ? "located in " + fileLocation : "")
                        + ". File/Folder may be in use or there is not enough privilege to"
                        + " perform a rename on it. Alternatives are to rename the file/folder"
                        + " outside of this application or through the Properties window,"
                        + " and then relocating the shortcut icon to the renamed file/folder"
                        + " using this application (by right-clicking it , and then choose"
                        + " Relocate shortcut icon)";
                    MessageBox.Show(msg, "Failed to rename file/folder"
                                    , MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public void RenameLabelText(string newName)
        {
            ShortcutIconBorder.TextBlock.Text = newName;
            LabelText = newName;
            ModifyLabelTextXAttrValue();
        }

        public void Restore()
        {
            FrameworkElementReferenceManager.AddNewFrameworkElement(ShortcutIconBorder);
            Category.CategoryXElement.Add(ShortcutIconXElement);
            ConfigManager.SaveConfigFile();
            Category.ShortcutIconGrid.LoadShortcutIcon(this);
            App.MainAppWindow.PopulateShortcutIconGrid(Category);
        }
    }
}
