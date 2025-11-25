namespace OldPhone.Core
{
    /// <summary>
    /// Defines a contract for keypad input decoders.
    /// </summary>
    public interface IKeypadDecoder
    {
        /// <summary>
        /// Decodes keypad input string to text.
        /// </summary>
        /// <param name="input">The keypad input string to decode.</param>
        /// <returns>The decoded text.</returns>
        /// <exception cref="ArgumentNullException">When input is null.</exception>
        /// <exception cref="KeypadException">When input contains invalid characters.</exception>
        string Decode(string input);
    }
}
