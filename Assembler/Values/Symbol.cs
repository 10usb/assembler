namespace Assembler.Values {
    /// <summary>
    /// A symbol is a string of characters that have no unsafe characters in them.
    /// And can there for be mixed in code and be used for labels and variables
    /// </summary>
    public class Symbol : IValue {
        private readonly string name;

        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Constructs a symbol from a names
        /// </summary>
        /// <param name="name"></param>
        public Symbol(string name) {
            this.name = name;
        }

        public IConstant GetValue(IScope scope) {
            IValue value = scope.Get(name);
            if (value == null)
                return null;
            return value.GetValue(scope);
        }

        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return name;
        }
    }
}