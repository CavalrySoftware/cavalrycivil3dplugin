using System;
using System.Collections.Generic;
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
using CavalryCivil3DPlugin.CavalryPlugins.MTO.ViewModel;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Views
{
    /// <summary>
    /// Interaction logic for SOPMainView.xaml
    /// </summary>
    public partial class SOPMainView : Window
    {
        SOPMainViewModel SOPMainViewModel_;
        public SOPMainView()
        {
            
            

            InitializeComponent();
            SOPMainViewModel_ = new SOPMainViewModel();
            this.DataContext = SOPMainViewModel_;
            SOPMainViewModel_.CloseAction = this.Close;
            SOPMainViewModel_.HideAction = this.Hide;
            SOPMainViewModel_.ShowAction = this.Show;
            this.Show();

            this.Closing += (s, e) =>
            {
                SOPMainViewModel_.ClosingWindow();
            };
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (msg == WM_SYSCOMMAND && wParam.ToInt32() == SC_CLOSE)
            {
                // X button clicked
                SOPMainViewModel_.ClosedByXButton = true;
            }

            return IntPtr.Zero;
        }
    }
}
