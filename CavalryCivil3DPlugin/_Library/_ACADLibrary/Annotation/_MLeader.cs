using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CavalryCivil3DPlugin._Library._ACADLibrary.Annotation
{
    public class _MLeader
    {

        public static void CreateMLead(Document _autocadDocument, string _text, Point3d _leaderPoint)
        {
            Database db = _autocadDocument.Database;

           using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                MLeader mLeader = new MLeader();

                Point3d leaderPoint = new Point3d(_leaderPoint.X, _leaderPoint.Y, 0);
                Point3d textPoint = new Point3d(_leaderPoint.X + 5, _leaderPoint.Y, 0);

                int leaderIndex = mLeader.AddLeader();
                int lineIndex = mLeader.AddLeaderLine(leaderIndex);

                mLeader.AddFirstVertex(lineIndex, leaderPoint);
                mLeader.AddLastVertex(lineIndex, textPoint);    

                mLeader.ContentType = ContentType.MTextContent;
                MText mtext = new MText();  
                mtext.Contents = _text;
                mtext.Location = textPoint;

                mLeader.MText = mtext;

                blockTableRecord.AppendEntity(mLeader);
                tr.AddNewlyCreatedDBObject(mLeader, true);

                tr.Commit();
            }
        }
    }
}
