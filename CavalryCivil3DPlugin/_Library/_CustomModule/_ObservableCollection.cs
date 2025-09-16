using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CavalryCivil3DPlugin._Library._CustomModule
{
    public class _ObservableCollection<T> : ObservableCollection<T>
    {

        public void Reset(IEnumerable<T> _objects)
        {
            this.Clear();
            foreach (var _object in _objects) this.Add(_object);    
        }
    }
}
