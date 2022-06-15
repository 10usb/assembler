using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public abstract class BaseInterpreter : IInterpreter {
        protected readonly Document document;
        protected readonly LocalScope scope;
        protected readonly Trace trace;

        protected abstract ScopeType DefaultScope { get; }

        protected BaseInterpreter(Document document, LocalScope scope, Trace trace) {
            this.document = document;
            this.scope = scope;
            this.trace = trace;
        }

        public virtual IValue Translate(IValue value) {
            return value;
        }

        public void Process(AssemblyLine line) {
            if (line.Label != null) {
                Label label = Translate(new Label(line.Label)) as Label;

                if (!document.AddReference(label.Name))
                    throw new AssemblerException("Duplicate label found", trace.Create(line));
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line); break;
                    case "throw": throw Throw(line);
                    case "macro": StartMacro(line); return;
                    case "enum": StartEnum(line); return;
                    case "include": StartInclude(line); return;
                    case "import": StartImport(line); return;
                    case "file": ReadFile(line); return;
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
                throw new AssemblerException("Unexpected argument count for org", trace.Create(line));

            if (!(line.Arguments[0].GetValue(null) is Number number))
                throw new AssemblerException("Can't resolve origin value", trace.Create(line));

            document.SetOrigin(number.Value);
        }

        protected virtual void PutByte(AssemblyLine line) {
            document.PutByte(line.Arguments.Select(argument => {
                IConstant constant = argument.GetValue(scope);
                if (constant != null)
                    return constant;

                return Translate(argument).Resolve(scope);
            }).ToArray(), trace.Create(line));
        }

        private Exception Throw(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for throw", trace.Create(line));

            IConstant constant = Translate(line.Arguments[0]).GetValue(scope);
            if (!(constant is Text message))
                throw new AssemblerException("Unexpected argument type for throw", trace.Create(line));

            return new AssemblerException(message.Value, trace.Create(line));
        }

        protected abstract void StartMacro(AssemblyLine line);

        protected abstract void StartEnum(AssemblyLine line);

        protected abstract void ProcessInstruction(AssemblyLine line);

        protected abstract void ProcessSection(AssemblyLine line);

        protected abstract void StartInclude(AssemblyLine line);

        protected abstract void StartImport(AssemblyLine line);

        protected void ReadFile(AssemblyLine line) {
            if (line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for file", trace.Create(line));

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
            }

            if (file == null || !file.Exists)
                throw new AssemblerException("File {0} not exists", trace.Create(line), path);

            byte[] buffer = new byte[4096];
            using (Stream stream = file.OpenRead()) {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    document.Writer.Write(buffer, read);
                }
            }
        }
    }
}
