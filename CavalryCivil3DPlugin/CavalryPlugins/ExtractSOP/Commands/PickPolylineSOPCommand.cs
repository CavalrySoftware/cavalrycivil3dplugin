using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Commands
{
    public class PickPolylineSOPCommand : CommandBase
    {

        private SOPMainViewModel _MainViewModel;

        public PickPolylineSOPCommand(SOPMainViewModel MainViewModel)
        {
            _MainViewModel = MainViewModel;
        }


        public override void Execute(object parameter)
        {
            try
            {
                _MainViewModel.HideAction.Invoke();
                _MainViewModel.SOPMainModel_.PolylineReferenceModel.SelectPolyline();
                _MainViewModel.SOPMainModel_.SetSop(_MainViewModel.DesignSurface, _MainViewModel.ExistingSurface);
                _MainViewModel.RefreshSOPs();
                _MainViewModel.ShowAction.Invoke();
            }

            catch (Exception ex) {_Console.ShowConsole(ex.ToString());}
            

   
        }
    }
}
