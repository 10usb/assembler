using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// A template instruction containing a template of instructions
    /// </summary>
    public class Macro {
        private readonly Macro parent;
        private readonly string name;
        private readonly string[] arguments;
        private readonly List<AssemblyLine> lines;

        /// <summary>
        /// Constructs an empty macro
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        public Macro(Macro parent, string name, string[] arguments) {
            this.parent = parent;
            this.name = name;
            this.arguments = arguments;
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
            if (this.name == name && arguments.Length == argumentCount)
                return this;

            if (parent != null)
                return parent.Find(name, argumentCount);

            return null;
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
            foreach (string argument in arguments)
                builder.AppendFormat(" {0}", argument);

            builder.AppendLine(" {");
            foreach (AssemblyLine line in lines)
                builder.AppendLine(line.ToString());

            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
