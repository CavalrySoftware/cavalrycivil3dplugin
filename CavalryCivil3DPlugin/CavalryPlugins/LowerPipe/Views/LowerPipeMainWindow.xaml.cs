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
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Views
{
    /// <summary>
    /// Interaction logic for LowerPipeMainWindow.xaml
    /// </summary>
    public partial class LowerPipeMainWindow : Window
    {
        LowerPipeMainViewModel mainViewModel;

        public LowerPipeMainWindow()
        {
            InitializeComponent();
            mainViewModel = new LowerPipeMainViewModel();
            //mainViewModel.CloseAction = this.Close;
            //mainViewModel.HideAction = this.Hide;
            this.DataContext = mainViewModel;
            this.Show();
        }

        private void ClippingBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var border = (Border)sender;
            var thickness = border.BorderThickness;
            var rect = new Rect(
                thickness.Left,
                thickness.Top,
                border.ActualWidth - thickness.Left - thickness.Right,
                border.ActualHeight - thickness.Top - thickness.Bottom);

            border.Clip = new RectangleGeometry(rect);
        }
    }


}
