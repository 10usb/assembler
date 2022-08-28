using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class FileSource : ISource {
        private readonly FileInfo file;
        public FileInfo File => file;

        public string Reference => file.FullName;

        public FileSource(FileInfo file) {
            this.file = file;
        }

        public TextReader Open() {
            return file.OpenText();
        }
    }
}
