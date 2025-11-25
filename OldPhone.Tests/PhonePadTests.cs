using System;
using System.Collections.Generic;
using Xunit;
using OldPhone;
using OldPhone.Core;
using OldPhone.Decoders;

namespace OldPhone.Tests
{
    public class PhonePadFacadeTests
    {
        #region Basic Decode Tests

        [Fact]
        public void Decode_WithSimpleInput_ReturnsCorrectText()
        {
            string result = PhonePad.Decode("2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_WithComplexInput_ReturnsCorrectText()
        {
            string result = PhonePad.Decode("44 33555 555666#");
            Assert.Equal("HELLO", result);
        }

        [Fact]
        public void Decode_UsesDefaultT9Configuration()
        {
            string result = PhonePad.Decode("222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_WithNullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PhonePad.Decode(null));
        }

        [Fact]
        public void Decode_WithEmptyString_ReturnsEmptyString()
        {
            string result = PhonePad.Decode("");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_WithInvalidInput_ThrowsKeypadException()
        {
            Assert.Throws<KeypadException>(() => PhonePad.Decode("0#"));
        }

        #endregion

        #region Decode with Custom Decoder Tests

        [Fact]
        public void Decode_WithCustomDecoder_UsesProvidedDecoder()
        {
            var config = KeypadConfiguration.T9;
            var decoder = new T9Decoder(config);

            string result = PhonePad.Decode("44 444#", decoder);
            Assert.Equal("HI", result);
        }

        [Fact]
        public void Decode_WithCustomDecoder_NullDecoder_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => PhonePad.Decode("2#", null));
        }

        [Fact]
        public void Decode_WithCustomConfiguredDecoder_UsesCustomConfiguration()
        {
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'X', 'Y', 'Z' } }
            };

            var config = new KeypadConfiguration(customMapping);
            var decoder = new T9Decoder(config);

            string result = PhonePad.Decode("2#", decoder);
            Assert.Equal("X", result);
        }

        #endregion

        #region CreateDecoder Tests

        [Fact]
        public void CreateDecoder_WithConfiguration_ReturnsDecoder()
        {
            var config = KeypadConfiguration.T9;
            var decoder = PhonePad.CreateDecoder(config);

            Assert.NotNull(decoder);
            Assert.IsAssignableFrom<IKeypadDecoder>(decoder);
        }

        [Fact]
        public void CreateDecoder_ReturnsT9Decoder()
        {
            var config = KeypadConfiguration.T9;
            var decoder = PhonePad.CreateDecoder(config);

            Assert.IsType<T9Decoder>(decoder);
        }

