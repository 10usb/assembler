using Assembler.Interpreters;
using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class MacroTranscriber {
        private readonly Macro macro;
        private readonly Document document;
        private readonly string prefix;

        public MacroTranscriber(Macro macro, Document document, long offset) {
            this.macro = macro;
            this.document = document;
            prefix = string.Format("${0:X4}_", offset);
        }

        public void Transcribe(string modifier, IValue[] arguments, Trace trace) {
            MacroInterpreter interpreter = new MacroInterpreter(macro, document, trace, prefix);

            interpreter.SetModifier(modifier);

            interpreter.SetParameters(arguments);

            foreach (AssemblyLine line in macro)
                interpreter.Process(line);
        }
    }
}
