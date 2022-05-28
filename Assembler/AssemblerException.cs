using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class AssemblerException : Exception {
        public int LineNr { get; }

        public AssemblerException(string message, int lineNr, params object[] arguments)
            : base(string.Format(message, arguments)) {
            LineNr = lineNr;
        }
    }
}
