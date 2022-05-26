using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class VariableScope : IScope {
        private Dictionary<string, IConstant> table;

        public VariableScope() {
            table = new Dictionary<string, IConstant>();
        }

        public IConstant Get(string name) {
            if (table.ContainsKey(name))
                return table[name];

            return null;
        }

        public void Set(string name, IConstant constant) {
            table[name] = constant;
        }
    }
}
