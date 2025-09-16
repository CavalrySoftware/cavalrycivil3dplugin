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
using CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.ViewModels;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SelectObjectsView : Window
    {

        SelectObjectsMainViewModel selectObjectsViewModel;

        public SelectObjectsView()
        {
            InitializeComponent();
            this.Show();

            InitializeComponent();
            selectObjectsViewModel = new SelectObjectsMainViewModel();
            this.DataContext = selectObjectsViewModel;
            selectObjectsViewModel.CloseAction = this.Close;
            selectObjectsViewModel.HideAction = this.Hide;
            selectObjectsViewModel.ShowAction = this.Show;

            this.Show();

            this.Closing += (s, e) =>
            {
                selectObjectsViewModel.ClosingWindow();
            };
        }
    }
}
