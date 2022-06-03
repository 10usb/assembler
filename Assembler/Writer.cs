using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// The writer is responsible for actually writing the bytes to an output file.
    /// And while doing so it keeps track of the current bytes written and the virtual
    /// offset.
    /// </summary>
    public class Writer : IDisposable {
        private readonly Stream stream;
        private long origin;
        private long offset;

        /// <summary>
        /// The current origin the offset is pointing to
        /// </summary>
        public long Origin {
            get { return origin; }
            set {
                origin = value;
                offset = 0;
            }
        }

        /// <summary>
        /// The current offset to the origin the next bytes will be written
        /// </summary>
        public long Offset {
            get {
                return offset;
            }
        }

        /// <summary>
        /// The current virtual address
        /// </summary>
        public long Position {
            get {
                return origin + offset;
            }
        }

        /// <summary>
        /// The current number of written bytes
        /// </summary>
        public long FileOffset {
            get {
                return stream.Position;
            }
        }

        /// <summary>
        /// Constructs a writer pointing to an output stream
        /// </summary>
        /// <param name="stream"></param>
        public Writer(Stream stream) {
            this.stream = stream;
        }

        /// <summary>
        /// Seet to a fixed offset into the output file for setting a byte value
        /// </summary>
        /// <param name="offset"></param>
        internal void Seek(long offset) {
            stream.Position = offset;
        }

        /// <summary>
        /// Set a byte value at the current location in the file
        /// </summary>
        /// <param name="value"></param>
        internal void SetByte(long value) {
            byte[] data = new byte[] { (byte)value };
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a string value as UTF-8 and incrementing the offset
        /// </summary>
        /// <param name="text"></param>
        internal void WriteString(string text) {
            Write(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Write a single byte value and incrementing the offset
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(long value) {
            Write(new byte[] { (byte)value });
        }

        /// <summary>
        /// Writes an byte array and incrementing the offset
        /// </summary>
        /// <param name="data"></param>
        private void Write(byte[] data) {
            offset += data.Length;
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Flushes any buffered data to the file
        /// </summary>
        public void Flush() {
            stream.Flush();
        }

        /// <summary>
        /// Makes sure the last changes to the file are flushed
        /// and then the stream is closed
        /// </summary>
        public void Dispose() {
            stream.Flush();
            stream.Close();
        }
    }
}
