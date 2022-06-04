using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class EnumInterpreter : IInterpreter {
        private readonly ClassType classType;
        private readonly LocalScope scope;
        private readonly Router router;

        public EnumInterpreter(ClassType classType, LocalScope scope, Router router) {
            this.classType = classType;
            this.scope = scope;
            this.router = router;
        }

        public IValue Translate(IValue value) {
            return value;
        }

        public void Process(AssemblyLine line) {
            if (line.Assignment != null) {
                if (scope.Get(line.Assignment) != null)
                    throw new AssemblerException("Can't redefine '{0}'", line.LineNumber, line.Assignment);

                scope.Set(ScopeType.Constant, line.Assignment, line.Arguments[0].Resolve(scope).Cast(classType));
            } else if (line.IsBlockClose) {
                router.PopState();
            }
        }
    }
}
