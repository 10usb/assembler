using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class MacroDefinitionInterpreter : IInterpreter {
        private readonly Macro macro;
        private readonly Router processor;
        private readonly List<string> labels;

        public MacroDefinitionInterpreter(Macro macro, Router processor) {
            this.macro = macro;
            this.processor = processor;
            labels = new List<string>();
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null) {
                labels.Add(line.Label);
            }

            if (line.IsBlockClose) {
                macro.SetLabels(labels.ToArray());
                processor.PopState();
            } else if (line.Instruction == "if") {
                throw new AssemblerException("Not yet supported", line.LineNumber);
            } else {
                macro.Add(line);
            }
        }
    }
}
