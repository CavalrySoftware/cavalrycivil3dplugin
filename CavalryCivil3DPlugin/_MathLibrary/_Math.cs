using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CavalryCivil3DPlugin._MathLibrary
{
    class _Math
    {

        public static double Tangent(double x)
        {
            return Math.Tan(x * (Math.PI / 180));
        }

        public static double ArcTangent(double x)
        {
            return Math.Atan(x) * (180 / Math.PI);
        }
    }
}
