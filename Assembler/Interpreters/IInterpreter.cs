using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    /// <summary>
    /// An interpreter can be fed AssemblyLine's to process, and translate
    /// it to the appropriate action to undertale
    /// </summary>
    public interface IInterpreter {
        /// <summary>
        /// Will translate the given line into a action
        /// </summary>
        /// <param name="line"></param>
        void Process(AssemblyLine line);
    }
}
