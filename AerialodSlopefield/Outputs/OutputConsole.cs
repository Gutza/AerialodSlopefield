using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield.Outputs
{
    class OutputConsole : IOutput
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
