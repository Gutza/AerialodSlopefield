using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AerialodSlopefield.Outputs
{
    class FileOutput : IOutput, IDisposable
    {
        private string filename;
        private StreamWriter fp;

        public FileOutput(string filename)
        {
            this.filename = filename;
            fp = new StreamWriter(filename);
        }

        public void WriteLine(string line)
        {
            fp.WriteLine(line);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    fp.Close();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
