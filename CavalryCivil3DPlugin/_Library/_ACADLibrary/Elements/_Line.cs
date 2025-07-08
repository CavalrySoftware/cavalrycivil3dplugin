using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CavalryCivil3DPlugin.ACADLibrary.Selection;

namespace CavalryCivil3DPlugin._ACADLibrary.Elements
{
    public class _Line
    {

        public static (Point3d, Point3d) GetPoints(Document _autocadDocument, ObjectId _lineId)
        {
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Line line = tr.GetObject(_lineId, OpenMode.ForRead) as Line;
                Point3d p1 = line.StartPoint;
                Point3d p2 = line.EndPoint;

                return (p1, p2);    
            }
        }

        public static (Point2d, Point2d) GetPoints2d(Document _autocadDocument, ObjectId _lineId)
        {
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Line line = tr.GetObject(_lineId, OpenMode.ForRead) as Line;
                Point3d p1 = line.StartPoint;
                Point3d p2 = line.EndPoint;

                Point2d p1xy = new Point2d(p1.X, p1.Y);
                Point2d p2xy = new Point2d(p2.X, p2.Y);

                return (p1xy, p2xy);
            }
        }

        public static (ObjectId, (Point3d, Point3d)) GetSegment(Document _autocadDocument, string _promptSelection = "Line")
        {
            ObjectId _lineId = ACADObjectSelection.PickLine(_autocadDocument, _promptSelection);

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Line line = tr.GetObject(_lineId, OpenMode.ForRead) as Line;

                return (_lineId, (line.StartPoint, line.EndPoint));
            }
        }
    }
}
