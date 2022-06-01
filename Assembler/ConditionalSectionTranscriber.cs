using Assembler.Interpreters;
using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class ConditionalSectionTranscriber {
        private readonly string prefix;
        private readonly VariableScope scope;
        private readonly Macro macro;
        private readonly IInterpreter interpreter;

        public ConditionalSectionTranscriber(string prefix, VariableScope scope, Macro macro, IInterpreter interpreter) {
            this.prefix = prefix;
            this.scope = scope;
            this.macro = macro;
            this.interpreter = interpreter;
        }

        public void Transcribe(ConditionalSection section) {
            bool success = true;

            if (section.Condition != null) {
                IConstant result = section.Condition.Resolve(scope).Derive(value => {
                    if (value is Label symbol && macro.HasLabel(symbol.Name))
                        return new Label(prefix + symbol.Name);

                    return null;
                }).GetValue(scope);

                if (!(result is Number number))
                    throw new Exception("Failed to resolve condition");

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
