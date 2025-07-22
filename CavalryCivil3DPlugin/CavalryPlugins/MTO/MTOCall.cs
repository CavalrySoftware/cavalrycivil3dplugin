using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.CavalryPlugins.MTO.View;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class MTOCall
    {

        [CommandMethod("MTO")]
        public void Mto()
        {
            MTOWindow MTOWindow_ = new MTOWindow(); 
        }
    }
}
