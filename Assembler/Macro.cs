using Assembler.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// A template instruction containing a template of instructions
    /// </summary>
    public class Macro : IEnumerable<AssemblyLine> {
        private readonly Macro parent;
        private readonly string name;
        private readonly string[] parameters;
        private readonly List<AssemblyLine> lines;

        public string[] Parameters => parameters;

        /// <summary>
        /// Constructs an empty macro
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public Macro(Macro parent, string name, string[] parameters) {
            this.parent = parent;
            this.name = name;
            this.parameters = parameters;
            lines = new List<AssemblyLine>();
        }

        /// <summary>
        /// Add a line of assembly to this macro
        /// </summary>
        /// <param name="line"></param>
        public void Add(AssemblyLine line) {
            lines.Add(line);
        }

        /// <summary>
        /// Finds a matching macro
        /// </summary>
        /// <param name="name"></param>
        /// <param name="argumentCount"></param>
        /// <returns></returns>
        public Macro Find(string name, int argumentCount) {
            if (this.name == name && parameters.Length == argumentCount)
                return this;

            if (parent != null)
                return parent.Find(name, argumentCount);

            return null;
        }

        public IEnumerator<AssemblyLine> GetEnumerator() {
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public string ToString(bool table) {
            StringBuilder builder = new StringBuilder();
            if (table && parent != null) {
                builder.AppendLine(parent.ToString(true));
            }

            builder.AppendLine(ToString());
            return builder.ToString();
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("macro {0}", name);
            foreach (string argument in parameters)
                builder.AppendFormat(" {0}", argument);

            builder.AppendLine(" {");
            foreach (AssemblyLine line in lines)
                builder.AppendLine(line.ToString());

            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
