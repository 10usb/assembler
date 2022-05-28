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
                if (!document.AddReference(line.Label))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line); break;
                    case "macro": StartMacro(line); return;
                }
            }

            if (line.Assignment != null) {
                scope.Set(line.Assignment, line.Arguments[0].GetValue(scope));
            }

            Console.WriteLine(line);
        }

        private void SetOrigin(AssemblyLine line) {
            if (line.Arguments == null || line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for org", line.LineNumber);

            if (!(line.Arguments[0].GetValue(null) is Number number))
                throw new AssemblerException("Can't resolve origin value", line.LineNumber);

            document.SetOrigin(number.Value);
        }
        private void PutByte(AssemblyLine line) {
            document.PutByte(line.Arguments.Select(argument => {
                IConstant constant = argument.GetValue(scope);
                if (constant != null)
                    return constant;

                return argument.Resolve(scope);
            }).ToArray());
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
