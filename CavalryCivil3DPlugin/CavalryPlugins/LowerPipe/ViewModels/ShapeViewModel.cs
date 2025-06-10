using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Aec.DatabaseServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{

    public enum ShapeType
    {
        Ellipse,
        Line,
        Text,
        Circle
    }

    public class ShapeViewModel : INotifyPropertyChanged
    {
        public ShapeType Type { get; set; } = ShapeType.Line;

        public double X { get; set; }      // Location point
        public double Y { get; set; }      // Location point

        public double X1 { get; set; }      // For Line: start X
        public double Y1 { get; set; }      // For Line: start Y
        public double X2 { get; set; }      // For Line: end X
        public double Y2 { get; set; }      // For Line: end Y

        public string Text { get; set; } = "";
        public double FontSize { get; set; } = 14;

        public Brush Stroke { get; set; } = Brushes.Blue;

        public double Diameter { get; set; }

        public Brush Fill { get; set; } = Brushes.Blue;
        public double StrokeThickness { get; set; } = 1;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
