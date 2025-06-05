using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CavalryCivil3DPlugin._ACADLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin._ACADLibrary.Geometry
{
    public class Lines
    {
        public static Point2d GetIntersectionPoint((Point2d, Point2d) _line1, (Point2d, Point2d) _line2)
        {
            (Point2d p1, Point2d p2) = _line1;
            (Point2d p3, Point2d p4) = _line2;

            // Analyzing line 1
            double mline1 = (p2.Y -  p1.Y) / (p2.X - p1.X);
            double bline1 = p1.Y - (mline1 * p1.X);

            // Analyzing line 2
            double mline2 = (p4.Y - p3.Y) / (p4.X - p3.X);
            double bline2 = p3.Y - (mline2 * p3.X);

            // Solve for intersection x point
            double x = (bline2 - bline1) / (mline1 - mline2);

            // Solve for intersection y point 
            double y = (mline1 * x) + bline1;

            Point2d intersectionPoint = new Point2d(x, y);

            return intersectionPoint;
        }


        public static Point2d GetInterectionPointOfLines(Document _autocadDocument, ObjectId _line1, ObjectId _line2)
        {

            (Point2d, Point2d) line1 = _Line.GetPoints2d(_autocadDocument, _line1);
            (Point2d, Point2d) line2 = _Line.GetPoints2d(_autocadDocument, _line2);

            return GetIntersectionPoint(_line1: line1, _line2: line2);
        }


        public static Point3d GetZValueAtPoint(Point3d _endPoint1, Point3d _endPoint2, Point2d _point)
        {
            double a = _endPoint2.X - _endPoint1.X;
            double b = _endPoint2.Y - _endPoint1.Y;
            double c = _endPoint2.Z - _endPoint1.Z;
            double length3d = Math.Sqrt((a * a) + (b * b) + (c * c));

            double zDelta = _endPoint2.Z - _endPoint1.Z;
            double zHeight = Math.Abs(zDelta);
            double xDistance = Math.Sqrt((length3d * length3d) - (zHeight * zHeight));

            double xDistanceAtPoint = Math.Sqrt(Math.Pow(_point.X - _endPoint1.X, 2) + Math.Pow(_point.Y - _endPoint1.Y, 2));
            double zDeltaAtPoint = (zHeight / xDistance) * xDistanceAtPoint;
            double factor = zDelta / Math.Abs(zDelta);
            double zValueAtPoint = (factor * zDeltaAtPoint) + _endPoint1.Z;

            Point3d coordinateAtPoint = new Point3d(_point.X, _point.Y, zValueAtPoint);

            return coordinateAtPoint;   
        }


        public static double GetAngle(Point2d p1, Point2d p2)
        {
            return Tangent((p2.Y - p1.Y) * (p2.X - p1.X));
        }

        private static double Tangent(double x)
        {
            return Math.Tan(x * (Math.PI / 180));
        }

    }
}
