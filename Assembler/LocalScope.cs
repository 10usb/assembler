using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class LocalScope : IScope {
        private readonly Document document;
        private readonly VariableScope variables;

        public LocalScope(Document document) {
            this.document = document;
            variables = new VariableScope();
        }

        /// <summary>
        /// It will fetch first local, then constant and then global
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IValue Get(string name) {
            IValue value = document.Types.Get(name);
            if (value != null)
                return value;

            value = variables.Get(name);
            if (value != null)
                return value;

            value = document.Constants.Get(name);
            if (value != null)
                return value;

            return document.Globals.Get(name);
        }

        /// <summary>
        /// Will set local always allowing to hide globals. Then it will set globals
        /// </summary>
        /// <param name="scopeType"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(ScopeType scopeType, string name, IValue value) {
            if (scopeType == ScopeType.Local) {
                variables.Set(name, value);
            } else {
                if (document.Constants.Get(name) != null)
                    throw new Exception(string.Format("Can't set '{0}' a constant value with this name already exists", name));

                switch (scopeType) {
                    case ScopeType.Constant: document.Constants.Set(name, value); break;
                    case ScopeType.Global: document.Globals.Set(name, value); break;
                    default: throw new Exception(string.Format("Can't set '{0}' of a unknown scope", name));
                }
            }
        }
    }
}
