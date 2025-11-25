using System;
using System.Text;
using OldPhone.Core;

namespace OldPhone.Decoders
{
    /// <summary>
    /// Decodes T9-style keypad input sequences into text.
    /// </summary>
    public sealed class T9Decoder : IKeypadDecoder
    {
        private readonly KeypadConfiguration _config;

        /// <summary>
        /// Creates a new T9 decoder with the standard T9 configuration.
        /// </summary>
        public T9Decoder() : this(KeypadConfiguration.T9)
        {
        }

        /// <summary>
        /// Creates a new T9 decoder with a custom configuration.
        /// </summary>
        /// <param name="configuration">The keypad configuration to use.</param>
        /// <exception cref="ArgumentNullException">When configuration is null.</exception>
        public T9Decoder(KeypadConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc/>
        public string Decode(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var result = new StringBuilder();
            var state = new DecoderState(input[0], _config);

            for (int i = 1; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (state.ShouldIncrementCount(currentChar, _config))
                {
                    state.IncrementCount();
                }
                else
                {
                    ProcessAccumulatedInput(result, state, currentChar);
                    state.Reset(currentChar);
                }
            }

            return result.ToString();
        }

        private void ProcessAccumulatedInput(StringBuilder result, DecoderState state, char nextChar)
        {
            if (ShouldDecodeCharacter(state.CurrentChar, nextChar))
            {
                AppendDecodedCharacter(result, state.CurrentChar, state.Count);
            }
            else if (ShouldPerformBackspace(state.CurrentChar, nextChar))
            {
                PerformBackspace(result);
            }
        }

        private bool ShouldDecodeCharacter(char current, char next)
        {
            return _config.IsValidKeypadDigit(current) && next != _config.BackspaceChar;
        }

        private bool ShouldPerformBackspace(char current, char next)
        {
            return (current == _config.SeparatorChar || current == _config.BackspaceChar) 
                   && next == _config.BackspaceChar;
        }

        private void AppendDecodedCharacter(StringBuilder result, char digit, int pressCount)
        {
            char[] availableChars = _config.KeyMapping[digit];
            int charIndex = (pressCount - 1) % availableChars.Length;
            result.Append(availableChars[charIndex]);
        }

        private void PerformBackspace(StringBuilder result)
        {
            if (result.Length > 0)
            {
                result.Length--;
            }
        }


        /// <summary>
        /// Internal state tracker for the decoding process.
        /// </summary>
        private sealed class DecoderState
        {
            private readonly KeypadConfiguration _config;
            public char CurrentChar { get; private set; }
            public int Count { get; private set; }

            public DecoderState(char initialChar, KeypadConfiguration config)
            {
                _config = config ?? throw new ArgumentNullException(nameof(config));
                ValidateCharacter(initialChar);
                CurrentChar = initialChar;
                Count = 1;
            }

            public bool ShouldIncrementCount(char nextChar, KeypadConfiguration config)
            {
                return CurrentChar == nextChar && CurrentChar != config.BackspaceChar;
            }

            public void IncrementCount() => Count++;

            public void Reset(char newChar)
            {
                ValidateCharacter(newChar);
                CurrentChar = newChar;
                Count = 1;
            }
            
            private void ValidateCharacter(char newChar)
            {
                if (!_config.IsControlChar(newChar) && !_config.IsValidKeypadDigit(newChar))
                {
                    throw new KeypadException(
                        $"Invalid keypad digit: '{newChar}'. Valid digits are: {string.Join(", ", _config.KeyMapping.Keys)}", 
                        newChar);
                }
            }
        }
    }
}