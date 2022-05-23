﻿using Assembler.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler {
    class Program {

        static void Main(string[] args) {
            FileInfo source = new FileInfo("I:\\Programming\\Assembler\\Resources\\source.asm");
            FileInfo output = new FileInfo("I:\\Programming\\Assembler\\Resources\\source.bin");

            using (Document document = new Document(output)) {
                Parser parser = new Parser(document);
                try {
                    using (StreamReader reader = source.OpenText()) {
                        parser.Parse(reader);
                    }
                } catch (AssemblerException e) {
                    Console.Error.WriteLine(e.Message, e.LineNr);
                }
            }

            Console.ReadKey();
        }

    }
}
