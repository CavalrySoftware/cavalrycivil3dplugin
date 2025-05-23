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
using System.Windows.Threading;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;
using CavalryCivil3DPlugin.Consoles;



namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Views
{
    public partial class ExtractCoordinatesWindow : Window
    {
        ExtractCoordinatesViewModel mainViewModel;

        public ExtractCoordinatesWindow()
        {
            InitializeComponent();
            mainViewModel = new ExtractCoordinatesViewModel();  
            mainViewModel.CloseAction = this.Close;
            mainViewModel.HideAction = this.Hide;
            this.DataContext = mainViewModel;
            this.Show();
        }

        private void FilterKeySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mainViewModel.UpdateAvailableObjects(sender);
            }
            catch (Exception ex) {_Console.ShowConsole(ex.ToString()); }
        }

        private void EntitySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainViewModel.UpdateSelectedEntities(sender);
        }

        private void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            var expander = sender as Expander;
            Dispatcher.BeginInvoke(new Action(() => expander.IsExpanded = false), DispatcherPriority.ContextIdle);
        }
    }
}
