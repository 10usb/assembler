namespace Assembler.Values {
    public interface IValue {
        ValueType Type { get; }

        bool GetValue(IScope scope, out long value);
        IValue Resolve(IScope scope);
    }
}
