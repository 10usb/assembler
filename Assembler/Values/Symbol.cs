namespace Assembler.Values {
    public class Symbol : IValue {
        private string name;

        public ValueType Type => ValueType.Symbol;

        public Symbol(string name) {
            this.name = name;
        }
        public IConstant GetValue(IScope scope) {
            return scope.Get(name);
        }

        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return name;
        }
    }
}