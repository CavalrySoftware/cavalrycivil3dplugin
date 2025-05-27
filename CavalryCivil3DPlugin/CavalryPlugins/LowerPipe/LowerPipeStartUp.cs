using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
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
                Test test = new Test();
            }

            catch (System.Exception ex) {_Console.ShowConsole(ex);}
            
        }
    }
}
