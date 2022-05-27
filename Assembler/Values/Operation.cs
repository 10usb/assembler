using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Values {
    /// <summary>
    /// The operatuins that can be performed in an expression
    /// </summary>
    public enum Operation {
        Add,
        Substract,
        Muliply,
        Divide,
        Modulo,
        And,
        Or,
        Xor,
        ShiftLeft,
        ShiftRight,
        Equal,
        Less,
        Greater,
        NotEqual,
        LessOrEqual,
        GreaterOrEqual,
        Is,
        Cast,
    }
}
