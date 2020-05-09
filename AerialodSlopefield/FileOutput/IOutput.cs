using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield.FileOutput
{
    interface IOutput: IDisposable
    {
        void WriteLine(string line);
    }
}
