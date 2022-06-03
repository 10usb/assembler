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
    public class Document : IScope, IDisposable {
        private readonly ReferenceTable referenceTable;
        private readonly SymbolTable symbolTable;
        private readonly Writer writer;
        private readonly VariableScope globalScope;
        private readonly VariableScope constantScope;
        private Macro macros;

        /// <summary>
        /// The current position at writing the output file
        /// </summary>
        public long Position => writer.FileOffset;

        /// <summary>
        /// Constructs a Documents and opens a stream to the output file
        /// </summary>
        /// <param name="output"></param>
        public Document(FileInfo output) {
            referenceTable = new ReferenceTable();
            symbolTable = new SymbolTable();
            writer = new Writer(output.Open(FileMode.Create));

            globalScope = new VariableScope();
            constantScope = new VariableScope();
        }

        public IValue Get(string name) {
            IValue value = constantScope.Get(name);
            if (value != null)
                return value;

            return globalScope.Get(name);
        }

        public void Set(ScopeType scopeType, string name, IValue value) {
            if (constantScope.Get(name) != null)
                throw new Exception(string.Format("Can't set '{0}' a constant value with this name already exists", name));

            switch (scopeType) {
                case ScopeType.Constant: constantScope.Set(name, value); break;
                case ScopeType.Global: constantScope.Set(name, value); break;
                default: throw new Exception("Can't set '{0}' of a unknown scope");
            }
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


            Console.WriteLine("----------------------------");
            Console.WriteLine(referenceTable);
            Console.WriteLine("----------------------------");
            Console.WriteLine(symbolTable);
            Console.WriteLine("----------------------------");
            //Console.WriteLine(macros.ToString(true));
        }
    }
}
