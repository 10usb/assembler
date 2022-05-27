namespace Assembler.Values {
    /// <summary>
    /// A instance that can represent a value
    /// </summary>
    public interface IValue {
        /// <summary>
        /// Get the constant value
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        IConstant GetValue(IScope scope);

        /// <summary>
        /// Uses the scope to resolve any value to a constant if it can
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        IValue Resolve(IScope scope);
    }
}
