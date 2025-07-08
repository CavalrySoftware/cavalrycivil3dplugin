using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Commands
{
    public class OkCommand : CommandBase
    {

        private LowerPipeMainViewModel _ViewModel;

        public OkCommand(LowerPipeMainViewModel _viewModel)
        {
            _ViewModel = _viewModel;
        }

        public override void Execute(object parameter)
        {
            if (_ViewModel.SelectedMethod == LowerPipeMainViewModel.NotificationMethod.ModifyPipe)
            {
                _ViewModel._ApplyProfileCommand.Apply();
                _ViewModel.LowerPipeMainModel_.FollowPipe();
            }

            _ViewModel.LowerPipeMainModel_.MainTransaction.Commit();
            _ViewModel.LowerPipeMainModel_.MainDocumentLock.Dispose();
            _ViewModel.LowerPipeMainModel_.AutocadDocument.Editor.Regen();
            _ViewModel.CloseAction.Invoke();
        }
    }
}
