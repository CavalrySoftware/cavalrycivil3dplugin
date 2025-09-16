using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace CavalryCivil3DPlugin._Library._C3DLibrary.Elements
{
    public class _Alignment
    {
        public static Point3d GetMidPoint(Document _autocadDocument, ObjectId _objectId)
        {
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Alignment alignment = tr.GetObject(_objectId, OpenMode.ForRead) as Alignment;

                double startStation = alignment.StartingStation;
                double endStation = alignment.EndingStation;

                // Midpoint station
                double midStation = (startStation + endStation) / 2.0;

                double x = 0;
                double y = 0;

                alignment.PointLocation(midStation, 0, ref x, ref y);

                return new Point3d(x, y, 0);
            }
        }
    }
}
