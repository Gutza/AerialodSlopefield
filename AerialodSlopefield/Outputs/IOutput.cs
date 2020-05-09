using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield.Outputs
{
    interface IOutput: IDisposable
    {
        void WriteLine(string line);
    }
}
