using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.DatabaseServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{

    public class ComponentsData
    {

		private int _Index;
		public int Index
		{
			get { return _Index; }
			set { _Index = value; }
		}

		private string  _CWP;
		public string  CWP
		{
			get { return _CWP; }
			set { _CWP = value; }
		}

		private string _WBSLevel9;
		public string WBSLevel9
		{
			get { return _WBSLevel9; }
			set { _WBSLevel9 = value; }
		}

		private string _CorridorId;
		public string CorridorId
		{
			get { return _CorridorId; }
			set { _CorridorId = value; }
		}

		private string _UniueTagNo;
		public string UniqueTagNo
		{
			get { return _UniueTagNo; }
			set { _UniueTagNo = value; }
		}

		private string _Feature;
		public string Feature
		{
			get { return _Feature; }
			set { _Feature = value; }
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

		private string _Size;
		public string Size
		{
			get { return _Size; }
			set { _Size = value; }
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

		private string _UOM;
		public string UOM
		{
			get { return _UOM; }
			set { _UOM = value; }
		}

		private string _Total;
		public string Total
		{
			get { return _Total; }
			set { _Total = value; }
		}

		private string _Type;
		public string Type
		{
			get { return _Type; }
			set { _Type = value; }
		}
	}
}
