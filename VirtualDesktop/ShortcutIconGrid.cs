using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Linq;

namespace VirtualDesktop
{
    public class ShortcutIconGrid
    {
        public Category Category { get; set; }
        public Canvas Canvas { get; set; }
        private List<ShortcutIcon> shortcutIconList
            = new List<ShortcutIcon>();
        private List<ShortcutIcon> shortcutIconToBeAddedList
            = new List<ShortcutIcon>();
        // Key = Column no , Value = List of ShortcutIconGridPosition obj within that column
        private SortedDictionary<int, List<ShortcutIconGridPosition>> dictShortcutIconFreePos =
            new SortedDictionary<int, List<ShortcutIconGridPosition>>();

        public int NoOfRows = RowManager.MinNumberOfRows;

        public ShortcutIconGrid(Category category , Canvas canvas)
        {
            Category = category;
            Canvas = canvas;
        }

        public void AddNewRow()
        {
            AddShortcutIconFreePositionToRow(NoOfRows);
            NoOfRows++;
            Canvas.Height += RowManager.GetShortcutIconInclExtraSpaceHeight();
            ModifyNumberOfRowsXAttrValue();
        }

        public void AddShortcutIcon(ShortcutIcon shortcutIcon)
        {
            if (shortcutIconToBeAddedList.Contains(shortcutIcon))
                shortcutIconToBeAddedList.Remove(shortcutIcon);
            shortcutIconList.Add(shortcutIcon);
        }

        public void AddShortcutIconFreePosition(ShortcutIconGridPosition position)
        {
            if (position == null)
                return;

            List<ShortcutIconGridPosition> listShortcutIconGridPosition = null;
            dictShortcutIconFreePos.TryGetValue(position.ColNo , out listShortcutIconGridPosition);
            if (listShortcutIconGridPosition == null)
            {
                listShortcutIconGridPosition = new List<ShortcutIconGridPosition>();
                dictShortcutIconFreePos.Add(position.ColNo, listShortcutIconGridPosition);
            }
            else
            {
                foreach (ShortcutIconGridPosition pos in listShortcutIconGridPosition)
                {
                    if(pos.RowNo == position.RowNo && pos.ColNo == position.ColNo)
                    {
                        return;
                    }
                }
            }

            listShortcutIconGridPosition.Add(position);
            dictShortcutIconFreePos.Remove(position.ColNo);
            dictShortcutIconFreePos.Add(position.ColNo , listShortcutIconGridPosition);
            App.MainAppWindow.AddFreeSpaceCanvasToMainCanvas(Category.Index , position.RowNo , position.ColNo);
            SortShortcutIconFreePositionList(position.ColNo);
        }

        public void AddShortcutIconFreePositionToRow(int rowNo)
        {
            for(int i = 0 ; i < ColumnManager.NumberOfColumns ; i++)
            {
                ShortcutIconGridPosition pos = new ShortcutIconGridPosition(Category , rowNo , i);
                AddShortcutIconFreePosition(pos);
            }
        }

        public void DecrementNoOfRows()
        {
            if (NoOfRows - 1 >= RowManager.MinNumberOfRows)
            {
                NoOfRows--;
                ModifyNumberOfRowsXAttrValue();
            }
        }

        public void DeselectAllShortcutIcon()
        {
            foreach (var shortcutIcon in shortcutIconList)
            {
                if (shortcutIcon.IsSelected)
                {
                    ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;
                    shortcutIconBorder.Deselect();
                }
            }
        }

        public ShortcutIconGridPosition ExtractShortcutIconFreePositionIfAvailable(int rowNo , int colNo)
        {
            List<ShortcutIconGridPosition> listShortcutIconGridPosition = null;
            dictShortcutIconFreePos.TryGetValue(colNo , out listShortcutIconGridPosition);
            if (listShortcutIconGridPosition != null)
            {
                ShortcutIconGridPosition posToExtract = null;
                foreach (ShortcutIconGridPosition pos in listShortcutIconGridPosition)
                {
                    if(pos.RowNo == rowNo && pos.ColNo == colNo)
                    {
                        posToExtract = pos;
                        break;
                    }
                }
                RemoveShortcutIconFreePosition(posToExtract);
                return posToExtract;
            }
            return null;
        }
        
