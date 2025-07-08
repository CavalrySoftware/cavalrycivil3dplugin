using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil;
using CavalryCivil3DPlugin.Consoles;


namespace CavalryCivil3DPlugin.C3DLibrary.Elements
{

    public class _FeatureLines
    {

        public static List<Point3d> GetPIPoints(Document _autocadDocument, ObjectId _featureLineId)
        {

            List<Point3d > points = new List<Point3d>();

            using(Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                FeatureLine featureline = tr.GetObject(_featureLineId, OpenMode.ForRead) as FeatureLine;
                var piPoints = featureline.GetPoints(FeatureLinePointType.PIPoint);
                points.AddRange(piPoints.Cast<Point3d>());
            }

            return points;
        }
    }
}
