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
        private readonly Stack<IInterpreter> states;
        private IInterpreter current;

        /// <summary>
        /// Construct an empty router which has yet to be given a state
        /// </summary>
        public Router() {
            states = new Stack<IInterpreter>();
            current = new ErrorInterpreter();
        }

        public IValue Translate(IValue value) {
            throw new BadProgrammerException("Router is incapable of translating values");
        }

        public void Process(AssemblyLine line) {
            if(!line.IsEmptyOrComment)
                current.Process(line);
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
                throw new BadProgrammerException("Corrupt processing state occured");

            current = states.Pop();
        }
    }
}
