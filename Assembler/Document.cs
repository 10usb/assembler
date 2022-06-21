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
        private readonly List<DirectoryInfo> importDirectories;
        private readonly List<DirectoryInfo> includeDirectories;
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

        public Writer Writer => writer;

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

            importDirectories = new List<DirectoryInfo>();
            includeDirectories = new List<DirectoryInfo>();
        }

        public void AddImport(DirectoryInfo directory) {
            importDirectories.Add(directory);
        }

        public FileInfo ResolveImport(string path) {
            foreach (DirectoryInfo directory in importDirectories) {
                string fullName = Path.Combine(directory.FullName, path);

                if (File.Exists(fullName))
                    return new FileInfo(fullName);

                fullName = Path.ChangeExtension(fullName, ".asm");

                if (File.Exists(fullName))
                    return new FileInfo(fullName);
            }

            return null;
        }

        public void AddInclude(DirectoryInfo directory) {
            importDirectories.Add(directory);
        }

        public FileInfo ResolveInclude(string path) {
            foreach (DirectoryInfo directory in includeDirectories) {
                string fullName = Path.Combine(directory.FullName, path);

                if (File.Exists(fullName))
                    return new FileInfo(fullName);
            }

            return null;
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
        public void PutByte(IValue[] values, Trace trace) {
            foreach (IValue value in values) {
                IConstant constant = value as IConstant;

                if (constant == null) {
                    symbolTable.Add(writer.FileOffset, value, trace);
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

                throw new BadProgrammerException("Don't know how to convert value to bytes");
            }
        }

        /// <summary>
        /// Resolves any entries in the symbol table
        /// </summary>
        public void Resolve() {
            writer.Flush();

            foreach (SymbolTable.Entry entry in symbolTable) {
                IConstant value = entry.Reference.GetValue(referenceTable) as Number;
                if (value == null)
                    throw new AssemblerException("Unknown symbol in '{0}'", entry.Trace, entry.Reference);

                if (!(value is Number number))
                    throw new AssemblerException("Invalid data type for symbol", entry.Trace);

                writer.Seek(entry.Offset);
                writer.SetByte(number.Value);
            }

            writer.Flush();
        }

        /// <summary>
        /// Makes sure the current in-memory changes are flushed to the output. This to make sure the
        /// file is of complete length. Then is tries to resolve all the symbols.
        /// </summary>
        public void Dispose() {
            writer.Flush();
            writer.Dispose();

        }

        public override string ToString() {
            StringWriter writer = new StringWriter();
            writer.WriteLine("-------- References --------");
            writer.Write(referenceTable);
            writer.WriteLine("-------- Symbols -----------");
            writer.Write(symbolTable);
            writer.WriteLine("-------- Types -------------");
            writer.Write(types);
            writer.WriteLine("-------- Constants ---------");
            writer.Write(constants);
            writer.WriteLine("-------- Globals -----------");
            writer.WriteLine(globals);
            return writer.ToString();
        }
    }
}
