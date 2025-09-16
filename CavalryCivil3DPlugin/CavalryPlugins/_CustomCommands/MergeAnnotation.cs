using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins._CustomCommands
{
    public class MergeAnnotation
    {

        [CommandMethod("MergeLeaderAnnotation")]
        public void MergeAnnotationLeader()
        {
            try
            {
                Document autocadDocument = Application.DocumentManager.MdiActiveDocument;
    
                ObjectId mainMLeaderId = ACADObjectSelection.PickMLeader(autocadDocument, _selectMessage: "Select Main MLeader");

                if (mainMLeaderId != ObjectId.Null)
                {
                    while (true)
                    {
                        ObjectId referenceMLeaderId = ACADObjectSelection.PickMLeader(autocadDocument, _selectMessage: "Select Reference MLeader");
                        if (referenceMLeaderId != ObjectId.Null)
                        {
                            using (DocumentLock documentLock = autocadDocument.LockDocument())
                            {
                                using (Transaction tr = autocadDocument.Database.TransactionManager.StartTransaction())
                                {
                                    MLeader mainMLeader = tr.GetObject(mainMLeaderId, OpenMode.ForWrite) as MLeader;
                                    MLeader referenceMLeader = tr.GetObject(referenceMLeaderId, OpenMode.ForRead) as MLeader;

                                    string newContent = mainMLeader.MText.Contents + "\n" + referenceMLeader.MText.Contents;
                                    double textHeight = mainMLeader.MText.TextHeight;

                                    mainMLeader.MText = new MText() { Contents = newContent, TextStyleId = mainMLeader.TextStyleId, TextHeight = textHeight};
                                    autocadDocument.Editor.Regen();
                                    tr.Commit();
                                }
                            }
                        }

                        else
                        {
                            break;
                        }
                    }
                }
            }

            catch (System.Exception ex) { _Console.ShowConsole(ex.ToString()); }
            
         }
    }
}
