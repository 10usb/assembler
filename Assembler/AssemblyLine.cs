using Assembler.Values;
using System.Text;

namespace Assembler {
    public class AssemblyLine {
        private readonly int lineNr;
        private string label;
        private string scope;
        private string assignment;
        private string modifier;
        private string instruction;
        private string comments;

        public AssemblyLine(int lineNr) {
            this.lineNr = lineNr;
        }

        public int LineNumber { get => lineNr; }

        public string Label {
            get { return label; }
            set { label = string.IsNullOrWhiteSpace(value) ? null : value; }
        }
        public string Assignment {
            get { return assignment; }
            set { assignment = string.IsNullOrWhiteSpace(value) ? null : value; }
        }
        public string Modifier {
            get { return modifier; }
            set { modifier = string.IsNullOrWhiteSpace(value) ? null : value; }
        }
        public string Instruction {
            get { return instruction; }
            set { instruction = string.IsNullOrWhiteSpace(value) ? null : value; }
        }
        public IValue[] Arguments { get; set; }
        public bool IsBlockOpen { get; set; }
        public bool IsBlockClose { get; set; }
        public string Comments {
            get { return comments; }
            set { comments = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string Scope {
            get { return scope; }
            set { scope = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            if (Label != null)
                builder.AppendFormat("{0}:", Label);

            if (Assignment != null) {
                builder.AppendFormat("\t{0} =", Assignment);
            }

            if (Instruction != null) {
                if (Modifier != null) {
                    builder.AppendFormat("{0}\t{1}", Modifier, Instruction);
                } else {
                    builder.AppendFormat("\t{0}", Instruction);
                }
            }

            if (Arguments!=null) {
                foreach (IValue argument in Arguments) {
                    builder.AppendFormat(" {0}", argument);
                }
            }

            if (IsBlockOpen) {
                builder.Append(" {");
            }

            if (IsBlockClose) {
                builder.Append("}");
            }

            if (Comments != null) {
                builder.Append(Comments);
            }

            return builder.ToString();
        }
    }
}