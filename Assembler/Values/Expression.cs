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
        private ClassType classType;

        public ClassType Class => classType;

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
            // We first try to get left and right to see if thay can be resolve
            IValue left = this.left.GetValue(scope);
            IValue right = this.right.GetValue(scope);

            if (right is ClassType classType) {
                left = this.left.Resolve(scope);

                switch (operation) {
                    case Operation.Cast: return left.Cast(classType) as IConstant;
                    case Operation.Is: return new Number(classType.Equals(left.Class) ? 1 : 0, NumberFormat.Decimal);
                    case Operation.IsNot: return new Number(classType.Equals(left.Class) ? 0 : 1, NumberFormat.Decimal);
                    default: throw new Exception("Unsupported operation");
                }
            }

            if (left == null)
                return null;

            if (right == null)
                return null;

            if (left is Number && right is Number)
                return Execute(left as Number, right as Number);

            if (left is Text && right is Text)
                return Execute(left as Text, right as Text);

            throw new Exception(string.Format("Can't perform the operation '{0}'. Both sides need to be of same type", this));
        }

        private IConstant Execute(Number left, Number right) {
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
                default: throw new BadProgrammerException("Unsupported operation");
            }
        }

        private IConstant Execute(Text left, Text right) {
            switch (operation) {
                case Operation.Equal: return new Number(left.Value == right.Value ? 1 : 0, NumberFormat.Decimal);
                default: throw new BadProgrammerException("Unsupported operation");
            }
        }

        /// <summary>
        /// Returns a copy of it's self where it has tried to resolve the values
        /// of the left and right parameter
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IValue Resolve(IScope scope) {
            IValue left = this.left.GetValue(scope);
            if (left == null)
                left = this.left.Resolve(scope);

            IValue right = this.right.GetValue(scope);
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

        public IValue Cast(ClassType classType) {
            return new Expression(operation, left, right) {
                classType = classType
            };
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