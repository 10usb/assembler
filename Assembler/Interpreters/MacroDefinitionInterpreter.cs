using Assembler.Values;
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

        public IValue Translate(IValue value) {
            throw new NotImplementedException();
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null) {
                labels.Add(line.Label);
            }

            if (line.IsBlockClose) {
                macro.SetLabels(labels.ToArray());
                processor.PopState();
            } else if (line.Instruction == "if") {
                StartIfElse(line);
            } else {
                macro.Add(line);
            }
        }

        private void StartIfElse(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("An if requires one argument", line.LineNumber);

            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", line.LineNumber);

            ConditionalSection section = new ConditionalSection(line.Arguments[0]);

            macro.Add(new AssemblyLine(line.LineNumber) {
                Section = section
            });

            IfElseDefinitionInterpreter interpreter = new IfElseDefinitionInterpreter(section, processor);
            processor.PushState(interpreter);
        }
    }
}
