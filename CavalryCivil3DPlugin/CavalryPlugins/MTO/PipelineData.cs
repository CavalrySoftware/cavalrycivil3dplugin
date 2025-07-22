using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Gis.Map.ObjectData;
using Autodesk.Gis.Map.Project;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;
using CivilEntity = Autodesk.Civil.DatabaseServices.Entity;
using AcadEntity = Autodesk.Civil.DatabaseServices.Entity;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.AutoCAD.EditorInput;
using CavalryCivil3DPlugin._Library._ExcelLibrary;


namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class PipelineData
    {

        private string _LayerName;
        public string LayerName => _LayerName;

        private List<string> _PipeNetworkNames;
        public List<string> PipeNetworkNames => _PipeNetworkNames;

        private string _PropertySetName;
        public string PropertySetName => _PropertySetName;

        private Document _AutocadDocument;
        private CivilDocument _CivilDocument;

        private List<PipeRunData> _PipeRunDataCollection = new List<PipeRunData>();
        public List<PipeRunData> PipeRunDataCollection => _PipeRunDataCollection;

        private Dictionary<string, Dictionary<string, string>> _PayItemData;

        public Dictionary<string, Dictionary<string, string>> PayItemData => _PayItemData;


        private string _PayItemFilePath;
        public string PatItemFilePath
        {
            get { return _PayItemFilePath; }
            set { _PayItemFilePath = value; }
        }


        public readonly string PropBranchKey = "BRANCH";
        public readonly string PropCorridorKey = "FA_CORRIDOR";
        public readonly string PropNetworkKey = "NETWORK_TYPE";
        public readonly string PropSizeKey = "NOMINAL_SIZE";
        public readonly string PropPipeRunKey = "PIPERUN_ID";
        public readonly string PropPartListKey = "PART_LIST_DESCRIPTION";
        public readonly string PropDescriptionKey = "Item Description-USC";
        public readonly string PropSpecKey = "Spec";
        public readonly string PropClassificationKey = "Classification";
        public readonly string PropTypeKey = "Type";
        public readonly string PropFactorKey = "Factor";
        public readonly string PropUnitKey = "UNIT_E";


        public PipelineData(string _layerName, Document _autocadDocument, CivilDocument _civilDocument, List<string> _pipeNetworkNames, string _propertySetName, string _payItemPath)
        {
            _LayerName = _layerName;
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;
            _PipeNetworkNames = _pipeNetworkNames;
            _PropertySetName = _propertySetName;
            _PayItemFilePath = _payItemPath;

            _PayItemData = _CSV.GetDictionaryByFirstColumn(_PayItemFilePath);
            GetPropertyData();
        }


        public void GetPropertyData()
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                var pressureNetworkCollection = _CivilDocument.GetPressurePipeNetworkIds();
                int index = 1;
                foreach (ObjectId objectId in pressureNetworkCollection)
                {
                    PressurePipeNetwork pressurePipeNetwork = tr.GetObject(objectId, OpenMode.ForRead) as PressurePipeNetwork;

                    if (_PipeNetworkNames.Contains(pressurePipeNetwork.Name))
                    {
                        PressurePipeRunCollection pressurePipeRunCollection = pressurePipeNetwork.PipeRuns;
                        
                        foreach (PressurePipeRun pipeRun in pressurePipeRunCollection)
                        {
                            var partIds = pipeRun.GetPartIds();
                            PipeRunData newPipeRunData = new PipeRunData();

                            // Exclude
                            if (pipeRun.Name.Contains("Connector")) continue;

                            // Getting properties from 1 sample pipe part from 1 pipe run
                            foreach (ObjectId partId in partIds)
                            {
                                PressurePart pressurePart = tr.GetObject(partId, OpenMode.ForRead) as PressurePart;
                                
                                if (pressurePart.PartType.ToString() == "PressurePipe")
                                {
                                    
                                    Dictionary<string, string> properties = C3DPropertySet.GetObjectPropertySet(partId, _PropertySetName, _AutocadDocument);
                                    newPipeRunData.Branch = properties[PropBranchKey];
                                    newPipeRunData.Network = properties[PropNetworkKey];
                                    newPipeRunData.CorridorId = properties[PropCorridorKey];
                                    newPipeRunData.Size = properties[PropSizeKey];
                                    newPipeRunData.PartListDescription = properties[PropPartListKey];
                                    newPipeRunData.PipeRunId = properties[PropPipeRunKey];
                                    newPipeRunData.SAPCode = C3DPropertySet.GetPayItemCode(partId, _AutocadDocument);
                                    newPipeRunData.Description = PayItemData[newPipeRunData.SAPCode][PropDescriptionKey];
                                    newPipeRunData.Classification = PayItemData[newPipeRunData.SAPCode][PropClassificationKey];
                                    newPipeRunData.Spec = PayItemData[newPipeRunData.SAPCode][PropSpecKey];
                                    newPipeRunData.Type = PayItemData[newPipeRunData.SAPCode][PropTypeKey];
                                    newPipeRunData.Factor = PayItemData[newPipeRunData.SAPCode][PropFactorKey];
                                    newPipeRunData.Unit = PayItemData[newPipeRunData.SAPCode][PropUnitKey];

                                    break;
                                }
                            }

                            // Getting all 3d Lengths
                            double totalLength = 0;
                            foreach (ObjectId partId in partIds)
                            {
                                PressurePart pressurePart = tr.GetObject(partId, OpenMode.ForRead) as PressurePart;

                                if (pressurePart.PartType.ToString() == "PressurePipe")
                                {
                                    PressurePipe pressurePipe = pressurePart as PressurePipe;
                                    double length = pressurePipe.Length3DCenterToCenter;
                                    totalLength += length;
                                }
                            }

                            newPipeRunData.Length = totalLength;
                            newPipeRunData.Index = index;
                            index++;
                            _PipeRunDataCollection.Add(newPipeRunData);
                        }
                     }
                }
            }
        }

       
    }
}
