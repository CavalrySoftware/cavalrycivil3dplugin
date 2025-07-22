using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using Autodesk.Gis.Map.Project;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin._Library._ExcelLibrary;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class FeatureData
    {

		private Document _AutocadDocument;
		private CivilDocument _CivilDocument;
		private ProjectModel _ProjectModel;


		private List<string> _LayerNames;

		public List<string> LayerNames
		{
			get { return _LayerNames; }
			set { _LayerNames = value; }
		}

		private string _PropertySetName;
		public string PropertySetName
		{
			get { return _PropertySetName; }
			set { _PropertySetName = value; }
		}

		private string _PayItemFilePath;
		public string PayItemFilePath
		{
			get { return _PayItemFilePath; }
			set { _PayItemFilePath = value; }
		}

		private Dictionary<string, Dictionary<string, string>> _PayItemData;
		public Dictionary<string, Dictionary<string, string>> PayItemData => _PayItemData;

		private List<ComponentsData> _ComponentDataCollection = new List<ComponentsData>();

		public List<ComponentsData> ComponentDataCollection => _ComponentDataCollection;


		public readonly string PropTagNoKey = "DESCRIPTION";
		public readonly string PropCorridorKey = "RefAlignment";
		public readonly string PropFeatureKey = "PL_DESCRIPTION";
		public readonly string PropDescriptionKey = "Item Description-USC";
		public readonly string PropSizeKey = "Size";
		public readonly string PropSpecKey = "Spec";
		public readonly string PropClassificationKey = "Classification";
		public readonly string PropUOMKey = "UNIT_E";
		public readonly string PropTotalKey = "Factor";
		public readonly string PropTypeKey = "Type";


        public FeatureData(List<string> _layerNames, Document _autocadDocument, CivilDocument _civilDocument, ProjectModel _projectModel, string _propertySetName, string _payItemPath)
		{
			_LayerNames = _layerNames;
			_AutocadDocument = _autocadDocument;
			_ProjectModel = _projectModel;
			_PropertySetName = _propertySetName;
			_CivilDocument = _civilDocument;
			_PayItemFilePath= _payItemPath;

			_PayItemData = _CSV.GetDictionaryByFirstColumn(_PayItemFilePath);

			GetPropertyData();
		}


		public void GetPropertyData()
		{
			using(Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
			{
				List<ObjectId> componentsId = C3DObjectSelection.GetFittingsAndAppurtenancesByLayer(_AutocadDocument, _CivilDocument, _LayerNames);

				int index = 1;
				foreach (ObjectId componentId in componentsId)
				{
					Dictionary<string, string> properties = C3DPropertySet.GetObjectPropertySet(componentId, _PropertySetName, _AutocadDocument);
					string corridorId = properties[PropCorridorKey];
					string uniqueTag = properties[PropTagNoKey];
					string feature = properties[PropFeatureKey];

					List<string> payItemCodes = C3DPropertySet.GetPayItemCodes(componentId, _AutocadDocument);
					
					foreach(string payItemCode in payItemCodes)
					{
						ComponentsData component = new ComponentsData();
						component.Index = index;
						component.SAPCode = payItemCode;
						component.Feature = feature;
						component.CorridorId = corridorId;
						component.UniqueTagNo = uniqueTag;
						component.Description = _PayItemData[payItemCode][PropDescriptionKey];
						component.Size = _PayItemData[payItemCode][PropSizeKey];
						component.Spec = _PayItemData[payItemCode][PropSpecKey];
						component.Classification = _PayItemData[payItemCode][PropClassificationKey];
						component.UOM = _PayItemData[payItemCode][PropUOMKey];
						component.Total = _PayItemData[payItemCode][PropTotalKey];
						component.Type = _PayItemData[payItemCode][PropTypeKey];

						_ComponentDataCollection.Add(component);
						index++;
					}
				}
				//ObjectId sampleComponent = componentsId.FirstOrDefault();
				//var prop = C3DPropertySet.GetObjectPropertySet(sampleComponent, _PropertySetName, _AutocadDocument);
				//List<string> payItemCodes = C3DPropertySet.GetPayItemCodes(sampleComponent, _AutocadDocument);

				//_Console.ShowConsole(prop);
			}
		}
	}
}
