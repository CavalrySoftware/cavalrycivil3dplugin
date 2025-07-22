using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class CorridorData
    {

        private Document _AutocadDocument;
        private CivilDocument _CivilDocument;

        private List<ObjectId> _ReferenceAlignmentIds;
        public List<ObjectId> ReferenceAlignmentIds => _ReferenceAlignmentIds;

        private List<AlignmentData> _AlignmentDataCollection = new List<AlignmentData>();
        public List<AlignmentData> AlignmentDataCollection => _AlignmentDataCollection;

        private string _LayerName;
        public string LayerName
        {
            get { return _LayerName; }
            set { _LayerName = value; }
        }

        private string _PropertySetName;
        public string PropertySetName
        {
            get { return _PropertySetName; }
            set { _PropertySetName = value; }
        }

        public readonly string PropCorridorIdKey = "CORRIDOR";
        public readonly string PropCWPKey = "CWP";
        public readonly string PropSAPKey = "SAP CODE";
        public readonly string PropDescriptionKey = "DESCRIPTION";
        public readonly string PropTrenchKey = "TRENCH DETAILS";



        public CorridorData(string _layerName, Document _autocadDocument, CivilDocument _civilDocument, string _propertySetName)
        {
            _LayerName = _layerName;
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;
            _PropertySetName = _propertySetName;

            _ReferenceAlignmentIds = GetReferenceAlignmentIds();
            GetPropertyData();
        }


        public List<ObjectId> GetReferenceAlignmentIds()
        {
            List<string> layers = new List<string>() { _LayerName };
            List<ObjectId> referenceAlignmentIds = C3DObjectSelection.GetAlignmentIdsByLayer(_AutocadDocument, _CivilDocument, layers);
            return referenceAlignmentIds;
        }


        public void GetPropertyData()
        {
            using(Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                int index = 1;
                foreach(ObjectId id in _ReferenceAlignmentIds)
                {
                    AlignmentData alignmentData = new AlignmentData();
                    Dictionary<string, string> properties = C3DPropertySet.GetObjectPropertySet(id, _PropertySetName, _AutocadDocument);
                    Alignment alignment = tr.GetObject(id, OpenMode.ForRead) as Alignment;

                    alignmentData.Index = index;
                    alignmentData.TrenchDetails = properties[PropTrenchKey];
                    alignmentData.SAPCode = properties[PropSAPKey];
                    alignmentData.CorridorId = properties[PropCorridorIdKey];
                    alignmentData.CWP = properties[PropCWPKey];
                    alignmentData.Description = properties[PropDescriptionKey];
                    alignmentData.Length = alignment.Length;
                    alignmentData.WBSLevel9 = "";

                    _AlignmentDataCollection.Add(alignmentData);
                    index++;
                }
            }
        }
    }
}



    
