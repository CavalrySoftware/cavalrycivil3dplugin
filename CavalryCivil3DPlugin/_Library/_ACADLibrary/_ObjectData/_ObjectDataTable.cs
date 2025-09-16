using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.Windows.Data;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using Autodesk.Gis.Map.Project;
using Autodesk.Gis.Map.Utilities;
using CavalryCivil3DPlugin.Consoles;

using ACADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using ACADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using GISOpenmode = Autodesk.Gis.Map.Constants.OpenMode;
using GISTable = Autodesk.Gis.Map.ObjectData.Table;


namespace CavalryCivil3DPlugin.ACADLibrary._ObjectData
{
    public class _ObjectDataTable
    {

        private static ProjectModel project = HostMapApplicationServices.Application.ActiveProject;
        private static Tables objectDataTables = project.ODTables;

        public static Dictionary<string, List<string>> GetAllFields(Document _autocadDocument)
        {

            Dictionary<string, List<string>> allFields = new Dictionary<string, List<string>>();

            try
            {

                List<string> tableNames = new List<string>(objectDataTables.GetTableNames().Cast<string>());
                
                foreach (string tableName in tableNames)
                {
                    GISTable table = objectDataTables[tableName];
                    int totalFields = table.FieldDefinitions.Count;

                    List<string> fields = new List<string>();

                    for (int i = 0; i < totalFields; i++)
                    {
                        fields.Add(table.FieldDefinitions[i].Name);
                    }

                    allFields[tableName] = fields;
                }
            }
            catch (Exception ex)
            {
                ConsoleBasic console = new ConsoleBasic(ex.ToString());
            }

            return allFields;
        }


        public static Dictionary<string, Dictionary<string, string>> GetObjectDataValuesfromPolyline(Document _autocadDocument, ACADObjectId _polylineId)
        {

            Dictionary<string, Dictionary<string, string>> allTables = new Dictionary<string, Dictionary<string, string>>();

            using(Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {

                Records plineRecords = objectDataTables.GetObjectRecords(0, _polylineId, GISOpenmode.OpenForRead, true);
                List<Record> records = plineRecords.Cast<Record>().ToList();

                List<string> tableNames = records.Select(x => x.TableName).ToList();
                
                foreach(string name in tableNames)
                {
                    GISTable table = objectDataTables[name];

                    List<MapValue> values = new List<MapValue>();
                    List<string> valueString = new List<string>();

                    foreach (Record record in records)
                    {
                        if (record.TableName == name)
                        {
                            Dictionary<string, string> fieldValue = new Dictionary<string, string>();
                            int totalFields = record.Count;

                            for (int index = 0; index < totalFields; index++)
                            {
                                fieldValue[table.FieldDefinitions[index].Name] = record[index].StrValue;
                            }

                            allTables[name] = fieldValue;
                        }
                    }
                }
            }

            return allTables;
        }


        public static List<string> GetAllTableNamesFromObject(Document _autocadDocument, ObjectId _objectId)
        {
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {

                Records plineRecords = objectDataTables.GetObjectRecords(0, _objectId, GISOpenmode.OpenForRead, true);
                List<Record> records = plineRecords.Cast<Record>().ToList();

                List<string> tableNames = records.Select(x => x.TableName).ToList();

                return tableNames;
            }
        }



        public static Dictionary<string, string> GetPropFromObject(Document _autocadDocument, ObjectId _objectId, string _tableName)
        {
            Dictionary<string, string> fieldValue = new Dictionary<string, string>();
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Records objectRecords = objectDataTables.GetObjectRecords(0, _objectId, GISOpenmode.OpenForRead, true);
                
                if (objectRecords.Count > 0)
                {
                    List<Record> records = objectRecords.Cast<Record>().ToList();

                    List<string> tableNames = records.Select(x => x.TableName).ToList();

                    if (tableNames.Contains(_tableName))
                    {
                        GISTable table = objectDataTables[_tableName];
                        List<MapValue> values = new List<MapValue>();

                        foreach (Record record in records)
                        {
                            if (record.TableName == _tableName)
                            {
                                int totalFields = record.Count;

                                for (int index = 0; index < totalFields; index++)
                                {
                                    fieldValue[table.FieldDefinitions[index].Name] = record[index].StrValue;
                                }
                            }
                        }
                    }
                }
                
                return fieldValue;
            }
        }
    }
}
