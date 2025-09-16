using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;
using Autodesk.Aec.PropertyData;
using System.Collections;

namespace CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet
{
    public class C3DPropertySet
    {

        public static Dictionary<string, string> GetObjectPropertySet(ObjectId _objectId, string _propertySetName, Document _autocadDocument)
        {
            Dictionary<string, string> fullProp = new Dictionary<string, string>();

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                DBObject dbObject = tr.GetObject(_objectId, OpenMode.ForRead);
                ObjectIdCollection propertySetDefinitions = PropertyDataServices.GetPropertySetDefinitionsUsed(dbObject);
                ObjectIdCollection properySetIds = PropertyDataServices.GetPropertySets(dbObject);

                foreach (ObjectId objectId in properySetIds)
                {
                    PropertySet propertySet = (PropertySet)tr.GetObject(objectId, OpenMode.ForRead);
                    ObjectId propertSetDefinitionId = propertySet.PropertySetDefinition;
                    PropertySetDefinition propertSetDefinition = (PropertySetDefinition)tr.GetObject(propertSetDefinitionId, OpenMode.ForRead);

                    if (propertSetDefinition.Name == _propertySetName)
                    {
                        PropertyDefinitionCollection propertyDefinitionCollection = propertSetDefinition.Definitions;
                        int index = 0;

                        foreach (PropertyDefinition propertyDefinition in propertyDefinitionCollection)
                        {
                            fullProp[propertyDefinition.Name] = propertySet.GetAt(index).ToString();
                            index++;
                        }
                    }
                }
            }

            return fullProp;
        }


        public static string GetPayItemCode(ObjectId _entityId, Document _autocadDocument)
        {
            string payitem = "";

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Entity entity = tr.GetObject(_entityId, OpenMode.ForRead) as Entity;

                ResultBuffer xData = entity.XData;
                if (xData != null)
                {
                    foreach (TypedValue tv in xData)
                    {
                        if (tv.TypeCode.ToString() == "1000")
                        {
                            payitem += tv.Value;
                        }
                    }
                }
            }

            return payitem;
        }

        public static List<string> GetPayItemCodes(ObjectId _entityId, Document _autocadDocument)
        {
            List<string> payItemCodes = new List<string>();

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Entity entity = tr.GetObject(_entityId, OpenMode.ForRead) as Entity;

                ResultBuffer xData = entity.XData;
                if (xData != null)
                {
                    foreach (TypedValue tv in xData)
                    {
                        if (tv.TypeCode.ToString() == "1000")
                        {
                            payItemCodes.Add(tv.Value.ToString());
                        }
                    }
                }
            }

            return payItemCodes; 
        }


        public static Dictionary<string, List<string>> GetAllPropertySets(Document _autocadDocument)
        {
            Dictionary<string, List<string>> propertySets = new Dictionary<string, List<string>>();
            Database db = _autocadDocument.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    DBDictionary nod = tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary;

                    // Search through all possible dictionary locations
                    SearchForPropertySets(nod, tr, propertySets);

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error retrieving property sets: {ex.Message}");
                    tr.Abort();
                }
            }

            return propertySets;
        }


        private static void SearchForPropertySets(DBDictionary dictionary, Transaction tr, Dictionary<string, List<string>> propertySets)
        {
            foreach (DBDictionaryEntry entry in dictionary)
            {
                try
                {
                    DBObject obj = tr.GetObject(entry.Value, OpenMode.ForRead);

                    // Check if it's a PropertySetDefinition
                    if (obj is PropertySetDefinition psd)
                    {
                        if (!propertySets.Keys.Contains(psd.Name))
                        {
                            
                            List<string> fields = new List<string>();
                            foreach (PropertyDefinition definition in psd.Definitions)
                            {
                                fields.Add(definition.Name);
                            }
                            propertySets[psd.Name] = fields;
                        }
                    }

                    // If it's another dictionary, search recursively
                    else if (obj is DBDictionary subDict)
                    {
                        // Only search certain known dictionary types to avoid infinite recursion
                        string entryKey = entry.Key.ToUpper();
                        if (entryKey.Contains("PROPERTY") || entryKey.Contains("AEC") || entryKey.Contains("ARCH"))
                        {
                            SearchForPropertySets(subDict, tr, propertySets);
                        }
                    }
                }
                catch
                {
                    // Skip entries that can't be opened or aren't the right type
                    continue;
                }
            }
        }
    }
}
