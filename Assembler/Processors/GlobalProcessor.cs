using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Processors {
    public class GlobalProcessor : IProcessor {
        private readonly Processor processor;
        private readonly Document document;
        private readonly VariableScope scope;

        public GlobalProcessor(Processor processor, Document document) {
            this.processor = processor;
            this.document = document;
            scope = new VariableScope();
        }

        public void ProcessLine(AssemblyLine line) {
            if (line.Label != null) {
                //if (!document.Add(line.Label))
                //    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    //case "org": SetOrigin(line); break;
                    //case "db": PutByte(line, variableScope); break;
                    case "macro": StartMacro(line); return;
                }
            }

            if (line.Assignment != null) {
                //variableScope.Set(line.Assignment, line.Arguments[0].GetValue(variableScope));
            }

            Console.WriteLine(line);
        }

        private void StartMacro(AssemblyLine line) {
            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", line.LineNumber);

            // The only arguments of a macro must of of symbol type
            Symbol[] lineArguments = line.Arguments.Select(arg => arg as Symbol).ToArray();

            // We need a string array of strings of the remaining
            string[] arguments = lineArguments.Skip(1).Select(arg => arg.Name).ToArray();

            Macro macro = document.AddMacro(lineArguments[0].Name, arguments);

            MacroProcessor macroProcessor = new MacroProcessor(macro, processor);
            processor.PushState(macroProcessor);
        }
    }
}
