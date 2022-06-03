﻿using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class MacroInterpreter : IInterpreter {
        private readonly Macro macro;
        private readonly Document document;
        private readonly string prefix;
        private readonly LocalScope scope;

        public MacroInterpreter(Macro macro, Document document, string prefix) {
            this.macro = macro;
            this.document = document;
            this.prefix = prefix;
            scope = new LocalScope(document);
        }

        public IValue Translate(IValue source) {
            return source.Derive(value => {
                if (value is Label symbol && macro.HasLabel(symbol.Name))
                    return new Label(prefix + symbol.Name);

                return null;
            });
        }

        public void SetParameters(IValue[] arguments) {
            int index = 0;
            foreach (string label in macro.Parameters)
                scope.Set(ScopeType.Local, label, arguments[index++]);
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null) {
                if (!document.AddReference(prefix + line.Label))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line); break;
                    default: ProcessInstruction(line); return;
                }
            }

            if (line.Assignment != null) {
                scope.Set(line.Scope, line.Assignment, Translate(line.Arguments[0]).Resolve(scope));
            }

            if (line.Section != null) {
                ProcessSection(line);
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

                return Translate(argument).Resolve(scope);
            }).ToArray());
        }

        private void ProcessInstruction(AssemblyLine line) {
            Macro macro = this.macro.Find(line.Instruction, line.Arguments.Length);
            if (macro == null)
                throw new AssemblerException("Unknown instruction '{0}'", line.LineNumber, line.Instruction);

            MacroTranscriber transcriber = new MacroTranscriber(macro, document, document.Position);
            transcriber.Transcribe(line.Arguments.Select(arg => Translate(arg).Resolve(scope)).ToArray());
        }

        private void ProcessSection(AssemblyLine line) {
            ConditionalSectionTranscriber transcriber = new ConditionalSectionTranscriber(scope, this);
            transcriber.Transcribe(line.Section);
        }
    }
}
