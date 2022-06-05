using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Interpreters {
    public class MacroInterpreter : BaseInterpreter {
        private readonly Macro macro;
        private readonly string prefix;
        private string modifier;

        protected override ScopeType DefaultScope => ScopeType.Local;

        public MacroInterpreter(Macro macro, Document document, string prefix) {
            this.macro = macro;
            this.document = document;
            this.prefix = prefix;
            scope = new LocalScope(document);
        }

        public override IValue Translate(IValue source) {
            return source.Derive(value => {
                if (value is Label symbol && macro.HasLabel(symbol.Name))
                    return new Label(prefix + symbol.Name);

                return null;
            });
        }

        public void SetModifier(string modifier) {
            this.modifier = modifier;
            scope.Set(ScopeType.Local, "$modifier", new Text(modifier ?? ""));
        }

        public void SetParameters(IValue[] arguments) {
            int index = 0;
            foreach (string label in macro.Parameters)
                scope.Set(ScopeType.Local, label, arguments[index++]);
        }

        protected override void ProcessInstruction(AssemblyLine line) {
            Macro macro = this.macro.Parent.Find(line.Instruction, line.Arguments.Length);
            if (macro == null)
                throw new AssemblerException("Unknown instruction '{0}'", line.LineNumber, line.Instruction);

            MacroTranscriber transcriber = new MacroTranscriber(macro, document, document.Position);

            string modifier = line.Modifier == "$&" ? this.modifier : line.Modifier;

            transcriber.Transcribe(modifier, line.Arguments.Select(arg => Translate(arg).Resolve(scope)).ToArray());
        }

        protected override void StartMacro(AssemblyLine line) {
            throw new AssemblerException("Can't declare a macro from within a macro", line.LineNumber);
        }

        protected override void StartEnum(AssemblyLine line) {
            throw new AssemblerException("Can't declare an enum from within a macro", line.LineNumber);
        }

        protected override void ProcessSection(AssemblyLine line) {
            ConditionalSectionTranscriber transcriber = new ConditionalSectionTranscriber(scope, this);
            transcriber.Transcribe(line.Section);
        }

        protected override void StartInclude(AssemblyLine line) {
            throw new NotImplementedException();
        }
    }
}
