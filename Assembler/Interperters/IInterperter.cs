using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interperters {
    public interface IInterperter {
        void ProcessLine(AssemblyLine line);
    }
}
