using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.ViewModels;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Views
{
    /// <summary>
    /// Interaction logic for TagMainView.xaml
    /// </summary>
    public partial class TagMainView : Window
    {
        TagMainViewModel TagMainViewModel_;
        public TagMainView()
        {

            InitializeComponent();
            TagMainViewModel_ = new TagMainViewModel();
            this.DataContext = TagMainViewModel_;
            TagMainViewModel_.CloseAction = this.Close;
            TagMainViewModel_.HideAction = this.Hide;
            TagMainViewModel_.ShowAction = this.Show;

            this.Show();

            this.Closing += (s, e) =>
            {
                TagMainViewModel_.ClosingWindow();
            };
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (msg == WM_SYSCOMMAND && wParam.ToInt32() == SC_CLOSE)
            {
                // X button clicked
                TagMainViewModel_.ClosedByXButton = true;
            }

            return IntPtr.Zero;
        }


        private void FilterKeysUpdate(object sender, DataTransferEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo != null && combo.Items.Count > 0)
            {
                _Console.ShowConsole();
                combo.SelectedIndex = 0; // reset selection whenever ItemsSource changes
            }
        }

    }
}
