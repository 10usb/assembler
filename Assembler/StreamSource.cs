using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class StreamSource : ISource, IDisposable {
        private readonly string reference;
        private readonly Stream stream;
        private readonly bool owner;

        public string Reference => reference;

        public StreamSource(string reference, Stream stream, bool owner) {
            this.reference = reference;
            this.stream = stream;
            this.owner = owner;
        }

        /// <summary>
        /// TODO Add a wrapper around the returned StreamReader, so we can catch
        /// the call to dispose. So we can track to see is the stream is activly
        /// used. And if so throw an exception that it can only one at a time.
        /// </summary>
        /// <returns></returns>
        public TextReader Open() {
            // internal StreamReader.DefaultBufferSize = 1024
            return new StreamReader(stream, Encoding.UTF8, true, 1024, true);
        }

        public void Dispose() {
            if (owner)
                stream.Dispose();
        }
    }
}
