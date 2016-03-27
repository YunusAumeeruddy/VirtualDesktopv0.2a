using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VirtualDesktop
{
    public class ShortcutIconBorder : Border
    {
        public ShortcutIcon ShortcutIcon { get; set; }
        public UnderInlineTextBlock TextBlock { get; set; }
        private MouseEventHandler mouseEnterEvent;
        private MouseEventHandler mouseLeaveEvent;
        private MouseButtonEventHandler previewMouseLeftButtonDownEvent;
        private MouseButtonEventHandler previewMouseRightButtonDownEvent;
        private DragEventHandler previewDragEnterEvent;
        private DragEventHandler previewDragLeaveEvent;
        private DragEventHandler previewDropEvent;

        public ShortcutIconBorder() { }

        public void Deselect()
        {
            Background = Brushes.Transparent;
            BorderBrush = Brushes.Transparent;
            ShortcutIcon.IsSelected = false;
        }

        public void DoMouseEnter()
        {
            if (!ShortcutIcon.IsSelected && MainWindow.MultiShortcutIconSelectRect == null)
            {
                SolidColorBrush whiteBrushBackground = new SolidColorBrush(Colors.White);
                whiteBrushBackground.Opacity = 0.5;
                Background = whiteBrushBackground;
                BorderBrush = Brushes.White;
            }
        }

        public void DoMouseLeave()
        {
            if (!ShortcutIcon.IsSelected && MainWindow.MultiShortcutIconSelectRect == null) {
                BorderBrush = Brushes.Transparent;
                Background = Brushes.Transparent;
            }
        }

        public static ShortcutIconBorder GenerateShortcutIconBorderWOEvent(ShortcutIcon shortcutIcon)
        {
            if (shortcutIcon == null)
                return null;

            StackPanel shortcutIconPanel = new StackPanel();
            shortcutIconPanel.Orientation = Orientation.Vertical;
            shortcutIconPanel.VerticalAlignment = VerticalAlignment.Center;
            shortcutIconPanel.HorizontalAlignment = HorizontalAlignment.Center;
            shortcutIconPanel.Width = ColumnManager.CellWidth;
            shortcutIconPanel.Height = RowManager.CellHeight;
            shortcutIconPanel.Background = Brushes.Transparent;

            Image shortcutIconImage = new Image();
            shortcutIconImage.Source = shortcutIcon.IconImage.Source;
            shortcutIconImage.Width = 48;
            shortcutIconImage.Height = 48;
            shortcutIconImage.VerticalAlignment = VerticalAlignment.Center;
            shortcutIconImage.HorizontalAlignment = HorizontalAlignment.Center;

            Border imageIconBorder = new Border();
            imageIconBorder.HorizontalAlignment = HorizontalAlignment.Center;
            imageIconBorder.Width = shortcutIconPanel.Width;
            imageIconBorder.Height = shortcutIconImage.Height + 4; // 2 px space top + bottom
            imageIconBorder.Background = Brushes.Transparent;

            imageIconBorder.Child = shortcutIconImage;

            UnderInlineTextBlock textBlock = new UnderInlineTextBlock();
            textBlock.Text = shortcutIcon.LabelText;
            textBlock.FontFamily = new FontFamily("Arial");
            textBlock.FontSize = 12;
            textBlock.Width = shortcutIconPanel.Width;
            textBlock.Height = shortcutIconPanel.Height - imageIconBorder.Height;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
            textBlock.Background = Brushes.Transparent;
            textBlock.Foreground = Brushes.White;

            Border textBlockBorder = new Border();
            textBlockBorder.VerticalAlignment = VerticalAlignment.Center;
            textBlockBorder.HorizontalAlignment = HorizontalAlignment.Center;
            textBlockBorder.Width = shortcutIconPanel.Width;
            textBlockBorder.MaxHeight = textBlock.MaxHeight;
            textBlockBorder.BorderBrush = Brushes.Transparent;
            textBlockBorder.BorderThickness = new Thickness(0.1);
            textBlockBorder.Background = Brushes.Transparent;

            textBlockBorder.Child = textBlock;

            shortcutIconPanel.Children.Add(imageIconBorder);
            shortcutIconPanel.Children.Add(textBlockBorder);

            ShortcutIconBorder shortcutIconBorder = new ShortcutIconBorder();
            shortcutIconBorder.ShortcutIcon = shortcutIcon;
            shortcutIconBorder.Width = shortcutIconPanel.Width;
            shortcutIconBorder.Height = shortcutIconPanel.Height;
            shortcutIconBorder.BorderThickness = new Thickness(2);
            shortcutIconBorder.Background = Brushes.Transparent;
            shortcutIconBorder.AllowDrop = false;

            shortcutIconBorder.Child = shortcutIconPanel;

            shortcutIconBorder.ShortcutIcon = shortcutIcon;
            shortcutIconBorder.TextBlock = textBlock;

            return shortcutIconBorder;
        }

        public static string GetFormattedName(int categoryIndex, int rowNo, int colNo)
        {
            // NAME FORMAT: ShortcutIconBorder_<category_index>_<row_no>_<col_no>
            return "ShortcutIconBorder_" + categoryIndex
                    + "_" + rowNo
                    + "_" + colNo;
        }

        public MouseEventHandler MouseEnterEvent
        {
            get
            {
                return mouseEnterEvent;
            }
            set
            {
                mouseEnterEvent = value;
            }
        }

        public MouseEventHandler MouseLeaveEvent
        {
            get
            {
                return mouseLeaveEvent;
            }
            set
            {
                mouseLeaveEvent = value;
            }
        }

        public void PositionOnGrid()
        {
            ShortcutIconGridPosition gridPos = ShortcutIcon.GridPosition;
            int rowNo = gridPos.RowNo;
            int colNo = gridPos.ColNo;
            Canvas.SetLeft(this , ColumnManager.GetShortcutIconCanvasLeftMargin(colNo));
            Canvas.SetTop(this , RowManager.GetShortcutIconCanvasTopMargin(rowNo));
        }

        public DragEventHandler PreviewDragEnterEvent
        {
            get
            {
                return previewDragEnterEvent;
            }
            set
            {
                previewDragEnterEvent = value;
            }
        }

        public DragEventHandler PreviewDragLeaveEvent
        {
            get
            {
                return previewDragLeaveEvent;
            }
            set
            {
                previewDragLeaveEvent = value;
            }
        }

        public DragEventHandler PreviewDropEvent
        {
            get
            {
                return previewDropEvent;
            }
            set
            {
                previewDropEvent = value;
            }
        }

        public MouseButtonEventHandler PreviewMouseLeftButtonDownEvent
        {
            get
            {
                return previewMouseLeftButtonDownEvent;
            }
            set
            {
                previewMouseLeftButtonDownEvent = value;
            }
        }

        public MouseButtonEventHandler PreviewMouseRightButtonDownEvent
        {
            get
            {
                return previewMouseRightButtonDownEvent;
            }
            set
            {
                previewMouseRightButtonDownEvent = value;
            }
        }

        public void Select()
        {
            var background = new SolidColorBrush(Colors.White);
            background.Opacity = 0.5;
            Background = background;
            BorderBrush = Brushes.White;
            ShortcutIcon.IsSelected = true;
        }

        public void ReformatName()
        {
            FrameworkElementReferenceManager.RemoveFrameworkElement(this);
            Name = GetFormattedName(ShortcutIcon.Category.Index
                                    , ShortcutIcon.GridPosition.RowNo
                                    , ShortcutIcon.GridPosition.ColNo);
            FrameworkElementReferenceManager.AddNewFrameworkElement(this);
        }
    }
}
