using Assembler.Processors;
using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using String = Assembler.Values.String;

namespace Assembler {
    public class Document : IProcessor, IDisposable {
        private readonly ReferenceTable referenceTable;
        private readonly SymbolTable symbolTable;
        private readonly VariableScope variableScope;
        private readonly Writer writer;
        private Macro macros;

        public long Position => writer.FileOffset;

        public Document(FileInfo output) {
            referenceTable = new ReferenceTable();
            symbolTable = new SymbolTable();
            variableScope = new VariableScope();
            writer = new Writer(output.Open(FileMode.Create));
        }

        public void Dispose() {
            foreach (SymbolTable.Entry entry in symbolTable) {
                IConstant value = entry.Reference.GetValue(referenceTable) as Number;
                if (value == null)
                    throw new AssemblerException(string.Format("Unknown symbol in '{0}'", entry.Reference), 0);

                if (!(value is Number number))
                    throw new AssemblerException("Invalid data type for symbol", 0);

                writer.Seek(entry.Offset);
                writer.SetByte(number.Value);
            }


            Console.WriteLine("----------------------------");
            Console.WriteLine(referenceTable);
            Console.WriteLine("----------------------------");
            Console.WriteLine(symbolTable);
            Console.WriteLine("----------------------------");
            Console.WriteLine(macros.ToString(true));
            writer.Dispose();
        }

        public void ProcessLine(AssemblyLine line) {
            if (line.Label != null) {
                if (!referenceTable.Add(line.Label, writer.Position))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line, variableScope); break;
                }
            }

            if (line.Assignment != null) {
                variableScope.Set(line.Assignment, line.Arguments[0].GetValue(variableScope));
            }

            Console.WriteLine(line);

            //Console.WriteLine("Label      : {0}", line.Label);
            //Console.WriteLine("Assignment : {0}", line.Assignment);
            //Console.WriteLine("Modifier   : {0}", line.Modifier);
            //Console.WriteLine("Instruction: {0}", line.Instruction);
            //Console.WriteLine("Arguments  : {0}", line.Arguments);
            //Console.WriteLine("Block open : {0}", line.IsBlockOpen);
            //Console.WriteLine("Block close: {0}", line.IsBlockClose);
            //Console.WriteLine("Comments   : {0}", line.Comments);
            //Console.WriteLine("-----------------------------------------------");
        }

        public Macro AddMacro(string name, string[] arguments) {
            return macros = new Macro(macros, name, arguments);
        }

        public Macro GetMacro(string name, int argumentCount) {
            if (macros == null)
                return null;

            return macros.Find(name, argumentCount);
        }

        public bool AddReference(string label) {
            return referenceTable.Add(label, writer.Position);
        }

        public void SetOrigin(long value) {
            writer.Origin = value;
        }

        public void PutByte(IValue[] values) {
            foreach (IValue value in values) {
                IConstant constant = value as IConstant;

                if (constant == null) {
                    symbolTable.Add(writer.FileOffset, value);
                    writer.WriteByte(0);
                    continue;
                }

                if (constant is String text) {
                    writer.WriteString(text.Text);
                    continue;
                }

                if (constant is Number number) {
                    writer.WriteByte(number.Value);
                    continue;
                }

                throw new Exception("This shouldn't happen");
            }
        }

        private void PutByte(AssemblyLine line, IScope scope) {
            foreach (IValue argument in line.Arguments) {
                IConstant constant = argument.GetValue(scope);

                if (constant is String strValue) {
                    writer.WriteString(strValue.Text);
                    continue;
                }

                Number number = constant as Number;
                if (constant == null) {
                    symbolTable.Add(writer.FileOffset, argument.Resolve(scope));

                    writer.WriteByte(0);
                } else {
                    writer.WriteByte(number.Value);
                }
            }
        }

        private void SetOrigin(AssemblyLine line) {
            if (line.Arguments == null || line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for org", line.LineNumber);

            if (!(line.Arguments[0].GetValue(null) is Number number))
                throw new AssemblerException("Can't resolve origin value", line.LineNumber);

            writer.Origin = number.Value;
        }
    }
}
