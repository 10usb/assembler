using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class Trace : ISourcePointer {
        public static Trace Empty => new Trace();

        ISourcePointer reference;

        public Trace Previous { get; private set; }

        public FileInfo Source => reference.Source;

        public int LineNumber => reference.LineNumber;

        /// <summary>
        /// Creates an empty trace point
        /// </summary>
        private Trace() {
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

        public override string ToString() {
            StringBuilder builder = new StringBuilder();


            builder.AppendFormat(" - {0} on {1}", Source.FullName, LineNumber);

            if (Previous != null) {
                builder.AppendLine();
                builder.Append(Previous.ToString());
            }

            return builder.ToString();
        }
    }
}
