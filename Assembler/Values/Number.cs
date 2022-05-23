﻿using System;

namespace Assembler.Values {
    public class Number : IValue {
        private long value;
        private NumberFormat format;

        public ValueType Type => ValueType.Number;
        public Number(long value, NumberFormat format) {
            this.value = value;
            this.format = format;
        }

        public long GetValue(IScope scope) {
            return value;
        }
        public override string ToString() {
            switch (format) {
                case NumberFormat.Binary: return string.Format("{0}b", Convert.ToString(value, 2));
                case NumberFormat.Octal: return string.Format("0{0}", Convert.ToString(value, 8));
                case NumberFormat.Decimal: return Convert.ToString(value, 10);
                default:
                case NumberFormat.Hex: return string.Format("0x{0}", Convert.ToString(value, 16));
            }
        }
    }
}