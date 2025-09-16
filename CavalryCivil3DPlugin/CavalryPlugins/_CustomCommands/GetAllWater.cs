using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CavalryCivil3DPlugin.CavalryPlugins._CustomCommands
{
    public class GetAllWater
    {

        [CommandMethod("GetWater")]
        public static void GetAllWaterGIS()
        {
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Editor editor = doc.Editor;

                List<string> layers = new List<string>()
                {
                    "MAP_PIPELINES"
                };

                List<ObjectId> objectIds = ACADObjectSelection.GetAllPolylineId(doc);
                List<ObjectId> objectIds_ = ACADObjectSelection.GetAllPolylineIdByLayers(doc, layers);
                List<ObjectId> selectedIds = new List<ObjectId>();
                string odTable = "PIPELINESLine";
                string propField = "CLASSIFICA";

                using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in objectIds)
                    {
                        Dictionary<string, string> properties = _ObjectDataTable.GetPropFromObject(doc, id, odTable);

                        if (properties.Keys.Contains(propField))
                        {
                            if (properties[propField] == "Water")
                            {
                                selectedIds.Add(id);
                            }
                        }
                    }
                }

                SelectionSet ss = SelectionSet.FromObjectIds(selectedIds.Cast<ObjectId>().ToArray());

                // Set the active selection
                editor.SetImpliedSelection(ss);
                editor.UpdateScreen();
            }

            catch (System.Exception ex)
            {
                _Console.ShowConsole(ex.ToString());    
            }
            
        }
    }
}
