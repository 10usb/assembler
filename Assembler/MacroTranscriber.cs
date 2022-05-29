﻿using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class MacroTranscriber : IProcessor {
        private readonly Macro macro;
        private readonly Document document;
        private VariableScope scope;

        public MacroTranscriber(Macro macro, Document document) {
            this.macro = macro;
            this.document = document;

        }

        public void Transcribe(IValue[] arguments) {
            scope = new VariableScope();

            int index = 0;
            foreach (string label in macro.Parameters)
                scope.Set(label, arguments[index++]);

            foreach (AssemblyLine line in macro)
                ProcessLine(line);
        }

        public void ProcessLine(AssemblyLine line) {
            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line); break;
                    ///default: ProcessInstruction(line); return;
                }
            }

            if (line.Assignment != null) {
                //scope.Set(line.Assignment, line.Arguments[0].GetValue(scope));
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
    }
}