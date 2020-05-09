using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield.ConsoleOutput
{
    interface IConsoleOutput
    {
        void WriteLine(string line);
        void Write(string v);
    }
}
