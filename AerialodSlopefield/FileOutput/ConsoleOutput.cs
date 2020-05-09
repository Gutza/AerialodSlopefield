using System;
using System.Collections.Generic;
using System.Text;

namespace AerialodSlopefield.FileOutput
{
    class ConsoleOutput : IOutput
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Dispose()
        {
            // Nothing to dispose for ConsoleOutput
        }

    }
}
