﻿using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class VariableScope : IScope {
        private readonly Dictionary<string, IValue> table;

        public VariableScope() {
            table = new Dictionary<string, IValue>();
        }

        public IValue Get(string name) {
            if (table.ContainsKey(name))
                return table[name];

            return null;
        }

        public void Set(string name, IValue constant) {
            table[name] = constant;
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, IValue> entry in table) {
                builder.AppendFormat("{0} = {1}\n", entry.Key, entry.Value);
            }

            return builder.ToString();
        }
    }
}
