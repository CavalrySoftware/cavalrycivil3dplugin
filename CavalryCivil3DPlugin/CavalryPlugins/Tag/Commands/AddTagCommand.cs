using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.ViewModels;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Commands
{
    public class AddTagCommand : CommandBase
    {

        private TagMainViewModel _MainViewModel;

        public AddTagCommand(TagMainViewModel MainViewModel)
        {
            _MainViewModel = MainViewModel;
        }


        public override void Execute(object parameter)
        {
            try
            {
                _MainViewModel.HideAction.Invoke();
                _MainViewModel.SelectedTag.TagObjectExecute(_MainViewModel.SelectedMethod, _MainViewModel.SelectedFilterType.Name, _MainViewModel.SelectedFilterKey_);
                _MainViewModel.ShowAction.Invoke();
            }

            catch (Exception ex) 
            { 
                _Console.ShowConsole(ex.ToString()); 
            }
        }
    }
}
