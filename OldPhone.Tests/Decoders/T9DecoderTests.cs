using System;
using System.Collections.Generic;
using Xunit;
using OldPhone.Core;
using OldPhone.Decoders;

namespace OldPhone.Tests.Decoders
{
    public class T9DecoderTests
    {
        private readonly IKeypadDecoder _decoder;

        public T9DecoderTests()
        {
            _decoder = new T9Decoder();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithDefaultConfiguration_CreatesDecoder()
        {
            var decoder = new T9Decoder();
            Assert.NotNull(decoder);
        }

        [Fact]
        public void Constructor_WithCustomConfiguration_CreatesDecoder()
        {
            var config = KeypadConfiguration.T9;
            var decoder = new T9Decoder(config);
            Assert.NotNull(decoder);
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new T9Decoder(null));
        }

        #endregion

        #region Basic Single Key Tests

        [Theory]
        [InlineData("2#", "A")]
        [InlineData("22#", "B")]
        [InlineData("222#", "C")]
        public void Decode_Key2_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("3#", "D")]
        [InlineData("33#", "E")]
        [InlineData("333#", "F")]
        public void Decode_Key3_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("4#", "G")]
        [InlineData("44#", "H")]
        [InlineData("444#", "I")]
        public void Decode_Key4_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("5#", "J")]
        [InlineData("55#", "K")]
        [InlineData("555#", "L")]
        public void Decode_Key5_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("6#", "M")]
        [InlineData("66#", "N")]
        [InlineData("666#", "O")]
        public void Decode_Key6_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("7#", "P")]
        [InlineData("77#", "Q")]
        [InlineData("777#", "R")]
        [InlineData("7777#", "S")]
        public void Decode_Key7_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("8#", "T")]
        [InlineData("88#", "U")]
        [InlineData("888#", "V")]
        public void Decode_Key8_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("9#", "W")]
        [InlineData("99#", "X")]
        [InlineData("999#", "Y")]
        [InlineData("9999#", "Z")]
        public void Decode_Key9_ReturnsCorrectLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        #endregion

        #region Wrapping Behavior Tests

        [Theory]
        [InlineData("2222#", "A")]     // Wraps back to A
        [InlineData("22222#", "B")]    // Wraps to B
        [InlineData("222222#", "C")]   // Wraps to C
        [InlineData("2222222#", "A")]  // Wraps back to A again
        public void Decode_Key2Wrapping_ReturnsCorrectLetter(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("77777#", "P")]    // Key 7 has 4 letters, 5 presses wraps to P
        [InlineData("777777#", "Q")]   // 6 presses wraps to Q
        [InlineData("7777777#", "R")]  // 7 presses wraps to R
        [InlineData("77777777#", "S")] // 8 presses wraps to S
        [InlineData("777777777#", "P")]// 9 presses wraps back to P
        public void Decode_Key7Wrapping_ReturnsCorrectLetter(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("99999#", "W")]    // Key 9 has 4 letters, 5 presses wraps to W
        [InlineData("999999999#", "W")]// 9 presses (2 complete cycles + 1) = W
        public void Decode_Key9Wrapping_ReturnsCorrectLetter(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        #endregion

        #region Space Separator Tests

        [Theory]
        [InlineData("2 2#", "AA")]
        [InlineData("2 22#", "AB")]
        [InlineData("22 2#", "BA")]
        [InlineData("22 22#", "BB")]
        public void Decode_SameKeyWithSpace_ReturnsMultipleLetters(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Theory]
        [InlineData("2 2 2#", "AAA")]
        [InlineData("22 22 22#", "BBB")]
        [InlineData("2 22 222#", "ABC")]
        public void Decode_MultipleSpaceSeparations_ReturnsCorrectSequence(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Fact]
        public void Decode_DifferentKeysNoSpace_ReturnsMultipleLetters()
        {
            // Different keys don't need spaces
            string result = _decoder.Decode("23#");
            Assert.Equal("AD", result);
        }

        [Fact]
        public void Decode_MixedSameAndDifferentKeys_ReturnsCorrectText()
        {
            string result = _decoder.Decode("22 233#");
            Assert.Equal("BAE", result);
        }

        #endregion

        #region Backspace Tests

        [Fact]
        public void Decode_SingleBackspace_RemovesLastCharacter()
        {
            string result = _decoder.Decode("2*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceAfterMultipleCharacters_RemovesOne()
        {
            string result = _decoder.Decode("222 2*#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_MultipleBackspaces_RemovesMultipleCharacters()
        {
            string result = _decoder.Decode("222 2 3**#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_BackspaceAll_ReturnsEmptyString()
        {
            string result = _decoder.Decode("222 2***#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceOnEmptyString_DoesNothing()
        {
            string result = _decoder.Decode("*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_MultipleBackspacesOnEmpty_DoesNothing()
        {
            string result = _decoder.Decode("***#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceWithSpace_RemovesLastCharacter()
        {
            string result = _decoder.Decode("222 2 *#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_ConsecutiveBackspaces_RemovesMultiple()
        {
            string result = _decoder.Decode("222 2 3 4***#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_BackspaceMoreThanAvailable_ResultsInEmpty()
        {
            string result = _decoder.Decode("2*******#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceInMiddleOfSequence_Works()
        {
            string result = _decoder.Decode("222 2*3#");
            Assert.Equal("CD", result);
        }

        #endregion

        #region Complex Message Tests

        [Theory]
        [InlineData("44 33555 555666#", "HELLO")]
        [InlineData("9666 777555 3#", "WORLD")]
        [InlineData("222666 3 33#", "CODE")]
        [InlineData("8 337777 8#", "TEST")]
        [InlineData("7 777666 4777 2 6#", "PROGRAM")]
        public void Decode_CommonWords_ReturnsCorrectText(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Fact]
        public void Decode_ComplexMessageWithBackspace_ReturnsCorrectText()
        {
            string result = _decoder.Decode("8 88777444666*664#");
            Assert.Equal("TURING", result);
        }

        [Fact]
        public void Decode_AlphabetSequence_ReturnsAllLetters()
        {
            string result = _decoder.Decode("2 22 222 3 33 333 4 44 444 5 55 555 6 66 666 7 77 777 7777 8 88 888 9 99 999 9999#");
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result);
        }

        [Fact]
        public void Decode_SentenceWithSpaces_ReturnsText()
        {
            // HI (44 444) space is encoded as part of the sequence
            string result = _decoder.Decode("44 444#");
            Assert.Equal("HI", result);
        }

        #endregion

        #region Edge Cases - Input Validation

        [Fact]
        public void Decode_NullInput_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _decoder.Decode(null));
            Assert.Equal("input", ex.ParamName);
        }

        [Fact]
        public void Decode_EmptyString_ReturnsEmptyString()
        {
            string result = _decoder.Decode("");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_OnlyEndMarker_ReturnsEmptyString()
        {
            string result = _decoder.Decode("#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_OnlySpaces_ReturnsEmptyString()
        {
            string result = _decoder.Decode("   #");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_OnlyBackspaces_ReturnsEmptyString()
        {
            string result = _decoder.Decode("***#");
            Assert.Equal("", result);
        }

        #endregion

        #region Invalid Input Tests

        [Theory]
        [InlineData("0#", '0')]
        [InlineData("1#", '1')]
        public void Decode_InvalidDigits_ThrowsKeypadException(string input, char expectedChar)
        {
            var ex = Assert.Throws<KeypadException>(() => _decoder.Decode(input));
            Assert.NotNull(ex.InvalidCharacter);
            Assert.Equal(expectedChar, ex.InvalidCharacter.Value);
            Assert.Contains("Invalid keypad digit", ex.Message);
            Assert.Contains(expectedChar.ToString(), ex.Message);
        }

        [Theory]
        [InlineData("a#", 'a')]
        [InlineData("Z#", 'Z')]
        [InlineData("!#", '!')]
        [InlineData("@#", '@')]
        public void Decode_InvalidCharacters_ThrowsKeypadException(string input, char expectedChar)
        {
            var ex = Assert.Throws<KeypadException>(() => _decoder.Decode(input));
            Assert.Equal(expectedChar, ex.InvalidCharacter);
        }

        [Fact]
        public void Decode_InvalidCharacterAtStart_ThrowsImmediately()
        {
            var ex = Assert.Throws<KeypadException>(() => _decoder.Decode("x222#"));
            Assert.Equal('x', ex.InvalidCharacter);
        }

        [Fact]
        public void Decode_InvalidCharacterInMiddle_ThrowsKeypadException()
        {
            var ex = Assert.Throws<KeypadException>(() => _decoder.Decode("222x333#"));
            Assert.Equal('x', ex.InvalidCharacter);
        }

        [Fact]
        public void Decode_InvalidCharacterBeforeEnd_ThrowsKeypadException()
        {
            var ex = Assert.Throws<KeypadException>(() => _decoder.Decode("222 333y#"));
            Assert.Equal('y', ex.InvalidCharacter);
        }

        #endregion

        #region Control Character Tests

        [Fact]
        public void Decode_MultipleEndMarkers_FirstEndsProcessing()
        {
            // Everything after first # should be ignored (or processed as is)
            string result = _decoder.Decode("222##");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_MultipleSpaces_TreatedAsSeparators()
        {
            string result = _decoder.Decode("2  2#");
            Assert.Equal("AA", result);
        }

        [Fact]
        public void Decode_SpaceAtStart_Ignored()
        {
            string result = _decoder.Decode(" 2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_BackspaceAtStart_DoesNothing()
        {
            string result = _decoder.Decode("*2#");
            Assert.Equal("A", result);
        }

        #endregion

        #region Custom Configuration Tests

        [Fact]
        public void Decode_WithCustomConfiguration_UsesCustomMapping()
        {
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'X', 'Y', 'Z' } },
                { '3', new[] { '1', '2', '3' } }
            };

            var config = new KeypadConfiguration(customMapping);
            var decoder = new T9Decoder(config);

            string result = decoder.Decode("2#");
            Assert.Equal("X", result);
        }

        [Fact]
        public void Decode_WithCustomSeparator_UsesCustomSeparator()
        {
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B', 'C' } }
            };

            var config = new KeypadConfiguration(customMapping)
            {
                SeparatorChar = '-'
            };

            var decoder = new T9Decoder(config);
            string result = decoder.Decode("2-22#");
            Assert.Equal("AB", result);
        }

        [Fact]
        public void Decode_WithCustomBackspace_UsesCustomBackspace()
        {
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(customMapping)
            {
                BackspaceChar = 'X'
            };

            var decoder = new T9Decoder(config);
            string result = decoder.Decode("22X#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_WithCustomEndMarker_UsesCustomEndMarker()
        {
            var customMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(customMapping)
            {
                EndMarker = '!'
            };

            var decoder = new T9Decoder(config);
            string result = decoder.Decode("22!");
            Assert.Equal("B", result);
        }

        #endregion

        #region Performance and Stress Tests

        [Fact]
        public void Decode_VeryLongInput_CompletesSuccessfully()
        {
            // Create a very long input (1000 characters)
            var input = string.Concat(System.Linq.Enumerable.Repeat("222 ", 250)) + "#";
            string result = _decoder.Decode(input);
            Assert.Equal(250, result.Length);
            Assert.All(result, c => Assert.Equal('C', c));
        }

        [Fact]
        public void Decode_ManyConsecutivePresses_HandlesCorrectly()
        {
            // 100 presses of key 2 (should wrap many times)
            var input = new string('2', 100) + "#";
            string result = _decoder.Decode(input);
            // 100 % 3 = 1, so it should be 'A' (index 0)
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_AlternatingKeysAndSpaces_HandlesCorrectly()
        {
            string input = "2 3 4 5 6 7 8 9#";
            string result = _decoder.Decode(input);
            Assert.Equal("ADGJMPTW", result);
        }

        [Fact]
        public void Decode_ComplexBackspacePattern_HandlesCorrectly()
        {
            string input = "222 2 3*4*5*#";
            string result = _decoder.Decode(input);
            Assert.Equal("CA", result);
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Decode_SingleCharacterInput_ReturnsEmpty()
        {
            string result = _decoder.Decode("#");
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_TwoCharacterInput_ReturnsSingleLetter()
        {
            string result = _decoder.Decode("2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_MaxPressesForEachKey_ReturnsLastLetter()
        {
            Assert.Equal("C", _decoder.Decode("222#"));    // Key 2: 3 letters
            Assert.Equal("F", _decoder.Decode("333#"));    // Key 3: 3 letters
            Assert.Equal("I", _decoder.Decode("444#"));    // Key 4: 3 letters
            Assert.Equal("L", _decoder.Decode("555#"));    // Key 5: 3 letters
            Assert.Equal("O", _decoder.Decode("666#"));    // Key 6: 3 letters
            Assert.Equal("S", _decoder.Decode("7777#"));   // Key 7: 4 letters
            Assert.Equal("V", _decoder.Decode("888#"));    // Key 8: 3 letters
            Assert.Equal("Z", _decoder.Decode("9999#"));   // Key 9: 4 letters
        }

        #endregion

        #region Real-world Scenarios

        [Theory]
        [InlineData("4 666 9*#", "GO")]        // Typo correction
        [InlineData("9 44 99*#", "WH")]        // Backspace at end
        [InlineData("222 2 8*#", "CA")]         // Delete middle character
        public void Decode_TypoCorrection_ReturnsCorrectText(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Fact]
        public void Decode_QuickMessage_YES()
        {
            string result = _decoder.Decode("999 337777#");
            Assert.Equal("YES", result);
        }

        [Fact]
        public void Decode_QuickMessage_NO()
        {
            string result = _decoder.Decode("66 666#");
            Assert.Equal("NO", result);
        }

        [Fact]
        public void Decode_QuickMessage_OK()
        {
            string result = _decoder.Decode("666 55#");
            Assert.Equal("OK", result);
        }

        [Fact]
        public void Decode_Name_ALICE()
        {
            string result = _decoder.Decode("2 555 444222 33#");
            Assert.Equal("ALICE", result);
        }

        [Fact]
        public void Decode_Name_BOB()
        {
            string result = _decoder.Decode("22 666 22#");
            Assert.Equal("BOB", result);
        }

        #endregion
    }
}
/*
using System;
using Xunit;
using OldPhone.Core;
using OldPhone.Decoders;

namespace OldPhone.Tests.Decoders
{
    public class T9DecoderTests
    {
        private readonly IKeypadDecoder _decoder;

        public T9DecoderTests()
        {
            _decoder = new T9Decoder();
        }

        [Fact]
        public void Decode_SimpleInput_ReturnsCorrectText()
        {
            string result = _decoder.Decode("2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_ComplexMessage_ReturnsCorrectText()
        {
            string result = _decoder.Decode("8 88777444666*664#");
            Assert.Equal("TURING", result);
        }

        [Theory]
        [InlineData("44 33555 555666#", "HELLO")]
        [InlineData("9666 777555 3#", "WORLD")]
        [InlineData("222666 3 33#", "CODE")]
        public void Decode_CommonWords_ReturnsCorrectText(string input, string expected)
        {
            Assert.Equal(expected, _decoder.Decode(input));
        }

        [Fact]
        public void Decode_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _decoder.Decode(null));
        }

        [Fact]
        public void Decode_EmptyString_ReturnsEmptyString()
        {
            Assert.Equal("", _decoder.Decode(""));
        }

        [Fact]
        public void Decode_InvalidDigit_ThrowsKeypadException()
        {
            var ex = Assert.Throws<KeypadException>(() => _decoder.Decode("0#"));
            Assert.NotNull(ex.InvalidCharacter);
            Assert.Equal('0', ex.InvalidCharacter);
        }

        [Fact]
        public void Decode_WithBackspace_RemovesCharacter()
        {
            string result = _decoder.Decode("222 2*#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_WrappingBehavior_ReturnsFirstLetter()
        {
            string result = _decoder.Decode("2222#");
            Assert.Equal("A", result);
        }
    }
}
*/
