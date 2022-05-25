namespace Assembler.Values {
    public class Symbol : IValue {
        private string name;

        public ValueType Type => ValueType.Symbol;

        public Symbol(string name) {
            this.name = name;
        }

        public bool GetValue(IScope scope, out long value) {
            IValue variable = scope.Get(name);
            if (variable != null)
                return variable.GetValue(scope, out value);

            value = default;
            return false;
        }
        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return name;
        }
    }
}