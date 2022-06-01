using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class IfElseDefinitionInterpreter : IInterpreter {
        private ConditionalSection section;
        private Router router;

        public IfElseDefinitionInterpreter(ConditionalSection section, Router router) {
            this.section = section;
            this.router = router;
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null)
                throw new AssemblerException("Can't define a label from within a if/elseif/else", line.LineNumber);

            if (line.Modifier == "}") {
                StartElse(line);
            } else if (line.IsBlockClose) {
                router.PopState();
            } else if (line.Instruction == "if") {
                StartIfElse(line);
            } else {
                section.Add(line);
            }
        }

        private void StartElse(AssemblyLine line) {
            if(line.Instruction != "else" && line.Instruction != "elseif")
                throw new AssemblerException("Unexpected token '{0}' after }", line.LineNumber, line.Instruction);

            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid else/elseif", line.LineNumber);

            if (line.Instruction == "else" && line.Arguments.Length != 0)
                throw new AssemblerException("Unexpected else syntax", line.LineNumber);

            if (line.Instruction == "elseif" && line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected elseif syntax", line.LineNumber);

            section.Next = new ConditionalSection(line.Arguments.Length > 0 ? line.Arguments[0] : null);
            section = section.Next;
        }

        private void StartIfElse(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("An if requires one argument", line.LineNumber);

            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", line.LineNumber);

            ConditionalSection section = new ConditionalSection(line.Arguments[0]);

            section.Add(new AssemblyLine(line.LineNumber) {
                Section = section
            });

            IfElseDefinitionInterpreter interpreter = new IfElseDefinitionInterpreter(section, router);
            router.PushState(interpreter);
        }
    }
}
