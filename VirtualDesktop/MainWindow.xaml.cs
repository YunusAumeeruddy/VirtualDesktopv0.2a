using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SliderWindow SliderWindow;

        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private bool IsWinVisible = true;
        
        public string BackgroundImagePath = null;

        private ShortcutIconBorder lastSelectedShortcutIconBorder = null;

        public static FreeSpaceCanvas StartFSCanvas = null;
        public static System.Windows.Shapes.Rectangle
            MultiShortcutIconSelectRect { get; set; } = null;
        private Point freeSpaceCanvas_ClickPos = new Point(-1,-1);

        private static ContextMenu FreeSpaceContextMenu = null;
        private static MenuItem SetBackgroundImgMenuItem = null;
        private static MenuItem RemoveBackgroundImgMenuItem = null;

        public MainWindow()
        {
            InitializeComponent();
            SetAppIcon();
            SliderWindow = new SliderWindow(this);
            ResizeUIControls();
            GenerateFreeSpaceContextMenu();

            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            MouseHook.OnMouseUp += MouseHook_OnMouseUp;
        }

        // CUSTOM USER METHODS

        public void CreateNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("AppIcon.ico");
            notifyIcon.Text = "Virtual Desktop";
            notifyIcon.Click += NotifyIcon_Click;    

            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem showAvailableGridPosItem = new System.Windows.Forms.MenuItem();
            showAvailableGridPosItem.Text = "Show available grid positions";
            showAvailableGridPosItem.Click += NotifyIcon_ShowAvailableGridPos_Click;

            System.Windows.Forms.MenuItem exitMenuItem = new System.Windows.Forms.MenuItem();
            exitMenuItem.Text = "Exit";
            exitMenuItem.Click += delegate {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                App.ExitApp();
            };

            contextMenu.MenuItems.Add(showAvailableGridPosItem);
            contextMenu.MenuItems.Add(exitMenuItem);
            notifyIcon.ContextMenu = contextMenu;

            notifyIcon.Visible = true;
        }

        public void DefreezeWindow()
        {
            IsEnabled = true;
            OverlayWindow.MainOverlayWindow.Close();
            Focus();
        }

        public void FreezeWindow()
        {
            IsEnabled = false;
            new OverlayWindow().Show();
        }

        public void GenerateFreeSpaceContextMenu()
        {
            FreeSpaceContextMenu = new ContextMenu();
            SetBackgroundImgMenuItem = new MenuItem();
            SetBackgroundImgMenuItem.Header = "Set background image";
            SetBackgroundImgMenuItem.Click += FreeSpace_SetBackgroundImage_Click;

            RemoveBackgroundImgMenuItem = new MenuItem();
            RemoveBackgroundImgMenuItem.Header = "Remove background image";
            RemoveBackgroundImgMenuItem.Click += FreeSpace_RemoveBackgroundImage_Click;

            FreeSpaceContextMenu.Items.Add(SetBackgroundImgMenuItem);
            FreeSpaceContextMenu.Items.Add(RemoveBackgroundImgMenuItem);

            gridPanel.ContextMenu = FreeSpaceContextMenu;
        }

        private void SetAppIcon()
        {
            string iconPath = "AppIcon.ico";
            Uri iconUri = new Uri(iconPath , UriKind.Relative);
            this.Icon = BitmapFrame.Create(iconUri);
        }

        public void SetWinPosition()
        {
            Top = SliderWindow.Top + SliderWindow.Height;
            Left = 0;
            if (TaskbarUtils.LeftArea)
            {
                Left = TaskbarUtils.TaskBarWidth;
            }
        }

        public void ResizeUIControls()
        {
            ResizeWindow();
            ResizeGridPanel();
            Resize_scroll_ShortcutIconGridCanvas();
        }

        private void ResizeWindow()
        {
            Width = ScreenWorkAreaUtils.ScreenWorkAreaWidth;
            Height = ScreenWorkAreaUtils.ScreenWorkAreaHeight - SliderWindow.Height;
            MinWidth = Width;
            MaxWidth = Width;
            MinHeight = Height;
            MaxHeight = Height;
        }

        private void ResizeGridPanel()
        {
            gridPanel.Width = Width - (gridPanel.Margin.Left * 2);
            gridPanel.Height = Height;
        }

        private void Resize_scroll_ShortcutIconGridCanvas()
        {
            scroll_ShortcutIconGridCanvas.Width = gridPanel.Width
                                                - scroll_ShortcutIconGridCanvas.Margin.Left;
            scroll_ShortcutIconGridCanvas.Height = gridPanel.Height;
        }

        public void ResizeShortcutIconGridCanvas(Canvas canvas)
        {
            canvas.Width = gridPanel.Width;
            canvas.Height = gridPanel.Height;
        }

        public void PrepareShortcutIconGrid()
        {
            RowManager.CalculateMinNumberOfRows();
            RowManager.CalculateShortcutIconExtraVerticalSpace();
            ColumnManager.CalculateMinNumberOfColumns();
            ColumnManager.CalculateShortcutIconExtraHorizontalSpace();
        }

        public void SetBackgroundImage(bool updateConfigFile)
        {
            if (BackgroundImagePath == null
                || (!System.IO.File.Exists(BackgroundImagePath)
                && !InternalResourceManager.HasResource(BackgroundImagePath)))
            {
                BrushConverter bc = new BrushConverter();
                Brush brush = (Brush)bc.ConvertFrom("#003566");
                brush.Freeze();
                gridPanel.Background = brush;
                RemoveBackgroundImgMenuItem.IsEnabled = false;
                goto SaveChangesToConfigFile;
            }
            ImageBrush backgroundBrush = new ImageBrush();
            backgroundBrush.ImageSource =
                new BitmapImage(new Uri(@BackgroundImagePath , UriKind.Absolute));
            gridPanel.Background = backgroundBrush;
            RemoveBackgroundImgMenuItem.IsEnabled = true;
            SaveChangesToConfigFile:
            if (updateConfigFile)
            {
                ConfigManager.BackgroundElement.Attribute("ImagePath").SetValue(
                    BackgroundImagePath == null ? "" : BackgroundImagePath);
                ConfigManager.SaveConfigFile();
            }
        }

        public void AddFreeSpaceCanvasToMainCanvas(int categoryIndex , int rowNo , int colNo)
        {
            string freeSpaceCanvasName = FreeSpaceCanvas.GetFormattedName(categoryIndex
                                                                            , rowNo
                                                                            , colNo);
            if (FrameworkElementReferenceManager.ExistFrameworkElement(freeSpaceCanvasName))
                return;

            Category category = CategoryManager.GetCategory(categoryIndex);
            ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;

            Canvas mainCanvas = shortcutIconGrid.Canvas;

            ShortcutIconGridPosition shortcutIconPosition = new ShortcutIconGridPosition(category , rowNo , colNo);
                    
            FreeSpaceCanvas freeSpaceCanvas = new FreeSpaceCanvas(categoryIndex , shortcutIconPosition , mainCanvas);
            freeSpaceCanvas.ReformatName();
            double freeSpaceCanvasLeft = ColumnManager.GetShortcutIconCanvasLeftMargin(colNo);
            double freeSpaceCanvasTop = RowManager.GetShortcutIconCanvasTopMargin(rowNo);
            Canvas.SetLeft(freeSpaceCanvas, freeSpaceCanvasLeft);
            Canvas.SetTop(freeSpaceCanvas, freeSpaceCanvasTop);
            freeSpaceCanvas.Width = ColumnManager.CellWidth;
            freeSpaceCanvas.Height = RowManager.CellHeight;
            freeSpaceCanvas.Background = FreeSpaceCanvas.CurrentBackgroundBrush;
            freeSpaceCanvas.AllowDrop = true;

            freeSpaceCanvas.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(FreeSpace_PreviewMouseLeftButtonDown));
            freeSpaceCanvas.AddHandler(PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(All_Controls_PreviewMouseLeftButtonUp));
            freeSpaceCanvas.AddHandler(MouseMoveEvent, new MouseEventHandler(FreeSpace_MouseMove));
            freeSpaceCanvas.AddHandler(DragEnterEvent, new DragEventHandler(Canvas_FreeSpace_DragEnter));
            freeSpaceCanvas.AddHandler(DragLeaveEvent, new DragEventHandler(Canvas_FreeSpace_DragLeave));
            freeSpaceCanvas.AddHandler(DropEvent, new DragEventHandler(Canvas_FreeSpace_Drop));

            
            freeSpaceCanvas.ContextMenu = FreeSpaceContextMenu;

            mainCanvas.Children.Add(freeSpaceCanvas);
            FrameworkElementReferenceManager.AddNewFrameworkElement(freeSpaceCanvas);
        }

        private ShortcutIconBorder GenerateShortcutIconBorder(ShortcutIcon shortcutIcon)
        {
            if (shortcutIcon == null)
                return null;

            ShortcutIconBorder shortcutIconBorder = ShortcutIconBorder.GenerateShortcutIconBorderWOEvent(shortcutIcon);
            shortcutIconBorder.ReformatName();
            shortcutIconBorder.PositionOnGrid();
            shortcutIconBorder.AllowDrop = true;

            shortcutIcon.ShortcutIconBorder = shortcutIconBorder;

            shortcutIconBorder.MouseEnterEvent = new MouseEventHandler(ShortcutIconBorder_MouseEnter);
            shortcutIconBorder.MouseLeaveEvent = new MouseEventHandler(ShortcutIconBorder_MouseLeave);
            shortcutIconBorder.PreviewMouseLeftButtonDownEvent = new MouseButtonEventHandler(ShortcutIconBorder_PreviewMouseLeftButtonDown);
            shortcutIconBorder.PreviewMouseRightButtonDownEvent = new MouseButtonEventHandler(ShortcutIconBorder_PreviewMouseRightButtonDown);
            shortcutIconBorder.PreviewDragEnterEvent = new DragEventHandler(ShortcutIconBorder_PreviewDragEnter);
            shortcutIconBorder.PreviewDragLeaveEvent = new DragEventHandler(ShortcutIconBorder_PreviewDragLeave);
            shortcutIconBorder.PreviewDropEvent = new DragEventHandler(ShortcutIconBorder_PreviewDrop);

            shortcutIconBorder.AddHandler(MouseEnterEvent, shortcutIconBorder.MouseEnterEvent);
            shortcutIconBorder.AddHandler(MouseLeaveEvent, shortcutIconBorder.MouseLeaveEvent);
            shortcutIconBorder.AddHandler(PreviewMouseLeftButtonDownEvent, shortcutIconBorder.PreviewMouseLeftButtonDownEvent);
            shortcutIconBorder.AddHandler(PreviewMouseRightButtonDownEvent, shortcutIconBorder.PreviewMouseRightButtonDownEvent);
            shortcutIconBorder.AddHandler(PreviewDragEnterEvent, shortcutIconBorder.PreviewDragEnterEvent);
            shortcutIconBorder.AddHandler(PreviewDragLeaveEvent, shortcutIconBorder.PreviewDragLeaveEvent);
            shortcutIconBorder.AddHandler(PreviewDropEvent, shortcutIconBorder.PreviewDropEvent);

            ContextMenu contextMenu = new ContextMenu();

            MenuItem open_ShortcutIcon_menuItem = new MenuItem();
            open_ShortcutIcon_menuItem.Header = "Open";
            open_ShortcutIcon_menuItem.Click += delegate {
                WinFileOperations.OpenFile(shortcutIcon.ShortcutFilePathInfo.FullName);
            };

            MenuItem relocate_ShortcutIcon_menuItem = new MenuItem();
            relocate_ShortcutIcon_menuItem.Header = "Relocate shortcut icon";
            relocate_ShortcutIcon_menuItem.Click += delegate {
                shortcutIcon.Relocate();
            };

            MenuItem rename_ShortcutIcon_menuItem = new MenuItem();
            rename_ShortcutIcon_menuItem.Header = "Rename";
            rename_ShortcutIcon_menuItem.Click += delegate {
                FreezeWindow();
                new RenameShortcutIconWindow(shortcutIcon).Show();
            };

            MenuItem delete_ShortcutIcon_menuItem = new MenuItem();
            delete_ShortcutIcon_menuItem.Header = "Delete";
            delete_ShortcutIcon_menuItem.Click += delegate {
                RemoveSelectedShortcutIconBorderFromCurrentGrid();
            };

            MenuItem properties_ShortcutIcon_menuItem = new MenuItem();
            properties_ShortcutIcon_menuItem.Header = "Properties";
            properties_ShortcutIcon_menuItem.Click += delegate {
                WinFileProperties.ShowFileProperties(shortcutIcon.ShortcutFilePathInfo.FullName);
            };

            contextMenu.Items.Add(open_ShortcutIcon_menuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(relocate_ShortcutIcon_menuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(delete_ShortcutIcon_menuItem);
            contextMenu.Items.Add(rename_ShortcutIcon_menuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(properties_ShortcutIcon_menuItem);

            shortcutIconBorder.ContextMenu = contextMenu;

            ToolTip toolTip = new ToolTip();
            toolTip.Content = shortcutIcon.LabelText;
            shortcutIconBorder.ToolTip = toolTip;

            return shortcutIconBorder;
        }

        private void AddShortcutIconToGrid(ShortcutIcon shortcutIcon)
        {
            Category category = shortcutIcon.Category;
            ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;

            ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;
            if (shortcutIconBorder == null)
            {
                shortcutIconBorder = GenerateShortcutIconBorder(shortcutIcon);
            }
            else
            {
                shortcutIconBorder.ReformatName();
                shortcutIconBorder.PositionOnGrid();
            }

            shortcutIconGrid.Canvas.Children.Add(shortcutIconBorder);
        }

        public void PopulateSelectedShortcutIconGrid()
        {
            PopulateShortcutIconGrid(CategoryManager.GetSelectedCategory());
        }

        public void PopulateShortcutIconGrid(Category category)
        {
            ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;

            ShortcutIcon shortcutIcon = null;
            while ((shortcutIcon = shortcutIconGrid.GetNextShortcutIconToBeAdded()) != null)
            {
                ShortcutIconGridPosition position = shortcutIcon.GridPosition;
                if (position == null)
                {
                    position = shortcutIconGrid.GetNextAvailableShortcutIconGridPositionFromStart();
                    shortcutIcon.GridPosition = position;
                }
                else
                {
                    if (shortcutIconGrid.HasPositionAlreadyTaken(position))
                    {
                        position = shortcutIconGrid.GetNextAvailableShortcutIconGridPositionFromPosition(position);
                        if(position == null)
                            position = shortcutIconGrid.GetNextAvailableShortcutIconGridPositionFromStart();
                    }
                }

                if(position == null)
                {
                    shortcutIconGrid.AddNewRow();
                    position = shortcutIconGrid.GetNextAvailableShortcutIconGridPositionFromStart();
                }

                shortcutIcon.GridPosition = position;
                shortcutIconGrid.RemoveShortcutIconFreePosition(position);
                AddShortcutIconToGrid(shortcutIcon);
                shortcutIconGrid.AddShortcutIcon(shortcutIcon);
                shortcutIcon.CreatePositionXElement();

                ActionSet actionSet = new ActionSet();
                actionSet.AddAction(new RemoveFreeShortcutIconPositionAction(shortcutIconGrid , position));
                actionSet.AddAction(new AddShortcutIconAction(shortcutIcon));
                ActionManager.AddNewActionSet(actionSet);
            }
            shortcutIconGrid.RefreshShortcutIconGridPositionValues();
        }

        public void RemoveLastShortcutIconGridCanvas()
        {
            ShortcutIconGrid selectedShortcutIconGrid =
                CategoryManager.GetSelectedShortcutIconGrid();
            gridPanel.Children.Remove(selectedShortcutIconGrid.Canvas);
        }

        public void RemoveMultiShortcutIconSelectionRectangle()
        {
            if (MultiShortcutIconSelectRect != null)
            {
                CategoryManager.GetSelectedShortcutIconGrid().Canvas.Children.Remove(MultiShortcutIconSelectRect);
                MultiShortcutIconSelectRect = null;
            }
        }

        public void RemoveSelectedShortcutIconBorderFromCurrentGrid()
        {
            Category category = CategoryManager.GetSelectedCategory();
            ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;
            List<ShortcutIcon> listSelectedShortcutIcon = shortcutIconGrid.SelectedShortcutIconList;

            if (listSelectedShortcutIcon.Count == 0)
                return;

            string msgBoxTitle = "Remove selected shortcut icon";
            string msgBoxQuestion = "Are you sure you want to remove the selected shortcut icons?";
            if (listSelectedShortcutIcon.Count > 1)
            {
                msgBoxTitle += "s [Total: " + listSelectedShortcutIcon.Count + "]";
            }
            else
            {
                msgBoxQuestion = "Are you sure you want to remove the \""
                                 + listSelectedShortcutIcon[0].LabelText
                                 + "\" shortcut icon from the current grid?";
            }

            MessageBoxResult result = MessageBox.Show(msgBoxQuestion
                                                      , msgBoxTitle
                                                      , MessageBoxButton.YesNo
                                                      , MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                shortcutIconGrid.RemoveSelectedShortcutIcons();
                if (ConfigManager.AutoSortMode)
                    SortCategoryShortcutIcons(category);
                shortcutIconGrid.RefreshShortcutIconGridPositionValues();
            }
        }

        public void SelectMultiShortcutIconBordersWithinRect()
        {
            if (MultiShortcutIconSelectRect == null)
            {
                return;
            }

            double rectLeft = Canvas.GetLeft(MultiShortcutIconSelectRect);
            double rectTop = Canvas.GetTop(MultiShortcutIconSelectRect);
            double rectWidth = MultiShortcutIconSelectRect.Width;
            double rectHeight = MultiShortcutIconSelectRect.Height;

            rectLeft = MathUtils.GetDoubleValue(rectLeft);
            rectTop = MathUtils.GetDoubleValue(rectTop);
            rectWidth = MathUtils.GetDoubleValue(rectWidth);
            rectHeight = MathUtils.GetDoubleValue(rectHeight);

            double rectLeftAddWidth = rectLeft + rectWidth;
            double rectTopAddHeight = rectTop + rectHeight;

            foreach (ShortcutIcon shortcutIcon in CategoryManager.GetSelectedShortcutIconGrid().ShortcutIconList)
            {
                bool isShortcutIconSelected = shortcutIcon.IsSelected;
                ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;

                double shortcutIconLeft = Canvas.GetLeft(shortcutIconBorder);
                double shortcutIconTop = Canvas.GetTop(shortcutIconBorder);
                double shortcutIconLeftAddWidth = shortcutIconLeft + shortcutIconBorder.Width;
                double shortcutIconTopAddHeight = shortcutIconTop + shortcutIconBorder.Height;

                // To the left of rectangle
                bool outOfBoundsCond1 = shortcutIconLeftAddWidth < rectLeft;
                // To the right of rectangle
                bool outOfBoundsCond2 = shortcutIconLeft > rectLeftAddWidth;
                // To the top of rectangle
                bool outOfBoundsCond3 = shortcutIconTopAddHeight < rectTop;
                // To the bottom of rectangle
                bool outOfBoundsCond4 = shortcutIconTop > rectTopAddHeight;

                if (!outOfBoundsCond1 && !outOfBoundsCond2
                    && !outOfBoundsCond3 && !outOfBoundsCond4)
                {
                    shortcutIconBorder.Select();
                }
            }
            SliderWindow.RefreshBottomDockPanelButtons();
        }

        public void ShowHideWindow()
        {
            if (IsWinVisible)
            {
                Hide();
                SliderWindow.Hide();
            }
            else
            {
                Show();
                SliderWindow.Show();
            }
            IsWinVisible = !IsWinVisible;
        }

        public void ShowSelectedShortcutIconGridCanvas()
        {
            ShortcutIconGrid shortcutIconGrid = CategoryManager.GetSelectedShortcutIconGrid();
            scroll_ShortcutIconGridCanvas.Content = shortcutIconGrid.Canvas;
            PopulateSelectedShortcutIconGrid();
        }

        public void SwapShortcutIconBorders(ShortcutIconBorder border1 , ShortcutIconBorder border2)
        {
            if (border1 == null || border2 == null || border1.Name.Equals(border2.Name))
                return;

            ShortcutIconGridPosition border1Pos = border1.ShortcutIcon.GridPosition;
            border1.ShortcutIcon.GridPosition = border2.ShortcutIcon.GridPosition;
            border2.ShortcutIcon.GridPosition = border1Pos;

            border1.PositionOnGrid();
            border2.PositionOnGrid();
            border1.ReformatName();
            border2.ReformatName();

            border1.ShortcutIcon.ModifyGridPosXAttrValues();
            border2.ShortcutIcon.ModifyGridPosXAttrValues();
        }

        public void SortAllCategoryShortcutIcons()
        {
            Category selectedCategory = CategoryManager.GetSelectedCategory();
            SortCategoryShortcutIcons(selectedCategory);
            foreach (Category category in CategoryManager.CategoryListSortedByIndex)
            {
                if (category != selectedCategory)
                {
                    SortCategoryShortcutIcons(category);
                }
            }
        }

        public void SortCategoryShortcutIcons(Category category)
        {
            string sortBy = ConfigManager.AutoSortBy;
            ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;
            shortcutIconGrid.SortShortcutIconList(sortBy);
            PopulateShortcutIconGrid(category);
        }

        private void MoveShortcutIconBorderGridPosition(ShortcutIconBorder shortcutIconBorder
                                            , int noOfRowsToMove
                                            , int noOfColsToMove)
        {
            if (noOfRowsToMove == 0 && noOfColsToMove == 0)
                return;

            Category category = CategoryManager.GetSelectedCategory();
            ShortcutIconGrid shortcutIconGrid = category.ShortcutIconGrid;
            ShortcutIcon shortcutIcon = shortcutIconBorder.ShortcutIcon;
            ShortcutIconGridPosition originalPosition = shortcutIcon.GridPosition;
            int expectedNewRowNo = originalPosition.RowNo + noOfRowsToMove;
            int expectedNewColNo = originalPosition.ColNo + noOfColsToMove;

            ShortcutIconGridPosition newPosition = null;

            bool hasNewPositionExtracted = false;

            if (expectedNewColNo >= 0
                && expectedNewColNo < ColumnManager.NumberOfColumns
                && expectedNewRowNo >= 0
                && expectedNewRowNo < shortcutIconGrid.NoOfRows)
            {
                newPosition = shortcutIconGrid.ExtractShortcutIconFreePositionIfAvailable(expectedNewRowNo , expectedNewColNo);
            }

            if (newPosition == null)
            {
                newPosition = new ShortcutIconGridPosition(category, expectedNewRowNo, expectedNewColNo);
                newPosition = shortcutIconGrid.GetNextAvailableShortcutIconGridPositionFromPosition(newPosition);
                if (newPosition != null)
                {
                    expectedNewRowNo = newPosition.RowNo;
                    expectedNewColNo = newPosition.ColNo;
                }
                else
                {
                    newPosition = shortcutIconGrid.GetNextAvailableShortcutIconGridPositionFromStart();
                    if (newPosition != null)
                    {
                        int newColNo = newPosition.ColNo;
                        int newRowNo = newPosition.RowNo;
                        if (newColNo > originalPosition.ColNo
                            || (newColNo == originalPosition.ColNo
                                && newRowNo >= originalPosition.RowNo))
                            newPosition = null;
                    }
                }
            }
            else
            {
                hasNewPositionExtracted = true;
            }

            if (newPosition != null)
            {
                if (!hasNewPositionExtracted)
                {
                    shortcutIconGrid.RemoveShortcutIconFreePosition(newPosition);
                }

                shortcutIcon.GridPosition = newPosition;
                shortcutIcon.ShortcutIconBorder.PositionOnGrid();
                shortcutIcon.ShortcutIconBorder.ReformatName();
                shortcutIcon.ModifyGridPosXAttrValues();
            }
        }

        // EVENTS

        // Multiple controls Events

        private void MouseHook_OnMouseUp(object sender, Point p)
        {
            if (StartFSCanvas != null && MultiShortcutIconSelectRect != null)
            {
                FreeSpaceCanvas endFSCanvas = sender as FreeSpaceCanvas;
                if (endFSCanvas != null && StartFSCanvas.Name.Equals(endFSCanvas.Name))
                {
                    goto DoNotSelectMultiShortcutIconBordersWithinRect;
                }
            }
            SelectMultiShortcutIconBordersWithinRect();
            DoNotSelectMultiShortcutIconBordersWithinRect:
            RemoveMultiShortcutIconSelectionRectangle();
            StartFSCanvas = null;
        }

        public void All_Controls_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (StartFSCanvas != null && MultiShortcutIconSelectRect != null)
            {
                FreeSpaceCanvas endFSCanvas = sender as FreeSpaceCanvas;
                if (endFSCanvas != null && StartFSCanvas.Name.Equals(endFSCanvas.Name))
                {
                    goto DoNotSelectMultiShortcutIconBordersWithinRect;
                }
            }
            SelectMultiShortcutIconBordersWithinRect();
            DoNotSelectMultiShortcutIconBordersWithinRect:
            RemoveMultiShortcutIconSelectionRectangle();
            StartFSCanvas = null;
        }

        // Window Events

        private void Window_KeyDown(object sender , KeyEventArgs e)
        {
            ShortcutIconGrid shortcutIconGrid = CategoryManager.GetSelectedShortcutIconGrid();
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)
                && Keyboard.IsKeyDown(Key.A))
            {
                shortcutIconGrid.SelectAllShortcutIcon();
            }
            else if (Keyboard.IsKeyDown(Key.Delete))
            {
                if (shortcutIconGrid.GetNumberOfSelectedShorcutIcon() > 0)
                {
                    RemoveMultiShortcutIconSelectionRectangle();
                }
            }
        }

        private void Window_MouseEnter(object sender , MouseEventArgs e)
        {
            SliderWindow.ShowHideMenu(false);
        }

        // Notify Icon Events

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            ShowHideWindow();
        }

        private void NotifyIcon_ShowAvailableGridPos_Click(object sender, EventArgs e)
        {
            ShowHideWindow();
            System.Windows.Forms.MenuItem showAvailableGridPosItem = sender as System.Windows.Forms.MenuItem;
            FreeSpaceCanvas.ToggleFreeSpaceCanvasVisibility();
            if (FreeSpaceCanvas.IsFreeSpaceCanvasVisible)
            {
                showAvailableGridPosItem.Checked = true;
            }
            else
            {
                showAvailableGridPosItem.Checked = false;
            }
        }

        // Shortcun Icon Border Events

        private void ShortcutIconBorder_MouseEnter(object sender , MouseEventArgs e)
        {
            (sender as ShortcutIconBorder).DoMouseEnter();
        }

        private void ShortcutIconBorder_MouseLeave(object sender , MouseEventArgs e)
        {
            (sender as ShortcutIconBorder).DoMouseLeave();
        }

        private void ShortcutIconBorder_PreviewMouseLeftButtonDown(object sender , MouseButtonEventArgs e)
        {
            lastSelectedShortcutIconBorder = (ShortcutIconBorder)sender;
            if (e.ClickCount == 2)
            {
                ShortcutIcon shortcutIcon = lastSelectedShortcutIconBorder.ShortcutIcon;
                if (shortcutIcon != null)
                {
                    WinFileOperations.OpenFile(shortcutIcon.ShortcutFilePathInfo.FullName);
                }
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (lastSelectedShortcutIconBorder.ShortcutIcon.IsSelected)
                {
                    lastSelectedShortcutIconBorder.Deselect();
                }
                else
                {
                    lastSelectedShortcutIconBorder.Select();
                }
            }
            else if (ConfigManager.AutoSortMode)
            {
                CategoryManager.GetSelectedShortcutIconGrid().DeselectAllShortcutIcon();
                lastSelectedShortcutIconBorder.Select();
            }
            else
            {
                DataObject data = new DataObject(DataFormats.Text , "ShortcutIconBorder");
                DragDrop.DoDragDrop(lastSelectedShortcutIconBorder, data, DragDropEffects.Move);
            }
            SliderWindow.RefreshBottomDockPanelButtons();
            e.Handled = true;
        }

        private void ShortcutIconBorder_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShortcutIconBorder shortcutIconBorder = sender as ShortcutIconBorder;
            if (!shortcutIconBorder.ShortcutIcon.IsSelected)
            {
                CategoryManager.GetSelectedShortcutIconGrid().DeselectAllShortcutIcon();
                shortcutIconBorder.Select();
                SliderWindow.RefreshBottomDockPanelButtons();
            }
            e.Handled = true;
        }

        private void ShortcutIconBorder_PreviewDragEnter(object sender , DragEventArgs e)
        {
            (sender as ShortcutIconBorder).DoMouseEnter();
        }

        private void ShortcutIconBorder_PreviewDragLeave(object sender , DragEventArgs e)
        {
            (sender as ShortcutIconBorder).DoMouseLeave();
        }

        private void ShortcutIconBorder_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedText = (string)e.Data.GetData(DataFormats.StringFormat);
                if (!draggedText.Equals("ShortcutIconBorder"))
                    return;
            }
            else
            {
                return;
            }

            ShortcutIcon drag_ShortcutIcon = lastSelectedShortcutIconBorder.ShortcutIcon;
            ShortcutIconGridPosition drag_ShortcutIconGridPosition = drag_ShortcutIcon.GridPosition;
            ShortcutIconBorder drop_ShortcutIconBorder = sender as ShortcutIconBorder;
            ShortcutIcon drop_ShortcutIcon = drop_ShortcutIconBorder.ShortcutIcon;
            ShortcutIconGridPosition drop_ShortcutIconGridPosition = drop_ShortcutIcon.GridPosition;
            ShortcutIconGrid shortcutIconGrid = CategoryManager.GetSelectedShortcutIconGrid();

            if (drop_ShortcutIconBorder.Name.Equals(lastSelectedShortcutIconBorder.Name))
            {
                if (!drag_ShortcutIcon.IsSelected)
                    shortcutIconGrid.DeselectAllShortcutIcon();
                lastSelectedShortcutIconBorder.Select();
                SliderWindow.RefreshBottomDockPanelButtons();
                return;
            }
            else
            {
                lastSelectedShortcutIconBorder.Select();
                drop_ShortcutIconBorder.Deselect();
                SliderWindow.RefreshBottomDockPanelButtons();
            }

            int noOfRowsFromDragToDrop = drop_ShortcutIconGridPosition.RowNo - drag_ShortcutIconGridPosition.RowNo;
            int noOfColsFromDragToDrop = drop_ShortcutIconGridPosition.ColNo - drag_ShortcutIconGridPosition.ColNo;

            if (noOfRowsFromDragToDrop == 0 && noOfColsFromDragToDrop == 0)
            {
                return;
            }

            List<ShortcutIcon> listSelectedShortcutIcon = null;

            if (noOfRowsFromDragToDrop > 0 || noOfColsFromDragToDrop > 0)
            {
                listSelectedShortcutIcon = shortcutIconGrid.SelectedShortcutIconListInReverseOrder;
            }
            else
            {
                listSelectedShortcutIcon = shortcutIconGrid.SelectedShortcutIconList;
            }

            if(listSelectedShortcutIcon.Count == 1)
            {
                SwapShortcutIconBorders(lastSelectedShortcutIconBorder , drop_ShortcutIconBorder);
                return;
            }

            foreach (ShortcutIcon shortcutIcon in listSelectedShortcutIcon)
            {
                shortcutIconGrid.AddShortcutIconFreePosition(shortcutIcon.GridPosition);
            }

            foreach (ShortcutIcon shortcutIcon in listSelectedShortcutIcon)
            {
                ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;
                MoveShortcutIconBorderGridPosition(shortcutIconBorder
                                                     , noOfRowsFromDragToDrop
                                                     , noOfColsFromDragToDrop);
            }
        }

        // Canvas Free Space Events

        private void Canvas_FreeSpace_DragEnter(object sender , DragEventArgs e)
        {
            FreeSpaceCanvas dragEnterFreeSpaceCanvas = (FreeSpaceCanvas)sender;
            dragEnterFreeSpaceCanvas.Background = FreeSpaceCanvas.MouseEnterBackgroundBrush;
        }

        private void Canvas_FreeSpace_DragLeave(object sender , DragEventArgs e)
        {
            FreeSpaceCanvas dragLeaveFreeSpaceCanvas = (FreeSpaceCanvas)sender;
            dragLeaveFreeSpaceCanvas.Background = FreeSpaceCanvas.CurrentBackgroundBrush;
        }

        private void Canvas_FreeSpace_Drop(object sender , DragEventArgs e)
        {
            FreeSpaceCanvas drop_FreeSpaceCanvas = sender as FreeSpaceCanvas;
            ShortcutIconGridPosition drop_FreeSpaceCanvasGridPosition = drop_FreeSpaceCanvas.GridPosition;

            drop_FreeSpaceCanvas.Background = FreeSpaceCanvas.CurrentBackgroundBrush;

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedText = (string)e.Data.GetData(DataFormats.StringFormat);
                if (!draggedText.Equals("ShortcutIconBorder"))
                    return;
            }
            else
            {
                return;
            }

            ShortcutIcon drag_ShortcutIcon = lastSelectedShortcutIconBorder.ShortcutIcon;
            ShortcutIconGridPosition drag_ShortcutIconGridPosition = drag_ShortcutIcon.GridPosition;
            ShortcutIconGrid shortcutIconGrid = CategoryManager.GetSelectedShortcutIconGrid();

            if (!drag_ShortcutIcon.IsSelected)
                shortcutIconGrid.DeselectAllShortcutIcon();
            lastSelectedShortcutIconBorder.Select();
            SliderWindow.RefreshBottomDockPanelButtons();

            int noOfRowsFromDragToDrop = drop_FreeSpaceCanvasGridPosition.RowNo - drag_ShortcutIconGridPosition.RowNo;
            int noOfColsFromDragToDrop = drop_FreeSpaceCanvasGridPosition.ColNo - drag_ShortcutIconGridPosition.ColNo;

            if (noOfRowsFromDragToDrop == 0 && noOfColsFromDragToDrop == 0)
            {
                return;
            }

            List<ShortcutIcon> listSelectedShortcutIcon = null;

            if (noOfRowsFromDragToDrop > 0 || noOfColsFromDragToDrop > 0)
            {
                listSelectedShortcutIcon = shortcutIconGrid.SelectedShortcutIconListInReverseOrder;
            }
            else
            {
                listSelectedShortcutIcon = shortcutIconGrid.SelectedShortcutIconList;
            }

            foreach (ShortcutIcon shortcutIcon in listSelectedShortcutIcon)
            {
                shortcutIconGrid.AddShortcutIconFreePosition(shortcutIcon.GridPosition);
                ShortcutIconBorder shortcutIconBorder = shortcutIcon.ShortcutIconBorder;
                MoveShortcutIconBorderGridPosition(shortcutIconBorder
                                                     , noOfRowsFromDragToDrop
                                                     , noOfColsFromDragToDrop);
            }
        }

        // Events for both FreeSpaceCanvas + gridPanel

        public void FreeSpace_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartFSCanvas = sender as FreeSpaceCanvas;
            ShortcutIconGrid shortcutIconGrid = CategoryManager.GetSelectedShortcutIconGrid();
            shortcutIconGrid.DeselectAllShortcutIcon();
            freeSpaceCanvas_ClickPos = e.GetPosition(shortcutIconGrid.Canvas);
            MultiShortcutIconSelectRect = new System.Windows.Shapes.Rectangle()
            {
                Opacity = 0.5,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 132, 255)),
                Fill = new SolidColorBrush(Color.FromRgb(0, 132, 255)),
                Width = 0,
                Height = 0
            };
            Canvas.SetLeft(MultiShortcutIconSelectRect, freeSpaceCanvas_ClickPos.X);
            Canvas.SetTop(MultiShortcutIconSelectRect, freeSpaceCanvas_ClickPos.X);
            shortcutIconGrid.Canvas.Children.Add(MultiShortcutIconSelectRect);
            e.Handled = true;
        }

        public void FreeSpace_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released
                || MultiShortcutIconSelectRect == null)
            {
                return;
            }

            Point currentPos = e.GetPosition(CategoryManager.GetSelectedShortcutIconGrid().Canvas);

            var rectLeft = Math.Min(currentPos.X, freeSpaceCanvas_ClickPos.X);
            var rectTop = Math.Min(currentPos.Y, freeSpaceCanvas_ClickPos.Y);

            var rectWidth = Math.Max(currentPos.X, freeSpaceCanvas_ClickPos.X) - rectLeft;
            var rectHeight = Math.Max(currentPos.Y, freeSpaceCanvas_ClickPos.Y) - rectTop;

            MultiShortcutIconSelectRect.Width = rectWidth;
            MultiShortcutIconSelectRect.Height = rectHeight;

            Canvas.SetLeft(MultiShortcutIconSelectRect, rectLeft);
            Canvas.SetTop(MultiShortcutIconSelectRect, rectTop);
        }

        private void FreeSpace_RemoveBackgroundImage_Click(object sender, EventArgs e)
        {
            BackgroundImagePath = null;
            SetBackgroundImage(true);
            RemoveBackgroundImgMenuItem.IsEnabled = false;
        }

        private void FreeSpace_SetBackgroundImage_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image Files|*.bmp;*.gif;*.ico;*.jpg;*.png;*.wdp;*.tiff";
            dialog.Multiselect = false;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                BackgroundImagePath = dialog.FileName;
                SetBackgroundImage(true);
            }
        }
    }

    internal static class MouseHook
    {
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        private static int _mouseHookHandle;
        private static HookProc _mouseDelegate;

        private static event MouseUpEventHandler MouseUp;
        public static event MouseUpEventHandler OnMouseUp
        {
            add
            {
                Subscribe();
                MouseUp += value;
            }
            remove
            {
                MouseUp -= value;
                Unsubscribe();
            }
        }

        private static void Unsubscribe()
        {
            if (_mouseHookHandle != 0)
            {
                int result = UnhookWindowsHookEx(_mouseHookHandle);
                _mouseHookHandle = 0;
                _mouseDelegate = null;
                if (result == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private static void Subscribe()
        {
            if (_mouseHookHandle == 0)
            {
                _mouseDelegate = MouseHookProc;
                _mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL,
                    _mouseDelegate,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);
                if (_mouseHookHandle == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT mouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                if (wParam == WM_LBUTTONUP)
                {
                    if (MouseUp != null)
                    {
                        MouseUp.Invoke(null, new Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y));
                    }
                }
            }
            return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONUP = 0x0202;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
           CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);
    }

    public delegate void MouseUpEventHandler(object sender, Point p);
}
