using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield
{
    class RenderFunction
    {
        /// <summary>
        /// Change this function in order to generate a different slope field or function graph.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <returns>The value of your function at (x,y)</returns>
        internal static double MyFunction(double x, double y)
        {
            return x * x / y;
        }
    }
}
