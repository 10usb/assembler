namespace Assembler.Values {
    public class String : IValue, IConstant {
        private readonly string value;

        public ValueType Type => ValueType.String;

        public string Text => value;

        public String(string value) {
            this.value = value;
        }
        public IConstant GetValue(IScope scope) {
            return this;
        }

        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
        }
    }
}