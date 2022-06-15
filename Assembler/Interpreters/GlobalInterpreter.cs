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

        public GlobalInterpreter(Router router, Document document)
            : base(document, new LocalScope(document), Trace.Empty){
            this.router = router;
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
            Macro macro = document.GetMacro(line.Instruction, line.Arguments.Length);
            if (macro == null)
                throw new AssemblerException("Unknown instruction '{0}'", trace.Create(line), line.Instruction);

            MacroTranscriber transcriber = new MacroTranscriber(macro, document, document.Position);
            transcriber.Transcribe(line.Modifier, line.Arguments.Select(arg => arg.Resolve(scope)).ToArray(), trace.Create(line));
        }

        protected override void ProcessSection(AssemblyLine line) {
            throw new NotImplementedException();
        }

        protected override void StartInclude(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for include", trace.Create(line));

            string path;

            if (line.Arguments[0] is Text text) {
                path = text.Value;
            } else {
                throw new AssemblerException("Argument must be a string", trace.Create(line));
            }


            FileInfo file;
            if (Path.IsPathRooted(path)) {
                file = new FileInfo(path);
            } else {
                file = new FileInfo(Path.Combine(line.Source.Directory.FullName, path));
                if (!file.Exists)
                    file = document.ResolveInclude(path);
            }

            if (file == null || !file.Exists)
                throw new AssemblerException("File {0} not exists", trace.Create(line), path);

            using (Parser parser = new Parser(file, router, trace.Create(line))) {
                parser.Parse();
            }
        }

        protected override void StartImport(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for include", trace.Create(line));

            ;
            FileInfo file;

            if (line.Arguments[0] is Text text) {
                string path = text.Value;

                if (Path.IsPathRooted(path)) {
                    file = new FileInfo(path);
                } else {
                    file = new FileInfo(Path.Combine(line.Source.Directory.FullName, path));
                    if (!file.Exists)
                        file = document.ResolveImport(path);
                }

                if (file == null || !file.Exists)
                    throw new AssemblerException("File {0} not exists", trace.Create(line), path);

            } else if (line.Arguments[0] is Symbol symbol) {
                file = document.ResolveImport(symbol.Name);

                if (file == null)
                    throw new AssemblerException("File {0} not exists", trace.Create(line), symbol.Name);
            } else {
                throw new AssemblerException("Argument must be a string", trace.Create(line));
            }

            ImportInterpreter interpreter = new ImportInterpreter(router, document, trace.Create(line));
            router.PushState(interpreter);
            using (Parser parser = new Parser(file, router, trace.Create(line))) {
                parser.Parse();
            }
            router.PopState();
        }
    }
}
