namespace Assembler.Values {
    /// <summary>
    /// A instance that can represent a value
    /// </summary>
    public interface IValue {
        /// <summary>
        /// The class type this value belongs to
        /// </summary>
        ClassType Class { get; }

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

        /// <summary>
        /// Makes a copy of the the current value, but allows mutator
        /// to alter the state
        /// </summary>
        /// <param name="mutator"></param>
        /// <returns></returns>
        IValue Derive(Mutator mutator);

        /// <summary>
        /// Make a copy of the current value and make it of the kind
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        IValue Cast(ClassType classType);
    }
}
