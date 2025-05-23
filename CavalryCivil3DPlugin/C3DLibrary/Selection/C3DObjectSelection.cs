using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace CavalryCivil3DPlugin.C3DLibrary.Selection
{
    public class C3DObjectSelection
    {

        private static TypedValue _FeatureLinesType = new TypedValue((int)DxfCode.Start, "AECC_FEATURE_LINE");
        private static SelectionFilter _FeatureLinesFilter = new SelectionFilter(new TypedValue[] { _FeatureLinesType });

        public static List<CadObjectId> GetAllFeatureLineIds(CivilDocument _civilDocument, Document _acadDocument)
        {
            List<CadObjectId> featureLinesIds = new List<CadObjectId>();
            
            using (Transaction tr = _acadDocument.Database.TransactionManager.StartTransaction())
            {
                var siteIds = _civilDocument.GetSiteIds();

                foreach (CadObjectId siteId in siteIds)
                {
                    Site site = tr.GetObject(siteId, OpenMode.ForRead) as Site;

                    if (site == null)
                    {
                        continue;
                    }

                    var objectIds = site.GetFeatureLineIds();

                    foreach(CadObjectId objectId in objectIds)
                    {
                        featureLinesIds.Add(objectId);
                    }
                }
            }

            return featureLinesIds;
        }


        public static List<CadObjectId> GetAllFeatureLineIdsByLayer(Document _autocaDocument, List<string> _layers)
        {
            List<CadObjectId> allFeatureLineIds = new List<CadObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>() { _FeatureLinesType, new TypedValue((int)DxfCode.Operator, "<OR") };


            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));


            SelectionFilter polylineFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult allPolylineResult = _autocaDocument.Editor.SelectAll(polylineFilter);


            if (allPolylineResult.Status == PromptStatus.OK)
            {
                SelectionSet allPolyLineSelectionSet = allPolylineResult.Value;
                allFeatureLineIds = allPolyLineSelectionSet.GetObjectIds().ToList();
            }

            return allFeatureLineIds;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_autocaDocument"></param>
        /// <param name="_civilDocument"></param>
        /// <returns></returns>
        public static List<string> GetAllSiteNames(Document _autocaDocument, CivilDocument _civilDocument)
        {
            List<string> allSiteNames = new List<string>();

            using (Transaction tr = _autocaDocument.Database.TransactionManager.StartTransaction())
            {
                var siteIds = _civilDocument.GetSiteIds();

                foreach (CadObjectId siteId in siteIds)
                {
                    Site site = tr.GetObject(siteId, OpenMode.ForRead) as Site;

                    if (site == null)
                    {
                        continue;
                    }

                    allSiteNames.Add(site.Name);    
                }
            }

            return allSiteNames;
        }


        public static List<CadObjectId> GetAllFeatureLineIdsBySiteName(Document _autocaDocument, CivilDocument _civilDocument, string _siteName)
        {

            List<CadObjectId> featureLineIds = new List<CadObjectId>(); 

            using (Transaction tr = _autocaDocument.Database.TransactionManager.StartTransaction())
            {
                var siteIds = _civilDocument.GetSiteIds();

                foreach (CadObjectId siteId in siteIds)
                {
                    Site site = tr.GetObject(siteId, OpenMode.ForRead) as Site;

                    if (site == null)
                    {
                        continue;
                    }

                    if (site.Name != _siteName)
                    {
                        continue;
                    }

                    featureLineIds.AddRange(site.GetFeatureLineIds().Cast<CadObjectId>().ToList());
                }
            }

            return featureLineIds;
        }


        public static List<ObjectId> PickFeatureLines(Document _autocadDocument)
        {
            List<ObjectId> polylineIds = new List<ObjectId>();

            PromptSelectionOptions promptOptions = new PromptSelectionOptions();
            promptOptions.MessageForAdding = "\nSelect Feature Lines";

            PromptSelectionResult selectionResult = _autocadDocument.Editor.GetSelection(promptOptions, _FeatureLinesFilter);

            if (selectionResult.Status == PromptStatus.OK)
            {
                var ids = selectionResult.Value.GetObjectIds().ToList();
                polylineIds.AddRange(ids);
            }

            return polylineIds;
        }
    }
}
