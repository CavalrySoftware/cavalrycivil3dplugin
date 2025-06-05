using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{
    public class ShapeViewModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; } = 50;
        public double Height { get; set; } = 50;
        public Brush Fill { get; set; } = Brushes.Blue;
    }
}
