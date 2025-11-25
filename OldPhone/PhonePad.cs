using OldPhone.Core;
using OldPhone.Decoders;

namespace OldPhone
{
    /// <summary>
    /// Provides a simple facade for keypad decoding operations.
    /// </summary>
    public static class PhonePad
    {
        private static readonly IKeypadDecoder _defaultDecoder = new T9Decoder();

        /// <summary>
        /// Decodes T9 keypad input using the standard configuration.
        /// </summary>
        /// <param name="input">The keypad input string to decode.</param>
        /// <returns>The decoded text.</returns>
        /// <exception cref="System.ArgumentNullException">When input is null.</exception>
        /// <exception cref="KeypadException">When input contains invalid characters.</exception>
        /// <example>
        /// <code>
        /// string result = PhonePad.Decode("44 444#");  // Returns "HI"
        /// </code>
        /// </example>
        public static string Decode(string input)
        {
            return _defaultDecoder.Decode(input);
        }

        /// <summary>
        /// Decodes keypad input using a custom decoder.
        /// </summary>
        /// <param name="input">The keypad input string to decode.</param>
        /// <param name="decoder">The decoder to use.</param>
        /// <returns>The decoded text.</returns>
        public static string Decode(string input, IKeypadDecoder decoder)
        {
            return decoder.Decode(input);
        }

        /// <summary>
        /// Creates a new T9 decoder with custom configuration.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>A configured T9 decoder.</returns>
        public static IKeypadDecoder CreateDecoder(KeypadConfiguration configuration)
        {
            return new T9Decoder(configuration);
        }
    }
}
/*
using System;
using System.Text;

namespace OldPhone
{
    /// <summary>
    /// Provides functionality to decode numeric sequences from old phone keypads into text.
    /// </summary>
    public static class PhonePad
    {
        private const char Space = ' ';
        private const char Backspace = '*';
        private const char EndMarker = '#';
        private const int FirstKeypadNumber = 2;

        private static readonly char[][] KeypadMapping =
        [
            ['A', 'B', 'C'],           // 2
            ['D', 'E', 'F'],           // 3
            ['G', 'H', 'I'],           // 4
            ['J', 'K', 'L'],           // 5
            ['M', 'N', 'O'],           // 6
            ['P', 'Q', 'R', 'S'],      // 7
            ['T', 'U', 'V'],           // 8
            ['W', 'X', 'Y', 'Z']       // 9
        ];

        /// <summary>
        /// Converts a numeric keypad sequence to text based on old phone keypad mapping.
        /// </summary>
        /// <param name="input">
        /// The numeric input string. Use digits 2-9 for letters, space to separate same-digit letters,
        /// * for backspace, and # to end input.
        /// </param>
        /// <returns>The decoded text message.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ArgumentException">Thrown when input contains invalid characters.</exception>
        /// <example>
        /// <code>
        /// string result = PhonePad.DecodeKeypadInput("44 444");  // Returns "HI"
        /// string result = PhonePad.DecodeKeypadInput("222*9#");  // Returns "W"
        /// </code>
        /// </example>
        public static string DecodeKeypadInput(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            var previousChar = input[0];
            var consecutiveCount = 1;

            for (int i = 1; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (ShouldIncrementCount(previousChar, currentChar))
                {
                    consecutiveCount++;
                }
                else
                {
                    ProcessCharacter(result, previousChar, currentChar, consecutiveCount);
                    previousChar = currentChar;
                    consecutiveCount = 1;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Determines if the consecutive count should be incremented.
        /// </summary>
        private static bool ShouldIncrementCount(char previous, char current)
        {
            return previous == current && previous != Backspace;
        }

        /// <summary>
        /// Processes a character and updates the result string accordingly.
        /// </summary>
        private static void ProcessCharacter(StringBuilder result, char previous, char current, int count)
        {
            if (IsRegularDigit(previous) && current != Backspace)
            {
                AppendDecodedCharacter(result, previous, count);
            }
            else if (IsBackspaceOperation(previous, current))
            {
                RemoveLastCharacter(result);
            }
        }

        /// <summary>
        /// Checks if the character is a regular digit (not space or special character).
        /// </summary>
        private static bool IsRegularDigit(char c)
        {
            return c != Space && c != Backspace;
        }

        /// <summary>
        /// Checks if this is a backspace operation.
        /// </summary>
        private static bool IsBackspaceOperation(char previous, char current)
        {
            return (previous == Space || previous == Backspace) && current == Backspace;
        }

        /// <summary>
        /// Appends the decoded character to the result based on keypad mapping.
        /// </summary>
        private static void AppendDecodedCharacter(StringBuilder result, char digit, int count)
        {
            int keypadIndex = digit - '0' - FirstKeypadNumber;

            if (keypadIndex < 0 || keypadIndex >= KeypadMapping.Length)
            {
                throw new ArgumentException($"Invalid keypad digit: {digit}. Only digits 2-9 are valid.");
            }

            char[] availableChars = KeypadMapping[keypadIndex];
            int charIndex = (count - 1) % availableChars.Length;

            result.Append(availableChars[charIndex]);
        }

        /// <summary>
        /// Removes the last character from the result (backspace operation).
        /// </summary>
        private static void RemoveLastCharacter(StringBuilder result)
        {
            if (result.Length > 0)
            {
                result.Length--;
            }
        }
    }
}
*/