        public void FillAllFreeSpaceCanvas()
        {
            for (int i = 0; i < ColumnManager.NumberOfColumns; i++)
            {
                for (int j = 0; j < NoOfRows ; j++)
                {
                    ShortcutIconGridPosition pos = new ShortcutIconGridPosition(Category , j , i);
                    AddShortcutIconFreePosition(pos);
                }
            }
        }

        public int GetNumberOfSelectedShorcutIcon()
        {
            return SelectedShortcutIconList.Count;
        }

        public ShortcutIconGridPosition GetNextAvailableShortcutIconGridPositionFromStart()
        {
            foreach(KeyValuePair<int , List<ShortcutIconGridPosition>> dictEntry in dictShortcutIconFreePos)
            {
                List<ShortcutIconGridPosition> listShortcutIconGridPosition = dictEntry.Value;
                if (listShortcutIconGridPosition != null && listShortcutIconGridPosition.Count > 0)
                    return listShortcutIconGridPosition[0];
            }
            return null;
        }

        public ShortcutIconGridPosition GetNextAvailableShortcutIconGridPositionFromPosition(ShortcutIconGridPosition position)
        {
            List<ShortcutIconGridPosition> listShortcutIconGridPosition = null;
            if (dictShortcutIconFreePos.ContainsKey(position.ColNo))
            {
                dictShortcutIconFreePos.TryGetValue(position.ColNo, out listShortcutIconGridPosition);

                if(listShortcutIconGridPosition != null
                    && listShortcutIconGridPosition.Count > 0
                    && listShortcutIconGridPosition[0].RowNo > position.RowNo)
                {
                    return listShortcutIconGridPosition[0];
                }
            }
            foreach (KeyValuePair<int , List<ShortcutIconGridPosition>> dictEntry in dictShortcutIconFreePos)
            {
                if (dictEntry.Key > position.ColNo)
                {
                    listShortcutIconGridPosition = dictEntry.Value;
                    if (listShortcutIconGridPosition != null
                        && listShortcutIconGridPosition.Count > 0)
                        return listShortcutIconGridPosition[0];
                }
            }
            return null;
        }

        public ShortcutIcon GetNextShortcutIconToBeAdded()
        {
            if (shortcutIconToBeAddedList.Count > 0)
                return shortcutIconToBeAddedList[0];
            return null;
        }

        public List<ShortcutIconGridPosition> GetShortcutIconFreePositionList(int colNo)
        {
            List<ShortcutIconGridPosition> list = null;
            if (dictShortcutIconFreePos.ContainsKey(colNo))
            {
                dictShortcutIconFreePos.TryGetValue(colNo , out list);
            }
            return list;
        }

        public bool HasPositionAlreadyTaken(ShortcutIconGridPosition position)
        {
            foreach (ShortcutIcon shortcutIcon in shortcutIconList)
            {
                ShortcutIconGridPosition shortcutIconGridPos = shortcutIcon.GridPosition;
                if (shortcutIconGridPos.ColNo == position.ColNo
                    && shortcutIconGridPos.RowNo == position.RowNo)
                    return true;
            }
            return false;
        }

        public void LoadShortcutIcon(ShortcutIcon shortcutIcon)
        {
            if (shortcutIcon.ShortcutIconXElement == null)
                shortcutIcon.CreateShortcutIconXElement();
            shortcutIconToBeAddedList.Add(shortcutIcon);
        }

        public void ModifyNumberOfRowsXAttrValue()
        {
            XAttribute valueAttr = Category.RowElement.Attribute("Value");
            valueAttr.SetValue(NoOfRows);
            ConfigManager.SaveConfigFile();
        }

