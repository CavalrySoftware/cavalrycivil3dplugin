using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Commands
{
    public class CancelCommand : CommandBase
    {
       
        private LowerPipeMainViewModel _ViewModel;

        public CancelCommand(LowerPipeMainViewModel _viewModel)
        {
            _ViewModel = _viewModel;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.LowerPipeMainModel_.CloseModel();
            _ViewModel.CloseAction.Invoke();
        }
    }
}
