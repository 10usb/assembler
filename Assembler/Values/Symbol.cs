namespace Assembler.Values {
    public class Symbol : IValue {
        private string value;
        public ValueType Type => ValueType.Symbol;
        public Symbol(string value) {
            this.value = value;
        }
        public long GetValue(IScope scope) {
            throw new System.NotImplementedException();
        }
        public override string ToString() {
            return value;
        }
    }
}