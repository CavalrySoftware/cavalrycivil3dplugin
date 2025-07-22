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
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.CavalryPlugins.MTO.ViewModel;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MTOWindow : Window
    {
        MTOMainViewModel MTOMainViewModel_;

        public MTOWindow()
        {
            InitializeComponent();
            MTOMainViewModel_ = new MTOMainViewModel();
            this.DataContext = MTOMainViewModel_;
            this.Show();
        }
    }
}
