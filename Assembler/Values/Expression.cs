namespace Assembler.Values {
    public class Expression : IValue {
        private string value;
        private IValue left;
        private IValue right;

        public ValueType Type => ValueType.Expression;

        public Expression(string value, IValue left, IValue right) {
            this.value = value;
            this.left = left;
            this.right = right;
        }
        public long GetValue(IScope scope) {
            throw new System.NotImplementedException();
        }

        public override string ToString() {
            return string.Format("({0} {1} {2})", left, value, right);
        }
    }
}