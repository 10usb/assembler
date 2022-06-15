using Assembler.Values;

namespace Assembler.Interpreters {
    internal class ErrorInterpreter : IInterpreter {
        public IValue Translate(IValue value) {
            throw new BadProgrammerException("Translation of value it not supported in this state");
        }

        public void Process(AssemblyLine line) {
            throw new BadProgrammerException("Can't process a line when there's no state active");
        }
    }
}