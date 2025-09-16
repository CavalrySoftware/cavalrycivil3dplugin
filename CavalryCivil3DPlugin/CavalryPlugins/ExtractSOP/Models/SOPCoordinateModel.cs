using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models
{
    public class SOPCoordinateModel
    {
        private List<Point3d> _DesignCoordinates;
        public List<Point3d> DesignCoordinates
        {
            get { return _DesignCoordinates; }
            set { _DesignCoordinates = value; }
        }

        private List<Point3d> _ExistingCoordinates;
        public List<Point3d> ExistingCoordinates
        {
            get { return _ExistingCoordinates; }
            set { _ExistingCoordinates = value; }
        }
    }
}
