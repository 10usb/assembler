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

        public bool GetValue(IScope scope, out long value) {
            if (!(this.left.GetValue(scope, out long left) && this.right.GetValue(scope, out long right))) {
                value = default;
                return false;
            }

            value = left | right;
            return true;
        }
        public IValue Resolve(IScope scope) {
            throw new System.NotImplementedException();
        }

        public override string ToString() {
            return string.Format("({0} {1} {2})", left, value, right);
        }
    }
}