using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Views;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects
{
    public class SelectObjectsStartUps
    {
        [CommandMethod("SelectObjects")]
        public void SelectObjects()
        {
            {
                try
                {
                    SelectObjectsView selectObjectView = new SelectObjectsView();
                }

                catch (System.Exception ex)
                {
                    _Console.ShowConsole(ex.ToString());
                }
            }
        }
    }
}
