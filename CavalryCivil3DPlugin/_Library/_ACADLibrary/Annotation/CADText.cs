using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.ACADLibrary.Annotation
{
    public class CADText
    {
        
        public static void AnnotateAtPoints(string _prefix, List<Point3d> _points, Document _autocadDocument, double _xOffset=0, double _yOffset=0)
        {
            try
            {
                using (DocumentLock docLock = _autocadDocument.LockDocument())
                {
                    using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
                    {
                        BlockTable blockTable = tr.GetObject(_autocadDocument.Database.BlockTableId, OpenMode.ForWrite) as BlockTable;
                        BlockTableRecord blockTableRecord = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        int index = 1;

                        foreach (Point3d point in _points)
                        {
                            DBText text = new DBText();
                            text.Position = new Point3d(point.X + _xOffset, point.Y + _yOffset, point.Z);
                            text.Height = 2.5;

                            text.TextString = $"{_prefix}{index}";

                            blockTableRecord.AppendEntity(text);
                            tr.AddNewlyCreatedDBObject(text, true);
                            index++;
                        }

                        tr.Commit();
                    }
                }
            }

            catch (Exception ex) { ConsoleBasic console = new ConsoleBasic(ex.ToString()); } 
        }


    }
}
