namespace Assembler.Values {
    /// <summary>
    /// The ways a number can be formatted in assembly
    /// </summary>
    public enum NumberFormat {
        /// <summary>
        /// 1010101b
        /// </summary>
        Binary,

        /// <summary>
        /// 0432
        /// </summary>
        Octal,

        /// <summary>
        /// 98374
        /// </summary>
        Decimal,

        /// <summary>
        /// 0xFE
        /// </summary>
        Hex
    }
}