        public void RefreshShortcutIconGridPositionValues()
        {
            foreach (ShortcutIcon shortcutIcon in ShortcutIconList)
            {
                shortcutIcon.GridPosition.CalculateBottomPos();
                shortcutIcon.ModifyGridPosXAttrValues();
            }
        }

        public void RemoveAllShortcutIconFreePositionsOnRowNo(int rowNo)
        {
            List<ShortcutIconGridPosition> listFreePosToRemove = new List<ShortcutIconGridPosition>();
            foreach (KeyValuePair<int, List<ShortcutIconGridPosition>> dictEntry in dictShortcutIconFreePos)
            {
                List<ShortcutIconGridPosition> listFreePos = dictEntry.Value;
                if (listFreePos.Count > 0)
                {
                    ShortcutIconGridPosition freePosition = listFreePos[listFreePos.Count - 1];
                    if (freePosition.RowNo == rowNo)
                        listFreePosToRemove.Add(freePosition);
                }
            }
            foreach (ShortcutIconGridPosition pos in listFreePosToRemove)
            {
                RemoveShortcutIconFreePosition(pos);
            }
            Canvas.Height -= RowManager.GetShortcutIconInclExtraSpaceHeight();
        }

        public void RemoveSelectedShortcutIcons()
        {
            List<ShortcutIcon> selectedShortcutIcons = SelectedShortcutIconList;
            foreach (ShortcutIcon shortcutIcon in selectedShortcutIcons)
            {
                shortcutIcon.Remove();
            }
        }

        public void RemoveShortcutIcon(ShortcutIcon shortcutIcon)
        {
            shortcutIconList.Remove(shortcutIcon);
        }

        public void RemoveShortcutIconFreePosition(ShortcutIconGridPosition position)
        {
            if (position == null)
                return;

            int rowNo = position.RowNo;
            int colNo = position.ColNo;

            List<ShortcutIconGridPosition> listFreeGridPosition = null;
            dictShortcutIconFreePos.TryGetValue(colNo , out listFreeGridPosition);
            if (listFreeGridPosition != null && listFreeGridPosition.Count > 0)
            {
                ShortcutIconGridPosition posObjToRemove = null;
                foreach (ShortcutIconGridPosition pos in listFreeGridPosition)
                {
                    if (pos.RowNo == rowNo && pos.ColNo == colNo)
                    {
                        FreeSpaceCanvas freeSpaceCanvasToRemove =
                        FreeSpaceCanvas.GetFreeSpaceCanvas(Category.Index , rowNo , colNo);
                        if (freeSpaceCanvasToRemove != null)
                        {
                            freeSpaceCanvasToRemove.RemoveControlFromMainCanvas();
                        }
                        posObjToRemove = pos;
                        break;
                    }
                }
                if(posObjToRemove != null)
                {
                    listFreeGridPosition.Remove(posObjToRemove);
                    dictShortcutIconFreePos.Remove(colNo);
                    dictShortcutIconFreePos.Add(colNo , listFreeGridPosition);
                }
            }
        }

        public void RemoveUnusedLastRowsIfGreaterThanMin()
        {
            bool continueLoop = true;
            while (NoOfRows > RowManager.MinNumberOfRows && continueLoop)
            {
                bool found = false;
                int lastRowNo = NoOfRows - 1;
                foreach (ShortcutIcon shortcutIcon in ShortcutIconList)
                {
                    ShortcutIconGridPosition gridPos = shortcutIcon.GridPosition;
                    if (gridPos.RowNo == lastRowNo)
                    {
                        found = true;
                        continueLoop = false;
                        break;
                    }
                }
                if (!found)
                {
                    RemoveAllShortcutIconFreePositionsOnRowNo(lastRowNo);
                    DecrementNoOfRows();
                }
            }
        }

        public void RestoreAllShortcutIconBorderMouseEnterAndLeaveEvent()
        {
            foreach (ShortcutIcon shortcutIcon in shortcutIconList)
            {
                ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;
                shortcutIconBorder.AddHandler(Border.MouseEnterEvent, shortcutIconBorder.MouseEnterEvent);
                shortcutIconBorder.AddHandler(Border.MouseLeaveEvent, shortcutIconBorder.MouseLeaveEvent);
            }
        }

