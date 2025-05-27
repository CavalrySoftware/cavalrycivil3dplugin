using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using CavalryCivil3DPlugin.Consoles;

using AutocadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Civil3DEntity = Autodesk.Civil.DatabaseServices.Entity;

namespace CavalryCivil3DPlugin.ACADLibrary.Selection
{
    public class ACADObjectSelection
    {

        private static TypedValue _polyLineType = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
        private static TypedValue _blockReferenceType = new TypedValue((int)DxfCode.BlockName, "*");
        private static TypedValue _xrefType = new TypedValue((int)DxfCode.Start, "*");

        private static SelectionFilter _polylineFilter = new SelectionFilter(new TypedValue[] { _polyLineType });

        public static List<ObjectId> GetAllPolylineId(Document _document)
        {
            List<ObjectId> allPolylineIds = new List<ObjectId>();

            
            PromptSelectionResult allPolylineResult = _document.Editor.SelectAll(_polylineFilter);

            if (allPolylineResult.Status == PromptStatus.OK)
            {
                SelectionSet allPolyLineSelectionSet = allPolylineResult.Value;
                allPolylineIds = allPolyLineSelectionSet.GetObjectIds().ToList();
            }

            return allPolylineIds;  
        }


        public static List<ObjectId> GetAllPolylineId(Document _document, SelectionFilter _selectionFilter)
        {
            List<ObjectId> allPolylineIds = new List<ObjectId>();

            PromptSelectionResult allPolylineResult = _document.Editor.SelectAll(_selectionFilter);

            if (allPolylineResult.Status == PromptStatus.OK)
            {
                SelectionSet allPolyLineSelectionSet = allPolylineResult.Value;
                allPolylineIds = allPolyLineSelectionSet.GetObjectIds().ToList();
            }

            return allPolylineIds;
        }


        public static List<ObjectId> GetAllPolylineIdByLayers(Document _document, List<string> LayerNames)
        {
            List<ObjectId> allPolylineIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>() {_polyLineType, new TypedValue((int)DxfCode.Operator, "<OR")};


            foreach (string layerName in LayerNames)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }

            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            SelectionFilter polylineFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult allPolylineResult = _document.Editor.SelectAll(polylineFilter);


            if (allPolylineResult.Status == PromptStatus.OK)
            {
                SelectionSet allPolyLineSelectionSet = allPolylineResult.Value;
                allPolylineIds = allPolyLineSelectionSet.GetObjectIds().ToList();
            }

            return allPolylineIds;
        }


        public static List<AutocadEntity> GetAllPolylineEntity(Document _autocadDocument)
        {
            List<ObjectId> allPolylineIds = GetAllPolylineId(_autocadDocument);
            List<AutocadEntity> allPolyLineEntity = new List<AutocadEntity>();

            using(Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId polylineId in allPolylineIds)
                {
                    AutocadEntity polylineEntity = tr.GetObject(polylineId, OpenMode.ForRead) as AutocadEntity;
                    allPolyLineEntity.Add(polylineEntity);
                }
            }
            return allPolyLineEntity;   
        }


        public static List<(ObjectId, string)> GetAllPolylineIdByObjectData(Document _autocadDocument, string _tableName, string _filterKey)
        {
            List<(ObjectId, string)> allPolylineIds = new List<(ObjectId, string)>();

            List<TypedValue> typedValues = new List<TypedValue>() 
            { 
                _polyLineType, 
                new TypedValue((int)DxfCode.Operator, "<NOT") ,
                _blockReferenceType,
                //_xrefType,
                new TypedValue((int)DxfCode.Operator, "NOT>")
            };

            SelectionFilter polylineFilter = new SelectionFilter(typedValues.ToArray());

            List<ObjectId> polylineIds =GetAllPolylineId(_autocadDocument, polylineFilter);

            
            foreach (ObjectId polylineId in polylineIds)
            {
                try
                {
                    var allTables = _ObjectDataTable.GetObjectDataValuesfromPolyline(_autocadDocument, polylineId);

                    string sam = allTables.Keys.FirstOrDefault() as string;
                    

                    Dictionary<string, string> table = allTables[_tableName];

                    string _filterValue = table[_filterKey];

                     if (!string.IsNullOrEmpty(_filterValue))
                    {
                        allPolylineIds.Add((polylineId, _filterValue));
                    }
                }
                
                catch { }
            }

            return allPolylineIds;
        }


        public static List<ObjectId> PickPolylines(Document _autocadDocument)
        {
            List<ObjectId> polylineIds = new List<ObjectId>();

            PromptSelectionOptions promptOptions = new PromptSelectionOptions();
            promptOptions.MessageForAdding = "\nSelect Polylines";

            PromptSelectionResult selectionResult =  _autocadDocument.Editor.GetSelection(promptOptions, _polylineFilter);

            if (selectionResult.Status == PromptStatus.OK)
            {
                var ids = selectionResult.Value.GetObjectIds().ToList();
                polylineIds.AddRange(ids);
            }

            return polylineIds;
        }


        public static List<string> GetAllLayerNames(Document _autocadDocument)
        {
            List<string> layerNames = new List<string>();

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = tr.GetObject(_autocadDocument.Database.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId layerId in layerTable)
                {
                    LayerTableRecord layer = tr.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord;
                    layerNames.Add(layer.Name);
                }
            }

            layerNames.Sort();
            return layerNames;
        }

        private static ObjectId PickElement(Document _autocadDocument, Type _elementType, string elementName)
        {
            string selectMessage = ($"\nSelect {elementName}");
            string rejectMessage = ($"\nSelect only {elementName}");

            PromptSelectionOptions promptOptions = new PromptSelectionOptions();

            PromptEntityOptions promptEntityOptions = new PromptEntityOptions($"\n{selectMessage}");
            promptEntityOptions.SetRejectMessage($"\n{rejectMessage}");
            promptEntityOptions.AddAllowedClass(_elementType, true);

            PromptEntityResult selectionResult = _autocadDocument.Editor.GetEntity(promptEntityOptions);

            if (selectionResult.Status == PromptStatus.OK)
            {
                return selectionResult.ObjectId;
            }

            return ObjectId.Null;
        }


        public static ObjectId PickLine(Document _autocadDocument)
        {
            return PickElement(_autocadDocument, typeof(Line), "Line");
        }


        public static ObjectId PickObject(Document _autocadDocument, string _selectMessage)
        {
            PromptSelectionOptions promptOptions = new PromptSelectionOptions();

            PromptEntityOptions promptEntityOptions = new PromptEntityOptions($"\n{_selectMessage}");
    
            PromptEntityResult selectionResult = _autocadDocument.Editor.GetEntity(promptEntityOptions);

            if (selectionResult.Status == PromptStatus.OK)
            {
                return selectionResult.ObjectId;
            }

            return ObjectId.Null;
        }

    }
}
