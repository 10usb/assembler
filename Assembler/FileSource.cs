using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public class FileSource : ISource {
        private readonly FileInfo file;

        /// <summary>
        /// The file this source points to
        /// </summary>
        public FileInfo File => file;

        /// <summary>
        /// Returns the full path to the file
        /// </summary>
        public string Reference => file.FullName;

        public FileSource(FileInfo file) {
            this.file = file;
        }

        public TextReader Open() {
            return file.OpenText();
        }
    }
}
