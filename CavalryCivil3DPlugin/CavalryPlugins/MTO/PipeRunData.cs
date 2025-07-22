using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class PipeRunData
    {

        private int _Index;

        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }


        private string _WBSLevel9;

        public string WBSLevel9
        {
            get { return _WBSLevel9; }
            set { _WBSLevel9 = value; }
        }


        private string _Branch;
        public string Branch
        {
            get { return _Branch; }
            set { _Branch = value; }
        }

        private string _CWP;


        public string CWP
        {
            get { return _CWP; }
            set { _CWP = value; }
        }


        private string _CorridorId;
        public string CorridorId
        {
            get { return _CorridorId; }
            set { _CorridorId = value; }
        }

        private string _Network;
        public string Network
        {
            get { return _Network; }
            set {_Network = value.ToLower().Contains("gas") ? "Gas" : "Water";}
        }

        private string _Size;
        public string Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        private string _PipeRunId;
        public string PipeRunId
        {
            get { return _PipeRunId; }
            set { _PipeRunId = value; }
        }

        private string _PartListDescription;
        public string PartListDescription
        {
            get { return _PartListDescription; }
            set { _PartListDescription = value; }
        }

        private double _Length;

        public double Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        private string _SAPCode;

        public string SAPCode
        {
            get { return _SAPCode; }
            set { _SAPCode = value; }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Spec;

        public string Spec
        {
            get { return _Spec; }
            set { _Spec = value; }
        }

        private string _Classification;

        public string Classification
        {
            get { return _Classification; }
            set { _Classification = value; }
        }

        private string _Type;

        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string  _Factor;

        public string  Factor
        {
            get { return _Factor; }
            set { _Factor = value; }
        }

        private string _Unit;

        public string Unit
        {
            get { return _Unit; }
            set { _Unit = value; }
        }



        public PipeRunData()
        {

        }

    }
}
