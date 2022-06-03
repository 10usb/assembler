using Assembler.Values;
using System.Text;

namespace Assembler {
    /// <summary>
    /// An assembly line is a carrier for all the information that can be stored
    /// into a single line of assembly. It will usually be provided by the parser
    /// to the interpreters.
    /// </summary>
    public class AssemblyLine {
        private readonly int lineNr;
        private string label;
        private string assignment;
        private string modifier;
        private string instruction;
        private string comments;
        private IValue[] arguments = new IValue[0];

        /// <summary>
        /// TODO add source file reference
        /// </summary>
        /// <param name="lineNr"></param>
        public AssemblyLine(int lineNr) {
            this.lineNr = lineNr;
        }

        /// <summary>
        /// The line number in the source file this assembly line was found
        /// </summary>
        public int LineNumber { get => lineNr; }

        /// <summary>
        /// The label name is defined
        /// </summary>
        public string Label {
            get { return label; }
            set { label = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        /// <summary>
        /// The target scope when assigning a variable
        /// </summary>
        public ScopeType Scope { get; set; } = ScopeType.None;

        /// <summary>
        /// The name of the varriable to assign
        /// </summary>
        public string Assignment {
            get { return assignment; }
            set { assignment = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        /// <summary>
        /// Modifier prefixing an instruction
        /// </summary>
        public string Modifier {
            get { return modifier; }
            set { modifier = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        /// <summary>
        /// The name of the instruction
        /// </summary>
        public string Instruction {
            get { return instruction; }
            set { instruction = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        /// <summary>
        /// The argument for an assignment of arguments for a instrction
        /// </summary>
        public IValue[] Arguments {
            get => arguments;
            set {
                if (value != null) {
                    arguments = value;
                } else {
                    arguments = new IValue[0];
                }
            }
        }

        /// <summary>
        /// Does this instrction have a template block after it
        /// </summary>
        public bool IsBlockOpen { get; set; }

        /// <summary>
        /// Is this the end of a template block
        /// </summary>
        public bool IsBlockClose { get; set; }

        /// <summary>
        /// Comment on the line including the comment start token
        /// </summary>
        public string Comments {
            get { return comments; }
            set { comments = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        /// <summary>
        /// The start of a conditional section
        /// </summary>
        public ConditionalSection Section { get; set; }

        /// <summary>
        /// An assembly-ish code representation of an assembly line
        /// </summary>
        /// <returns></returns>
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

            if (Arguments != null) {
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

            if (Section != null) {
                builder.Append(Section);
            }

            return builder.ToString();
        }
    }
}