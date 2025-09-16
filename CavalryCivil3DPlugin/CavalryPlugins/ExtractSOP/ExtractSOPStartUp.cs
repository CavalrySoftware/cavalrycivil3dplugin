using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Views;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP
{
    public class ExtractSOPStartUp
    {
        [CommandMethod ("ExtractSOP")]
        public void ExtractSOP()
        {
            try
            {
                SOPMainView sOPMainView = new SOPMainView();
            }
            
            catch (System.Exception ex)
            {
                _Console.ShowConsole(ex.ToString());
            }
        }
    }
}
