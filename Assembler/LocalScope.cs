using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    class LocalScope : IScope {
        private readonly Document document;
        private readonly VariableScope variables;

        public LocalScope(Document document) {
            this.document = document;
            variables = new VariableScope();
        }


        public IValue Get(string name) {
            IValue value = variables.Get(name);
            if (value != null)
                return value;

            return document.Get(name);
        }

        public void Set(ScopeType scopeType, string name, IValue value) {
            if (scopeType == ScopeType.Local) {
                variables.Set(name, value);
            } else {
                document.Set(scopeType, name, value);
            }
        }
    }
}
