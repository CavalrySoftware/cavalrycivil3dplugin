using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;

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
    }
}
