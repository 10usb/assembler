namespace Assembler.Values {
    public class String : IValue {
        private string value;
        public ValueType Type => ValueType.String;

        public string Text => value;

        public String(string value) {
            this.value = value;
        }

        public long GetValue(IScope scope) {
            throw new System.NotImplementedException();
        }
        public override string ToString() {
            return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
        }
    }
}