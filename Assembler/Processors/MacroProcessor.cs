using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Processors {
    public class MacroProcessor : IProcessor {
        private Macro macro;
        private Processor processor;

        public MacroProcessor(Macro macro, Processor processor) {
            this.macro = macro;
            this.processor = processor;
        }

        public void ProcessLine(AssemblyLine line) {
            if (line.IsBlockClose) {
                processor.PopState();
            } else {
                macro.Add(line);
            }
        }
    }
}
