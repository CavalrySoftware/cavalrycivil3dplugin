using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Commands
{
    public class PickPressurePipes : CommandBase
    {

        private LowerPipeMainViewModel _ViewModel;

        public PickPressurePipes(LowerPipeMainViewModel _viewModel)
        {
            _ViewModel = _viewModel;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.HideAction.Invoke();
            if (Pick())
            {
                _ViewModel.ResetCalculations();
            }
            _ViewModel.ShowAction.Invoke();
        }

        public bool Pick()
        {
            ObjectId pipeUpperId = C3DObjectSelection.PickPressurePipe(_ViewModel.AutocadDocument, "Pressure Pipe to be on top");

            if (pipeUpperId == ObjectId.Null)
            {
                return false;
            }

            ObjectId pipeLowerId = C3DObjectSelection.PickPressurePipe(_ViewModel.AutocadDocument, "Pressure Pipe to be lowered");

            if (pipeLowerId == ObjectId.Null)
            {
                return false;
            }

            _ViewModel.UpperPipe.SetPipe(pipeUpperId);
            _ViewModel.LowerPipe.SetPipe(pipeLowerId);  
            
            return true;
        }
    }
}
