﻿using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class ReferenceTable : IScope {
        private readonly Dictionary<string, long> dictionary;

        public ReferenceTable() {
            dictionary = new Dictionary<string, long>();
        }

        public bool Add(string label, long position) {
            if (dictionary.ContainsKey(label))
                return false;

            dictionary.Add(label, position);
            return true;
        }

        public IValue Get(string name) {
            if (!dictionary.ContainsKey(name))
                return null;

            return new Number(dictionary[name], NumberFormat.Hex);
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, long> entry in dictionary) {
                builder.AppendFormat("{1:X8}: {0}\n", entry.Key, entry.Value);
            }

            return builder.ToString();
        }
    }
}
