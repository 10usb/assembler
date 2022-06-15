using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class IfElseDefinitionInterpreter : IInterpreter {
        private ConditionalSection section;
        private readonly Router router;
        private readonly Trace trace;

        public IfElseDefinitionInterpreter(ConditionalSection section, Router router, Trace trace) {
            this.section = section;
            this.router = router;
            this.trace = trace;
        }

        public IValue Translate(IValue value) {
            throw new NotImplementedException();
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null)
                throw new AssemblerException("Can't define a label from within a if/elseif/else", trace.Create(line));

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
                throw new AssemblerException("Unexpected token '{0}' after }", trace.Create(line), line.Instruction);

            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid else/elseif", trace.Create(line));

            if (line.Instruction == "else" && line.Arguments.Length != 0)
                throw new AssemblerException("Unexpected else syntax", trace.Create(line));

            if (line.Instruction == "elseif" && line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected elseif syntax", trace.Create(line));

            section.Next = new ConditionalSection(line.Arguments.Length > 0 ? line.Arguments[0] : null);
            section = section.Next;
        }

        private void StartIfElse(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("An if requires one argument", trace.Create(line));

            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", trace.Create(line));

            ConditionalSection section = new ConditionalSection(line.Arguments[0]);

            this.section.Add(new AssemblyLine(line.Source, line.LineNumber) {
                Section = section
            });

            IfElseDefinitionInterpreter interpreter = new IfElseDefinitionInterpreter(section, router, trace);
            router.PushState(interpreter);
        }
    }
}
