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
            if (!_ViewModel.InitialEdit)
            {
                _ViewModel._ApplyProfileCommand.Apply();
            }

            if (_ViewModel.DeleteProfile)
            {
                _ViewModel.DeleteProfiles();
            }

            _ViewModel.MainTransaction.Commit();
            _ViewModel.MainDocumentLock.Dispose();
            _ViewModel.AutocadDocument.Editor.Regen();
            _ViewModel.CloseAction.Invoke();
        }
    }
}
