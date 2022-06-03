using Assembler.Interpreters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembler {
    class Program {
        private static FileInfo source;
        private static FileInfo output;
        private static bool wait;

        static int Main(string[] args) {
            try {
                if (!Parse(args))
                    return 0;

                Assemble();

                if (wait) Console.ReadKey();
                return 0;
            } catch (AssemblerException e) {
                Console.Error.WriteLine("{0} on line {1}", e.Message, e.LineNr);
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
            }

            if (wait) Console.ReadKey();
            return 1;
        }

        private static void ShowHelp() {
            Console.WriteLine("Usage: assembler [options...] filename");
            Console.WriteLine();
            Console.WriteLine("Options");
            Console.WriteLine(" -o <file>         write output to file");
            Console.WriteLine(" --wait            wait for any key");
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
                            throw new Exception("Missing argument for -0");
                        output = new FileInfo(enumerator.Current);
                    }
                    break;
                    case "--wait": wait = true; break;
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
            string filename = source.Name.Substring(0, source.Name.Length - source.Extension.Length) + ".o";
            output = new FileInfo(Path.Combine(source.DirectoryName, filename));
        }

        private static void Assemble() {
            if (output == null)
                GenerateOutput();

            using (StreamReader reader = source.OpenText())
            using (Document document = new Document(output)) {
                Parser parser = new Parser(new Router(document));
                parser.Parse(reader);
            }
        }
    }
}
