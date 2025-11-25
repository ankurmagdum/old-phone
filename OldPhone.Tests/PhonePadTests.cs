using System;
using Xunit;

namespace OldPhone.Tests
{
    public class PhonePadTests
    {
        #region Basic Functionality Tests

        [Fact]
        public void DecodeKeypadInput_SingleLetter_ReturnsCorrectLetter()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("2#");

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void DecodeKeypadInput_MultiplePressesSameKey_ReturnsCorrectLetter()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("22#");

            // Assert
            Assert.Equal("B", result);
        }

        [Fact]
        public void DecodeKeypadInput_ThreePressesSameKey_ReturnsCorrectLetter()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void DecodeKeypadInput_SpaceSeparatedLetters_ReturnsMultipleLetters()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("44 444#");

            // Assert
            Assert.Equal("HI", result);
        }

        [Fact]
        public void DecodeKeypadInput_ComplexMessage_ReturnsCorrectText()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("8 88777444666*664#");

            // Assert
            Assert.Equal("TURING", result);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void DecodeKeypadInput_EmptyString_ReturnsEmptyString()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void DecodeKeypadInput_OnlyEndMarker_ReturnsEmptyString()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("#");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void DecodeKeypadInput_NullInput_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => PhonePad.DecodeKeypadInput(null));
        }

        #endregion

        #region Backspace Tests

        [Fact]
        public void DecodeKeypadInput_SingleBackspace_RemovesLastCharacter()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222*#");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void DecodeKeypadInput_BackspaceAfterMultipleLetters_RemovesLastCharacter()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222 2*#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void DecodeKeypadInput_MultipleBackspaces_RemovesMultipleCharacters()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222 2 3**#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void DecodeKeypadInput_BackspaceOnEmptyString_DoesNothing()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("*#");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void DecodeKeypadInput_BackspaceWithSpace_RemovesLastCharacter()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222 2 *#");

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void DecodeKeypadInput_ConsecutiveBackspaces_RemovesMultipleCharacters()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222 2 3***#");

            // Assert
            Assert.Equal("", result);
        }

        #endregion

        #region All Keypad Mappings Tests

        [Theory]
        [InlineData("2#", "A")]
        [InlineData("22#", "B")]
        [InlineData("222#", "C")]
        public void DecodeKeypadInput_Key2_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("3#", "D")]
        [InlineData("33#", "E")]
        [InlineData("333#", "F")]
        public void DecodeKeypadInput_Key3_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("4#", "G")]
        [InlineData("44#", "H")]
        [InlineData("444#", "I")]
        public void DecodeKeypadInput_Key4_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("5#", "J")]
        [InlineData("55#", "K")]
        [InlineData("555#", "L")]
        public void DecodeKeypadInput_Key5_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("6#", "M")]
        [InlineData("66#", "N")]
        [InlineData("666#", "O")]
        public void DecodeKeypadInput_Key6_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("7#", "P")]
        [InlineData("77#", "Q")]
        [InlineData("777#", "R")]
        [InlineData("7777#", "S")]
        public void DecodeKeypadInput_Key7_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("8#", "T")]
        [InlineData("88#", "U")]
        [InlineData("888#", "V")]
        public void DecodeKeypadInput_Key8_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Theory]
        [InlineData("9#", "W")]
        [InlineData("99#", "X")]
        [InlineData("999#", "Y")]
        [InlineData("9999#", "Z")]
        public void DecodeKeypadInput_Key9_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        #endregion

        #region Wrapping Tests

        [Fact]
        public void DecodeKeypadInput_ExceedsAvailableLetters_WrapsAround()
        {
            // Key 2 has A, B, C - pressing 4 times should wrap to A
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("2222#");

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void DecodeKeypadInput_Key7FivePresses_WrapsToP()
        {
            // Key 7 has P, Q, R, S - pressing 5 times should wrap to P
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("77777#");

            // Assert
            Assert.Equal("P", result);
        }

        #endregion

        #region Invalid Input Tests

        [Theory]
        [InlineData("0#")]
        [InlineData("1#")]
        public void DecodeKeypadInput_InvalidKeypadDigits_ThrowsArgumentException(string input)
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => PhonePad.DecodeKeypadInput(input));
        }

        [Fact]
        public void DecodeKeypadInput_InvalidCharacter_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => PhonePad.DecodeKeypadInput("a#"));
        }

        #endregion

        #region Real World Examples

        [Theory]
        [InlineData("44 33555 555666#", "HELLO")]
        [InlineData("9666 777555 3#", "WORLD")]
        [InlineData("222666 3 33#", "CODE")]
        [InlineData("8 337777 8#", "TEST")]
        public void DecodeKeypadInput_CommonWords_ReturnsCorrectText(string input, string expected)
        {
            Assert.Equal(expected, PhonePad.DecodeKeypadInput(input));
        }

        [Fact]
        public void DecodeKeypadInput_AlphabetSequence_ReturnsABC()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("2 22 222#");

            // Assert
            Assert.Equal("ABC", result);
        }

        [Fact]
        public void DecodeKeypadInput_MixedWithBackspace_ReturnsCorrectText()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("44 444* 44 444#");

            // Assert
            Assert.Equal("HHI", result);
        }

        #endregion

        #region Performance and Stress Tests

        [Fact]
        public void DecodeKeypadInput_LongInput_CompletesSuccessfully()
        {
            // Arrange - Create a long input string
            string longInput = "2 22 222 3 33 333 4 44 444 5 55 555 6 66 666 7 77 777 7777 8 88 888 9 99 999 9999#";

            // Act
            string result = PhonePad.DecodeKeypadInput(longInput);

            // Assert
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result);
        }

        [Fact]
        public void DecodeKeypadInput_ManyBackspaces_HandlesCorrectly()
        {
            // Arrange & Act
            string result = PhonePad.DecodeKeypadInput("222 2 3 4 5**********#");

            // Assert
            Assert.Equal("", result);
        }

        #endregion
    }
}
