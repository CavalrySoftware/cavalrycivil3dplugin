using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.CADObject;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.ViewModels
{
    public class SOPMainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


        private SOPMainModel _SOPMainModel;
        public SOPMainModel SOPMainModel_ => _SOPMainModel;

        #region CONSTRUCTOR
        public SOPMainViewModel()
        {
            _SOPMainModel = new SOPMainModel();
            InitializeApp();
        }
        #endregion


        #region << WINDOW ACTIONS >>
        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }
        public Action ShowAction { get; set; }
        public bool ClosedByXButton = false;
        #endregion


        #region BINDING PROPERTIES
        private SurfaceModel _ExistingSurface;
        public SurfaceModel ExistingSurface
        {
            get
            { return _ExistingSurface;}
            set
            { 
                _ExistingSurface = value;
                _SOPMainModel.SetSop(DesignSurface, ExistingSurface);
                RefreshSOPs();
                OnPropertyChanged(nameof(ExistingSurface));
            }
        }

        private SurfaceModel _DesignSurface;

        public SurfaceModel DesignSurface
        {
            get { return _DesignSurface; }
            set { _DesignSurface = value; 
                _SOPMainModel.SetSop(value, ExistingSurface);
                RefreshSOPs();
                OnPropertyChanged(nameof(DesignSurface));
            }
        }

        private string _TableStyle;
        public string TableStyle
        {
            get { return _TableStyle; }
            set { _TableStyle = value; OnPropertyChanged(nameof(TableStyle)); }
        }

        private List<SOPObjectModel> _SOPObjectModel = new List<SOPObjectModel>();
        public List<SOPObjectModel> SOPObjectModel
        {
            get { return _SOPObjectModel; }
            set { _SOPObjectModel = value; OnPropertyChanged(nameof(SOPObjectModel)); }
        }

        public void RefreshSOPs()
        {
            SOPObjectModel = new List<SOPObjectModel>();
            SOPObjectModel = SOPMainModel_.SOPs;
        }


        private bool _IsAnnotate;

        public bool IsAnnotate
        {
            get { return _IsAnnotate; }
            set { _IsAnnotate = value; OnPropertyChanged(nameof(IsAnnotate));}
        }


        private string _Prefix;

        public string Prefix
        {
            get { return _Prefix; }
            set { _Prefix = value; OnPropertyChanged(nameof(Prefix)); }
        }
        #endregion


        #region COMMANDS
        private PickPolylineSOPCommand _PickPolylineCommand;
        public PickPolylineSOPCommand PickPolylineCommand => _PickPolylineCommand;

        private OkCommand _OkCommand;
        public OkCommand OkCommand => _OkCommand;
        #endregion


        #region INITIALIZE 
        private void InitializeApp()
        {
            ExistingSurface = _SOPMainModel.SurfaceModelCollection_.SurfaceModels.FirstOrDefault();
            DesignSurface = _SOPMainModel.SurfaceModelCollection_.SurfaceModels.FirstOrDefault();
            TableStyle = _SOPMainModel.TableStyleModel_.DefaultStyle;

            _PickPolylineCommand = new PickPolylineSOPCommand(this);
            _OkCommand = new OkCommand(this);
        }
        #endregion


        #region SYSTEM FUNCTIONS
        public void ClosingWindow()
        {
            if (ClosedByXButton)
            {
                
            }
        }
        #endregion
    }
}
