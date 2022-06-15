using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// This class helpt to build a trail that can be traced to it's origin of error
    /// </summary>
    public class Trace : ISourcePointer {
        /// <summary>
        /// An empty trace object from who all traces start
        /// </summary>
        public static Trace Empty => new Trace();

        /// <summary>
        /// Internal pointer
        /// </summary>
        ISourcePointer reference;

        /// <summary>
        /// The tracing point that came before this one
        /// </summary>
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

        /// <summary>
        /// Prints the list of all traces
        /// </summary>
        /// <returns></returns>
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
