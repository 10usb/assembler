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

        public virtual IConstant GetValue(IScope scope) {
            IValue value = scope.Get(name);
            if (value == null)
                return null;

            return value.GetValue(scope);
        }

        /// <summary>
        /// If the symbol can be found in the current scope, it will
        /// return the value of that. If it can't find it, it will assume
        /// it's a label reference and solidify it state.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual IValue Resolve(IScope scope) {
            IValue value = scope.Get(name);
            if (value == null)
                return new Label(name);

            IConstant result = value.GetValue(scope);
            if (result != null)
                return result;

            return value.Resolve(scope);
        }

        public IValue Derive(Mutator mutator) {
            IValue result = mutator(this);
            if (result != null)
                return result;

            return this;
        }

        public override string ToString() {
            return name;
        }
    }
}