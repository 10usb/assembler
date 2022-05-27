namespace Assembler.Values {
    public interface IValue {

        IConstant GetValue(IScope scope);

        IValue Resolve(IScope scope);
    }
}
