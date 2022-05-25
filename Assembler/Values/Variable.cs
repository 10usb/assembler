using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Values {
    public class Variable : IValue {
        public ValueType Type => throw new NotImplementedException();

        public bool GetValue(IScope scope, out long value) {
            throw new NotImplementedException();
        }

        public IValue Resolve(IScope scope) {
            throw new NotImplementedException();
        }
    }
}
