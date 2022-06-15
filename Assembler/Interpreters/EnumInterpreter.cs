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
        private readonly Trace trace;

        public EnumInterpreter(ClassType classType, LocalScope scope, Router router, Trace trace) {
            this.classType = classType;
            this.scope = scope;
            this.router = router;
            this.trace = trace;
        }

        public IValue Translate(IValue value) {
            return value;
        }

        public void Process(AssemblyLine line) {
            if (line.Assignment != null) {
                if (scope.Get(line.Assignment) != null)
                    throw new AssemblerException("Can't redefine '{0}'", trace.Create(line), line.Assignment);

                scope.Set(ScopeType.Constant, line.Assignment, line.Arguments[0].Resolve(scope).Cast(classType));
            } else if (line.IsBlockClose) {
                router.PopState();
            } else if(!line.IsEmptyOrComment) {
                throw new AssemblerException("Unexpected token inside enum '{0}'", trace.Create(line), classType);
            }
        }
    }
}
