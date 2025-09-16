using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Views;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.Views;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag
{
    public class TagStartUp
    {

        [CommandMethod ("CavalryTag")]
        public void TagElements()
        {
            {
                try
                {
                    TagMainView TagMainView = new TagMainView();
                }

                catch (System.Exception ex)
                {
                    _Console.ShowConsole(ex.ToString());
                }
            }
        }
    }
}
