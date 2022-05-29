using System;

namespace Assembler.Values {
    /// <summary>
    /// An expression is a structure that can perform assemble-time evaluation of
    /// a given operation.
    /// </summary>
    public class Expression : IValue {
        private readonly Operation operation;
        private readonly IValue left;
        private readonly IValue right;

        /// <summary>
        /// Constructs an expression
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Expression(Operation operation, IValue left, IValue right) {
            this.operation = operation;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Will try a get a constant for it's left and right parameter, and on success will
        /// perform the operation and return the result
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IConstant GetValue(IScope scope) {
            Number left = this.left.GetValue(scope) as Number;
            if (left == null)
                return null;

            Number right = this.right.GetValue(scope) as Number;
            if (left == null)
                return null;

            switch (operation) {
                case Operation.Add: return new Number(left.Value + right.Value, NumberFormat.Decimal);
                case Operation.Substract: return new Number(left.Value - right.Value, NumberFormat.Decimal);
                case Operation.Muliply: return new Number(left.Value * right.Value, NumberFormat.Decimal);
                case Operation.Divide: return new Number(left.Value / right.Value, NumberFormat.Decimal);
                case Operation.Modulo: return new Number(left.Value % right.Value, NumberFormat.Decimal);
                case Operation.And: return new Number(left.Value & right.Value, NumberFormat.Decimal);
                case Operation.Or: return new Number(left.Value | right.Value, NumberFormat.Decimal);
                case Operation.Xor: return new Number(left.Value ^ right.Value, NumberFormat.Decimal);
                case Operation.ShiftLeft: return new Number(left.Value << (int)right.Value, NumberFormat.Decimal);
                case Operation.ShiftRight: return new Number(left.Value >> (int)right.Value, NumberFormat.Decimal);
                case Operation.Equal: return new Number(left.Value == right.Value ? 1 : 0, NumberFormat.Decimal);
                case Operation.Less: return new Number(left.Value < right.Value ? 1 : 0, NumberFormat.Decimal);
                case Operation.Greater: return new Number(left.Value > right.Value ? 1 : 0, NumberFormat.Decimal);
                case Operation.NotEqual: return new Number(left.Value != right.Value ? 1 : 0, NumberFormat.Decimal);
                case Operation.LessOrEqual: return new Number(left.Value <= right.Value ? 1 : 0, NumberFormat.Decimal);
                case Operation.GreaterOrEqual: return new Number(left.Value >= right.Value ? 1 : 0, NumberFormat.Decimal);
                default: throw new Exception("Unknown operation");
            }
        }

        /// <summary>
        /// Returns a copy of it's self where it has tried to resolve the values
        /// of the left and right parameter
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IValue Resolve(IScope scope) {
            IValue left = (Number)this.left.GetValue(scope);
            if (left == null)
                left = this.left.Resolve(scope);

            IValue right = (Number)this.right.GetValue(scope);
            if (right == null)
                right = this.right.Resolve(scope);

            return new Expression(operation, left, right);
        }

        public IValue Derive(Mutator mutator) {
            IValue result = mutator(this);
            if (result != null)
                return result;

            return new Expression(operation, left.Derive(mutator), right.Derive(mutator));
        }

        /// <summary>
        /// Returns a string representation of this expression
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("({0} {1} {2})", left, operation, right);
        }
    }
}