using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class Writer : IDisposable {
        private Stream stream;
        private long origin;
        private long offset;

        public long Origin {
            get { return origin; }
            set {
                origin = value;
                offset = 0;
            }
        }

        public long Offset {
            get {
                return offset;
            }
        }

        public long Position {
            get {
                return origin + offset;
            }
        }

        public Writer(Stream stream) {
            this.stream = stream;
        }

        internal void WriteString(string text) {
            Write(Encoding.UTF8.GetBytes(text));
        }

        public void WriteByte(long value) {
            Write(new byte[] { (byte)value });
        }

        private void Write(byte[] data) {
            offset += data.Length;
            stream.Write(data, 0, data.Length);
        }

        public void Dispose() {
            stream.Flush();
            stream.Close();
        }
    }
}
