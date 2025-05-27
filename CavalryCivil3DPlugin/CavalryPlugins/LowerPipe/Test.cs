using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin._ACADLibrary.Elements;
using CavalryCivil3DPlugin._ACADLibrary.Geometry;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;
using CivilEntity = Autodesk.Civil.DatabaseServices.Entity;
using ACADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe
{
    public class Test
    {

        CivilDocument Civil3DDocument;
        Document AutocadDocument;
        Database AutocadDatabase;
        Editor _Editor;


        public Test()
        {
            Civil3DDocument = CivilApplication.ActiveDocument;
            AutocadDocument = Application.DocumentManager.MdiActiveDocument;
            AutocadDatabase = AutocadDocument.Database;
            _Editor = AutocadDocument.Editor;


            //TestProgram();
            Test5();

        }


        public void TestProgram()
        {
            ObjectId profileViewID = C3DObjectSelection.PickProfileView(AutocadDocument);
            //_Console.ShowConsole(profileViewID.ToString());


            using (Transaction tr = AutocadDatabase.TransactionManager.StartTransaction())
            {
                ProfileView profileView = tr.GetObject(profileViewID, OpenMode.ForRead) as ProfileView;
                ObjectId alignmentId = profileView.AlignmentId; 

                Alignment pipeRunAlignment = tr.GetObject(alignmentId, OpenMode.ForRead) as Alignment;

                var profiles = pipeRunAlignment.GetProfileIds();

                List<string> profileTypes = new List<string>();

                foreach (ObjectId id in profiles)
                {
                    Profile profile =  tr.GetObject(id, OpenMode.ForRead) as Profile;
                    profileTypes.Add(profile.ProfileType.ToString());
                }

                _Console.ShowConsole(profileTypes);   
            }

        }

        public void Test2 ()
        {
            ObjectId pressurePipeId = C3DObjectSelection.PickPressurePipe(AutocadDocument);
            _Console.ShowConsole(pressurePipeId.ToString());
        }


        public void Test3()
        {
            ObjectId line1 = ACADObjectSelection.PickLine(AutocadDocument);
            ObjectId line2 = ACADObjectSelection.PickLine(AutocadDocument);
            Point2d intersectionPoint = Lines.GetInterectionPointOfLines(AutocadDocument ,line1, line2);
            _Console.ShowConsole(intersectionPoint.ToString());
        }


        public void Test4()
        {
            ObjectId line1 = ACADObjectSelection.PickLine(AutocadDocument);
            (Point3d p1, Point3d p2) = _Line.GetPoints(AutocadDocument, line1);
            
            var point = _Line.GetPoints2d(AutocadDocument, line1);

            //_Console.ShowConsole(point.ToString());    
        }

        public void Test5()
        {
            using (Transaction tr = AutocadDatabase.TransactionManager.StartTransaction())
            {
                ObjectId objectId1 = ACADObjectSelection.PickObject(AutocadDocument, "Select 1st Element");
                ObjectId objectId2 = ACADObjectSelection.PickObject(AutocadDocument, "Select 2nd Element");
                ObjectId objectId3 = ACADObjectSelection.PickObject(AutocadDocument, "Select reference Element");

                Circle entity1 = tr.GetObject(objectId1, OpenMode.ForRead) as Circle;
                Circle entity2 = tr.GetObject(objectId2, OpenMode.ForRead) as Circle;
                Circle entity3 = tr.GetObject(objectId3, OpenMode.ForRead) as Circle;

                Point3d p1 = entity1.Center;
                Point3d p2 = entity2.Center;
                Point3d p3 = entity3.Center;

                Point2d point = new Point2d(p3.X, p3.Y);

                Point3d coordinateAtPoint = Lines.GetZValueAtPoint(p1, p2, point);

                //_Console.ShowConsole(coordinateAtPoint.ToString());

            }
        }



    }
}
