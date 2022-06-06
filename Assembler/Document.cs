using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    /// <summary>
    /// The class that represent the output in memory
    /// </summary>
    public class Document : IDisposable {
        private readonly ReferenceTable referenceTable;
        private readonly SymbolTable symbolTable;
        private readonly Writer writer;
        private readonly VariableScope globals;
        private readonly VariableScope constants;
        private readonly VariableScope types;
        private Macro macros;

        /// <summary>
        /// The current position at writing the output file
        /// </summary>
        public long Position => writer.FileOffset;

        /// <summary>
        /// The global scope accessible from anywhere
        /// </summary>
        public VariableScope Globals => globals;

        /// <summary>
        /// The constant scope is also global scope but variables can only be set once
        /// </summary>
        public VariableScope Constants => constants;

        public VariableScope Types => types;

        /// <summary>
        /// Constructs a Documents and opens a stream to the output file
        /// </summary>
        /// <param name="output"></param>
        public Document(FileInfo output) {
            referenceTable = new ReferenceTable();
            symbolTable = new SymbolTable();
            writer = new Writer(output.Open(FileMode.Create));

            globals = new VariableScope();
            constants = new VariableScope();
            types = new VariableScope();
        }

        public void AddImport(DirectoryInfo directoryInfo) {
            throw new NotImplementedException();
        }

        public FileInfo ResolveImport(string path) {
            throw new NotImplementedException();
        }

        public void AddInclude(DirectoryInfo directoryInfo) {
            throw new NotImplementedException();
        }

        public FileInfo ResolveInclude(string path) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a new macro to the list of defined macro's
        /// </summary>
        /// <param name="name">Name of the macro</param>
        /// <param name="parameters">Names for the parameters</param>
        /// <returns></returns>
        public Macro AddMacro(string name, string[] parameters) {
            return macros = new Macro(macros, name, parameters);
        }

        /// <summary>
        /// Get the first macro that matches the pattern
        /// </summary>
        /// <param name="name">Name of the macro</param>
        /// <param name="argumentCount">The number of arguments to accept</param>
        /// <returns></returns>
        public Macro GetMacro(string name, int argumentCount) {
            if (macros == null)
                return null;

            return macros.Find(name, argumentCount);
        }

        /// <summary>
        /// Added a new label reference at the current pointint to the
        /// current virtual position in the file
        /// </summary>
        /// <param name="label">Unique label to map it at</param>
        /// <returns></returns>
        public bool AddReference(string label) {
            return referenceTable.Add(label, writer.Position);
        }

        /// <summary>
        /// Set the current virtual address offset
        /// </summary>
        /// <param name="value"></param>
        public void SetOrigin(long value) {
            writer.Origin = value;
        }

        /// <summary>
        /// Writes the values as bytes to output
        /// </summary>
        /// <param name="values"></param>
        public void PutByte(IValue[] values) {
            foreach (IValue value in values) {
                IConstant constant = value as IConstant;

                if (constant == null) {
                    symbolTable.Add(writer.FileOffset, value);
                    writer.WriteByte(0);
                    continue;
                }

                if (constant is Text text) {
                    writer.WriteString(text.Value);
                    continue;
                }

                if (constant is Number number) {
                    writer.WriteByte(number.Value);
                    continue;
                }

                throw new Exception("This shouldn't happen");
            }
        }

        /// <summary>
        /// Makes sure the current in-memory changes are flushed to the output. This to make sure the
        /// file is of complete length. Then is tries to resolve all the symbols.
        /// </summary>
        public void Dispose() {
            writer.Flush();

            foreach (SymbolTable.Entry entry in symbolTable) {
                IConstant value = entry.Reference.GetValue(referenceTable) as Number;
                if (value == null)
                    throw new AssemblerException(string.Format("Unknown symbol in '{0}'", entry.Reference), 0);

                if (!(value is Number number))
                    throw new AssemblerException("Invalid data type for symbol", 0);

                writer.Seek(entry.Offset);
                writer.SetByte(number.Value);
            }

            writer.Dispose();


            Console.WriteLine("-------- References --------");
            Console.Write(referenceTable);
            Console.WriteLine("-------- Symbols -----------");
            Console.Write(symbolTable);
            Console.WriteLine("-------- Types -------------");
            Console.Write(types);
            Console.WriteLine("-------- Constants ---------");
            Console.Write(constants);
            Console.WriteLine("-------- Globals -----------");
            Console.WriteLine(globals);
        }
    }
}
