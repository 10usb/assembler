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
            current = new GlobalProcessor(this, this.document);
        }

        public void ProcessLine(AssemblyLine line) {
            current.ProcessLine(line);

            //Console.WriteLine("Label      : {0}", line.Label);
            //Console.WriteLine("Scope      : {0}", line.Scope);
            //Console.WriteLine("Assignment : {0}", line.Assignment);
            //Console.WriteLine("Modifier   : {0}", line.Modifier);
            //Console.WriteLine("Instruction: {0}", line.Instruction);
            //Console.WriteLine("Arguments  : {0}", line.Arguments);
            //Console.WriteLine("Block open : {0}", line.IsBlockOpen);
            //Console.WriteLine("Block close: {0}", line.IsBlockClose);
            //Console.WriteLine("Comments   : {0}", line.Comments);
            //Console.WriteLine("-----------------------------------------------");
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
