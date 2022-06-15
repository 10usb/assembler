using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// A reference to a point in a source file
    /// </summary>
    public interface ISourcePointer {
        /// <summary>
        /// The the source file this line is from
        /// </summary>
        FileInfo Source { get;  }

        /// <summary>
        /// The line number in the source file this assembly line was found
        /// </summary>
        int LineNumber { get; }
    }
}
