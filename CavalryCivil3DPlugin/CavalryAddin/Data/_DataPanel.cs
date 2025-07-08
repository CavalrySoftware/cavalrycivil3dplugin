using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Windows;
using CavalryCivil3DPlugin.CavalryAddin.PressurePipes;
using System.Windows.Media.Imaging;
using System.IO;

namespace CavalryCivil3DPlugin.CavalryAddin.Data
{
    internal class _DataPanel
    {

        private RibbonPanelSource _panelSource;
        private string _MainAssemblyPath;

        private RibbonPanel ribbonPanel;
        private RibbonTab _MainTab;

        public _DataPanel(RibbonTab _tab, string _mainAssemblyPath)
        {
            _MainTab = _tab;
            _MainAssemblyPath = _mainAssemblyPath;

            _panelSource = new RibbonPanelSource() { Title = "Data" };
            ribbonPanel = new RibbonPanel() { Source = _panelSource };

            _MainTab.Panels.Add(ribbonPanel);

            CreateExtractCoordinateseButton();
        }

        private void CreateExtractCoordinateseButton()
        {
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(_MainAssemblyPath), "Resources", "ExtractCoordinates_32.ico"));
            BitmapImage bitmapImage = new BitmapImage(uri);
            RibbonButton ribbonButton = new RibbonButton()
            {
                Text = "Extract Coordinates",
                ShowText = true,
                ShowImage = true,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                LargeImage = bitmapImage,
                Size = RibbonItemSize.Large,
                CommandHandler = new ExtractCoordinatesCommandHandler()
            };

            _panelSource.Items.Add(ribbonButton);
        }
    }
    }
