using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe;

namespace CavalryCivil3DPlugin.CavalryAddin.PressurePipes
{
    public class LowerPipeCommandHandler : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var cdoc = CivilApplication.ActiveDocument;

            if (cdoc == null)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("No Civil 3D Application is open.");
                return;
            }

            else
            {
                LowerPipeStartUp.LowerPipe();
            }
        }
    }
}
