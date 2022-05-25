namespace Assembler.Values {
    public class String : IValue {
        private readonly string value;

        public ValueType Type => ValueType.String;

        public string Text => value;

        public String(string value) {
            this.value = value;
        }

        public bool GetValue(IScope scope, out long value) {
            throw new System.NotImplementedException();
        }

        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
        }
    }
}