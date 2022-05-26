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

        public IConstant GetValue(IScope scope) {
            Number left = this.left.GetValue(scope) as Number;
            if (left == null)
                return null;

            Number right = this.right.GetValue(scope) as Number;
            if (left == null)
                return null;

            switch (operation) {
                case Operation.Add: return new Number(left.Value + right.Value, NumberFormat.Decimal); break;
                case Operation.Substract: return new Number(left.Value - right.Value, NumberFormat.Decimal); break;
                case Operation.Muliply: return new Number(left.Value * right.Value, NumberFormat.Decimal); break;
                case Operation.Divide: return new Number(left.Value / right.Value, NumberFormat.Decimal); break;
                case Operation.Modulo: return new Number(left.Value % right.Value, NumberFormat.Decimal); break;
                case Operation.And: return new Number(left.Value & right.Value, NumberFormat.Decimal); break;
                case Operation.Or: return new Number(left.Value | right.Value, NumberFormat.Decimal); break;
                case Operation.Xor: return new Number(left.Value ^ right.Value, NumberFormat.Decimal); break;
                case Operation.ShiftLeft: return new Number(left.Value << (int)right.Value, NumberFormat.Decimal); break;
                case Operation.ShiftRight: return new Number(left.Value >> (int)right.Value, NumberFormat.Decimal); break;
                case Operation.Equal: return new Number(left.Value == right.Value ? 1 : 0, NumberFormat.Decimal); break;
                case Operation.Less: return new Number(left.Value < right.Value ? 1 : 0, NumberFormat.Decimal); break;
                case Operation.Greater: return new Number(left.Value > right.Value ? 1 : 0, NumberFormat.Decimal); break;
                case Operation.NotEqual: return new Number(left.Value != right.Value ? 1 : 0, NumberFormat.Decimal); break;
                case Operation.LessOrEqual: return new Number(left.Value <= right.Value ? 1 : 0, NumberFormat.Decimal); break;
                case Operation.GreaterOrEqual: return new Number(left.Value >= right.Value ? 1 : 0, NumberFormat.Decimal); break;
                default: throw new Exception("Unknown operation");
            }
        }

        public IValue Resolve(IScope scope) {
            IValue left = (Number)this.left.GetValue(scope);
            if(left == null)
                left = this.left.Resolve(scope);

            IValue right = (Number)this.right.GetValue(scope);
            if (right == null)
                right = this.right.Resolve(scope);

            return new Expression(operation, left, right);
        }

        public override string ToString() {
            return string.Format("({0} {1} {2})", left, operation, right);
        }
    }
}