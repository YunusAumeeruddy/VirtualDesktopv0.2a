using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VirtualDesktop
{
    public class FreeSpaceCanvas : Canvas
    {
        public Canvas MainCanvas;
        public int CategoryIndex { get; set; }
        private ShortcutIconGridPosition position;

        public static bool IsFreeSpaceCanvasVisible = false;

        public static Brush TransparentBackgroundBrush = Brushes.Transparent;
        public static Brush VisibleBackgroundBrush = Brushes.Red;
        public static Brush MouseEnterBackgroundBrush = Brushes.White;
        public static Brush CurrentBackgroundBrush = TransparentBackgroundBrush;

        public FreeSpaceCanvas() { }

        public FreeSpaceCanvas(int categoryIndex , ShortcutIconGridPosition position , Canvas mainCanvas)
        {
            CategoryIndex = categoryIndex;
            this.position = position;
            MainCanvas = mainCanvas;
        }

        public double GetCanvasLeft()
        {
            return Canvas.GetLeft(this);
        }

        public double GetCanvasLeftInclWidth()
        {
            return Canvas.GetLeft(this) + Width;
        }

        public double GetCanvasTop()
        {
            return Canvas.GetTop(this);
        }

        public double GetCanvasTopInclHeight()
        {
            return Canvas.GetTop(this) + Height;
        }

        public static string GetFormattedName(int categoryIndex, int rowNo, int colNo)
        {
            // NAME FORMAT: FreeSpaceCanvas_<category_index>_<row_no>_<col_no>
            return "FreeSpaceCanvas_" + categoryIndex
                    + "_" + rowNo
                    + "_" + colNo;
        }

        public static FreeSpaceCanvas GetFreeSpaceCanvas(int categoryIndex , int rowNo , int colNo)
        {
            string freeSpaceCanvasToRemove_Name = GetFormattedName(categoryIndex, rowNo, colNo);
            FrameworkElement freeSpaceCanvas_Element =
                FrameworkElementReferenceManager.GetFrameworkElement(freeSpaceCanvasToRemove_Name);
            if (freeSpaceCanvas_Element == null)
                return null;
            FreeSpaceCanvas freeSpaceCanvas = (FreeSpaceCanvas)freeSpaceCanvas_Element;
            return freeSpaceCanvas;
        }

        public ShortcutIconGridPosition GridPosition
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public void ReformatName()
        {
            Name = GetFormattedName(CategoryIndex , position.RowNo , position.ColNo);
        }

        public void RemoveControlFromMainCanvas()
        {
            MainCanvas.Children.Remove(this);
            FrameworkElementReferenceManager.RemoveFrameworkElement(this);
        }

        public static void ToggleFreeSpaceCanvasVisibility()
        {
            IsFreeSpaceCanvasVisible = !IsFreeSpaceCanvasVisible;
            if (IsFreeSpaceCanvasVisible)
            {
                CurrentBackgroundBrush = VisibleBackgroundBrush;
            }
            else
            {
                CurrentBackgroundBrush = TransparentBackgroundBrush;
            }
            foreach (var fsc in FrameworkElementReferenceManager.GetAllFreeSpaceCanvas())
            {
                fsc.Background = CurrentBackgroundBrush;
            }
        }
    }
}
