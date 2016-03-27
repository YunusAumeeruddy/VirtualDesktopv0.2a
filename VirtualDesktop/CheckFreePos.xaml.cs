using System.Collections.Generic;
using System.Windows;


namespace VirtualDesktop
{
    /// <summary>
    /// Interaction logic for CheckFreePos.xaml
    /// </summary>
    public partial class CheckFreePos : Window
    {
        public CheckFreePos()
        {
            InitializeComponent();
        }

        private void btnCheckFreePos_Click(object sender, RoutedEventArgs e)
        {
            int colNo = -1;
            string colNoStr = txtColNo.Text;
            int.TryParse(colNoStr , out colNo);
            if(colNo != -1)
            {
                ShortcutIconGrid scGrid = CategoryManager.GetSelectedShortcutIconGrid();
                List<ShortcutIconGridPosition> list = scGrid.GetShortcutIconFreePositionList(colNo);
                if(list == null)
                {
                    txtResult.Text = "Column no not found";
                }
                else
                {
                    txtResult.Text = "";
                    foreach (ShortcutIconGridPosition pos in list)
                    {
                        txtResult.Text += pos.ToString() + "\n";
                    }
                }
            }
            else
            {
                txtResult.Text = "Invalid column no";
            }
        }
    }
}
