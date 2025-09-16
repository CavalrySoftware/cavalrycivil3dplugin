using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal.Calculator;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models
{
    public class SOPObjectModel
    {

		private double _X;
		public double X
		{
			get { return _X; }
			set { _X = value; }
		}

		private double _Y;
		public double Y
		{
			get { return _Y; }
			set { _Y = value; }
		}

		private double? _DesignElevation;

		public double? DesignElevation
		{
			get { return _DesignElevation; }
			set { _DesignElevation = value; }
		}

		private double? _ExistingElevation;
		public double? ExistingElevation
		{
			get { return _ExistingElevation; }
			set { _ExistingElevation = value; }
		}

		private int _PointNumber;
		public int PointNumber
		{
			get { return _PointNumber; }
			set { _PointNumber = value; }
		}


		public Point2d XY
		{
			get
			{
				return new Point2d(X, Y);
			}
		}

		public string PointName
		{
			get
			{
				return $"P{PointNumber}";
			}
		}


		public SOPObjectModel (Point3d _coordinates, int _pointNumber)
		{
			X = _coordinates.X;
			Y = _coordinates.Y;
			_PointNumber = _pointNumber;
		}

	}
}
