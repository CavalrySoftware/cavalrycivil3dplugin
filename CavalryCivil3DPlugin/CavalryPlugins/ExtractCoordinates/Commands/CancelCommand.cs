using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands
{
    public class CancelCommand : CommandBase
    {

        private ExtractCoordinatesViewModel _ViewModel;  

        public CancelCommand(ExtractCoordinatesViewModel _viewModel)
        {
            _ViewModel = _viewModel;    
        }
        public override void Execute(object parameter)
        {
            _ViewModel.CloseAction.Invoke();
            _ViewModel._Editor.Regen();
        }
    }
}
