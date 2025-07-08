using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Civil.DatabaseServices;
using System.Configuration.Assemblies;
using System.IO;

namespace CavalryCivil3DPlugin.CavalryAddin.PressurePipes
{
    internal class _PressurePipePanel 
    {

        private RibbonPanelSource _panelSource;
        private string _MainAssemblyPath;

        private RibbonPanel ribbonPanel;
        private RibbonTab _MainTab;

        public _PressurePipePanel(RibbonTab _tab, string _mainAssemblyPath)
        {
            _MainTab = _tab;
            _MainAssemblyPath = _mainAssemblyPath;

            _panelSource = new RibbonPanelSource () { Title = "Pressure Pipes" };
            ribbonPanel = new RibbonPanel() { Source = _panelSource };

            _MainTab.Panels.Add(ribbonPanel);

            CreateLowerPipeButton();
        }

        private void CreateLowerPipeButton()
        {
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(_MainAssemblyPath), "Resources", "LowerPipe_32.ico"));
            BitmapImage bitmapImage = new BitmapImage(uri);
            RibbonButton ribbonButton = new RibbonButton()
            {
                Text = "Pipe Lowering",
                ShowText = true,
                ShowImage = true,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                LargeImage = bitmapImage,
                Size = RibbonItemSize.Large,
                CommandHandler = new LowerPipeCommandHandler()
            };

            _panelSource.Items.Add(ribbonButton);
        }
    }
}
