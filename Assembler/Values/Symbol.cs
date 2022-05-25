namespace Assembler.Values {
    public class Symbol : IValue {
        private string value;

        public ValueType Type => ValueType.Symbol;

        public Symbol(string value) {
            this.value = value;
        }

        public bool GetValue(IScope scope, out long value) {
            value = default;
            return false;
        }
        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return value;
        }
    }
}