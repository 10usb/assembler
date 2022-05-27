using Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Processors {
    public class Processor : IProcessor {
        private readonly Document document;
        private readonly Stack<IProcessor> states;
        private IProcessor current;

        public Processor(Document document) {
            this.document = document;
            states = new Stack<IProcessor>();
            current = new GlobalProcessor(this, document);
        }

        public void ProcessLine(AssemblyLine line) {
            current.ProcessLine(line);
        }

        public void PushState(IProcessor state) {
            states.Push(current);
            current = state;
        }
        public void PopState() {
            if (states.Count == 0)
                throw new Exception("Corrupt processing state occured");

            current = states.Pop();
        }
    }
}
