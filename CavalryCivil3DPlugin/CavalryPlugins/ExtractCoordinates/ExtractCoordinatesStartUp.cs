using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Views;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates
{
    public class ExtractCoordinatesStartUp
    {
        [CommandMethod("ExtractCoordinates")]
        public static void ExtractCoordinates()
        {
            try{ ExtractCoordinatesWindow mainWindow = new ExtractCoordinatesWindow(); }
            catch (System.Exception e){ ConsoleBasic console = new ConsoleBasic(e.ToString());}
        }
    }
}

