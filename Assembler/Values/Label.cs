using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Values {
    /// <summary>
    /// A label is a refering symbol that has been resolved to
    /// not point to any variable in a scope and therefore is must
    /// be pointing to a label. This class exist to make sure it
    /// can't be resolved to a constant value when trying to resolve
    /// a second time
    /// </summary>
    public class Label : Symbol {
        private ClassType classType;

        public ClassType Class => classType;

        public Label(string name) : base(name) {
        }

        /// <summary>
        /// Overritten to make sure it stays the same
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public override IValue Resolve(IScope scope) {
            return this;
        }

        public override IValue Cast(ClassType classType) {
            return new Label(Name) {
                classType = classType
            };
        }
    }
}
