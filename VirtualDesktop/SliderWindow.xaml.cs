using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VirtualDesktop.Actions;

namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for SliderWindow.xaml
    /// </summary>
    public partial class SliderWindow : Window
    {
        private MainWindow mainWindow;

        private double OriginalTopMenuPanelTopMargin = 0;
        public double CollapsedWindowHeight = 0;
        public double FullWindowHeight = 0;

        private int firstVisibleCategoryIndex = 0;
        private int lastVisibleCategoryIndex = -1;

        private double TopMenuShowPanelRightWidthExclLastLabel = 0;
        private double[] lblTopMenuShowPanel3_4_Widths =
        {
            0  // When auto-sort mode is off
         , 103 // When "Sort by" combobox 1st item is selected (use selected combobox item index + 1)
         // Add new width as new combo box items are added
        };

        private double categoryMenuPanel_Fixed_Width = 0;
        private double categoryMenuPanel_Children_width = 0;
        private double categoryMenuItem_Fixed_Width = 0;

        public CategoryBorder SelectedCategoryBorder = null;

        private Point categoryBorderClickPoint = new Point(-1,-1);

        public SliderWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            OriginalTopMenuPanelTopMargin = MenuContentPanel.Margin.Top;
            SetWinPosition();
            ResizeUIControls();
            TopMenuShowPanelRightWidthExclLastLabel = lblTopMenuShowPanel3_0.Width
                                                    + lblTopMenuShowPanel3_1.Width
                                                    + lblTopMenuShowPanel3_2.Width
                                                    + lblTopMenuShowPanel3_3.Width;
            CollapsedWindowHeight = TopMenuShowPanel.Height;
            FullWindowHeight = TopMenuHidePanel.Height + topDockPanel.Height
                                + separatorDockPanel.Height + bottomDockPanel.Height;
        }

        public void AddCategoryMenuItem(Category category)
        {
            int categoryIndex = category.Index;

            StackPanel CategoryMenuItemPanel = new StackPanel();
            CategoryMenuItemPanel.Name = "CategoryMenuPanel" + categoryIndex;
            CategoryMenuItemPanel.Orientation = Orientation.Horizontal;
            CategoryMenuItemPanel.Width = categoryMenuItem_Fixed_Width;
            CategoryMenuItemPanel.Height = MathUtils.GetMaxOf3Double(
                                        btnCategoryMenuNavPrev.Height
                                        , CategoryMenuPanel.Height
                                        , btnCategoryMenuNavNext.Height);
            FrameworkElementReferenceManager.AddNewFrameworkElement(CategoryMenuItemPanel);

            Image menuItemImage = new Image();

            double menuItemImageWidth = 0;
            double menuItemImageLeftMargin = 0;

            if (category.ImagePath != null)
            {
                menuItemImage.Name = "CategoryMenuImage" + categoryIndex;
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(category.ImagePath);
                imageSource.EndInit();
                menuItemImage.Source = imageSource;
                menuItemImage.Width = 32;
                menuItemImage.Height = 32;
                menuItemImage.Margin = new Thickness(2, 0, 0, 0);
                menuItemImageWidth = menuItemImage.Width;
                menuItemImageLeftMargin = menuItemImage.Margin.Left;
                FrameworkElementReferenceManager.AddNewFrameworkElement(menuItemImage);
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Name = "CategoryMenuTextBlock" + categoryIndex;
            textBlock.Text = category.Name;
            textBlock.FontFamily = new System.Windows.Media.FontFamily("Arial");
            textBlock.FontSize = 12;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.Foreground = Brushes.White;
            textBlock.Width = CategoryMenuItemPanel.Width - menuItemImageWidth
                              - menuItemImageLeftMargin
                              - textBlock.Margin.Left;
            textBlock.TextAlignment = TextAlignment.Center;
            FrameworkElementReferenceManager.AddNewFrameworkElement(textBlock);

            Border textBlockBorder = new Border();
            textBlockBorder.Name = "CategoryMenuTextBlockBorder" + categoryIndex;
            textBlockBorder.Width = textBlock.Width;
            textBlockBorder.MaxHeight = CategoryMenuItemPanel.Height;
            textBlockBorder.VerticalAlignment = VerticalAlignment.Center;
            textBlockBorder.Background = Brushes.Transparent;
            textBlockBorder.BorderBrush = Brushes.Transparent;
            textBlockBorder.BorderThickness = new Thickness(1);

            textBlockBorder.Child = textBlock;

            if (category.ImagePath != null)
            {
                textBlock.TextAlignment = TextAlignment.Left;
                CategoryMenuItemPanel.Children.Add(menuItemImage);
            }

            CategoryMenuItemPanel.Children.Add(textBlockBorder);

            CategoryBorder categoryMenuItemPanelBorder = new CategoryBorder();
            categoryMenuItemPanelBorder.Name = "CategoryMenuPanelBorder" + categoryIndex;
            categoryMenuItemPanelBorder.Width = CategoryMenuItemPanel.Width;
            categoryMenuItemPanelBorder.Height = CategoryMenuItemPanel.Height;
            categoryMenuItemPanelBorder.BorderBrush = Brushes.Transparent;
            categoryMenuItemPanelBorder.BorderThickness = new Thickness(1.5);
            categoryMenuItemPanelBorder.CornerRadius = new CornerRadius(4);

            LinearGradientBrush backgroundGradientBrush =
                new LinearGradientBrush(Color.FromRgb(72, 85, 99)
                                      , Color.FromRgb(41, 50, 60)
                                      , new Point(0, 0)
                                      , new Point(0.5, 1));

            categoryMenuItemPanelBorder.Background = backgroundGradientBrush;

            ToolTip toolTip = new ToolTip();
            toolTip.Content = category.Name;
            categoryMenuItemPanelBorder.ToolTip = toolTip;
            categoryMenuItemPanelBorder.AllowDrop = true;

            categoryMenuItemPanelBorder.Child = CategoryMenuItemPanel;

            categoryMenuItemPanelBorder.AddHandler(MouseEnterEvent , new MouseEventHandler(CategoryBorder_MouseEnter));
            categoryMenuItemPanelBorder.AddHandler(MouseLeaveEvent , new MouseEventHandler(CategoryBorder_MouseLeave));
            categoryMenuItemPanelBorder.AddHandler(MouseMoveEvent , new MouseEventHandler(CategoryBorder_MouseMove));
            categoryMenuItemPanelBorder.AddHandler(MouseLeftButtonDownEvent , new MouseButtonEventHandler(CategoryBorder_MouseLeftButtonDown));
            categoryMenuItemPanelBorder.AddHandler(MouseLeftButtonUpEvent , new MouseButtonEventHandler(All_Controls_PreviewMouseLeftButtonUp));
            categoryMenuItemPanelBorder.AddHandler(MouseWheelEvent , new MouseWheelEventHandler(CategoryBorder_MouseWheel));
            categoryMenuItemPanelBorder.AddHandler(PreviewDragEnterEvent , new DragEventHandler(CategoryBorder_PreviewDragEnter));
            categoryMenuItemPanelBorder.AddHandler(PreviewDragLeaveEvent , new DragEventHandler(CategoryBorder_PreviewDragLeave));
            categoryMenuItemPanelBorder.AddHandler(PreviewDropEvent , new DragEventHandler(CategoryBorder_PreviewDrop));

            CategoryMenuPanel.Children.Add(categoryMenuItemPanelBorder);

            FrameworkElementReferenceManager.AddNewFrameworkElement(categoryMenuItemPanelBorder);

            if (Math.Round(categoryMenuPanel_Children_width + categoryMenuItemPanelBorder.Width, 2) <= Math.Round(categoryMenuPanel_Fixed_Width, 2))
            {
                lastVisibleCategoryIndex = categoryIndex;
            }

            categoryMenuPanel_Children_width += categoryMenuItemPanelBorder.Width;
            RefreshCategoryMenuNavButtons();

            categoryMenuItemPanelBorder.Category = category;
            category.CategoryBorder = categoryMenuItemPanelBorder;

            if (SelectedCategoryBorder == null)
            {
                categoryMenuItemPanelBorder.Select();
            }

            CategoryContextMenuItem mainTypeMenuItem = new CategoryContextMenuItem(category);
            CategoryContextMenuItem mixedTypeMenuItem = new CategoryContextMenuItem(category);
            CategoryContextMenuItem manageFileExtMenuItem = new CategoryContextMenuItem(category);
            CategoryContextMenuItem removeMenuItem = new CategoryContextMenuItem(category);

            category.MainTypeContextMenuItem = mainTypeMenuItem;
            category.MixedTypeContextMenuItem = mixedTypeMenuItem;
            category.ManageFileExtMenuItem = manageFileExtMenuItem;
            category.RemoveContextMenuItem = removeMenuItem;

            mainTypeMenuItem.Header = "Is Main Type";
            mainTypeMenuItem.IsCheckable = true;
            mainTypeMenuItem.Click += CategoryBorder_MainTypeMenuItem_Click;
            mainTypeMenuItem.IsChecked = category.IsMainType;

            mixedTypeMenuItem.Header = "Is Mixed Type";
            mixedTypeMenuItem.IsCheckable = true;
            mixedTypeMenuItem.Click += CategoryBorder_MixedTypeMenuItem_Click;
            mixedTypeMenuItem.IsChecked = category.IsMixedType;

            manageFileExtMenuItem.Header = "Manage file extensions";
            manageFileExtMenuItem.Click += CategoryBorder_ManageFileExtMenuItem_Click;

            removeMenuItem.Header = "Remove category";
            removeMenuItem.Click += CategoryBorder_RemoveMenuItem_Click;

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(mainTypeMenuItem);
            contextMenu.Items.Add(mixedTypeMenuItem);
            contextMenu.Items.Add(manageFileExtMenuItem);
            contextMenu.Items.Add(removeMenuItem);

            categoryMenuItemPanelBorder.ContextMenu = contextMenu;

            FrameworkElementReferenceManager.AddNewFrameworkElement(mainTypeMenuItem);
            FrameworkElementReferenceManager.AddNewFrameworkElement(mixedTypeMenuItem);
            FrameworkElementReferenceManager.AddNewFrameworkElement(removeMenuItem);

            category.ShortcutIconGrid.FillAllFreeSpaceCanvas();
        }

        public Thickness GetTopMenuShowPanelRightMargin()
        {
            double left = TopMenuShowPanelRight.MaxWidth
                        - TopMenuShowPanelRight.Width;
            if(left < 0)
            {
                left = 0;
            }
            return new Thickness(left , 0 , 0 , 0);
        }

        public void RefreshBottomDockPanelButtons()
        {
            ShortcutIconGrid shortcutIconGrid = CategoryManager.GetSelectedShortcutIconGrid();
            bool doEnableButton = false;
            if (shortcutIconGrid.GetNumberOfSelectedShorcutIcon() > 0)
            {
                doEnableButton = true;
            }
            btnRemove_ContextMenuItem_ShortcutIcon.IsEnabled = doEnableButton;
            btnMove.IsEnabled = doEnableButton;
            btnMoveCopy.IsEnabled = doEnableButton;
        }

        public void RefreshBtnMoveAndCopyContextMenuItems()
        {
            Button[] buttons = { btnMove, btnMoveCopy };
            foreach (Button button in buttons)
            {
                ContextMenu contextMenu = button.ContextMenu;
                if (contextMenu == null)
                {
                    contextMenu = new ContextMenu();
                }
                else if (contextMenu.HasItems)
                {
                    ItemCollection menuItemsCollection = contextMenu.Items;
                    List<MoveCategoryContextMenuItem> lstMenuItemsToRemove
                        = new List<MoveCategoryContextMenuItem>();
                    foreach (MoveCategoryContextMenuItem menuItem in menuItemsCollection)
                    {
                        lstMenuItemsToRemove.Add(menuItem);
                    }
                    foreach (MoveCategoryContextMenuItem menuItem in lstMenuItemsToRemove)
                    {
                        contextMenu.Items.Remove(menuItem);
                    }
                }

                bool moveAction = true;
                if (button == btnMoveCopy)
                {
                    moveAction = false;
                }

                List<Category> categoryList = CategoryManager.CategoryListSortedByName;
                foreach (Category category in categoryList)
                {
                    if (!category.IsSelected)
                    {
                        MoveCategoryContextMenuItem menuItem =
                            new MoveCategoryContextMenuItem(category, moveAction);
                        category.MoveCategoryContextMenuItem = menuItem;

                        menuItem.Header = category.Name;
                        contextMenu.Items.Add(menuItem);
                    }
                }
                button.ContextMenu = contextMenu;
            }
        }

        public void RefreshCategoryMenuNavButtons()
        {
            if (firstVisibleCategoryIndex > 0)
            {
                btnCategoryMenuNavPrev.IsEnabled = true;
            }
            else
            {
                btnCategoryMenuNavPrev.IsEnabled = false;
            }

            if (lastVisibleCategoryIndex < CategoryManager.LastCategoryIndex)
            {
                btnCategoryMenuNavNext.IsEnabled = true;
            }
            else
            {
                btnCategoryMenuNavNext.IsEnabled = false;
            }
        }

        public void RefreshTopMenuShowPanelLabels()
        {
            Category category = CategoryManager.GetSelectedCategory();
            int shortcutIconCount = category.ShortcutIconGrid.ShortcutIconList.Count;
            string text = "Selected Category : " + category.Name;
            text += " [ Total number of Shortcut Icons : " + shortcutIconCount + "]";
            lblTopMenuShowPanel1.Content = text;

            text = "OFF";
            Brush foregroundBrush = Brushes.Red;
            
            if (ConfigManager.AutoCategorisedMode)
            {
                text = "ON";
                foregroundBrush = Brushes.Green;
            }

            lblTopMenuShowPanel3_1.Content = text;
            lblTopMenuShowPanel3_1.Foreground = foregroundBrush;

            text = "OFF";
            foregroundBrush = Brushes.Red;

            lblTopMenuShowPanel3_4.Content = "";

            int index = 0;

            if (ConfigManager.AutoSortMode)
            {
                text = "ON";
                foregroundBrush = Brushes.Green;
                lblTopMenuShowPanel3_4.Content = "(Sorted by "
                                                + cmbx_sortBy.SelectedValue.ToString()
                                                + ")";
                index = cmbx_sortBy.SelectedIndex + 1;
            }

            lblTopMenuShowPanel3_4.Width = lblTopMenuShowPanel3_4_Widths[index];
            TopMenuShowPanelRight.Width = TopMenuShowPanelRightWidthExclLastLabel
                                        + lblTopMenuShowPanel3_4.Width;
            TopMenuShowPanelRight.Margin = GetTopMenuShowPanelRightMargin();

            lblTopMenuShowPanel3_3.Content = text;
            lblTopMenuShowPanel3_3.Foreground = foregroundBrush;
        }

        public void ResizeUIControls()
        {
            ResizeWindow();
            ResizeMainPanel();
            ResizeMenuContentPanel();
            ResizeSliderPanel();
            ResizeTopMenuPanelControls();
            ResizeTopDockPanel();
            ResizeBottomDockPanel();
            ResizeBottomDockPanelGrid();
            ResizeBottomDockPanelGridBoundColumns();
            ResizeCategoryMenuPanel();
        }

        private void ResizeTopDockPanel()
        {
            topDockPanel.Width = Width;
        }

        private void ResizeMenuContentPanel()
        {
            MenuContentPanel.Width = Width;
        }

        private void ResizeBottomDockPanel()
        {
            bottomDockPanel.Width = Width;
        }

        private void ResizeBottomDockPanelGrid()
        {
            bottomDockPanelGrid.Width = bottomDockPanel.Width;
            bottomDockPanelGrid.Height = bottomDockPanel.Height;
        }

        private void ResizeBottomDockPanelGridBoundColumns()
        {
            int noOfColDef = bottomDockPanelGrid.ColumnDefinitions.Count;
            double widthOfAllColDefExceptBoundCols = 0;
            for (int i = 1; i < noOfColDef - 1; i++)
            {
                GridLength gridLength = bottomDockPanelGrid.ColumnDefinitions[i].Width;
                widthOfAllColDefExceptBoundCols += gridLength.Value;
            }
            GridLength boundColumnWidth = new GridLength((bottomDockPanelGrid.Width - widthOfAllColDefExceptBoundCols) / 2);
            if (boundColumnWidth.Value >= 0)
            {
                bottomDockPanelGrid.ColumnDefinitions[0].Width = boundColumnWidth;
                bottomDockPanelGrid.ColumnDefinitions[noOfColDef - 1].Width = boundColumnWidth;
            }
        }

        private void ResizeCategoryMenuPanel()
        {
            CategoryMenuPanel.Width = this.Width
                                   - btnCategoryMenuNavPrev.Width - btnCategoryMenuNavNext.Width
                                   - (btnCategoryMenuNavPrev.Margin.Left * 2);
            CategoryMenuPanel.MinWidth = CategoryMenuPanel.Width;
            categoryMenuPanel_Fixed_Width = CategoryMenuPanel.Width;
            categoryMenuItem_Fixed_Width = categoryMenuPanel_Fixed_Width / 10;
        }

        private void ResizeMainPanel()
        {
            MainPanel.Width = Width;
        }

        private void ResizeSliderPanel()
        {
            TopMenuShowPanel.Width = Width;
            TopMenuHidePanel.Width = Width;
        }

        private void ResizeTopMenuPanelControls()
        {
            double width = TopMenuShowPanel.Width / 3;
            lblTopMenuShowPanel1.Width = width;
            lblTopMenuShowPanel2.Width = width;
            TopMenuShowPanelRight.MaxWidth = width;
            lblTopMenuHidePanel.Width = TopMenuHidePanel.Width;
        }

        private void ResizeWindow()
        {
            Width = ScreenWorkAreaUtils.ScreenWorkAreaWidth;
        }

        private void Scroll_CategoryMenu_Left()
        {
            if (firstVisibleCategoryIndex > 0)
            {
                scroll_CategoryMenuPanel.LineLeft();

                Category firstVisibleCategory = CategoryManager.GetCategory(firstVisibleCategoryIndex);
                Category lastVisibleCategory = CategoryManager.GetCategory(lastVisibleCategoryIndex);

                if (firstVisibleCategory.PreviousCategory != null)
                    firstVisibleCategoryIndex = firstVisibleCategory.PreviousCategory.Index;
                if (lastVisibleCategory.PreviousCategory != null)
                    lastVisibleCategoryIndex = lastVisibleCategory.PreviousCategory.Index;

                RefreshCategoryMenuNavButtons();
            }
        }

        private void Scroll_CategoryMenu_Right()
        {
            if (lastVisibleCategoryIndex < CategoryManager.LastCategoryIndex)
            {
                scroll_CategoryMenuPanel.LineRight();
                Category firstVisibleCategory = CategoryManager.GetCategory(firstVisibleCategoryIndex);
                Category lastVisibleCategory = CategoryManager.GetCategory(lastVisibleCategoryIndex);

                if (firstVisibleCategory.NextCategory != null)
                    firstVisibleCategoryIndex = firstVisibleCategory.NextCategory.Index;
                if (lastVisibleCategory.NextCategory != null)
                    lastVisibleCategoryIndex = lastVisibleCategory.NextCategory.Index;
                RefreshCategoryMenuNavButtons();
            }
        }

        public void SetAutoCategorisedValue(bool isChecked)
        {
            chkbx_Auto_Categorised.IsChecked = isChecked;
        }

        public void SetAutoSortValue(bool isChecked)
        {
            chkbx_Auto_Sort.IsChecked = isChecked;
        }

        public void SetWinPosition()
        {
            Top = 0;
            Left = 0;
            if (TaskbarUtils.TopArea)
            {
                Top = TaskbarUtils.TaskBarHeight;
                Left = 0;
            }
            else if (TaskbarUtils.BottomArea || TaskbarUtils.RightArea)
            {
                Top = 0;
                Left = 0;
            }
            else if (TaskbarUtils.LeftArea)
            {
                Top = 0;
                Left = TaskbarUtils.TaskBarWidth;
            }
        }

        public void ShowHideMenu(bool showMenu)
        {
            if (showMenu)
            {
                MenuContentPanel.Margin = new Thickness(0,0,0,0);
                Height = FullWindowHeight;
                TopMenuHidePanel.Visibility = Visibility.Visible;
                TopMenuShowPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                MenuContentPanel.Margin = new Thickness(0 , OriginalTopMenuPanelTopMargin, 0 , 0);
                Height = CollapsedWindowHeight;
                TopMenuHidePanel.Visibility = Visibility.Hidden;
                TopMenuShowPanel.Visibility = Visibility.Visible;
            }
        }

        // EVENTS

        // Multiple controls Events

        private void All_Controls_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.StartFSCanvas != null && MainWindow.MultiShortcutIconSelectRect != null)
            {
                FreeSpaceCanvas endFSCanvas = sender as FreeSpaceCanvas;
                if (endFSCanvas != null && MainWindow.StartFSCanvas.Name.Equals(endFSCanvas.Name))
                {
                    goto DoNotSelectMultiShortcutIconBordersWithinRect;
                }
            }
            mainWindow.SelectMultiShortcutIconBordersWithinRect();
            DoNotSelectMultiShortcutIconBordersWithinRect:
            mainWindow.RemoveMultiShortcutIconSelectionRectangle();
            MainWindow.StartFSCanvas = null;
        }

        // Slider label events

        private void lblTopMenuShow_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowHideMenu(true);
        }

        // Category Border Events

        private void BtnCategoryMenuNavPrev_Click(object sender, RoutedEventArgs e)
        {
            Scroll_CategoryMenu_Left();
        }

        private void BtnCategoryMenuNavNext_Click(object sender, RoutedEventArgs e)
        {
            Scroll_CategoryMenu_Right();
        }

        private void CategoryBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) // Mouse Wheel Up
            {
                Scroll_CategoryMenu_Left();
            }
            else // Mouse Wheel Down
            {
                Scroll_CategoryMenu_Right();
            }
        }

        private void CategoryBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            CategoryBorder categoryBorder = (CategoryBorder)sender;
            if (!categoryBorder.Category.IsSelected)
            {
                categoryBorder.BorderBrush = Brushes.White;
            }
        }

        private void CategoryBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            CategoryBorder border = (CategoryBorder)sender;
            border.BorderBrush = Brushes.Transparent;
        }

        private void CategoryBorder_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPos = e.GetPosition(this);
            CategoryBorder categoryBorder = sender as CategoryBorder;
            if(e.LeftButton == MouseButtonState.Released)
            {
                return;
            }

            if(currentPos.X != categoryBorderClickPoint.X
                || currentPos.Y != categoryBorderClickPoint.Y)
            {
                DataObject data = new DataObject(DataFormats.Text, "CategoryBorder");
                DragDrop.DoDragDrop(categoryBorder , data , DragDropEffects.Move);
            }
        }

        private void CategoryBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CategoryBorder categoryBorder = sender as CategoryBorder;
            categoryBorderClickPoint = e.GetPosition(this);
            if (!categoryBorder.Category.IsSelected)
            {
                mainWindow.RemoveLastShortcutIconGridCanvas();
                SelectedCategoryBorder.Deselect();
                categoryBorder.Select();
                SelectedCategoryBorder = categoryBorder;
                RefreshBottomDockPanelButtons();
                RefreshBtnMoveAndCopyContextMenuItems();
                mainWindow.ShowSelectedShortcutIconGridCanvas();
                RefreshTopMenuShowPanelLabels();
            }
        }

        private void CategoryBorder_PreviewDragEnter(object sender , DragEventArgs e)
        {
            CategoryBorder categoryBorder = (CategoryBorder)sender;
            if (!categoryBorder.Category.IsSelected)
            {
                categoryBorder.BorderBrush = Brushes.White;
            }
        }

        private void CategoryBorder_PreviewDragLeave(object sender , DragEventArgs e)
        {
            CategoryBorder border = (CategoryBorder)sender;
            border.BorderBrush = Brushes.Transparent;
        }

        private void CategoryBorder_PreviewDrop(object sender , DragEventArgs e)
        {
            string draggedText = (string)e.Data.GetData(DataFormats.StringFormat);
            if (!draggedText.Equals("CategoryBorder"))
                return;

            CategoryBorder categoryBorder = sender as CategoryBorder;
            Category category = categoryBorder.Category;
            Category selectedCategory = SelectedCategoryBorder.Category;

            if (category != selectedCategory)
            {
                CategoryManager.SwapCategory(category , selectedCategory);
                SwapCategoryAction action = new SwapCategoryAction(category , selectedCategory);
                ActionSet actionSet = new ActionSet();
                actionSet.AddAction(action);
                ActionManager.AddNewActionSet(actionSet);
            }
        }

        private void CategoryBorder_MainTypeMenuItem_Click(object sender, EventArgs e)
        {
            CategoryContextMenuItem mainTypeMenuItem = (CategoryContextMenuItem)sender;
            Category category = mainTypeMenuItem.Category;

            if (!chkbx_Auto_Categorised.IsChecked.Value)
            {
                category.ToggleMainType();
                return;
            }

            if (category.IsMainType)
            {
                string msg = "Please mark another \"Mixed type\" category as \"Main type\" to unmark the ";
                msg += category.Name + " category from \"Main type\".";
                MessageBox.Show(msg);
            }
            else
            {
                CategoryManager.MainTypeCategory.UnmarkAsMainType();
            }

            category.MarkAsMainType();
        }

        private void CategoryBorder_MixedTypeMenuItem_Click(object sender, EventArgs e)
        {
            CategoryContextMenuItem mixedTypeMenuItem = (CategoryContextMenuItem)sender;
            Category category = mixedTypeMenuItem.Category;

            if (!chkbx_Auto_Categorised.IsChecked.Value || !category.IsMainType)
            {
                category.ToggleMixedType();
                return;
            }

            if (category.IsMainType)
            {
                mixedTypeMenuItem.IsChecked = true;
                string msg = "Please mark another \"Mixed type\" category as \"Main type\" to unmark the ";
                msg += category.Name + " category from \"Mixed type\".";
                MessageBox.Show(msg);
            }
        }

        private void CategoryBorder_ManageFileExtMenuItem_Click(object sender, EventArgs e)
        {
            CategoryContextMenuItem modFileExtMenuItem = (CategoryContextMenuItem)sender;
            Category category = modFileExtMenuItem.Category;
            mainWindow.FreezeWindow();
            new ManageCategoryFileExt(category.Name).Show();
        }

        private void CategoryBorder_RemoveMenuItem_Click(object sender, EventArgs e)
        {
            CategoryContextMenuItem removeMenuItem = (CategoryContextMenuItem)sender;
            Category category = removeMenuItem.Category;

            string dialogTitle = "Remove category";
            string dialogMsg = "Are you sure you want to remove the \"" + category.Name + "\" category?";
            dialogMsg += "\nRemoving it will also remove the shortcut icon(s) under it.";
            MessageBoxResult result = MessageBox.Show(dialogMsg, dialogTitle
                                                , MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CategoryManager.RemoveCategory(category);
                Category selectedCategory = CategoryManager.GetSelectedCategory();
                removeMenuItem = selectedCategory.RemoveContextMenuItem;

                if (CategoryManager.CategoryCount == 1)
                {
                    removeMenuItem.IsEnabled = false;
                    btnRemove_ContextMenuItem_Category.IsEnabled = false;
                }

                RemoveCategoryAction action = new RemoveCategoryAction(category);
                ActionSet actionSet = new ActionSet();
                actionSet.AddAction(action);
                ActionManager.AddNewActionSet(actionSet);
            }
        }

        // Bottom Dock Panel Controls Events

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.ContextMenu.Visibility = Visibility.Visible;
            btnAdd.ContextMenu.IsOpen = true;
        }

        private void BtnAdd_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnAdd.ContextMenu.Visibility = Visibility.Hidden;
            btnAdd.ContextMenu.IsOpen = false;
        }

        private void BtnAdd_ContextMenuItem_ShortcutIcon_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FreezeWindow();
            (new NewShortcutIconWindow()).Show();
        }

        private void BtnAdd_ContextMenuItem_Category_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FreezeWindow();
            (new NewCategoryWindow()).Show();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            btnRemove.ContextMenu.Visibility = Visibility.Visible;
            btnRemove.ContextMenu.IsOpen = true;
        }

        private void BtnRemove_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnRemove.ContextMenu.Visibility = Visibility.Hidden;
            btnRemove.ContextMenu.IsOpen = false;
        }

        private void BtnRemove_ContextMenuItem_ShortcutIcon_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.RemoveSelectedShortcutIconBorderFromCurrentGrid();
        }

        private void BtnRemove_ContextMenuItem_Category_Click(object sender, RoutedEventArgs e)
        {
            Category selectedCategory = CategoryManager.GetSelectedCategory();
            CategoryContextMenuItem categoryContextMenuItem = selectedCategory.RemoveContextMenuItem;
            CategoryBorder_RemoveMenuItem_Click(categoryContextMenuItem , e);
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            ActionManager.Undo();
            if(ActionManager.GetUndoStackCount() == 0)
            {
                btnUndo.IsEnabled = false;
            }
            else
            {
                btnUndo.IsEnabled = true;
            }
            btnRedo.IsEnabled = true;
        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {
            ActionManager.Redo();
            if (ActionManager.GetRedoStackCount() == 0)
            {
                btnRedo.IsEnabled = false;
            }
            else
            {
                btnRedo.IsEnabled = true;
            }
            btnUndo.IsEnabled = true;
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            btnMove.ContextMenu.Visibility = Visibility.Visible;
            btnMove.ContextMenu.IsOpen = true;
        }

        private void btnMove_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnMove.ContextMenu.Visibility = Visibility.Hidden;
            btnMove.ContextMenu.IsOpen = false;
        }

        private void BtnMoveCopy_Click(object sender, RoutedEventArgs e)
        {
            btnMoveCopy.ContextMenu.Visibility = Visibility.Visible;
            btnMoveCopy.ContextMenu.IsOpen = true;
        }

        private void btnMoveCopy_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnMoveCopy.ContextMenu.Visibility = Visibility.Hidden;
            btnMoveCopy.ContextMenu.IsOpen = false;
        }

        private void Chkbx_Auto_Categorised_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.AutoCategorisedMode = chkbx_Auto_Categorised.IsChecked.Value;
            RefreshTopMenuShowPanelLabels();
            ConfigManager.ModifyAutoCategorisedModeXAttrValue();
        }

        private void Chkbx_Auto_Categorised_Checked(object sender, RoutedEventArgs e)
        {
            if (CategoryManager.CategoryCount == 0)
                return;

            if (CategoryManager.MainTypeCategory == null)
            {
                Category firstMixedCategory = CategoryManager.GetFirstMixedTypeCategory();

                string title = "New main type category set by default";
                StringBuilder message = new StringBuilder();

                if (firstMixedCategory == null)
                {
                    firstMixedCategory = CategoryManager.GetFirstCategory();
                    message.Append("No mixed type category found.");
                    message.Append("\n\nCategory \"" + firstMixedCategory.Name + "\"");
                    message.Append(" has been marked as the main type category by default since");
                    message.Append(" none was marked as one.");
                    message.Append("\nIt has also been marked as mixed type because main type");
                    message.Append(" category needs to be of mixed type.");
                }
                else
                {
                    message.Append("Mixed type category \"" + firstMixedCategory.Name + "\"");
                    message.Append(" has been marked as the main type category by default since");
                    message.Append(" none was marked as one.");
                }
                message.Append("\n\nNote: A main type category is always needed when");
                message.Append(" Auto-Categorised mode is on.");

                firstMixedCategory.MarkAsMainType();
                MessageBox.Show(message.ToString(), title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Chkbx_Auto_Sort_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.AutoSortMode = chkbx_Auto_Sort.IsChecked.Value;
            RefreshTopMenuShowPanelLabels();
            ConfigManager.ModifyAutoSortModeXAttrValue();
        }

        private void Chkbx_Auto_Sort_Checked(object sender, RoutedEventArgs e)
        {
            if (CategoryManager.CategoryCount == 0)
                return;
            mainWindow.SortAllCategoryShortcutIcons();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FreezeWindow();
            (new SearchWindow()).Show();
        }
    }
}