        [Fact]
        public void CreateDecoder_WithCustomConfig_CreatesDecoderWithConfig()
        {
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } },
                { '3', new[] { 'C', 'D' } }
            };

            var config = new KeypadConfiguration(customMapping);
            var decoder = PhonePad.CreateDecoder(config);

            // Test that the decoder uses the custom config
            string result = decoder.Decode("2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void CreateDecoder_WithNullConfig_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PhonePad.CreateDecoder(null));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void FacadeWorkflow_CreateDecoderThenUse_Works()
        {
            // Create decoder via facade
            var decoder = PhonePad.CreateDecoder(KeypadConfiguration.T9);

            // Use decoder directly
            string result1 = decoder.Decode("222#");
            Assert.Equal("C", result1);

            // Use decoder via facade
            string result2 = PhonePad.Decode("333#", decoder);
            Assert.Equal("F", result2);
        }

        [Fact]
        public void FacadeWorkflow_MultipleDecodeCallsWithDefaultDecoder_Works()
        {
            string result1 = PhonePad.Decode("2#");
            string result2 = PhonePad.Decode("22#");
            string result3 = PhonePad.Decode("222#");

            Assert.Equal("A", result1);
            Assert.Equal("B", result2);
            Assert.Equal("C", result3);
        }

        [Fact]
        public void FacadeWorkflow_MixingDefaultAndCustomDecoders_Works()
        {
            // Use default decoder
            string result1 = PhonePad.Decode("2#");
            Assert.Equal("A", result1);

            // Create custom decoder
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'X' } }
            };
            var config = new KeypadConfiguration(customMapping);
            var customDecoder = PhonePad.CreateDecoder(config);

            // Use custom decoder
            string result2 = PhonePad.Decode("2#", customDecoder);
            Assert.Equal("X", result2);

            // Use default decoder again
            string result3 = PhonePad.Decode("2#");
            Assert.Equal("A", result3);
        }

        #endregion

        #region Real-world Usage Patterns

        [Fact]
        public void UsagePattern_QuickOneLineDecode()
        {
            // Simplest usage pattern
            Assert.Equal("HI", PhonePad.Decode("44 444#"));
        }

        [Fact]
        public void UsagePattern_CustomConfigurationSetup()
        {
            // More advanced setup
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B', 'C' } },
                { '3', new[] { 'D', 'E', 'F' } }
            };

            var config = new KeypadConfiguration(customMapping)
            {
                SeparatorChar = '-',
                BackspaceChar = 'X'
            };

            var decoder = PhonePad.CreateDecoder(config);
            string result = PhonePad.Decode("2-22-222X#", decoder);
            Assert.Equal("AB", result);
        }

        [Fact]
        public void UsagePattern_ReuseDecoder()
        {
            var decoder = PhonePad.CreateDecoder(KeypadConfiguration.T9);

            var messages = new[] { "44 33555 555666#", "9666 777555 3#", "222666 3 33#" };
            var expected = new[] { "HELLO", "WORLD", "CODE" };

            for (int i = 0; i < messages.Length; i++)
            {
                string result = PhonePad.Decode(messages[i], decoder);
                Assert.Equal(expected[i], result);
            }
        }

        [Fact]
        public void UsagePattern_ErrorHandling()
        {
            try
            {
                PhonePad.Decode("0#");
                Assert.True(false, "Should have thrown exception");
            }
            catch (KeypadException ex)
            {
                Assert.NotNull(ex.InvalidCharacter);
                Assert.Equal('0', ex.InvalidCharacter.Value);
            }
        }

        #endregion

        #region Consistency Tests

        [Theory]
        [InlineData("2#")]
        [InlineData("44 444#")]
        [InlineData("8 88777444666*664#")]
        public void Decode_MultipleCallsSameInput_ReturnsSameResult(string input)
        {
            string result1 = PhonePad.Decode(input);
            string result2 = PhonePad.Decode(input);
            string result3 = PhonePad.Decode(input);

            Assert.Equal(result1, result2);
            Assert.Equal(result2, result3);
        }

        [Fact]
        public void Decode_DefaultDecoderVsCreateDecoder_ProduceSameResults()
        {
            string input = "44 33555 555666#";

            string result1 = PhonePad.Decode(input);
            
            var decoder = PhonePad.CreateDecoder(KeypadConfiguration.T9);
            string result2 = PhonePad.Decode(input, decoder);

            Assert.Equal(result1, result2);
        }

        #endregion

        #region Thread Safety Considerations

        [Fact]
        public void Decode_ConcurrentCalls_AllSucceed()
        {
            // Simple test to ensure no obvious thread safety issues
            var inputs = new[] { "2#", "22#", "222#", "3#", "33#" };
            var expected = new[] { "A", "B", "C", "D", "E" };

            System.Threading.Tasks.Parallel.For(0, 100, i =>
            {
                int index = i % inputs.Length;
                string result = PhonePad.Decode(inputs[index]);
                Assert.Equal(expected[index], result);
            });
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Decode_WithOnlyControlCharacters_ReturnsEmpty()
        {
            Assert.Equal("", PhonePad.Decode("   ***###"));
        }

        [Fact]
        public void Decode_AllKeys_ProducesAllLetters()
        {
            string input = "2 22 222 3 33 333 4 44 444 5 55 555 6 66 666 7 77 777 7777 8 88 888 9 99 999 9999#";
            string result = PhonePad.Decode(input);
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result);
        }

        #endregion
    }
}
/*
using Xunit;
using OldPhone.Core;

namespace OldPhone.Tests
{
    public class PhonePadTests
    {
        [Fact]
        public void Decode_WithDefaultDecoder_ReturnsCorrectText()
        {
            string result = PhonePad.Decode("44 444#");
            Assert.Equal("HI", result);
        }

        [Fact]
        public void Decode_WithCustomDecoder_ReturnsCorrectText()
        {
            var config = KeypadConfiguration.T9;
            var decoder = PhonePad.CreateDecoder(config);
            
            string result = PhonePad.Decode("44 444#", decoder);
            Assert.Equal("HI", result);
        }

        [Fact]
        public void CreateDecoder_WithConfiguration_ReturnsDecoder()
        {
            var config = KeypadConfiguration.T9;
            var decoder = PhonePad.CreateDecoder(config);
            
            Assert.NotNull(decoder);
            Assert.IsAssignableFrom<IKeypadDecoder>(decoder);
        }
    }
}
using System;
using Xunit;

namespace OldPhone.Tests
{
    public class PhonePadTests
    {
        #region Basic Functionality Tests

        [Fact]
        public void Decode_SingleLetter_ReturnsCorrectLetter()
        {
            // Arrange & Act
            string result = PhonePad.Decode("2#");

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_MultiplePressesSameKey_ReturnsCorrectLetter()
        {
            // Arrange & Act
            string result = PhonePad.Decode("22#");

            // Assert
            Assert.Equal("B", result);
        }

        [Fact]
        public void Decode_ThreePressesSameKey_ReturnsCorrectLetter()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_SpaceSeparatedLetters_ReturnsMultipleLetters()
        {
            // Arrange & Act
            string result = PhonePad.Decode("44 444#");

            // Assert
            Assert.Equal("HI", result);
        }

        [Fact]
        public void Decode_ComplexMessage_ReturnsCorrectText()
        {
            // Arrange & Act
            string result = PhonePad.Decode("8 88777444666*664#");

            // Assert
            Assert.Equal("TURING", result);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Decode_EmptyString_ReturnsEmptyString()
        {
            // Arrange & Act
            string result = PhonePad.Decode("");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_OnlyEndMarker_ReturnsEmptyString()
        {
            // Arrange & Act
            string result = PhonePad.Decode("#");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_NullInput_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => PhonePad.Decode(null));
        }

        #endregion

        #region Backspace Tests

        [Fact]
        public void Decode_SingleBackspace_RemovesLastCharacter()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222*#");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceAfterMultipleLetters_RemovesLastCharacter()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222 2*#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_MultipleBackspaces_RemovesMultipleCharacters()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222 2 3**#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_BackspaceOnEmptyString_DoesNothing()
        {
            // Arrange & Act
            string result = PhonePad.Decode("*#");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceWithSpace_RemovesLastCharacter()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222 2 *#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_ConsecutiveBackspaces_RemovesMultipleCharacters()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222 2 3***#");

            // Assert
            Assert.Equal("", result);
        }

        #endregion

        #region All Keypad Mappings Tests

        [Theory]
        [InlineData("2#", "A")]
        [InlineData("22#", "B")]
        [InlineData("222#", "C")]
        public void Decode_Key2_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("3#", "D")]
        [InlineData("33#", "E")]
        [InlineData("333#", "F")]
        public void Decode_Key3_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("4#", "G")]
        [InlineData("44#", "H")]
        [InlineData("444#", "I")]
        public void Decode_Key4_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("5#", "J")]
        [InlineData("55#", "K")]
        [InlineData("555#", "L")]
        public void Decode_Key5_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("6#", "M")]
        [InlineData("66#", "N")]
        [InlineData("666#", "O")]
        public void Decode_Key6_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("7#", "P")]
        [InlineData("77#", "Q")]
        [InlineData("777#", "R")]
        [InlineData("7777#", "S")]
        public void Decode_Key7_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("8#", "T")]
        [InlineData("88#", "U")]
        [InlineData("888#", "V")]
        public void Decode_Key8_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Theory]
        [InlineData("9#", "W")]
        [InlineData("99#", "X")]
        [InlineData("999#", "Y")]
        [InlineData("9999#", "Z")]
        public void Decode_Key9_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        #endregion

        #region Wrapping Tests

        [Fact]
        public void Decode_ExceedsAvailableLetters_WrapsAround()
        {
            // Key 2 has A, B, C - pressing 4 times should wrap to A
            // Arrange & Act
            string result = PhonePad.Decode("2222#");

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_Key7FivePresses_WrapsToP()
        {
            // Key 7 has P, Q, R, S - pressing 5 times should wrap to P
            // Arrange & Act
            string result = PhonePad.Decode("77777#");

            // Assert
            Assert.Equal("P", result);
        }

        #endregion

        #region Invalid Input Tests

        [Theory]
        [InlineData("0#")]
        [InlineData("1#")]
        public void Decode_InvalidKeypadDigits_ThrowsArgumentException(string input)
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => PhonePad.Decode(input));
        }

        [Fact]
        public void Decode_InvalidCharacter_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => PhonePad.Decode("a#"));
        }

        #endregion

        #region Real World Examples

        [Theory]
        [InlineData("44 33555 555666#", "HELLO")]
        [InlineData("9666 777555 3#", "WORLD")]
        [InlineData("222666 3 33#", "CODE")]
        [InlineData("8 337777 8#", "TEST")]
        public void Decode_CommonWords_ReturnsCorrectText(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.Decode(input));
        }

        [Fact]
        public void Decode_AlphabetSequence_ReturnsABC()
        {
            // Arrange & Act
            string result = PhonePad.Decode("2 22 222#");

            // Assert
            Assert.Equal("ABC", result);
        }

        [Fact]
        public void Decode_MixedWithBackspace_ReturnsCorrectText()
        {
            // Arrange & Act
            string result = PhonePad.Decode("44 444* 44 444#");

            // Assert
            Assert.Equal("HHI", result);
        }

        #endregion

        #region Performance and Stress Tests

        [Fact]
        public void Decode_LongInput_CompletesSuccessfully()
        {
            // Arrange - Create a long input string
            string longInput = "2 22 222 3 33 333 4 44 444 5 55 555 6 66 666 7 77 777 7777 8 88 888 9 99 999 9999#";

            // Act
            string result = PhonePad.Decode(longInput);

            // Assert
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result);
        }

        [Fact]
        public void Decode_ManyBackspaces_HandlesCorrectly()
        {
            // Arrange & Act
            string result = PhonePad.Decode("222 2 3 4 5**********#");

            // Assert
            Assert.Equal("", result);
        }

        #endregion
    }
}
*/