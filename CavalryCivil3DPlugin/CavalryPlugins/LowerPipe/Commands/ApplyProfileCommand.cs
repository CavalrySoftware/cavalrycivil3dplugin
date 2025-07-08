using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Commands
{
    public class ApplyProfileCommand
    {

        private LowerPipeMainViewModel _ViewModel;

        public ApplyProfileCommand(LowerPipeMainViewModel viewModel)
        {
            _ViewModel = viewModel;
        }


        public void Apply()
        {
            try
            {
                switch (_ViewModel.SelectedMethod)
                {
                    case LowerPipeMainViewModel.NotificationMethod.ModifyPipe:
                        _ViewModel.LowerPipeMainModel_.CreateProfile();
                        break;

                    case LowerPipeMainViewModel.NotificationMethod.ProfileRun:
                        _ViewModel.LowerPipeMainModel_.CreateProfile();
                        break;

                    case LowerPipeMainViewModel.NotificationMethod.ProfileRange:
                        _ViewModel.LowerPipeMainModel_.CreateProfile(_rangeOnly : true);
                        break;
                }
                
            }

            catch (Exception ex) { _Console.ShowConsole(ex.ToString()); }
        }
    }
}
