using Assembler.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// A section is a piece of template code that is part of some other
    /// template, a macro for example.
    /// </summary>
    public class ConditionalSection : IEnumerable<AssemblyLine> {
        private readonly IValue condition;
        private readonly List<AssemblyLine> lines;
        private ConditionalSection next;

        public ConditionalSection Next {
            get => next;
            set {
                if (condition == null)
                    throw new Exception("Can't chain if this is already the last section");

                next = value;
            }
        }

        public ConditionalSection(IValue condition) {
            this.condition = condition;
            lines = new List<AssemblyLine>();
        }

        public void Add(AssemblyLine line) {
            lines.Add(line);
        }

        public IEnumerator<AssemblyLine> GetEnumerator() {
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("section {0} {{\n", condition);
            foreach (AssemblyLine line in lines)
                builder.AppendLine(line.ToString());

            if (next != null) {
                builder.AppendFormat("}} {0}", next);
            } else {

                builder.AppendLine("}");
            }
            return builder.ToString();
        }
    }
}
