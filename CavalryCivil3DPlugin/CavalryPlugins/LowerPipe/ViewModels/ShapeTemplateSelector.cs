using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{
    public class ShapeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LineTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate CircleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ShapeViewModel shape)
            {
                switch (shape.Type)
                {
                    case ShapeType.Line:
                        return LineTemplate;

                    case ShapeType.Text:
                        return TextTemplate;

                    case ShapeType.Circle:
                        return CircleTemplate;

                    default:
                        return null;
                }
            }
            return null;
        }
    }
}

