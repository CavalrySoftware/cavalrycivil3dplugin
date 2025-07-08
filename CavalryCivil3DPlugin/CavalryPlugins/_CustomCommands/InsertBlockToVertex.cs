using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin._ACADLibrary.Elements;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins._CustomCommands
{
    public class InsertBlockToVertex
    {

        [CommandMethod("BlockToVertex")]
        public static void BlockToVertex()
        {
            try
            {
                _BlockToVertex();
            }
            catch (System.Exception ex) { _Console.ShowConsole(ex.ToString()); }
        }

        
        private static void _BlockToVertex()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = doc.Editor;

            TypedValue[] filter = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "LINE,LWPOLYLINE")
            };

            SelectionFilter selectionFilter = new SelectionFilter(filter);

            PromptSelectionOptions promptSelectionOptions = new PromptSelectionOptions();
            promptSelectionOptions.MessageForAdding = "\nSelect Lines or Polylines: ";

            PromptSelectionResult promptSelectionResult = editor.GetSelection(promptSelectionOptions, selectionFilter);

            if (promptSelectionResult.Status != PromptStatus.OK) return;

            SelectionSet selectionSet = promptSelectionResult.Value;


            using (DocumentLock documentLock = doc.LockDocument())
            {
                using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
                {

                    List<Point3d> points = new List<Point3d>();

                    foreach (SelectedObject obj in selectionSet)
                    {
                        if (obj != null)
                        {
                            Entity entity = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;

                            if (entity != null)
                            {
                                if (entity is Polyline)
                                {

                                    points.AddRange(_Polyline.GetAllVertices(doc, obj.ObjectId));
                                }

                                else if (entity is Line)
                                {
                                    var endPoints = _Line.GetPoints(doc, obj.ObjectId);
                                    points.Add(endPoints.Item1);
                                    points.Add(endPoints.Item2);
                                }
                            }
                        }
                    }


                    BlockTable blockTable = tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;

                    PromptStringOptions promptStringOptions = new PromptStringOptions("\nBlock Name: ");
                    promptStringOptions.AllowSpaces = true;

                    while (true)
                    {
                        PromptResult stringResult = editor.GetString(promptStringOptions);

                        if (stringResult.Status != PromptStatus.OK) break;

                        string blockName = stringResult.StringResult;

                        // Add current annotation scale
                        ObjectContextManager ocm = doc.Database.ObjectContextManager;
                        ObjectContextCollection occ = ocm.GetContextCollection("ACDB_ANNOTATIONSCALES");

                        AnnotationScale currentScale = doc.Database.Cannoscale;

                        if (blockTable.Has(blockName))
                        {
                            BlockTableRecord blockTableRecord = tr.GetObject(blockTable[blockName], OpenMode.ForRead) as BlockTableRecord;
                            BlockTableRecord ms = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                            foreach (Point3d insertionPoint in points)
                            {
                                BlockReference newBlock = new BlockReference(insertionPoint, blockTableRecord.ObjectId);
                                newBlock.AddContext(currentScale);
                                ms.AppendEntity(newBlock);
                                tr.AddNewlyCreatedDBObject(newBlock, true);
                            }

                            tr.Commit();
                            break;
                        }

                        editor.WriteMessage("\nBlock not found. Please a enter a valid Block name.");
                    }
                }
            }
        }
    }
}
