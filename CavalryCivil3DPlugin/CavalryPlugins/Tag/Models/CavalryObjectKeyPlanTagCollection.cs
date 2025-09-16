using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{
    public class CavalryObjectKeyPlanTagCollection
    {
        #region
        private Document _AutocadDocument;
        private CivilDocument _CivilDocument;
        #endregion


        #region OBJECT PROPERTIES
        private List<CavalryObjectKeyPlanTag> _AllTags = new List<CavalryObjectKeyPlanTag>();
        public List<CavalryObjectKeyPlanTag> AllTags => _AllTags;

        private List<CavalryObjectKeyPlanTag> _CurrentAvailableTags;
        public List<CavalryObjectKeyPlanTag> CurrentAvailableTags => _CurrentAvailableTags;

        private List<CavalryObjectKeyPlanTag> _AlignmentTags = new List<CavalryObjectKeyPlanTag>();
        public List<CavalryObjectKeyPlanTag> AlignmentTags => _AlignmentTags;

        private List<CavalryObjectKeyPlanTag> _FittingAppurtenanceTags = new List<CavalryObjectKeyPlanTag>();
        public List<CavalryObjectKeyPlanTag> FittingAppurtenanceTags => _FittingAppurtenanceTags;

        private List<CavalryObjectKeyPlanTag> _PointTags = new List<CavalryObjectKeyPlanTag>();
        public List<CavalryObjectKeyPlanTag> PointTags => _PointTags;

        private List<CavalryObjectKeyPlanTag> _COGOPointTags = new List<CavalryObjectKeyPlanTag>();
        public List<CavalryObjectKeyPlanTag> COGOPointTags => _COGOPointTags;

        private List<CavalryObjectKeyPlanTag> _PolylineTags = new List<CavalryObjectKeyPlanTag>();
        public List<CavalryObjectKeyPlanTag> PolylineTags => _PolylineTags;

        private List<ObjectTag> _ObjectTagCollection = new List<ObjectTag>();
        public List<ObjectTag> ObjectTagCollection => _ObjectTagCollection;
        #endregion

        public CavalryObjectKeyPlanTagCollection(Document _autocadDocument, CivilDocument _civilDocument)
        {
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;

            InitializeAlignmentObjectTag();
            InitializePressurePartObjectTag();
            InitializePointsObjectTag();
            InitializeCogoPointObjectTag();
            InitializePolylineObjectTag();
            SetAvailableTags(_ObjectTagCollection[0].ObjectType);
        }


        #region ALIGNMENT
        private void InitializeAlignmentObjectTag()
        {
            ObjectTag objectTag = new ObjectTag(_name: "Alignment", typeof(Alignment), this);
            _ObjectTagCollection.Add(objectTag);
            InitializeCorridorAlignmentTag();
        }


        private void InitializeCorridorAlignmentTag()
        {
            string tagName = "Trench Services Alignment Key Plan";
            string propertySet = "TRENCH_SERVICES_PKP";
            List<string> fields = new List<string>()
            {
                "CORRIDOR",
                "SERVICE_1",
                "SERVICE_2",
                "SERVICE_3",
                "SERVICE_4",
                "SERVICE_5",
                "SERVICE_6",
                "<Length>"
            };

            CavalryObjectKeyPlanTag CorridorAlignmentTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Alignment), tagName, propertySet, fields);

            _AllTags.Add(CorridorAlignmentTag);
            AlignmentTags.Add(CorridorAlignmentTag);
        }
        #endregion


        #region POLYLINE
        private void InitializePolylineObjectTag()
        {
            ObjectTag objectTag = new ObjectTag(_name: "Polyline", typeof(Polyline), this);
            _ObjectTagCollection.Add(objectTag);
            InitializeElectricalCableTag();
        }

        private void InitializeElectricalCableTag()
        {
            string tagName = "Electrical Cable Key Plan";
            string propertySet = "SANTOS_ELEC_DATA";
            List<string> fields = new List<string>()
            {
                "PKP CABLE TAG",
                "PKP DRUM NUMBER",
                "DESIGN_LENGTH"
            };

            CavalryObjectKeyPlanTag electricalCableTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Polyline), tagName, propertySet, fields);

            _AllTags.Add(electricalCableTag);
            _PolylineTags.Add(electricalCableTag);
        }
        #endregion


        #region PRESSURE COMPONENT
        private void InitializePressurePartObjectTag()
        {
            InitialzeFittingAppurtenanceTag();
            ObjectTag objectTag = new ObjectTag(_name: "Pressure Part Components", typeof (PressurePart), this);
            _ObjectTagCollection.Add(objectTag);
        }


        private void InitialzeFittingAppurtenanceTag()
        {
            string tagName = "Fitting/Appurtenance Key Plan";
            string propertySet = "SANTOS_ENG_DATA";
            List<string> fields = new List<string>()
            {
                "KEYP_NAME"
            };

            CavalryObjectKeyPlanTag fittingAppurtenanceTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(PressurePart), tagName, propertySet, fields);

            _AllTags.Add(fittingAppurtenanceTag);
            _FittingAppurtenanceTags.Add(fittingAppurtenanceTag);
        }
        #endregion


        #region COGO POINT
        private void InitializeCogoPointObjectTag()
        {
            ObjectTag objectTag = new ObjectTag(_name: "COGO Point", typeof(CogoPoint), this);
            _ObjectTagCollection.Add(objectTag);
            InitializeCogoPointNameTag();
        }


        private void InitializeCogoPointNameTag()
        {
            string tagName = "Name";
            List<string> fields = new List<string>()
            {
                "Name"
            };

            CavalryObjectKeyPlanTag CorridorAlignmentTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, _objectType: typeof(CogoPoint), _name: tagName, _objectProp: "Name", _fields: fields);

            _AllTags.Add(CorridorAlignmentTag);
            COGOPointTags.Add(CorridorAlignmentTag);
        }
        #endregion


        #region POINT
        private void InitializePointsObjectTag()
        {
             ObjectTag pointObjectTag = new ObjectTag(_name: "Point", typeof(Point), this);
            _ObjectTagCollection.Add(pointObjectTag);

            InitialzePointWellCenterTag();
            InitializePointVegetationTag();
            InitializePointWaterCrossingTag();
            InitializePointRoadCrossingTag();
            InitialzePointValveTag();
            InitialzeLPDTag();
            InitialzeHPVTag();
            InitialzeAllPointInfraCommentsTag();
        }


        private void InitialzePointWellCenterTag()
        {
            string tagName = "Well Center Key Plan";
            string propertySet = "DFP_CONCEPT_INFRA_SCOUTED";
            List<string> fields = new List<string>()
            {
                "RELATED_IN"
            };

            CavalryObjectKeyPlanTag pointWellCenterTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: "INFRASTRU0", valueFilter: "Well Centre");

            _AllTags.Add(pointWellCenterTag);
            _PointTags.Add(pointWellCenterTag);
        }


        private void InitialzePointValveTag()
        {
            string tagName = "GIS - VALVE";
            string propertySet = "PIPELINE_VALVESPoint";
            string keyFilter = "OBJECT_TYP";
            string valueFilter = "P_VALVE";
            List<string> fields = new List<string>()
            {
                "OBJECT_TYP"
            };

            CavalryObjectKeyPlanTag pointValveTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(pointValveTag);
            _PointTags.Add(pointValveTag);
        }


        private void InitialzeAllPointInfraCommentsTag()
        {
            string tagName = "GIS - ALL POINTS INFRA0 + COMMENTS";
            string propertySet = "DFP_CONCEPT_INFRA_SCOUTED";
            string keyFilter = null;
            string valueFilter = null;

            List<string> fields = new List<string>()
            {
                "INFRASTRU0",
                "COMMENTS"
            };

            CavalryObjectKeyPlanTag pointInfraCommentTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(pointInfraCommentTag);
            _PointTags.Add(pointInfraCommentTag);
        }


        private void InitialzeLPDTag()
        {
            string tagName = "GIS - LPD";
            string propertySet = "PIPELINE_LP_DRAINSPoint";
            string keyFilter = "OBJECT_TYP";
            string valueFilter = "P_DRAINLPT";
            List<string> fields = new List<string>()
            {
                "OBJECT_TYP"
            };

            CavalryObjectKeyPlanTag pointLPDTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(pointLPDTag);
            _PointTags.Add(pointLPDTag);
        }

        private void InitialzeHPVTag()
        {
            string tagName = "GIS - HPV";
            string propertySet = "PIPELINE_HP_VENTSPoint";
            string keyFilter = "OBJECT_TYP";
            string valueFilter = "P_HPV";
            List<string> fields = new List<string>()
            {
                "OBJECT_TYP"
            };

            CavalryObjectKeyPlanTag pointHPVTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(pointHPVTag);
            _PointTags.Add(pointHPVTag);
        }



        private void InitializePointVegetationTag()
        {
            string tagName = "Vegetation Key Plan";
            string propertySet = "DFP_CONCEPT_INFRA_SCOUTED";
            string keyFilter = "INFRASTRU0";
            string valueFilter = "Veg to Avoid";

            List<string> fields = new List<string>()
            {
                "INFRASTRU0",
                "COMMENTS"
            };

            CavalryObjectKeyPlanTag poinVegetationTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(poinVegetationTag);
            _PointTags.Add(poinVegetationTag);
        }


        private void InitializePointWaterCrossingTag()
        {
            string tagName = "Water Crossing Key Plan";
            string propertySet = "DFP_CONCEPT_INFRA_SCOUTED";
            string keyFilter = "INFRASTRU0";
            string valueFilter = "Crossing - FlowWC";

            List<string> fields = new List<string>()
            {
                "INFRASTRU0",
                "DRAWING_NU"
            };

            CavalryObjectKeyPlanTag poinWaterCrossingTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(poinWaterCrossingTag);
            _PointTags.Add(poinWaterCrossingTag);
        }


        private void InitializePointRoadCrossingTag()
        {
            string tagName = "Road Crossing Key Plan";
            string propertySet = "DFP_CONCEPT_INFRA_SCOUTED";
            string keyFilter = "INFRASTRU0";
            string valueFilter = "Crossing";

            List<string> fields = new List<string>()
            {
                "INFRASTRU0",
                "DRAWING_NU"
            };

            CavalryObjectKeyPlanTag poinRoadCrossingTag = new CavalryObjectKeyPlanTag(_AutocadDocument, _CivilDocument, typeof(Point), tagName, propertySet, fields, keyFilter: keyFilter, valueFilter: valueFilter);

            _AllTags.Add(poinRoadCrossingTag);
            _PointTags.Add(poinRoadCrossingTag);
        }
        #endregion



        public void SetAvailableTags(Type _objectType)
        {
            if (_objectType == typeof(Alignment))
            {
                _CurrentAvailableTags = _AlignmentTags;
            }

            else if (_objectType == typeof(PressurePart))
            {
                _CurrentAvailableTags = _FittingAppurtenanceTags;
            }

            else if (_objectType == typeof(Point))
            {
                _CurrentAvailableTags = _PointTags;
            }

            else if (_objectType == typeof(CogoPoint))
            {
                _CurrentAvailableTags = _COGOPointTags;
            }
        }
    }
}
