using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    /// <summary>
    /// The router is the glue that passes the parsed lines to the currently
    /// active interpreter. And keeps track of where to return to when an
    /// interpreter has reached the end.
    /// </summary>
    public class Router : IInterpreter {
        private readonly Document document;
        private readonly Stack<IInterpreter> states;
        private IInterpreter current;

        /// <summary>
        /// Construct a router with a document as it's target
        /// </summary>
        /// <param name="document"></param>
        public Router(Document document) {
            this.document = document;
            states = new Stack<IInterpreter>();
            current = new GlobalInterpreter(this, this.document);
        }

        public IValue Translate(IValue value) {
            throw new NotImplementedException();
        }

        public void Process(AssemblyLine line) {
            current.Process(line);

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

        /// <summary>
        /// Push a new interpreter as a new state for next line to proccess
        /// </summary>
        /// <param name="state"></param>
        public void PushState(IInterpreter state) {
            states.Push(current);
            current = state;
        }

        /// <summary>
        /// Pop last state back as active state
        /// </summary>
        public void PopState() {
            if (states.Count == 0)
                throw new Exception("Corrupt processing state occured");

            current = states.Pop();
        }
    }
}
