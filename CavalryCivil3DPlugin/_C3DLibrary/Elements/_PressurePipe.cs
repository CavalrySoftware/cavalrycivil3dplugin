using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin._ACADLibrary.Geometry;

namespace CavalryCivil3DPlugin._C3DLibrary.Elements
{
    public class _PressurePipe
    {

        /// <summary>
        /// Gets the AlignmentId of the PipeRun of the pipe if it is belong to PipeRun, otherwise return Null Id
        /// </summary>
        /// <param name="_pipe"></param>
        public static ObjectId GetAlignmentId(ObjectId _pipe, Document _autocadDocument)
        {
            Database db = _autocadDocument.Database;
           
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PressurePipe pressurePipe = tr.GetObject(_pipe, OpenMode.ForRead) as PressurePipe;

                PressurePipeNetwork pressureNetwork = tr.GetObject(pressurePipe.NetworkId, OpenMode.ForRead) as PressurePipeNetwork;
                PressurePipeRunCollection pipeRuns = pressureNetwork.PipeRuns;

                foreach (PressurePipeRun pressurePipeRun in pipeRuns)
                {
                    var parts = pressurePipeRun.GetPartIds();

                    foreach (ObjectId part in parts)
                    {
                        if (_pipe.Equals(part))
                        {
                            return pressurePipeRun.AlignmentId; 
                        }
                    }
                }
                return ObjectId.Null;
            }
        }


        /// <summary>
        /// Gets the ProfileId of the PipeRun of the pipe if it is belong to PipeRun, otherwise return Null Id
        /// </summary>
        /// <param name="_pipe"></param>
        /// <param name="_autocadDocument"></param>
        /// <returns></returns>
        public static ObjectId GetProfileId(ObjectId _pipe, Document _autocadDocument)
        {
            Database db = _autocadDocument.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PressurePipe pressurePipe = tr.GetObject(_pipe, OpenMode.ForRead) as PressurePipe;

                PressurePipeNetwork pressureNetwork = tr.GetObject(pressurePipe.NetworkId, OpenMode.ForRead) as PressurePipeNetwork;
                PressurePipeRunCollection pipeRuns = pressureNetwork.PipeRuns;

                foreach (PressurePipeRun pressurePipeRun in pipeRuns)
                {
                    var parts = pressurePipeRun.GetPartIds();

                    foreach (ObjectId part in parts)
                    {
                        if (_pipe.Equals(part))
                        {
                            return pressurePipeRun.ProfileId;
                        }
                    }
                }
                return ObjectId.Null;
            }
        }


        public static ObjectId GetPipeRunId(ObjectId _pipe, Document _autocadDocument)
        {
            Database db = _autocadDocument.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PressurePipe pressurePipe = tr.GetObject(_pipe, OpenMode.ForRead) as PressurePipe;

                PressurePipeNetwork pressureNetwork = tr.GetObject(pressurePipe.NetworkId, OpenMode.ForRead) as PressurePipeNetwork;
                PressurePipeRunCollection pipeRuns = pressureNetwork.PipeRuns;

                foreach (PressurePipeRun pressurePipeRun in pipeRuns)
                {
                    var parts = pressurePipeRun.GetPartIds();

                    foreach (ObjectId part in parts)
                    {
                        if (_pipe.Equals(part))
                        {
                            return pressurePipeRun.ProfileId;
                        }
                    }
                }
                return ObjectId.Null;
            }
        }


        public static void SetProfileToPipeRun(Document _autocadDocument,  ObjectId _pressurePipeId, ObjectId profileId)
        {
            using (DocumentLock docLock = _autocadDocument.LockDocument())
            {
                using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
                {
                    PressurePipe pressurePipe = tr.GetObject(_pressurePipeId, OpenMode.ForWrite) as PressurePipe;

                    PressurePipeNetwork pressureNetwork = tr.GetObject(pressurePipe.NetworkId, OpenMode.ForWrite) as PressurePipeNetwork;
                    PressurePipeRunCollection pipeRuns = pressureNetwork.PipeRuns;

                    foreach (PressurePipeRun pressurePipeRun in pipeRuns)
                    {
                        var parts = pressurePipeRun.GetPartIds();

                        foreach (ObjectId part in parts)
                        {
                            if (_pressurePipeId.Equals(part))
                            {
                                pressurePipeRun.FollowProfile(profileId, 0, false);
                                break;
                            }
                        }
                    }

                    tr.Commit();

                }
            }
        }


        public static Point2d GetIntersectionPoint(Document _autocadDocument, ObjectId _pressurePipeId1, ObjectId _pressurePipeId2)
        {
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                PressurePipe pressurePipeUpper = tr.GetObject(_pressurePipeId1, OpenMode.ForRead) as PressurePipe;
                PressurePipe pressurePipeLower = tr.GetObject(_pressurePipeId2, OpenMode.ForRead) as PressurePipe;

                Point3d pipeUpperp1 = pressurePipeUpper.StartPoint;
                Point3d pipeUpperp2 = pressurePipeUpper.EndPoint;
                Point3d pipeLowerp1 = pressurePipeLower.StartPoint;
                Point3d pipeLowerp2 = pressurePipeLower.EndPoint;

                (Point2d, Point2d) line1 = (new Point2d(pipeUpperp1.X, pipeUpperp1.Y), new Point2d(pipeUpperp2.X, pipeUpperp2.Y));
                (Point2d, Point2d) line2 = (new Point2d(pipeLowerp1.X, pipeLowerp1.Y), new Point2d(pipeLowerp2.X, pipeLowerp2.Y));

                Point2d intersectionPoint = Lines.GetIntersectionPoint(line1, line2);

                return intersectionPoint;
            }
        }

    }
}
