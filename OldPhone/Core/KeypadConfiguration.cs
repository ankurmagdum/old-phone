using System;
using System.Collections.Generic;
using System.Linq;

namespace OldPhone.Core
{
    /// <summary>
    /// Represents the configuration for keypad decoding operations.
    /// </summary>
    public sealed class KeypadConfiguration
    {
        /// <summary>
        /// Gets the standard T9 keypad configuration.
        /// </summary>
        public static KeypadConfiguration T9 { get; } = new KeypadConfiguration(
            new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B', 'C' } },
                { '3', new[] { 'D', 'E', 'F' } },
                { '4', new[] { 'G', 'H', 'I' } },
                { '5', new[] { 'J', 'K', 'L' } },
                { '6', new[] { 'M', 'N', 'O' } },
                { '7', new[] { 'P', 'Q', 'R', 'S' } },
                { '8', new[] { 'T', 'U', 'V' } },
                { '9', new[] { 'W', 'X', 'Y', 'Z' } }
            }
        );

        /// <summary>
        /// Gets the mapping of keypad digits to their corresponding letters.
        /// </summary>
        public IReadOnlyDictionary<char, char[]> KeyMapping { get; }

        /// <summary>
        /// Gets or initializes the character used to separate consecutive presses of the same key.
        /// Default is space (' ').
        /// </summary>
        public char SeparatorChar { get; init; } = ' ';

        /// <summary>
        /// Gets or initializes the character used for backspace/delete operations.
        /// Default is asterisk ('*').
        /// </summary>
        public char BackspaceChar { get; init; } = '*';

        /// <summary>
        /// Gets or initializes the character that marks the end of input.
        /// Default is hash ('#').
        /// </summary>
        public char EndMarker { get; init; } = '#';

        /// <summary>
        /// Creates a new keypad configuration with the specified key mapping.
        /// </summary>
        /// <param name="keyMapping">Dictionary mapping keypad digits to their letter arrays.</param>
        /// <exception cref="ArgumentNullException">When keyMapping is null.</exception>
        /// <exception cref="ArgumentException">When keyMapping is empty or contains invalid data.</exception>
        public KeypadConfiguration(Dictionary<char, char[]> keyMapping)
        {
            if (keyMapping == null)
                throw new ArgumentNullException(nameof(keyMapping));

            if (keyMapping.Count == 0)
                throw new ArgumentException("Key mapping cannot be empty.", nameof(keyMapping));

            if (keyMapping.Any(kvp => kvp.Value == null || kvp.Value.Length == 0))
                throw new ArgumentException("Key mapping values cannot be null or empty.", nameof(keyMapping));

            KeyMapping = keyMapping;
        }

        /// <summary>
        /// Validates whether a character is a valid keypad digit in this configuration.
        /// </summary>
        public bool IsValidKeypadDigit(char digit) => KeyMapping.ContainsKey(digit);

        /// <summary>
        /// Validates whether a character is a special control character.
        /// </summary>
        public bool IsControlChar(char c) => 
            c == SeparatorChar || c == BackspaceChar || c == EndMarker;
    }
}
