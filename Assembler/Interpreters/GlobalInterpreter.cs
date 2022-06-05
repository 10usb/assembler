﻿using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class GlobalInterpreter : IInterpreter {
        private readonly Router router;
        private readonly Document document;
        private readonly LocalScope scope;

        public GlobalInterpreter(Router router, Document document) {
            this.router = router;
            this.document = document;
            scope = new LocalScope(document);
        }

        public IValue Translate(IValue value) {
            return value;
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
                    case "throw": throw Throw(line);
                    case "macro": StartMacro(line); return;
                    case "enum": StartEnum(line); return;
                    default: ProcessInstruction(line); break;
                }
            }

            if (line.Assignment != null) {
                scope.Set(line.Scope, line.Assignment, line.Arguments[0].Resolve(scope));
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

        private Exception Throw(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for throw", line.LineNumber);

            IConstant constant = line.Arguments[0].GetValue(scope);
            if (!(constant is Text message))
                throw new AssemblerException("Unexpected argument type for throw", line.LineNumber);

            return new AssemblerException(message.Value, line.LineNumber);
        }

        private void StartMacro(AssemblyLine line) {
            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", line.LineNumber);

            // The only arguments of a macro must of of symbol type
            Symbol[] lineArguments = line.Arguments.Select(arg => arg as Symbol).ToArray();

            // We need a string array of strings of the remaining
            string[] arguments = lineArguments.Skip(1).Select(arg => arg.Name).ToArray();

            Macro macro = document.AddMacro(lineArguments[0].Name, arguments);

            MacroDefinitionInterpreter macroProcessor = new MacroDefinitionInterpreter(macro, router);
            router.PushState(macroProcessor);
        }

        private void StartEnum(AssemblyLine line) {
            string name = (line.Arguments[0] as Symbol).Name;

            if (document.Types.Get(name) != null)
                throw new AssemblerException("Type with name '{0}' already exists", line.LineNumber, name);

            ClassType classType = new ClassType(name);

            document.Types.Set(name, classType);

            EnumInterpreter interpreter = new EnumInterpreter(classType, scope, router);
            router.PushState(interpreter);

        }

        private void ProcessInstruction(AssemblyLine line) {
            Macro macro = document.GetMacro(line.Instruction, line.Arguments.Length);
            if (macro == null)
                throw new AssemblerException("Unknown instruction '{0}'", line.LineNumber, line.Instruction);

            MacroTranscriber transcriber = new MacroTranscriber(macro, document, document.Position);
            transcriber.Transcribe(line.Modifier, line.Arguments.Select(arg => arg.Resolve(scope)).ToArray());
        }
    }
}
