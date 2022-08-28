using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class StringSource : ISource {
        private readonly string reference;
        private readonly string code;

        public string Reference => reference;

        /// <summary>
        /// Constriucts a StringSource from a string containing the code
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="code"></param>
        public StringSource(string reference, string code) {
            this.reference = reference;
            this.code = code;
        }

        /// <summary>
        /// Constructs a StringSource from a TextReader of who the
        /// contents will be consumed
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="reader"></param>
        public StringSource(string reference, TextReader reader) {
            this.reference = reference;
            code = reader.ReadToEnd();
        }

        public TextReader Open() {
            return new StringReader(code);
        }
    }
}
