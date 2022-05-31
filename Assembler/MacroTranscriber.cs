using Assembler.Interperters;
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

        public void Transcribe(IValue[] arguments) {
            MacroInterperter interperter = new MacroInterperter(macro, document, prefix);

            interperter.SetParameters(arguments);

            foreach (AssemblyLine line in macro)
                interperter.ProcessLine(line);
        }
    }
}