        public void SelectAllShortcutIcon()
        {
            foreach (var shortcutIcon in shortcutIconList)
            {
                if (!shortcutIcon.IsSelected)
                {
                    ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;
                    shortcutIconBorder.Select();
                }
            }
        }

        public void SetNoOfRows(int noOfRows)
        {
            if (noOfRows > RowManager.MinNumberOfRows)
            {
                int extraNoOfRows = noOfRows - RowManager.MinNumberOfRows;

                for(int i = 0 ; i < extraNoOfRows ; i++)
                {
                    AddNewRow();
                }
            }
            else if (noOfRows == RowManager.MinNumberOfRows)
            {
                Canvas.Height = App.MainAppWindow.gridPanel.Height;
            }
            else
            {
                NoOfRows = RowManager.MinNumberOfRows;
                return;
            }
            NoOfRows = noOfRows;
        }

        public void SortAllShortcutIconFreePositionList()
        {
            foreach (KeyValuePair<int, List<ShortcutIconGridPosition>> dictEntry in dictShortcutIconFreePos)
            {
                List<ShortcutIconGridPosition> listShortcutIconGridPosition = dictEntry.Value;
                listShortcutIconGridPosition = listShortcutIconGridPosition.OrderBy(o => o.ColNo).ThenBy(o => o.RowNo).ToList();
            }
        }

        public void SortShortcutIconFreePositionList(int colNo)
        {
            if (dictShortcutIconFreePos.ContainsKey(colNo))
            {
                List<ShortcutIconGridPosition> listShortcutIconGridPosition = null;
                dictShortcutIconFreePos.TryGetValue(colNo , out listShortcutIconGridPosition);
                listShortcutIconGridPosition = listShortcutIconGridPosition.OrderBy(o => o.ColNo).ThenBy(o => o.RowNo).ToList();
                dictShortcutIconFreePos.Remove(colNo);
                dictShortcutIconFreePos.Add(colNo , listShortcutIconGridPosition);
            }  
        }

        public void SortShortcutIconList(string sortBy)
        {
            foreach (ShortcutIcon shortcutIcon in shortcutIconList)
            {
                if(shortcutIcon.ShortcutIconBorder != null)
                    Canvas.Children.Remove(shortcutIcon.ShortcutIconBorder);
                AddShortcutIconFreePosition(shortcutIcon.GridPosition);
                shortcutIcon.GridPosition = null;
                LoadShortcutIcon(shortcutIcon);
            }

            shortcutIconList.Clear();

            if (sortBy.Equals("name"))
            {
                shortcutIconToBeAddedList = shortcutIconToBeAddedList.OrderBy(o => o.LabelText).ToList();
            }
        }

        public List<ShortcutIcon> SelectedShortcutIconList
        {
            get
            {
                List<ShortcutIcon> listSelectedShortcutIcon = new List<ShortcutIcon>();
                foreach (ShortcutIcon shortcutIcon in shortcutIconList)
                {
                    if (shortcutIcon.IsSelected)
                    {
                        listSelectedShortcutIcon.Add(shortcutIcon);
                    }
                }
                return listSelectedShortcutIcon;
            }
        }

        public List<ShortcutIcon> SelectedShortcutIconListInReverseOrder
        {
            get
            {
                List<ShortcutIcon> listSelectedShortcutIcon = new List<ShortcutIcon>();
                for(int i = shortcutIconList.Count - 1 ; i >= 0 ; i--)
                {
                    ShortcutIcon shortcutIcon = shortcutIconList[i];
                    if (shortcutIcon.IsSelected)
                    {
                        listSelectedShortcutIcon.Add(shortcutIcon);
                    }
                }
                return listSelectedShortcutIcon;
            }
        }

        public List<ShortcutIcon> ShortcutIconList
        {
            get
            {
                return shortcutIconList;
            }
        }
    }
}
