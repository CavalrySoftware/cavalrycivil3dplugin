using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.ACADLibrary.AutoCADTable
{
    public class CADTable
    {

        public static void CreateTable(Document _autocadDocument, List<List<string>> _data, Point3d _insertionPoint, string _tableStyle = null, List<string> _headers = null, string _title = null)
        {

            try
            {
                int nrows = _data.Count;
                int ncolumns = _data[0].Count;

                using (DocumentLock docLock = _autocadDocument.LockDocument())
                {
                    using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
                    {
                        BlockTable blockTable = (BlockTable)tr.GetObject(_autocadDocument.Database.BlockTableId, OpenMode.ForWrite);
                        BlockTableRecord blockTableRecord = (BlockTableRecord)tr.GetObject(_autocadDocument.Database.CurrentSpaceId, OpenMode.ForWrite);
                        DBDictionary styleDict = tr.GetObject(_autocadDocument.Database.TableStyleDictionaryId, OpenMode.ForRead) as DBDictionary;


                        Table table = new Table();

                        if (styleDict != null)
                        {
                            if (styleDict.Contains(_tableStyle))
                            {
                                table.TableStyle = styleDict.GetAt(_tableStyle);
                            }
                        }

                        int n = 0;
                        bool withTitle = false;
                        bool withHeaders = false;


                        // Set Title
                        if (_title != null)
                        {
                            n++;
                            withTitle = true;
                        }

                        // Set Headers
                        if (_headers != null && _headers.Count == _data[0].Count)
                        {
                            n++;
                            withHeaders = true;
                        }

                        table.SetSize(nrows + n, ncolumns);
                        table.SetRowHeight(10);
                        table.SetColumnWidth(30);
                        table.Position = _insertionPoint;

                        if (withTitle)
                        {
                            table.Cells[0, 0].TextString = _title;
                        }

                        if (withHeaders)
                        {
                            for(int column=0; column < ncolumns; column++)
                            {
                                table.Cells[n==2?1:0, column].TextString = _headers[column];
                            }
                        }


                        //Set Data
                        for (int row = 0; row < nrows; row++)
                        {
                            for (int column = 0; column < ncolumns; column++)
                            {
                                table.Cells[row + n, column].TextString = _data[row][column];
                            }
                        }

                        blockTableRecord.AppendEntity(table);
                        tr.AddNewlyCreatedDBObject(table, true);
                        _autocadDocument.Editor.Regen();

                        tr.Commit();
                    }
                }
            }

            catch (System.Exception ex) { ConsoleBasic console = new ConsoleBasic(ex.ToString() ); }
            
        }


        public static List<string> GetAllTableStyleNames(Document _autocadDocument)
        {
            Database db = _autocadDocument.Database;

            List<string> styleNames = new List<string>();
            
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBDictionary tableStyleDict = (DBDictionary)tr.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead);

                foreach (DBDictionaryEntry entry in tableStyleDict)
                {
                    styleNames.Add(entry.Key); 
                }
            }
            return styleNames;
        }
    }
}
