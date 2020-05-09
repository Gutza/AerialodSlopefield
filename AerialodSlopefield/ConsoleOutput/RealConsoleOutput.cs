using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield.ConsoleOutput
{
    class RealConsoleOutput : IConsoleOutput
    {
        public void Write(string v)
        {
            Console.Write(v);
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
