using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Views;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe
{
    public class LowerPipeStartUp
    {
        [CommandMethod("LowerPipe")]
        public void LowerPipe()
        {
            try
            {
                LowerPipeMainWindow mainWindow = new LowerPipeMainWindow();
            }

            catch (System.Exception ex) {_Console.ShowConsole(ex);}
            
        }
    }
}
