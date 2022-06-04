namespace Assembler.Values {
    public class Kind : IConstant {
        private readonly string name;

        public string Name => name;

        public Kind(string value) {
            this.name = value;
        }

        public IConstant GetValue(IScope scope) {
            return this;
        }

        public IValue Resolve(IScope scope) {
            return this;
        }

        public IValue Derive(Mutator mutator) {
            IValue result = mutator(this);
            if (result != null)
                return result;

            return this;
        }

        public override string ToString() {
            return name;
        }
    }
}