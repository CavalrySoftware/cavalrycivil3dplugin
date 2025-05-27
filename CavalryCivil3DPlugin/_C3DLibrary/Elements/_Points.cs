using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace CavalryCivil3DPlugin.C3DLibrary.Elements
{
    public class _Points
    {
        public static void CreatePointGroup(Document _autocadDocument, CivilDocument _civilDocument, List<Point3d> _points, string _prefix, string _groupName)
        {
            Database db = _autocadDocument.Database;

            using (DocumentLock docLock = _autocadDocument.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    CogoPointCollection cogoPoints = _civilDocument.CogoPoints;
                    PointGroupCollection pointGroups = _civilDocument.PointGroups;
                    List<uint> pointNumbers = new List<uint>();

                    

                    int index = 1;
                    foreach (Point3d point in _points)
                    {
                        
                        ObjectId cogoPointId = cogoPoints.Add(point, true);
                        CogoPoint cogoPoint = tr.GetObject(cogoPointId, OpenMode.ForWrite) as CogoPoint;

                        cogoPoint.PointName = $"{_prefix}{index}";
                        cogoPoint.RawDescription = _groupName;
                        pointNumbers.Add(cogoPoint.PointNumber);
                        index++;
                    }

                    ObjectId pointGroupId = ObjectId.Null;

                    bool done = false;

                    while (!done)
                    {
                        try
                        {
                            pointGroupId = pointGroups.Add(_groupName);
                            done = true;
                        }
                        catch
                        {
                            try
                            {
                                _groupName = _groupName + " (1)";
                                pointGroupId = pointGroups.Add(_groupName);
                                done = true;
                            }

                            catch { _groupName = _groupName + " (1)"; }
                        }
                    }
                    

                    PointGroup newgroup = tr.GetObject(pointGroupId, OpenMode.ForWrite) as PointGroup;

                    StandardPointGroupQuery query = new StandardPointGroupQuery();
                    query.IncludeNumbers = $"{pointNumbers.First()} - {pointNumbers.Last()}";

                    newgroup.SetQuery(query);
                    newgroup.Update();

                    tr.Commit();
                }
            }
        }


        public static List<string> GetAllGroupNames(Document _autocadDocument, PointGroupCollection _pointGroupCollection)
        {
            List<string> groupNames = new List<string>();
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in _pointGroupCollection)
                {
                    PointGroup pointGroup = tr.GetObject(id, OpenMode.ForRead) as PointGroup;
                    groupNames.Add(pointGroup.Name);
                }
            }
            return groupNames;
        }

    }
}
