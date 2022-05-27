namespace Assembler.Values {
    public class Symbol : IValue {
        private readonly string name;

        public string Name {
            get {
                return name;
            }
        }

        public Symbol(string name) {
            this.name = name;
        }
        public IConstant GetValue(IScope scope) {
            return scope.Get(name);
        }

        public IValue Resolve(IScope scope) {
            return this;
        }

        public override string ToString() {
            return name;
        }
    }
}