using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class ImportInterpreter : BaseInterpreter {
        private readonly Router router;

        protected override ScopeType DefaultScope => ScopeType.None;

        public ImportInterpreter(Router router, Document document, Trace trace)
            : base(document, new LocalScope(document), trace) {
            this.router = router;
        }

        protected override void PutByte(AssemblyLine line) {
            throw new AssemblerException("Can't output any bytes from within an import", trace.Create(line));
        }

        protected override void StartMacro(AssemblyLine line) {
            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", trace.Create(line));

            // The only arguments of a macro must of of symbol type
            Symbol[] lineArguments = line.Arguments.Select(arg => arg as Symbol).ToArray();

            // We need a string array of strings of the remaining
            string[] arguments = lineArguments.Skip(1).Select(arg => arg.Name).ToArray();

            Macro macro = document.AddMacro(lineArguments[0].Name, arguments);

            MacroDefinitionInterpreter macroProcessor = new MacroDefinitionInterpreter(macro, router, trace);
            router.PushState(macroProcessor);
        }

        protected override void StartEnum(AssemblyLine line) {
            string name = (line.Arguments[0] as Symbol).Name;

            if (document.Types.Get(name) != null)
                throw new AssemblerException("Type with name '{0}' already exists", trace.Create(line), name);

            ClassType classType = new ClassType(name);

            document.Types.Set(name, classType);

            EnumInterpreter interpreter = new EnumInterpreter(classType, scope, router, trace);
            router.PushState(interpreter);
        }

        protected override void ProcessInstruction(AssemblyLine line) {
            throw new AssemblerException("Can't process any instruction from within an import", trace.Create(line));
        }

        protected override void ProcessSection(AssemblyLine line) {
            throw new NotImplementedException();
        }

        protected override void StartInclude(AssemblyLine line) {
            throw new NotImplementedException();
        }

        protected override void StartImport(AssemblyLine line) {
            throw new NotImplementedException();
        }
    }
}
