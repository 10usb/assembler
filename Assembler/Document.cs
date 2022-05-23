using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using String = Assembler.Values.String;

namespace Assembler {
    public class Document : IProcessor {
        private FileInfo output;
        private BinaryWriter writer;

        public Document(FileInfo output) {
            this.output = output;
            writer = new BinaryWriter(output.OpenWrite(), Encoding.UTF8, false);
        }

        public void ProcessLine(AssemblyLine line) {
            IScope scope = null;
            if (line.Instruction == "db") {
                foreach (IValue value in line.Arguments) {
                    if (value is String strValue) {
                        writer.Write(strValue.Text.ToCharArray());
                    } else {
                        long number = value.GetValue(scope);
                        writer.Write((byte)number);
                    }
                }
            }

            writer.Flush();

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
