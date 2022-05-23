namespace Assembler.Values {
    public interface IValue {
        ValueType Type { get; }

        long GetValue(IScope scope);
    }
}
