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
            using (DocumentLock docLock = _ViewModel.AutocadDocument.LockDocument())
            {
                try
                {

                    ObjectId newProfileId = _Profile.CreateProfile
                        (
                        _ViewModel.AutocadDocument,
                        _ViewModel.Civil3DDocument,
                        _ViewModel.LowerPipe.RunAlignmentId,
                        _ViewModel.UpperPipe.RunProfileId,
                        _ViewModel.LowerAnalysisModel.ProfileData
                        );

                        _ViewModel.AutocadDocument.Editor.Regen();

                    if (_ViewModel.ModifyPipe)
                    {
                        _PressurePipe.SetProfileToPipeRun(_ViewModel.AutocadDocument, _ViewModel.LowerPipe.ObjectId_, newProfileId);
                    }

                    _ViewModel.InitialEdit = true;
                    _ViewModel._NewProfileIds.Add( newProfileId );
                }

                catch (Exception ex)
                {
                    _Console.ShowConsole(ex.ToString() + "\n" + ex.Message);
                }
            }
            
        }

    }
}
