using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.Internal.Render.RapidRT;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Windows;
using CavalryCivil3DPlugin.CavalryAddin.Data;
using CavalryCivil3DPlugin.CavalryAddin.PressurePipes;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin
{
    public class MainApp : IExtensionApplication
    {

        private bool _Initialized = false;

        public void Initialize()
        {
            ComponentManager.ItemInitialized += ComponentManagerInitialized;
        }


        public void Terminate()
        {
            ComponentManager.ItemInitialized -= ComponentManagerInitialized;
        }


        public void ComponentManagerInitialized(object sender, EventArgs e)
        {
            if (_Initialized) return;

            _Initialized = true;
            ComponentManager.ItemInitialized -= ComponentManagerInitialized;

            try
            {
                InitializeTab();
            }

            catch (System.Exception ex)
            {
                _Console.ShowConsole(ex.ToString());
            }
        }


        public void InitializeTab ()
        {
            RibbonControl ribbonControl = ComponentManager.Ribbon;

            if (ribbonControl.Tabs.Any(tab => tab.Id == "{3D97F344-DA15-485B-8201-41A7A81DAE5A}")) return;

            string AssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            RibbonTab ribbonTab = new RibbonTab
            {
                Title = "Cavalry",
                Id = "{3D97F344-DA15-485B-8201-41A7A81DAE5A}"
            };

            ribbonControl.Tabs.Add(ribbonTab);

            _DataPanel dataPanel = new _DataPanel(ribbonTab, AssemblyPath);
            _PressurePipePanel pressurePipePanel = new _PressurePipePanel(ribbonTab, AssemblyPath);
            _ElementsPanel elementsPanel = new _ElementsPanel(ribbonTab, AssemblyPath);
        }

    }
}
