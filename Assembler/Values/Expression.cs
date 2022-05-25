using System;

namespace Assembler.Values {
    public class Expression : IValue {
        private readonly Operation operation;
        private readonly IValue left;
        private readonly IValue right;

        public ValueType Type => ValueType.Expression;

        public Expression(Operation operation, IValue left, IValue right) {
            this.operation = operation;
            this.left = left;
            this.right = right;
        }

        public bool GetValue(IScope scope, out long value) {
            if (!(this.left.GetValue(scope, out long left) && this.right.GetValue(scope, out long right))) {
                value = default;
                return false;
            }

            switch (operation) {
                case Operation.Add: value = left + right; break;
                case Operation.Substract: value = left - right; break;
                case Operation.Muliply: value = left * right; break;
                case Operation.Divide: value = left / right; break;
                case Operation.Modulo: value = left % right; break;
                case Operation.And: value = left & right; break;
                case Operation.Or: value = left | right; break;
                case Operation.Xor: value = left ^ right; break;
                case Operation.Equal: value = left == right ? 1 : 0; break;
                case Operation.Less: value = left < right ? 1 : 0; break;
                case Operation.Greater: value = left > right ? 1 : 0; break;
                case Operation.NotEqual: value = left != right ? 1 : 0; break;
                case Operation.LessOrEqual: value = left <= right ? 1 : 0; break;
                case Operation.GreaterOrEqual: value = left >= right ? 1 : 0; break;
                default: throw new Exception("Unknown operation");
            }

            return true;
        }

        public IValue Resolve(IScope scope) {
            IValue left, right;
            if (this.left.GetValue(scope, out long value)) {
                left = new Number(value, NumberFormat.Hex);
            } else {
                left = this.left.Resolve(scope);
            }

            if (this.right.GetValue(scope, out value)) {
                right = new Number(value, NumberFormat.Hex);
            } else {
                right = this.left.Resolve(scope);
            }

            return new Expression(operation, left, right);
        }

        public override string ToString() {
            return string.Format("({0} {1} {2})", left, operation, right);
        }
    }
}