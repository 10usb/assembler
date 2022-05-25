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
        private readonly Writer writer;

        public Document(FileInfo output) {
            referenceTable = new ReferenceTable();
            symbolTable = new SymbolTable();
            writer = new Writer(output.OpenWrite());
        }

        public void Dispose() {
            foreach (SymbolTable.Entry entry in symbolTable) {
                if (!entry.Reference.GetValue(referenceTable, out long value))
                    throw new AssemblerException("Unknown symbol", 0);

                writer.Seek(entry.Offset);
                writer.SetByte(value);
            }


            Console.WriteLine("----------------------------");
            Console.WriteLine(referenceTable);
            Console.WriteLine("----------------------------");
            Console.WriteLine(symbolTable);
            writer.Dispose();
        }

        public void ProcessLine(AssemblyLine line) {
            IScope scope = null;

            if (line.Label != null) {
                if (!referenceTable.Add(line.Label, writer.Position))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction != null) {
                switch (line.Instruction) {
                    case "org": SetOrigin(line); break;
                    case "db": PutByte(line, scope); break;
                }
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

        private void PutByte(AssemblyLine line, IScope scope) {
            foreach (IValue argument in line.Arguments) {
                if (argument is String strValue) {
                    writer.WriteString(strValue.Text);
                    continue;
                }

                if (!argument.GetValue(scope, out long value)) {
                    symbolTable.Add(writer.FileOffset, argument.Resolve(scope));
                }

                writer.WriteByte(value);
            }
        }

        private void SetOrigin(AssemblyLine line) {
            if (line.Arguments == null || line.Arguments.Length != 1)
                throw new AssemblerException("Unexpected argument count for org", line.LineNumber);

            if (!line.Arguments[0].GetValue(null, out long value))
                throw new AssemblerException("Can't resolve origin value", line.LineNumber);

            writer.Origin = value;
        }
    }
}
