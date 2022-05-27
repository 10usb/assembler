using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Values {
    /// <summary>
    /// A constant is a value that can't be resolved any further
    /// </summary>
    public interface IConstant : IValue {
    }
}
