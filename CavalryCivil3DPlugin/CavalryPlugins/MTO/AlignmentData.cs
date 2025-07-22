using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class AlignmentData
    {

		private string _CWP;
		public string CWP
		{
			get { return _CWP; }
			set { _CWP = value; }
		}

		private string  _WBSLevel9;
		public string  WBSLevel9
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

		private string _TrenchDetails;

		public string TrenchDetails
		{
			get { return _TrenchDetails; }
			set { _TrenchDetails = value; }
		}

		private double _Length;
		public double Length
		{
			get { return _Length; }
			set { _Length = value; }
		}

		private int _Index;

		public int Index
		{
			get { return _Index; }
			set { _Index = value; }
		}

	}
}
