﻿using Assembler.Interpreters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembler {
    class Program {
        private static FileInfo source;
        private static FileInfo output;
        private static bool wait;
        private static bool verbose;
        private static readonly List<DirectoryInfo> includePaths = new List<DirectoryInfo>();
        private static readonly List<DirectoryInfo> importPaths = new List<DirectoryInfo>();
        private static readonly List<string> imports = new List<string>();

        static int Main(string[] args) {
            try {
                if (!Parse(args))
                    return 0;

                Assemble();

                if (wait) Console.ReadKey();
                return 0;
            } catch (AssemblerException e) {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.Trace);
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
            }

            if (wait) Console.ReadKey();
            return 1;
        }

        private static void ShowHelp() {
            Console.WriteLine("Usage: assembler [options...] <source>");
            Console.WriteLine();
            Console.WriteLine("Options");
            Console.WriteLine(" -o <file>                   write output to file");
            Console.WriteLine(" --wait                      wait for any key");
            Console.WriteLine(" --include-path <folder>     the folder to look in when including files");
            Console.WriteLine(" --import-path <folder>      the folder to look in file when importing");
            Console.WriteLine(" --import <file>             import a file before processing the source file");
            Console.WriteLine(" --verbose                   print detailed assembler info");
        }

        private static bool Parse(string[] args) {
            IEnumerator<string> enumerator = args.ToList().GetEnumerator();
            if (!enumerator.MoveNext()) {
                ShowHelp();
                return false;
            }

            while (enumerator.Current[0] == '-') {
                switch (enumerator.Current) {
                    case "-o": {
                        if (!enumerator.MoveNext())
                            throw new Exception("Missing argument for -o");
                        output = new FileInfo(enumerator.Current);
                    }
                    break;
                    case "--wait": wait = true; break;
                    case "--include-path": {
                        if (!enumerator.MoveNext())
                            throw new Exception("Missing argument for --include-path");
                        includePaths.Add(new DirectoryInfo(enumerator.Current));
                    }
                    break;
                    case "--import-path": {
                        if (!enumerator.MoveNext())
                            throw new Exception("Missing argument for --import-path");
                        importPaths.Add(new DirectoryInfo(enumerator.Current));
                    }
                    break;
                    case "--import": {
                        if (!enumerator.MoveNext())
                            throw new Exception("Missing argument for --import");
                        imports.Add(enumerator.Current);
                    }
                    break;
                    case "--verbose": {
                        verbose = true;
                    }
                    break;
                    default: throw new Exception(string.Format("Unknown commandline option '{0}'", enumerator.Current));
                }

                if (!enumerator.MoveNext())
                    throw new Exception("No source file specified");
            }

            source = new FileInfo(enumerator.Current);

            if (enumerator.MoveNext())
                throw new Exception(string.Format("Unexpected output {0}", enumerator.Current));

            return true;
        }

        private static void GenerateOutput() {
            string filename = Path.ChangeExtension(source.Name, ".o");
            output = new FileInfo(Path.Combine(source.DirectoryName, filename));
        }

        private static void Assemble() {
            if (output == null)
                GenerateOutput();

            using (Document document = new Document(output)) {
                foreach (DirectoryInfo directoryInfo in includePaths)
                    document.AddInclude(directoryInfo);

                foreach (DirectoryInfo directoryInfo in importPaths)
                    document.AddImport(directoryInfo);

                Router router = new Router();

                if (imports.Count > 0) {
                    foreach (string path in imports) {
                        router.PushState(new ImportInterpreter(router, document, Trace.Empty));
                        using (Parser parser = new Parser(new FileSource(document.ResolveImport(path)), router, Trace.Empty)) {
                            parser.Parse();
                        }
                        router.PopState();
                    }
                }

                router.PushState(new GlobalInterpreter(router, document));
                using (Parser parser = new Parser(new FileSource(source), router, Trace.Empty)) {
                    parser.Parse();
                }

                document.Resolve();

                Console.WriteLine("Written {0} bytes to {1}", document.Position, output);
                if (verbose)
                    Console.WriteLine(document);
            }
        }
    }
}
