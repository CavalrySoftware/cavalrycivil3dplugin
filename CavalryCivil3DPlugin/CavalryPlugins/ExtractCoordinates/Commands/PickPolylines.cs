using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands
{
    public class PickPolylines : CommandBase
    {

        private ExtractCoordinatesViewModel _ViewModel;

        public PickPolylines(ExtractCoordinatesViewModel _viewModel)
        {
            _ViewModel = _viewModel;    
        }

        public override void Execute(object parameter)
        {
            _ViewModel.HideAction.Invoke();

            List<ObjectId> objectids;    

            if (_ViewModel.SelectedObjectType.ObjectName == "Polyline")
            {
                objectids = ACADObjectSelection.PickPolylines(_ViewModel.AutocadDocument);
            }

            else
            {
                objectids = C3DObjectSelection.PickFeatureLines(_ViewModel.AutocadDocument);
            }


            try
            {
                _ViewModel.ExtractCoordinatesBaseCommand.CreateCoordinatesTableFromPickedElements(objectids);
            }

            catch (Exception ex) {_Console.ShowConsole(ex.ToString()); }
            _ViewModel.CloseAction.Invoke();
        }
    }
}
