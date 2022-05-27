using System;

namespace Assembler.Values {
    /// <summary>
    /// A representation of a numeric value within the assembly language
    /// </summary>
    public class Number : IConstant {
        private readonly long value;
        private readonly NumberFormat format;

        /// <summary>
        /// The internal long value used for storing the value
        /// </summary>
        public long Value => value;

        /// <summary>
        /// Constructs a number, with a prefered format assigned to it.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public Number(long value, NumberFormat format) {
            this.value = value;
            this.format = format;
        }

        /// <summary>
        /// A number is already a constant and therefor resolved any further
        /// so it can return is self
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IConstant GetValue(IScope scope) {
            return this;
        }

        /// <summary>
        /// A number is already a constant and therefor resolved any further
        /// so it can return is self
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IValue Resolve(IScope scope) {
            return this;
        }

        /// <summary>
        /// Prints the value of the number using the defined format
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            switch (format) {
                case NumberFormat.Binary: return string.Format("{0}b", Convert.ToString(value, 2));
                case NumberFormat.Octal: return string.Format("0{0}", Convert.ToString(value, 8));
                case NumberFormat.Decimal: return Convert.ToString(value, 10);
                default:
                case NumberFormat.Hex: return string.Format("0x{0}", Convert.ToString(value, 16).ToUpper());
            }
        }
    }
}