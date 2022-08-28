using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    public interface ISource {
        string Reference { get; }

        TextReader Open();
    }
}
