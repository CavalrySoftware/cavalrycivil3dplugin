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
    internal class _ElementsPanel
    {

        private RibbonPanelSource _panelSource;
        private string _MainAssemblyPath;

        private RibbonPanel ribbonPanel;
        private RibbonTab _MainTab;

        public _ElementsPanel(RibbonTab _tab, string _mainAssemblyPath)
        {
            _MainTab = _tab;
            _MainAssemblyPath = _mainAssemblyPath;

            _panelSource = new RibbonPanelSource() { Title = "Elements" };
            ribbonPanel = new RibbonPanel() { Source = _panelSource };

            _MainTab.Panels.Add(ribbonPanel);

            CreateBlockToVertexButton();
        }


        private void CreateBlockToVertexButton()
        {
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(_MainAssemblyPath), "Resources", "Vertex_32.ico"));
            BitmapImage bitmapImage = new BitmapImage(uri);
            RibbonButton ribbonButton = new RibbonButton()
            {
                Text = "Block to Pline Vertex",
                ShowText = true,
                ShowImage = true,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                LargeImage = bitmapImage,
                Size = RibbonItemSize.Large,
                CommandHandler = new BlockToVertexCommandHandler()
            };

            _panelSource.Items.Add(ribbonButton);
        }
    }
}
