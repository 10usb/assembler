using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class GlobalInterpreter : BaseInterpreter {
        private readonly Router router;

        protected override ScopeType DefaultScope => ScopeType.Global;

        public GlobalInterpreter(Router router, Document document) {
            this.router = router;
            this.document = document;
            scope = new LocalScope(document);
        }

        protected override void StartMacro(AssemblyLine line) {
            if (!line.IsBlockOpen)
                throw new AssemblerException("Invalid macro", line.LineNumber);

            // The only arguments of a macro must of of symbol type
            Symbol[] lineArguments = line.Arguments.Select(arg => arg as Symbol).ToArray();

            // We need a string array of strings of the remaining
            string[] arguments = lineArguments.Skip(1).Select(arg => arg.Name).ToArray();

            Macro macro = document.AddMacro(lineArguments[0].Name, arguments);

            MacroDefinitionInterpreter macroProcessor = new MacroDefinitionInterpreter(macro, router);
            router.PushState(macroProcessor);
        }

        protected override void StartEnum(AssemblyLine line) {
            string name = (line.Arguments[0] as Symbol).Name;

            if (document.Types.Get(name) != null)
                throw new AssemblerException("Type with name '{0}' already exists", line.LineNumber, name);

            ClassType classType = new ClassType(name);

            document.Types.Set(name, classType);

            EnumInterpreter interpreter = new EnumInterpreter(classType, scope, router);
            router.PushState(interpreter);
        }

        protected override void ProcessInstruction(AssemblyLine line) {
            Macro macro = document.GetMacro(line.Instruction, line.Arguments.Length);
            if (macro == null)
                throw new AssemblerException("Unknown instruction '{0}'", line.LineNumber, line.Instruction);

            MacroTranscriber transcriber = new MacroTranscriber(macro, document, document.Position);
            transcriber.Transcribe(line.Modifier, line.Arguments.Select(arg => arg.Resolve(scope)).ToArray());
        }

        protected override void ProcessSection(AssemblyLine line) {
            throw new NotImplementedException();
        }

        protected override void StartInclude(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for include", line.LineNumber);

            if (!(line.Arguments[0] is Text text))
                throw new AssemblerException("Argument must be a string", line.LineNumber);

            string path = Path.Combine(line.Source.Directory.FullName, text.Value);

            FileInfo file = new FileInfo(path);

            using (StreamReader reader = file.OpenText()) {
                Parser parser = new Parser(file, router);
                parser.Parse(reader);
            }
        }

        protected override void StartImport(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for include", line.LineNumber);

            if (!(line.Arguments[0] is Text text))
                throw new AssemblerException("Argument must be a string", line.LineNumber);

            string path = Path.Combine(line.Source.Directory.FullName, text.Value);

            FileInfo file = new FileInfo(path);

            using (StreamReader reader = file.OpenText()) {
                Parser parser = new Parser(file, router);

                ImportInterpreter interpreter = new ImportInterpreter(router, document, scope);
                router.PushState(interpreter);
                parser.Parse(reader);
                router.PopState();
            }
        }
    }
}
