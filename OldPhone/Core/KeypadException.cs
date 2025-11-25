using System;

namespace OldPhone.Core
{
    /// <summary>
    /// Exception thrown when keypad operations encounter invalid input.
    /// </summary>
    public class KeypadException : Exception
    {
        /// <summary>
        /// Gets the invalid character that caused the exception, if applicable.
        /// </summary>
        public char? InvalidCharacter { get; }

        /// <summary>
        /// Creates a new KeypadException.
        /// </summary>
        public KeypadException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new KeypadException with information about an invalid character.
        /// </summary>
        public KeypadException(string message, char invalidCharacter) : base(message)
        {
            InvalidCharacter = invalidCharacter;
        }

        /// <summary>
        /// Creates a new KeypadException with an inner exception.
        /// </summary>
        public KeypadException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
