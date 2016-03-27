using VirtualDesktop.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace VirtualDesktop
{
    public class ConfigManager
    {
        private const string CONFIG_FILE_NAME = "Config.xml";
        private static XDocument xmlDoc = null;

        private static string latestBackupFileName = null;

        public static bool HasAppBeenRunBefore { get; set; } = false;
        public static double ScreenAreaWidth { get; set; } = ScreenWorkAreaUtils.ScreenWorkAreaWidth;
        public static double ScreenAreaHeight { get; set; } = ScreenWorkAreaUtils.ScreenWorkAreaHeight;
        public static bool AutoCategorisedMode { get; set; } = true;
        public static bool AutoSortMode { get; set; } = false;
        public static string AutoSortBy { get; set; } = "name";

        public static XElement RootElement { get; set; } = null;
        public static XElement BackgroundElement { get; set; } = null;
        public static XElement AutoCategorisedModeXElement { get; set; } = null;
        public static XElement AutoSortModeXElement { get; set; } = null;

        public static int MainScreenProfileId { get; set; } = 0;

        public static SortedDictionary<int , string> DictScreenProfiles { get; set; }
            = new SortedDictionary<int , string>();

        public static void BuildDefaultConfigFile()
        {
            string defaultConfigFileXMLString = Resources.ConfigTemplate;
            xmlDoc = XDocument.Parse(defaultConfigFileXMLString);
            SaveConfigFile();
            xmlDoc = XDocument.Load(GetConfigFilePath());
            XElement screenProfileElement = xmlDoc.Root.Element("ScreenProfile");
            screenProfileElement.SetAttributeValue("ScreenAreaWidth" , ScreenAreaWidth);
            screenProfileElement.SetAttributeValue("ScreenAreaHeight" , ScreenAreaHeight);
            SaveConfigFile();
        }

        public static void CreateScreenProfileElementForCurrentScreen(int id , XElement latestScreenProfileElement)
        {
            MainScreenProfileId = id;
            XElement screenProfileElement = new XElement("ScreenProfile");
            screenProfileElement.Add(new XAttribute("Id" , id));
            screenProfileElement.Add(new XAttribute("ScreenAreaWidth" , ScreenAreaWidth));
            screenProfileElement.Add(new XAttribute("ScreenAreaHeight" , ScreenAreaHeight));
            latestScreenProfileElement.AddAfterSelf(screenProfileElement);
            SaveConfigFile();
        }

        public static string GetConfigFilePath()
        {
            return ExternalResourceDirManager.GetAbsolutePathAsOfAppDirectory(CONFIG_FILE_NAME);
        }

        public static void LoadConfigFile()
        {
            if (!File.Exists(GetConfigFilePath()) || !ValidateConfigFileAgainstXSD())
            {
                BuildDefaultConfigFile();
            }
            LoadConfigValues();
        }

        public static void LoadConfigValues()
        {
            xmlDoc = XDocument.Load(GetConfigFilePath());
            RootElement = xmlDoc.Root;
            IEnumerable<XElement> screenProfileElements_IEnum = xmlDoc.Root.Elements("ScreenProfile");
            XElement screenProfileElement = null;
            XElement latestScreenProfileElement = null;
            int maxScreenProfileId = 0;
            foreach (XElement element in screenProfileElements_IEnum)
            {
                latestScreenProfileElement = element;

                XAttribute screenProfileIdAttr = element.Attribute("Id");
                XAttribute screenAreaWidthAttr = element.Attribute("ScreenAreaWidth");
                XAttribute screenAreaHeightAttr = element.Attribute("ScreenAreaHeight");

                int screenProfileId = 0;
                double width = 0;
                double height = 0;

                int.TryParse(screenProfileIdAttr.Value, out screenProfileId);
                double.TryParse(screenAreaWidthAttr.Value, out width);
                double.TryParse(screenAreaHeightAttr.Value, out height);

                if (screenProfileId > maxScreenProfileId)
                    maxScreenProfileId = screenProfileId;

                string screenAreaRes = width + " x " + height;
                DictScreenProfiles.Add(screenProfileId, screenAreaRes);

                if (width == ScreenAreaWidth && height == ScreenAreaHeight)
                {
                    screenProfileElement = element;
                    MainScreenProfileId = screenProfileId;
                }
            }

            if(screenProfileElement == null)
            {
                CreateScreenProfileElementForCurrentScreen(maxScreenProfileId + 1 , latestScreenProfileElement);
            }

            XElement modesElement = xmlDoc.Root.Element("Modes");
            IEnumerable<XElement> modeElements_IEnum = modesElement.Elements("Mode");
            foreach (XElement modeElement in modeElements_IEnum)
            {
                XAttribute nameAttr = modeElement.Attribute("Name");
                XAttribute enabledAttr = modeElement.Attribute("Enabled");

                bool enabledValue = false;
                bool.TryParse(enabledAttr.Value, out enabledValue);

                string modeName = nameAttr.Value;
                if (ModeManager.IsModeValid(modeName))
                {
                    if (modeName.ToLower().Equals("auto-categorised"))
                    {
                        AutoCategorisedMode = enabledValue;
                        AutoCategorisedModeXElement = modeElement;
                    }
                    else if (modeName.ToLower().Equals("auto-sort"))
                    {
                        AutoSortMode = enabledValue;
                        AutoSortModeXElement = modeElement;
                        XAttribute valueAttr = modeElement.Attribute("Value");
                        if (valueAttr == null)
                        {
                            valueAttr = new XAttribute("Value" , "Name");
                            modeElement.Add(valueAttr);
                            SaveConfigFile();
                        }
                        else
                            AutoSortBy = valueAttr.Value.ToLower();
                    }
                }
            }

            App.MainAppWindow.SliderWindow.SetAutoCategorisedValue(AutoCategorisedMode);
            App.MainAppWindow.SliderWindow.SetAutoSortValue(AutoSortMode);
            
            BackgroundElement = xmlDoc.Root.Element("Background");
            string backgroundImagePath = null;
            if(BackgroundElement != null)
            {
                XAttribute imagePathAttr = BackgroundElement.Attribute("ImagePath");
                if(imagePathAttr != null)
                {
                    string imagePath = imagePathAttr.Value;
                    if(imagePath != null
                        && (File.Exists(imagePath) || InternalResourceManager.HasResource(imagePath)))
                    {
                        backgroundImagePath = imagePath;
                    }
                }
                else
                {
                    BackgroundElement.Add(new XAttribute("ImagePath", ""));
                    SaveConfigFile();
                }
            }
            else
            {
                BackgroundElement = new XElement("Background");
                BackgroundElement.Add(new XAttribute("ImagePath", ""));
                modesElement.AddAfterSelf(BackgroundElement);
                SaveConfigFile();
            }

            App.MainAppWindow.BackgroundImagePath = backgroundImagePath;
            App.MainAppWindow.SetBackgroundImage(false);

            IEnumerable<XElement> categoryElements_IEnum = xmlDoc.Root.Elements("Category");
            foreach (XElement categoryElement in categoryElements_IEnum)
            {
                XAttribute nameAttr = categoryElement.Attribute("Name");
                XAttribute imageAttr = categoryElement.Attribute("Image");
                XAttribute mainTypeAttr = categoryElement.Attribute("MainType");
                XAttribute mixedTypeAttr = categoryElement.Attribute("MixedType");

                XElement rowSettingsElement = categoryElement.Element("RowSettings");

                string name = nameAttr.Value;
                string imagePath = imageAttr.Value;
                string mainTypeStr = mainTypeAttr.Value;
                string mixedTypeStr = mixedTypeAttr.Value;

                if (!File.Exists(imagePath) && !InternalResourceManager.HasResource(imagePath))
                {
                    imagePath = null;
                }

                bool mainType = mainTypeStr.ToLower().Equals("true") ? true : false;
                bool mixedType = mixedTypeStr.ToLower().Equals("true") ? true : false;

                int noOfRows = RowManager.MinNumberOfRows;
                XElement rowElement = null;
                foreach (XElement element in rowSettingsElement.Elements("Row"))
                {
                    XAttribute screenProfileIdAttr = element.Attribute("ScreenProfileId");
                    XAttribute valueAttr = element.Attribute("Value");

                    int screenProfileId = -1;
                    int.TryParse(screenProfileIdAttr.Value, out screenProfileId);
                    if (screenProfileId == MainScreenProfileId)
                    {
                        int.TryParse(valueAttr.Value, out noOfRows);

                        if (noOfRows < RowManager.MinNumberOfRows)
                        {
                            noOfRows = RowManager.MinNumberOfRows;
                            valueAttr.SetValue(noOfRows);
                            SaveConfigFile();
                        }
                        rowElement = element;
                        break;
                    }
                }

                if (rowElement == null)
                {
                    rowElement = new XElement("Row"
                                  , new XAttribute("ScreenProfileId", MainScreenProfileId)
                                  , new XAttribute("Value", RowManager.MinNumberOfRows));
                    rowSettingsElement.Add(rowElement);
                    SaveConfigFile();
                }

                CategoryManager.AddCategory(name, imagePath
                                           , mainType, mixedType, categoryElement);

                Category category = CategoryManager.LatestCategoryAdded;
                ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;
                category.RowElement = rowElement;
                shortcutIconGrid.SetNoOfRows(noOfRows);

                IEnumerable<XElement> shortcutIconElements_IEnum = categoryElement.Elements("ShortcutIcon");
                foreach (XElement shortcutIconElement in shortcutIconElements_IEnum)
                {
                    XAttribute labelTextAttr = shortcutIconElement.Attribute("LabelText");
                    XAttribute shortcutPathAttr = shortcutIconElement.Attribute("ShortcutPath");

                    string labelText = labelTextAttr.Value;
                    string shortcutPath = shortcutPathAttr.Value;

                    int screenProfileId = 0;
                    int gridLeftPos = 0;
                    int gridRightPos = 0;
                    int gridTopPos = 0;
                    int gridBottomPos = 0;

                    IEnumerable<XElement> positionElements_IEnum = shortcutIconElement.Elements("Position");
                    XElement positionElement = null;
                    foreach (XElement element in positionElements_IEnum)
                    {
                        XAttribute screenProfileIdAttr = element.Attribute("ScreenProfileId");
                        XAttribute gridLeftPosAttr = element.Attribute("GridLeftPos");
                        XAttribute gridTopPosAttr = element.Attribute("GridTopPos");
                        XAttribute gridRightPosAttr = element.Attribute("GridRightPos");
                        XAttribute gridBottomPosAttr = element.Attribute("GridBottomPos");

                        int.TryParse(screenProfileIdAttr.Value, out screenProfileId);
                        int.TryParse(gridLeftPosAttr.Value, out gridLeftPos);
                        int.TryParse(gridTopPosAttr.Value, out gridTopPos);
                        int.TryParse(gridRightPosAttr.Value, out gridRightPos);
                        int.TryParse(gridBottomPosAttr.Value, out gridBottomPos);

                        if (screenProfileId == MainScreenProfileId)
                        {
                            positionElement = element;
                            break;
                        }
                    }

                    if (positionElement == null)
                    {
                        positionElement = new XElement("Position");
                        positionElement.Add(new XAttribute("ScreenProfileId", MainScreenProfileId));
                        positionElement.Add(new XAttribute("GridLeftPos", gridLeftPos));
                        positionElement.Add(new XAttribute("GridTopPos", gridTopPos));
                        positionElement.Add(new XAttribute("GridRightPos", gridRightPos));
                        positionElement.Add(new XAttribute("GridBottomPos", gridBottomPos));
                        shortcutIconElement.Add(positionElement);
                        SaveConfigFile();
                    }

                    ShortcutIconGridPosition gridPosition
                        = new ShortcutIconGridPosition(category, gridLeftPos, gridRightPos, gridTopPos, gridBottomPos);

                    ShortcutIcon shortcutIcon = ShortcutIcon.GenerateShortcutIconFromFile(shortcutPath, category);
                    shortcutIcon.ShortcutIconXElement = shortcutIconElement;
                    shortcutIcon.PositionXElement = positionElement;
                    shortcutIcon.LabelText = labelText;
                    shortcutIcon.GridPosition = null;
                    if (!AutoSortMode)
                        shortcutIcon.GridPosition = gridPosition;
                    shortcutIconGrid.LoadShortcutIcon(shortcutIcon);
                }

                if (AutoSortMode)
                {
                    shortcutIconGrid.SortShortcutIconList(AutoSortBy);
                }
                App.MainAppWindow.PopulateShortcutIconGrid(category);
            }
        }

        public static void ModifyAutoCategorisedModeXAttrValue()
        {
            AutoCategorisedModeXElement.Attribute("Enabled").SetValue(AutoCategorisedMode);
            SaveConfigFile();
        }

        public static void ModifyAutoSortModeXAttrValue()
        {
            AutoSortModeXElement.Attribute("Enabled").SetValue(AutoSortMode);
            SaveConfigFile();
        }

        public static bool ValidateConfigFileAgainstXSD()
        {
            string xsdString = Resources.Config;
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("" , XmlReader.Create(new StringReader(xsdString)));
            xmlDoc = XDocument.Load(@GetConfigFilePath());

            bool validated = true;

            xmlDoc.Validate(schema, (s, e) =>
            {
                ConsoleManager.ShowMessage(e.Message);
                validated = false;
            });
            if (!validated)
                SaveBackupConfigFile();
            return validated;
        }

        public static void SaveBackupConfigFile()
        {
            string currentDate = DateTime.Now.ToString("yyyy-M-d");
            string currentTime = DateTime.Now.ToString("h-mm-ss");
            latestBackupFileName = "Config_Backup_" + currentDate + "_" + currentTime + ".xml";
            if (xmlDoc != null)
                xmlDoc.Save(latestBackupFileName);
        }

        public static void SaveConfigFile()
        {
            if (xmlDoc != null)
                xmlDoc.Save(CONFIG_FILE_NAME);
        }

        public static void ShowMainWindow()
        {
            App.MainAppWindow.Show();
        }
    }
}
