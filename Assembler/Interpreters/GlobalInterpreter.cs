using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class GlobalInterpreter : IInterpreter {
        private readonly Router processor;
        private readonly Document document;
        private readonly VariableScope scope;

        public GlobalInterpreter(Router processor, Document document) {
            this.processor = processor;
            this.document = document;
            scope = new VariableScope();
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null) {
                if (!document.AddReference(line.Label))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line); break;
                    case "macro": StartMacro(line); return;
                    default: ProcessInstruction(line); return;
                }
            }

            if (line.Assignment != null) {
                scope.Set(line.Assignment, line.Arguments[0].Resolve(scope));
            }

            Console.WriteLine(line);
        }

        private void SetOrigin(AssemblyLine line) {
            if (line.Arguments.Length != 1)
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

            MacroDefinitionInterpreter macroProcessor = new MacroDefinitionInterpreter(macro, processor);
            processor.PushState(macroProcessor);
        }

        private void ProcessInstruction(AssemblyLine line) {
            Macro macro = document.GetMacro(line.Instruction, line.Arguments.Length);
            if (macro == null)
                throw new AssemblerException("Unknown instruction '{0}'", line.LineNumber, line.Instruction);

            MacroTranscriber transcriber = new MacroTranscriber(macro, document, document.Position);
            transcriber.Transcribe(line.Arguments.Select(arg => arg.Resolve(scope)).ToArray());
        }
    }
}
