using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO.ViewModel
{
    public class MTOMainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


        private MTOMain _MTOMainModel;
        public MTOMain MTOMainModel => _MTOMainModel;


        #region MAIN PROPERTIES
        private string _PipeTabName;

        public string PipeTabName
        {
            get { return _PipeTabName; }
            set { _PipeTabName = value; OnPropertyChanged(nameof(PipeTabName)); }
        }

        private string _CorridorTabName;
        public string CorridorTabName
        {
            get { return _CorridorTabName; }
            set { _CorridorTabName = value; OnPropertyChanged(nameof(CorridorTabName)); }
        }

        private string _FeatureTabName;
        public string FeatureTabName
        {
            get { return _FeatureTabName; }
            set { _FeatureTabName = value; OnPropertyChanged(nameof(FeatureTabName)); }
        }

        #endregion


        #region FEATURE HEADER
        private string _FHeader0;
        public string FHeader0
        {
            get { return _FHeader0; }
            set { _FHeader0 = value; OnPropertyChanged(nameof(FHeader0)); }
        }

        private string _FHeader1;
		public string FHeader1
		{
			get { return _FHeader1; }
			set { _FHeader1 = value; OnPropertyChanged(nameof(FHeader1));}
		}

        private string _FHeader2;
        public string FHeader2
        {
            get { return _FHeader2; }
            set { _FHeader2 = value; OnPropertyChanged(nameof(FHeader2)); }
        }

        private string _FHeader3;
        public string FHeader3
        {
            get { return _FHeader3; }
            set { _FHeader3 = value; OnPropertyChanged(nameof(FHeader3)); }
        }

        private string _FHeader4;
        public string FHeader4
        {
            get { return _FHeader4; }
            set { _FHeader4 = value; OnPropertyChanged(nameof(FHeader4)); }
        }

        private string _FHeader5;
        public string FHeader5
        {
            get { return _FHeader5; }
            set { _FHeader5 = value; OnPropertyChanged(nameof(FHeader5)); }
        }

        private string _FHeader6;
        public string FHeader6
        {
            get { return _FHeader6; }
            set { _FHeader6= value; OnPropertyChanged(nameof(FHeader6)); }
        }

        private string _FHeader7;
        public string FHeader7
        {
            get { return _FHeader7; }
            set { _FHeader7 = value; OnPropertyChanged(nameof(FHeader7)); }
        }

        private string _FHeader8;
        public string FHeader8
        {
            get { return _FHeader8; }
            set { _FHeader8= value; OnPropertyChanged(nameof(FHeader8)); }
        }

        private string _FHeader9;
        public string FHeader9
        {
            get { return _FHeader9; }
            set { _FHeader9 = value; OnPropertyChanged(nameof(FHeader9)); }
        }

        private string _FHeader10;
        public string FHeader10
        {
            get { return _FHeader10; }
            set { _FHeader10 = value; OnPropertyChanged(nameof(FHeader10)); }
        }

        private string _FHeader11;
        public string FHeader11
        {
            get { return _FHeader11; }
            set { _FHeader11 = value; OnPropertyChanged(nameof(FHeader11)); }
        }

        private string _FHeader12;
        public string FHeader12
        {
            get { return _FHeader12; }
            set { _FHeader12 = value; OnPropertyChanged(nameof(FHeader12)); }
        }

        private string _FHeader13;
        public string FHeader13
        {
            get { return _FHeader13; }
            set { _FHeader13 = value; OnPropertyChanged(nameof(FHeader13)); }
        }
        #endregion


        #region PIPELINE HEADER
        private string _PHeader0;
        public string PHeader0
        {
            get { return _PHeader0; }
            set { _PHeader0 = value; OnPropertyChanged(nameof(PHeader0)); }
        }

        private string _PHeader1;
        public string PHeader1
        {
            get { return _PHeader1; }
            set { _PHeader1 = value; OnPropertyChanged(nameof(PHeader1));}
        }

        private string _PHeader2;
        public string PHeader2
        {
            get { return _PHeader2; }
            set { _PHeader2 = value; OnPropertyChanged(nameof(PHeader2)); }
        }

        private string _PHeader3;
        public string PHeader3
        {
            get { return _PHeader3; }
            set { _PHeader3 = value; OnPropertyChanged(nameof(PHeader3)); }
        }

        private string _PHeader4;
        public string PHeader4
        {
            get { return _PHeader4; }
            set { _PHeader4 = value; OnPropertyChanged(nameof(PHeader4)); }
        }

        private string _PHeader5;
        public string PHeader5
        {
            get { return _PHeader5; }
            set { _PHeader5 = value; OnPropertyChanged(nameof(PHeader5)); }
        }

        private string _PHeader6;
        public string PHeader6
        {
            get { return _PHeader6; }
            set { _PHeader6 = value; OnPropertyChanged(nameof(PHeader6)); }
        }

        private string _PHeader7;
        public string PHeader7
        {
            get { return _PHeader7; }
            set { _PHeader7 = value; OnPropertyChanged(nameof(PHeader7)); }
        }

        private string _PHeader8;
        public string PHeader8
        {
            get { return _PHeader8; }
            set { _PHeader8 = value; OnPropertyChanged(nameof(PHeader8)); }
        }

        private string _PHeader9;
        public string PHeader9
        {
            get { return _PHeader9; }
            set { _PHeader9 = value; OnPropertyChanged(nameof(PHeader9)); }
        }

        private string _PHeader10;
        public string PHeader10
        {
            get { return _PHeader10; }
            set { _PHeader10 = value; OnPropertyChanged(nameof(PHeader10)); }
        }

        private string _PHeader11;
        public string PHeader11
        {
            get { return _PHeader11; }
            set { _PHeader11 = value; OnPropertyChanged(nameof(PHeader11)); }
        }

        private string _PHeader12;
        public string PHeader12
        {
            get { return _PHeader12; }
            set { _PHeader12 = value; OnPropertyChanged(nameof(PHeader12)); }
        }

        private string _PHeader13;
        public string PHeader13
        {
            get { return _PHeader13; }
            set { _PHeader13 = value; OnPropertyChanged(nameof(PHeader13)); }
        }

        private string _Pheader14;
        public string PHeader14
        {
            get { return _Pheader14; }
            set { _Pheader14 = value; OnPropertyChanged(nameof(PHeader14)); }
        }

        private string _PHeader15;
        public string PHeader15
        {
            get { return _PHeader15; }
            set { _PHeader15 = value; OnPropertyChanged(nameof(PHeader15)); }
        }
        #endregion


        #region CORRIDOR HEADER
        private string _CHeader0;
        public string CHeader0
        {
            get { return _CHeader0; }
            set { _CHeader0 = value; OnPropertyChanged(nameof(CHeader0)); }
        }

        private string _CHeader1;
        public string CHeader1
        {
            get { return _CHeader1; }
            set { _CHeader1 = value; OnPropertyChanged(nameof(CHeader1)); }
        }

        private string _CHeader2;
        public string CHeader2
        {
            get { return _CHeader2; }
            set { _CHeader2 = value; OnPropertyChanged(nameof(CHeader2)); }
        }

        private string _CHeader3;
        public string CHeader3 
        {
            get { return _CHeader3; }
            set { _CHeader3 = value; OnPropertyChanged(nameof(CHeader3)); }
        }

        private string _CHeader4;
        public string CHeader4
        {
            get { return _CHeader4; }
            set { _CHeader4 = value; OnPropertyChanged(nameof(CHeader4)); }
        }

        private string _CHeader5;
        public string CHeader5
        {
            get { return _CHeader5; }
            set { _CHeader5 = value; OnPropertyChanged(nameof(CHeader5)); }
        }

        private string _CHeader6;
        public string CHeader6
        {
            get { return _CHeader6; }
            set { _CHeader6 = value; OnPropertyChanged(nameof(CHeader6)); }
        }

        private string _CHeader7;
        public string CHeader7
        {
            get { return _CHeader7; }
            set { _CHeader7 = value; OnPropertyChanged(nameof(CHeader7)); }
        }
        #endregion



        public MTOMainViewModel()
        {
            _MTOMainModel = new MTOMain();
            SetTabNames();
            SetFeatureHeader();
            SetPipelineHeader();
            SetCorridorHeader();
        }

        public void SetTabNames()
        {
            int totalPipeData = _MTOMainModel.PipelineData.PipeRunDataCollection.Count;
            int totalCorridorData = _MTOMainModel.CorridorData.AlignmentDataCollection.Count;
            int totalFeatureData = _MTOMainModel.FeatureData.ComponentDataCollection.Count;

            _PipeTabName = $"Pipeline Data ({totalPipeData})";
            _CorridorTabName = $"Corrdor Data ({totalCorridorData})";
            _FeatureTabName = $"Feature Summary ({totalFeatureData})";
        }

        public void SetFeatureHeader()
        {
            FHeader0 = "Item";
            FHeader1 = "CWP";
            FHeader2 = "WBS Level 9";
            FHeader3 = "Corridor";
            FHeader4 = "Unique Tag No";
            FHeader5 = "Feature";
            FHeader6 = "SAP Code";
            FHeader7 = "Description";
            FHeader8 = "Size";
            FHeader9 = "Spec";
            FHeader10 = "Classification";
            FHeader11 = "UOM";
            FHeader12 = "Total";
            FHeader13 = "Type";
        }

        public void SetPipelineHeader()
        {
            PHeader0 = "Item";
            PHeader1 = "CWP";
            PHeader2 = "WBS Level 9";
            PHeader3 = "Corridor";
            PHeader4 = "SAP Code";
            PHeader5 = "Description";
            PHeader6 = "Network Type";
            PHeader7 = "Size";
            PHeader8 = "Spec";
            PHeader9 = "Length (m)";
            PHeader10 = "UNIT_E";
            PHeader11 = "Piperun Id";
            PHeader12 = "Partlist";
            PHeader13 = "Classification";
            PHeader14 = "Factor";
            PHeader15 = "Type";
        }

        public void SetCorridorHeader()
        {
            CHeader0 = "Item";
            CHeader1 = "CWP";
            CHeader2 = "WBS Level 9";
            CHeader3 = "Corridor";
            CHeader4 = "SAP Code";
            CHeader5 = "Description";
            CHeader6 = "Trench Details";
            CHeader7 = "Length";
        }
    }
}
