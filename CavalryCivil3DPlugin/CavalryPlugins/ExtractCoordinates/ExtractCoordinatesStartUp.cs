using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Views;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Views;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates
{
    public class ExtractCoordinatesStartUp
    {
        [CommandMethod("ExtractCoordinates")]
        public static void ExtractCoordinates()
        {
            try
            {
                var wnd = ExtractCoordinateWindowManager.MainWindow;

                if (wnd == null || !wnd.IsLoaded)
                {
                    wnd = new ExtractCoordinatesWindow();
                    ExtractCoordinateWindowManager.MainWindow = wnd;

                    wnd.Closed += (s, e) => ExtractCoordinateWindowManager.MainWindow = null;
                    wnd.Show();
                    wnd.Activate();
                    wnd.Topmost = true;
                    wnd.Topmost = false;
                    wnd.Focus();
                }

                else
                {
                    if (wnd.WindowState == WindowState.Minimized)
                        wnd.WindowState = WindowState.Normal;

                    wnd.Activate();
                    wnd.Topmost = true;
                    wnd.Topmost = false;
                    wnd.Focus();
                }
            }
            catch (System.Exception ex)
            {
                _Console.ShowConsole(ex);
            }
        }
    }
}

