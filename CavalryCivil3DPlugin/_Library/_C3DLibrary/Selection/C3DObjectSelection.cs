using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Aec.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin.Consoles;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace CavalryCivil3DPlugin.C3DLibrary.Selection
{
    public class C3DObjectSelection
    {

        private static TypedValue _FeatureLinesType = new TypedValue((int)DxfCode.Start, "AECC_FEATURE_LINE");
        private static TypedValue _AlignmentType = new TypedValue((int)DxfCode.Start, "AECC_ALIGNMENT");
        private static TypedValue _ProfileViewType = new TypedValue((int)DxfCode.Start, "AECC_PROFILE_VIEW");
        private static TypedValue _FittingType = new TypedValue((int)DxfCode.Start, "AECC_FITTING");
        private static TypedValue _AppurtenanceType = new TypedValue((int)DxfCode.Start, "AECC_APPURTENANCE");
        private static TypedValue _PointType = new TypedValue((int)DxfCode.Start, "POINT");
        private static TypedValue _COGOPointType = new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT");


        private static SelectionFilter _FeatureLinesFilter = new SelectionFilter(new TypedValue[] { _FeatureLinesType });
        private static SelectionFilter _ProfileViewFilter = new SelectionFilter(new TypedValue[] { _ProfileViewType });


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


        public static ObjectId PickProfileView(Document _autocadDocument)
        {
            return PickElement(_autocadDocument, typeof(ProfileView), "Profile View");
        }


        public static ObjectId PickAlignment(Document _autocadDocument)
        {
            return PickElement(_autocadDocument, typeof(Alignment), "Alignment");
        }


        public static (ObjectId, Point3d) PickAlignmentWithPoint(Document _autocadDocument)
        {
            List<Type> types = new List<Type>() { typeof(Alignment) };
            return PickElementWithPoint(_autocadDocument, types, "Alignment");
        }


        public static (ObjectId, Point3d) PickCogoPointWithPoint(Document _autocadDocument)
        {
            List<Type> types = new List<Type>() { typeof(CogoPoint) };
            return PickElementWithPoint(_autocadDocument, types, "COGO Point");
        }


        public static (ObjectId, Point3d) PickPointWithPoint(Document _autocadDocument)
        {
            List<Type> types = new List<Type>() { typeof(DBPoint) };
            return PickElementWithPoint(_autocadDocument, types, "Point");
        }

        public static (ObjectId, Point3d) PickPolylineWithPoint(Document _autocadDocument)
        {
            List<Type> types = new List<Type>() { typeof(Polyline) };
            return PickElementWithPoint(_autocadDocument, types, "Polyline");
        }


        public static (ObjectId, Point3d) PickPressureComponentsWithPoint(Document _autocadDocument)
        {
            List<Type> types = new List<Type>() { typeof(PressureFitting), typeof(PressureAppurtenance) };
            return PickElementWithPoint(_autocadDocument, types, "Pressure Fitting / Appurtenance");
        }


        public static ObjectId PickPressurePipe(Document _autocadDocument, string _select)
        {
            return PickElement(_autocadDocument, typeof(PressurePipe), _select);
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



        private static (ObjectId, Point3d) PickElementWithPoint(Document _autocadDocument, List<Type> _Elementypes, string elementName)
        {
            string selectMessage = ($"\nSelect {elementName}");
            string rejectMessage = ($"\nSelect only {elementName}");

            PromptSelectionOptions promptOptions = new PromptSelectionOptions();

            PromptEntityOptions promptEntityOptions = new PromptEntityOptions($"\n{selectMessage}");
            promptEntityOptions.SetRejectMessage($"\n{rejectMessage}");
          
            foreach (Type type in _Elementypes)
            {
                promptEntityOptions.AddAllowedClass(type, true);
            }

            PromptEntityResult selectionResult = _autocadDocument.Editor.GetEntity(promptEntityOptions);

            if (selectionResult.Status == PromptStatus.OK)
            {
                return (selectionResult.ObjectId,selectionResult.PickedPoint);
            }

            return (ObjectId.Null, new Point3d());
        }



        public static List<ObjectId> GetAlignmentIdsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> allAlignmentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                _AlignmentType,
                new TypedValue((int)DxfCode.Operator, "<OR")
            };

            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));



            SelectionFilter alignmentFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult allAlignmentResult = _autocadDocument.Editor.SelectAll(alignmentFilter);

            if (allAlignmentResult.Status == PromptStatus.OK)
            {
                SelectionSet alignmentSelectionSet = allAlignmentResult.Value;
                allAlignmentIds = alignmentSelectionSet.GetObjectIds().ToList();
            }

            return allAlignmentIds;
        }


        public static List<ObjectId> PickAlignmentsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> allAlignmentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                _AlignmentType,
                new TypedValue((int)DxfCode.Operator, "<OR")
            };

            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect alignments: ",
                MessageForRemoval = "\nRemove alignments: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter alignmentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult allAlignmentResult = _autocadDocument.Editor.GetSelection(selectionOptions, alignmentFilter);

            if (allAlignmentResult.Status == PromptStatus.OK)
            {
                SelectionSet alignmentSelectionSet = allAlignmentResult.Value;
                allAlignmentIds = alignmentSelectionSet.GetObjectIds().ToList();
            }

            return allAlignmentIds;
        }


        public static List<ObjectId> PickPressureComponentsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> pressureComponentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                new TypedValue((int)DxfCode.Operator, "<OR"),
                _FittingType,
                _AppurtenanceType,
                new TypedValue((int)DxfCode.Operator, "OR>"),
                new TypedValue((int)DxfCode.Operator, "<OR")
            };

            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect Fitting/Appurtenances: ",
                MessageForRemoval = "\nRemove Fitting/Appurtenances: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter componentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult pressureComponentResult = _autocadDocument.Editor.GetSelection(selectionOptions, componentFilter);

            if (pressureComponentResult.Status == PromptStatus.OK)
            {
                SelectionSet pressureComponentSelectionSet = pressureComponentResult.Value;
                pressureComponentIds = pressureComponentSelectionSet.GetObjectIds().ToList();
            }

            return pressureComponentIds;
        }


        public static List<ObjectId> PickPointsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> pointIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                _PointType,
                new TypedValue((int)DxfCode.Operator, "<OR")
            };

            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect Fitting/Appurtenances: ",
                MessageForRemoval = "\nRemove Fitting/Appurtenances: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter componentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult pressureComponentResult = _autocadDocument.Editor.GetSelection(selectionOptions, componentFilter);

            if (pressureComponentResult.Status == PromptStatus.OK)
            {
                SelectionSet pressureComponentSelectionSet = pressureComponentResult.Value;
                pointIds = pressureComponentSelectionSet.GetObjectIds().ToList();
            }

            return pointIds;
        }


        public static List<ObjectId> PickCogoPointsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> pointIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                _COGOPointType,
                new TypedValue((int)DxfCode.Operator, "<OR")
            };

            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect COGO Points: ",
                MessageForRemoval = "\nRemove COGO Points: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter componentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult pressureComponentResult = _autocadDocument.Editor.GetSelection(selectionOptions, componentFilter);

            if (pressureComponentResult.Status == PromptStatus.OK)
            {
                SelectionSet pressureComponentSelectionSet = pressureComponentResult.Value;
                pointIds = pressureComponentSelectionSet.GetObjectIds().ToList();
            }

            return pointIds;
        }


        public static List<ObjectId> PickAlignments(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> allAlignmentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _AlignmentType,
            };

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect Alignments: ",
                MessageForRemoval = "\nRemove Alignments: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter alignmentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult allAlignmentResult = _autocadDocument.Editor.GetSelection(selectionOptions, alignmentFilter);

            if (allAlignmentResult.Status == PromptStatus.OK)
            {
                SelectionSet alignmentSelectionSet = allAlignmentResult.Value;
                allAlignmentIds = alignmentSelectionSet.GetObjectIds().ToList();
            }

            return allAlignmentIds;
        }


        public static List<ObjectId> PickPolylines(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> allAlignmentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _AlignmentType,
            };

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect Polylines: ",
                MessageForRemoval = "\nRemove Polylines: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter alignmentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult allAlignmentResult = _autocadDocument.Editor.GetSelection(selectionOptions, alignmentFilter);

            if (allAlignmentResult.Status == PromptStatus.OK)
            {
                SelectionSet alignmentSelectionSet = allAlignmentResult.Value;
                allAlignmentIds = alignmentSelectionSet.GetObjectIds().ToList();
            }

            return allAlignmentIds;
        }


        public static List<ObjectId> PickPressureComponents(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> pressureComponentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<OR"),
                _FittingType,
                _AppurtenanceType,
                new TypedValue((int)DxfCode.Operator, "OR>")
            };

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect Fitting/Appurtenances: ",
                MessageForRemoval = "\nRemove Fitting/Appurtenances: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter pressuerComponentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult pressureComponentResult = _autocadDocument.Editor.GetSelection(selectionOptions, pressuerComponentFilter);

            if (pressureComponentResult.Status == PromptStatus.OK)
            {
                SelectionSet pressureComponentSelectionSet = pressureComponentResult.Value;
                pressureComponentIds = pressureComponentSelectionSet.GetObjectIds().ToList();
            }

            return pressureComponentIds;
        }


        public static List<ObjectId> PickPoints(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> pointIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _PointType
            };

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect Points: ",
                MessageForRemoval = "\nRemove Points: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter pressuerComponentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult pressureComponentResult = _autocadDocument.Editor.GetSelection(selectionOptions, pressuerComponentFilter);

            if (pressureComponentResult.Status == PromptStatus.OK)
            {
                SelectionSet pressureComponentSelectionSet = pressureComponentResult.Value;
                pointIds = pressureComponentSelectionSet.GetObjectIds().ToList();
            }

            return pointIds;
        }


        public static List<ObjectId> PickCOGOPoints(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> pointIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _COGOPointType
            };

            PromptSelectionOptions selectionOptions = new PromptSelectionOptions()
            {
                MessageForAdding = "\nSelect COGO Points: ",
                MessageForRemoval = "\nRemove COGO Points: ",
                AllowDuplicates = false,
                RejectObjectsOnLockedLayers = true
            };

            SelectionFilter pressuerComponentFilter = new SelectionFilter(typedValues.ToArray());

            PromptSelectionResult pressureComponentResult = _autocadDocument.Editor.GetSelection(selectionOptions, pressuerComponentFilter);

            if (pressureComponentResult.Status == PromptStatus.OK)
            {
                SelectionSet pressureComponentSelectionSet = pressureComponentResult.Value;
                pointIds = pressureComponentSelectionSet.GetObjectIds().ToList();
            }

            return pointIds;
        }


        public static List<ObjectId> GetAllAlignments(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> allAlignmentIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _AlignmentType,
            };


            SelectionFilter alignmentFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult allAlignmentResult = _autocadDocument.Editor.SelectAll(alignmentFilter);

            if (allAlignmentResult.Status == PromptStatus.OK)
            {
                SelectionSet alignmentSelectionSet = allAlignmentResult.Value;
                allAlignmentIds = alignmentSelectionSet.GetObjectIds().ToList();
            }

            return allAlignmentIds;
        }


        public static List<ObjectId> GetFittingsAndAppurtenancesByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> objectIds = new List<ObjectId>();


            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                new TypedValue((int)DxfCode.Operator, "<OR"),
                _FittingType,
                _AppurtenanceType,
                new TypedValue((int)DxfCode.Operator, "OR>"),
                new TypedValue((int)DxfCode.Operator, "<OR"),
            };


            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            SelectionFilter componentsFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult componentsResult = _autocadDocument.Editor.SelectAll(componentsFilter);

            if (componentsResult.Status == PromptStatus.OK)
            {
                SelectionSet componentsSelectionSet = componentsResult.Value;
                objectIds = componentsSelectionSet.GetObjectIds().ToList();
            }

            return objectIds;
        }


        public static List<ObjectId> GetAllFittingsAndAppurtenances(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> objectIds = new List<ObjectId>();


            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<OR"),
                _FittingType,
                _AppurtenanceType,
                new TypedValue((int)DxfCode.Operator, "OR>"),
            };


            SelectionFilter componentsFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult componentsResult = _autocadDocument.Editor.SelectAll(componentsFilter);

            if (componentsResult.Status == PromptStatus.OK)
            {
                SelectionSet componentsSelectionSet = componentsResult.Value;
                objectIds = componentsSelectionSet.GetObjectIds().ToList();
            }

            return objectIds;
        }

        public static List<ObjectId> GetAllPoints(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> objectIds = new List<ObjectId>();


            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _PointType
            };


            SelectionFilter componentsFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult componentsResult = _autocadDocument.Editor.SelectAll(componentsFilter);

            if (componentsResult.Status == PromptStatus.OK)
            {
                SelectionSet componentsSelectionSet = componentsResult.Value;
                objectIds = componentsSelectionSet.GetObjectIds().ToList();
            }

            return objectIds;
        }


        public static List<ObjectId> GetAllCOGOPoints(Document _autocadDocument, CivilDocument _civilDocument)
        {
            List<ObjectId> objectIds = new List<ObjectId>();


            List<TypedValue> typedValues = new List<TypedValue>()
            {
                _COGOPointType
            };


            SelectionFilter COGOPointsFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult cogoPointsResult = _autocadDocument.Editor.SelectAll(COGOPointsFilter);

            if (cogoPointsResult.Status == PromptStatus.OK)
            {
                SelectionSet cogoPointSelectionSet = cogoPointsResult.Value;
                objectIds = cogoPointSelectionSet.GetObjectIds().ToList();
            }

            return objectIds;
        }


        public static List<ObjectId> GetAllPointsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> objectIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                _PointType,
                new TypedValue((int)DxfCode.Operator, "<OR"),
            };


            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            SelectionFilter componentsFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult componentsResult = _autocadDocument.Editor.SelectAll(componentsFilter);

            if (componentsResult.Status == PromptStatus.OK)
            {
                SelectionSet componentsSelectionSet = componentsResult.Value;
                objectIds = componentsSelectionSet.GetObjectIds().ToList();
            }

            return objectIds;
        }


        public static List<ObjectId> GetAllCOGOPointsByLayer(Document _autocadDocument, CivilDocument _civilDocument, List<string> _layers)
        {
            List<ObjectId> objectIds = new List<ObjectId>();

            List<TypedValue> typedValues = new List<TypedValue>()
            {
                new TypedValue((int)DxfCode.Operator, "<AND"),
                _COGOPointType,
                new TypedValue((int)DxfCode.Operator, "<OR"),
            };


            foreach (string layerName in _layers)
            {
                TypedValue layerType = new TypedValue((int)DxfCode.LayerName, layerName);
                typedValues.Add(layerType);
            }
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
            typedValues.Add(new TypedValue((int)DxfCode.Operator, "AND>"));

            SelectionFilter cogoPointsFilter = new SelectionFilter(typedValues.ToArray());
            PromptSelectionResult cogoPointsResult = _autocadDocument.Editor.SelectAll(cogoPointsFilter);

            if (cogoPointsResult.Status == PromptStatus.OK)
            {
                SelectionSet cogoPointsSelectionSet = cogoPointsResult.Value;
                objectIds = cogoPointsSelectionSet.GetObjectIds().ToList();
            }

            return objectIds;
        }
    }
}
