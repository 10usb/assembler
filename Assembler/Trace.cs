using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class Trace : ISourcePointer {
        ISourcePointer reference;

        public Trace Previous { get; private set; }

        public FileInfo Source => reference.Source;

        public int LineNumber => reference.LineNumber;

        /// <summary>
        /// Creates an empty trace point
        /// </summary>
        public Trace() {
        }

        /// <summary>
        /// Create a new layer in the stack trace
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public Trace Create(ISourcePointer reference) {
            if (reference == null)
                throw new BadProgrammerException("Can't extend a trace with null");

            return new Trace {
                Previous = this.reference != null ? this : null,
                reference = reference
            };
        }
    }
}
