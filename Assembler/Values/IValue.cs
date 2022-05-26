namespace Assembler.Values {
    public interface IValue {
        ValueType Type { get; }

        IConstant GetValue(IScope scope);

        IValue Resolve(IScope scope);
    }
}
