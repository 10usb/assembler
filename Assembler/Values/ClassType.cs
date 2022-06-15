namespace Assembler.Values {
    public class ClassType : IConstant {
        private readonly string name;

        public string Name => name;

        public ClassType Class => throw new System.NotImplementedException();

        public ClassType(string name) {
            this.name = name;
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

        public IValue Cast(ClassType classType) {
            throw new System.NotImplementedException();
        }

        public bool Equals(ClassType other) {
            if (other is null) return false;
            return name == other.Name;
        }

        public override bool Equals(object obj) {
            if (obj is ClassType classType)
                return Equals(classType);

            return false;
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }

        public override string ToString() {
            return name;
        }
    }
}