using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// A instance able to supply the parser code to parse
    /// </summary>
    public interface ISource {
        /// <summary>
        /// Reference name used in debugging output
        /// </summary>
        string Reference { get; }

        /// <summary>
        /// Creates text reader to be used by the parser
        /// </summary>
        /// <returns></returns>
        TextReader Open();
    }
}
