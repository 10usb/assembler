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
        private readonly Writer writer;

        public Document(FileInfo output) {
            referenceTable = new ReferenceTable();
            writer = new Writer(output.OpenWrite());
        }

        public void Dispose() {

            Console.WriteLine("----------------------------");
            Console.WriteLine(referenceTable);
            writer.Dispose();
        }

        public void ProcessLine(AssemblyLine line) {
            IScope scope = null;

            if (line.Label != null) {
                if (!referenceTable.Add(line.Label, writer.Position))
                    throw new AssemblerException("Duplicate label found", line.LineNumber);
            }

            if (line.Instruction == "db") {
                foreach (IValue value in line.Arguments) {
                    if (value is String strValue) {
                        writer.WriteString(strValue.Text);
                    } else {
                        writer.WriteByte(value.GetValue(scope));
                    }
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
    }
}
