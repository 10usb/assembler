using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public abstract class BaseInterpreter : IInterpreter {
        protected Document document;
        protected LocalScope scope;

        protected abstract ScopeType DefaultScope { get; }

        public virtual IValue Translate(IValue value) {
            return value;
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null) {
                Label label = Translate(new Label(line.Label)) as Label;

                if (!document.AddReference(label.Name))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line); break;
                    case "throw": throw Throw(line);
                    case "macro": StartMacro(line); return;
                    case "enum": StartEnum(line); return;
                    case "include": StartInclude(line); return;
                    default: ProcessInstruction(line); break;
                }
            }

            if (line.Assignment != null) {
                ScopeType scopeType = line.Scope != ScopeType.None ? line.Scope : DefaultScope;
                scope.Set(scopeType, line.Assignment, Translate(line.Arguments[0]).Resolve(scope));
            }

            if (line.Section != null) {
                ProcessSection(line);
            }
        }


        private void SetOrigin(AssemblyLine line) {
            if (line.Arguments == null || line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for org", line.LineNumber);

            if (!(line.Arguments[0].GetValue(null) is Number number))
                throw new AssemblerException("Can't resolve origin value", line.LineNumber);

            document.SetOrigin(number.Value);
        }

        private void PutByte(AssemblyLine line) {
            document.PutByte(line.Arguments.Select(argument => {
                IConstant constant = argument.GetValue(scope);
                if (constant != null)
                    return constant;

                return Translate(argument).Resolve(scope);
            }).ToArray());
        }

        private Exception Throw(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for throw", line.LineNumber);

            IConstant constant = Translate(line.Arguments[0]).GetValue(scope);
            if (!(constant is Text message))
                throw new AssemblerException("Unexpected argument type for throw", line.LineNumber);

            return new AssemblerException(message.Value, line.LineNumber);
        }

        protected abstract void StartMacro(AssemblyLine line);

        protected abstract void StartEnum(AssemblyLine line);


        protected abstract void ProcessInstruction(AssemblyLine line);

        protected abstract void ProcessSection(AssemblyLine line);

        protected abstract void StartInclude(AssemblyLine line);
    }
}
