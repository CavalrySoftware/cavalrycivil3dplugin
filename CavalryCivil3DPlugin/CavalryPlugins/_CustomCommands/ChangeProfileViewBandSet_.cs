using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins._CustomCommands
{
    public class ChangeProfileViewBandSet_
    {
        [CommandMethod("SubtractProfileViewMinimum")]
        public static void SubtractMinElevation()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;

            // Filter for ProfileView
            TypedValue[] filter = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "AECC_PROFILE_VIEW")
            };

            PromptEntityOptions peo = new PromptEntityOptions("\nSelect reference Profile View:");
            peo.SetRejectMessage("\nSelected object is not a Profile View.");
            peo.AddAllowedClass(typeof(ProfileView), exactMatch: true);
            peo.AllowNone = false;

            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
            {
                return;
            }


            //PromptSelectionOptions pso = new PromptSelectionOptions();
            //pso.MessageForAdding = "\nSelect Profile Views: ";
            //pso.AllowDuplicates = false;
         
            //PromptSelectionResult psr = ed.GetSelection(pso, new SelectionFilter(filter));
            //if (psr.Status != PromptStatus.OK)
            //    return;

            // Example: change BandSetStyle for all selected ProfileViews
            using (DocumentLock documentLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    ProfileView profileView = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as ProfileView;
                    profileView.ElevationRangeMode = ElevationRangeType.UserSpecified;
                    profileView.ElevationMin = profileView.ElevationMin + 1;
    
                    tr.Commit();
                }
            }
        }


        [CommandMethod("AddProfileViewMax")]
        public static void AddMaxElevation()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;

            // Filter for ProfileView
            TypedValue[] filter = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "AECC_PROFILE_VIEW")
            };

            PromptEntityOptions peo = new PromptEntityOptions("\nSelect reference Profile View:");
            peo.SetRejectMessage("\nSelected object is not a Profile View.");
            peo.AddAllowedClass(typeof(ProfileView), exactMatch: true);
            peo.AllowNone = false;

            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
            {
                return;
            }


            //PromptSelectionOptions pso = new PromptSelectionOptions();
            //pso.MessageForAdding = "\nSelect Profile Views: ";
            //pso.AllowDuplicates = false;

            //PromptSelectionResult psr = ed.GetSelection(pso, new SelectionFilter(filter));
            //if (psr.Status != PromptStatus.OK)
            //    return;

            // Example: change BandSetStyle for all selected ProfileViews
            using (DocumentLock documentLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    ProfileView profileView = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as ProfileView;

                    profileView.ElevationRangeMode = ElevationRangeType.UserSpecified;
                    profileView.ElevationMax = profileView.ElevationMax + 1;

                    tr.Commit();
                }
            }
        }


        [CommandMethod("SubtractProfileViewMax")]
        public static void SubtractMaxElevation()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;

            // Filter for ProfileView
            TypedValue[] filter = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "AECC_PROFILE_VIEW")
            };

            PromptEntityOptions peo = new PromptEntityOptions("\nSelect reference Profile View:");
            peo.SetRejectMessage("\nSelected object is not a Profile View.");
            peo.AddAllowedClass(typeof(ProfileView), exactMatch: true);
            peo.AllowNone = false;

            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
            {
                return;
            }


            //PromptSelectionOptions pso = new PromptSelectionOptions();
            //pso.MessageForAdding = "\nSelect Profile Views: ";
            //pso.AllowDuplicates = false;

            //PromptSelectionResult psr = ed.GetSelection(pso, new SelectionFilter(filter));
            //if (psr.Status != PromptStatus.OK)
            //    return;

            // Example: change BandSetStyle for all selected ProfileViews
            using (DocumentLock documentLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    ProfileView profileView = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as ProfileView;

                    profileView.ElevationRangeMode = ElevationRangeType.UserSpecified;
                    profileView.ElevationMax = profileView.ElevationMax - 1;

                    tr.Commit();
                }
            }
        }


        public static void ChangeProfileViewBandSets()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;

            // Filter for ProfileView
            TypedValue[] filter = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "AECC_PROFILE_VIEW")
            };

            PromptEntityOptions peo = new PromptEntityOptions("\nSelect reference Profile View:");
            peo.SetRejectMessage("\nSelected object is not a Profile View.");
            peo.AddAllowedClass(typeof(ProfileView), exactMatch: true);
            peo.AllowNone = false;

            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
            {
                return;
            }


            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\nSelect Profile Views: ";
            pso.AllowDuplicates = false;





            PromptSelectionResult psr = ed.GetSelection(pso, new SelectionFilter(filter));
            if (psr.Status != PromptStatus.OK)
                return;

            // Example: change BandSetStyle for all selected ProfileViews
            using (DocumentLock documentLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    ProfileView pvRef = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as ProfileView;
                    var bandItems = pvRef.Bands.GetBottomBandItems();

                    // Get a target BandSetStyle
                    ObjectId bandSetStyleId = civDoc.Styles.ProfileViewBandSetStyles["SLOPE ASSESSMENT SET"];
                    ProfileViewBandSetStyle setStyleCollection = tr.GetObject(bandSetStyleId, OpenMode.ForRead) as ProfileViewBandSetStyle;
                    var bands = setStyleCollection.GetBottomBandSetItems();

                    foreach (SelectedObject selObj in psr.Value)
                    {
                        if (selObj != null)
                        {
                            ProfileView pv = tr.GetObject(selObj.ObjectId, OpenMode.ForWrite) as ProfileView;
                            if (pv != null)
                            {
                                pv.Bands.SetBottomBandItems(bandItems);
                            }
                        }
                    }

                    tr.Commit();
                }
            }

        }
    }
}
