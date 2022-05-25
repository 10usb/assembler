using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class SymbolTable {
        public List<Entry> entries;

        public SymbolTable() {
            entries = new List<Entry>();
        }

        public void Add(long offset, IValue reference) {
            entries.Add(new Entry {
                Offset = offset,
                Reference = reference
            });
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();

            foreach (Entry entry in entries) {
                builder.AppendFormat("{0:X8}: {1}\n", entry.Offset, entry.Reference);
            }

            return builder.ToString();
        }

        public class Entry {
            public long Offset { get; set; }
            public IValue Reference { get; set; }
        }

    }
}
