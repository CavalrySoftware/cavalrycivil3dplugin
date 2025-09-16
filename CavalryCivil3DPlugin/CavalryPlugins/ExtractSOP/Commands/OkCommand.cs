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
    public class OkCommand : CommandBase
    {

        private SOPMainViewModel _MainViewModel;

        public OkCommand(SOPMainViewModel MainViewModel)
        {
            _MainViewModel = MainViewModel;
        }


        public override void Execute(object parameter)
        {
            try
            {
                _MainViewModel.HideAction.Invoke();
                _MainViewModel.SOPMainModel_.CreateTable(_MainViewModel.TableStyle, _MainViewModel.IsAnnotate, _MainViewModel.Prefix);
                _MainViewModel.ShowAction.Invoke();
            }

            catch (Exception ex) { _Console.ShowConsole(ex.ToString()); }
        }
    }
}
