using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AerialodSlopefield
{
    class RenderFunction
    {
        /// <summary>
        /// Change this function in order to generate a different slope field or function graph.
        /// </summary>
        /// <param name="x">The x coordinate in the function domain</param>
        /// <param name="y">The y coordinate in the function domain</param>
        /// <param name="xindex">The x coordinate in the voxel domain</param>
        /// <param name="yindex">The y coordinate in the voxel domain</param>
        /// <returns>The value of your function at (x,y)</returns>
        internal static double MyFunction(double x, double y, int xindex, int yindex)
        {
            int computeEvery = 3;
            bool doCompute = (xindex % computeEvery == 1) && (yindex % computeEvery == 1);

            // Comment the next line, see what you get.
            doCompute = true;
            // Then change the value for computeEvery above, see what you get then.

            if (doCompute)
            {
                return x / y;
            }
            else
            {
                return double.NaN;
            }
        }
    }
}
