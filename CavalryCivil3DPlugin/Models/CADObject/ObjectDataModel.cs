using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class ObjectDataModel
    {

        #region << PROPERTIES >>
        private List<string> _Fields;
        public List<string> Fields
        {
            get { return _Fields; }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
        }

        public List<string> Keys { get { return _Fields; } }
        #endregion

        private string _TableName;

        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }



        #region << CONSTRUCTOR >>
        public ObjectDataModel(string _tableName, List<string> _fields)
        {
            _Name = $"Object Data: {_tableName}";    
            _Fields = _fields;   
            _TableName = _tableName;
        }
        #endregion  
    }
}
