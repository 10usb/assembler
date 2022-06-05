using Assembler.Interpreters;
using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class ConditionalSectionTranscriber {
        private readonly IScope scope;
        private readonly IInterpreter interpreter;

        public ConditionalSectionTranscriber(IScope scope, IInterpreter interpreter) {
            this.scope = scope;
            this.interpreter = interpreter;
        }

        public void Transcribe(ConditionalSection section) {
            bool success = true;

            if (section.Condition != null) {
                IConstant result = interpreter.Translate(section.Condition).GetValue(scope);

                if (!(result is Number number))
                    throw new Exception(string.Format("Failed to resolve condition '{0}'", section.Condition));

                success = number.Value != 0;
            }

            if (success) {
                foreach (AssemblyLine line in section)
                    interpreter.Process(line);
            } else if (section.Next != null) {
                Transcribe(section.Next);
            }
        }
    }
}
