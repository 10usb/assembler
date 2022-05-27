namespace Assembler.Values {
    /// <summary>
    /// A constant in the form of human readable values UTF-8
    /// </summary>
    public class String : IValue, IConstant {
        private readonly string value;

        /// <summary>
        /// The value of the stringn
        /// </summary>
        public string Text => value;

        /// <summary>
        /// Constructs a string object from a value
        /// </summary>
        /// <param name="value"></param>
        public String(string value) {
            this.value = value;
        }

        /// <summary>
        /// A string is already a constant and therefor resolved any further
        /// so it can return is self
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IConstant GetValue(IScope scope) {
            return this;
        }

        /// <summary>
        /// A string is already a constant and therefor resolved any further
        /// so it can return is self
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IValue Resolve(IScope scope) {
            return this;
        }

        /// <summary>
        /// Returns an assembly formatted representation of this string
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
        }
    }
}