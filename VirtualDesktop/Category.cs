using System.Collections.Generic;
using System.Xml.Linq;

namespace VirtualDesktop
{
    public class Category
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; } = null;
        public ShortcutIconGrid ShortcutIconGrid { get; set; }
        private bool mainType = false;
        private bool mixedType = false;
        public CategoryBorder CategoryBorder { get; set; }
        public CategoryContextMenuItem MainTypeContextMenuItem { get; set; }
        public CategoryContextMenuItem MixedTypeContextMenuItem { get; set; }
        public CategoryContextMenuItem ManageFileExtMenuItem { get; set; }
        public CategoryContextMenuItem RemoveContextMenuItem { get; set; }

        public MoveCategoryContextMenuItem MoveCategoryContextMenuItem { get; set; }

        public Category NextCategory { get; set; } = null;
        public Category PreviousCategory { get; set; } = null;

        private bool selected = false;

        public XElement CategoryXElement { get; set; }
        public XElement RowElement { get; set; }

        public Category(XElement categoryElement)
        {
            CategoryXElement = categoryElement;
        }

        public void CreateCategoryXElement()
        {
            if (CategoryXElement == null)
            {
                CategoryXElement = new XElement("Category");
                List<XAttribute> listCategoryAttributes = new List<XAttribute>();
                listCategoryAttributes.Add(new XAttribute("Name", Name));
                listCategoryAttributes.Add(new XAttribute("Image" , ImagePath==null?"":ImagePath));
                listCategoryAttributes.Add(new XAttribute("MainType" , mainType));
                listCategoryAttributes.Add(new XAttribute("MixedType" , mixedType));
                foreach (XAttribute attribute in listCategoryAttributes)
                {
                    CategoryXElement.Add(attribute);
                }
                XElement rowSettingsElement = new XElement("RowSettings");
                RowElement = new XElement("Row"
                      , new XAttribute("ScreenProfileId", ConfigManager.MainScreenProfileId)
                      , new XAttribute("Value", ShortcutIconGrid.NoOfRows));
                rowSettingsElement.Add(RowElement);
                CategoryXElement.Add(rowSettingsElement);
                ConfigManager.RootElement.Add(CategoryXElement);
                ConfigManager.SaveConfigFile();
            }
        }

        public void FetchRowXElement()
        {
            XElement rowSettingsElement = CategoryXElement.Element("RowSettings");
            foreach (XElement element in rowSettingsElement.Elements("Row"))
            {
                XAttribute screenProfIdAttr = element.Attribute("ScreenProfileId");
                XAttribute valueAttr = element.Attribute("Value");

                int screenProfId = -1;
                int.TryParse(screenProfIdAttr.Value , out screenProfId);

                if (screenProfId == ConfigManager.MainScreenProfileId)
                {
                    RowElement = element;
                    break;
                }
            }
        }

        public bool IsMainType
        {
            get
            {
                return mainType;
            }
            set
            {
                mainType = value;
            }
        }

        public bool IsMixedType
        {
            get
            {
                return mixedType;
            }
            set
            {
                mixedType = value;
            }
        }

        public bool IsSelected
        {
            get { return selected; }
            set { selected = value; }
        }

        public void MarkAsMainType()
        {
            MainTypeContextMenuItem.IsChecked = true;
            MixedTypeContextMenuItem.IsChecked = true;
            mainType = true;
            mixedType = true;
            CategoryManager.MainTypeCategory = this;
            ModifyMainTypeXAttrValue();
            ModifyMixedTypeXAttrValue();
        }

        public void MarkAsMixedType()
        {
            mixedType = true;
            ModifyMixedTypeXAttrValue();
        }

        public void ModifyImageXAttrValue()
        {
            CategoryXElement.Attribute("Image").SetValue(ImagePath==null ? "" : ImagePath);
            ConfigManager.SaveConfigFile();
        }

        public void ModifyNameXAttrValue()
        {
            CategoryXElement.Attribute("Name").SetValue(Name);
            ConfigManager.SaveConfigFile();
        }

        public void ModifyMainTypeXAttrValue()
        {
            XAttribute mainTypeAttr = CategoryXElement.Attribute("MainType");
            bool mainTypeAttrVal = false;
            bool.TryParse(mainTypeAttr.Value, out mainTypeAttrVal);
            if (mainTypeAttrVal != mainType)
            {
                mainTypeAttr.SetValue(mainType);
                ConfigManager.SaveConfigFile();
            }
        }

        public void ModifyMixedTypeXAttrValue()
        {
            XAttribute mixedTypeAttr = CategoryXElement.Attribute("MixedType");
            bool mixedTypeAttrVal = false;
            bool.TryParse(mixedTypeAttr.Value, out mixedTypeAttrVal);
            if (mixedTypeAttrVal != mixedType)
            {
                mixedTypeAttr.SetValue(mixedType);
                ConfigManager.SaveConfigFile();
            }
        }

        public void RemoveCategoryXElementFromConfigFile()
        {
            CategoryXElement.Remove();
            ConfigManager.SaveConfigFile();
        }

        public void ToggleMainType()
        {
            if (!mainType)
            {
                MarkAsMainType();
            }
            else
            {
                UnmarkAsMainType();
            }
        }

        public void ToggleMixedType()
        {
            if (!mixedType)
            {
                MarkAsMixedType();
            }
            else
            {
                UnmarkAsMixedType();
            }
        }

        public void UnmarkAsMainType()
        {
            if (mainType)
            {
                MainTypeContextMenuItem.IsChecked = false;
                mainType = false;
                CategoryManager.MainTypeCategory = null;
                ModifyMainTypeXAttrValue();
            }
        }

        public void UnmarkAsMixedType()
        {
            MainTypeContextMenuItem.IsChecked = false;
            MixedTypeContextMenuItem.IsChecked = false;
            mixedType = false;
            ModifyMixedTypeXAttrValue();
        }
    }
}