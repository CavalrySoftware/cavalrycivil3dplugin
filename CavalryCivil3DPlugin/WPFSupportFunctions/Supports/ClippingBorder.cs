using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace CavalryCivil3DPlugin.WPFSupportFunctions.Supports
{
    public class ClippingBorder
    {
        private void ClippingBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var border = (Border)sender;
            var thickness = border.BorderThickness;
            var rect = new Rect(
                thickness.Left,
                thickness.Top,
                border.ActualWidth - thickness.Left - thickness.Right,
                border.ActualHeight - thickness.Top - thickness.Bottom);

            border.Clip = new RectangleGeometry(rect);
        }
    }
}
