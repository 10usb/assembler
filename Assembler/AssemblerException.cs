using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class AssemblerException : Exception {
        public Trace Trace { get; }

        public AssemblerException(string message, Trace trace, params object[] arguments)
            : base(string.Format(message, arguments)) {
            Trace = trace;
        }
    }
}
