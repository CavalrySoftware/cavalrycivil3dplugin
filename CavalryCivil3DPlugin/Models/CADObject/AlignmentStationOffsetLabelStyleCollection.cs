using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class AlignmentStationOffsetLabelStyleCollection
    {
        private Dictionary<string, ObjectId> _StyleId = new Dictionary<string, ObjectId>();
        public Dictionary<string, ObjectId> StyleId => _StyleId;

        private Document _AutocadDocument;
        private CivilDocument _CivilDocumentl;

        private List<LabelStyleModel> _LabelStyleModels = new List<LabelStyleModel>();
        public List<LabelStyleModel> LabelStyleModels => _LabelStyleModels;

        private LabelStyleModel _DefaultLabelStyleModel;
        public LabelStyleModel DefaultLabelStyleModel => _DefaultLabelStyleModel;

        private string _DefaultStyleName;


        public AlignmentStationOffsetLabelStyleCollection(Document _autocadDocument, CivilDocument _civilDocument, string _defaultStyleName = null)
        {
           _AutocadDocument = _autocadDocument;
           _CivilDocumentl = _civilDocument;
           _DefaultStyleName= _defaultStyleName;

            GetAllStyles();
            SetDefault();
        }


        private void GetAllStyles()
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                LabelStyleCollection styleCollection = _CivilDocumentl.Styles.LabelStyles.AlignmentLabelStyles.StationOffsetLabelStyles;
                foreach (var style in styleCollection)
                {
                    LabelStyle labelStyle = tr.GetObject(style, OpenMode.ForRead) as LabelStyle;
                    LabelStyleModel labelStyleModel = new LabelStyleModel()
                    {
                        Id = labelStyle.Id,
                        Name = labelStyle.Name,
                    };  
                    
                    _LabelStyleModels.Add(labelStyleModel);
                }
            }
        }

        private void SetDefault()
        {
            _DefaultLabelStyleModel = _LabelStyleModels[0];

            if (_DefaultStyleName != null)
            {
                if (_LabelStyleModels.Any(x => x.Name == _DefaultStyleName))
                {
                    _DefaultLabelStyleModel = _LabelStyleModels.FirstOrDefault(x => x.Name == _DefaultStyleName);
                }
            }
        }
     }
}
