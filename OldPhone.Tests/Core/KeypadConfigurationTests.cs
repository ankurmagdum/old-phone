using System;
using System.Collections.Generic;
using Xunit;
using OldPhone.Core;

namespace OldPhone.Tests.Core
{
    public class KeypadConfigurationTests
    {
        #region Default T9 Configuration Tests

        [Fact]
        public void T9_IsNotNull()
        {
            Assert.NotNull(KeypadConfiguration.T9);
        }

        [Fact]
        public void T9_HasEightKeys()
        {
            Assert.Equal(8, KeypadConfiguration.T9.KeyMapping.Count);
        }

        [Fact]
        public void T9_HasCorrectKey2Mapping()
        {
            Assert.Equal(new[] { 'A', 'B', 'C' }, KeypadConfiguration.T9.KeyMapping['2']);
        }

        [Fact]
        public void T9_HasCorrectKey3Mapping()
        {
            Assert.Equal(new[] { 'D', 'E', 'F' }, KeypadConfiguration.T9.KeyMapping['3']);
        }

        [Fact]
        public void T9_HasCorrectKey4Mapping()
        {
            Assert.Equal(new[] { 'G', 'H', 'I' }, KeypadConfiguration.T9.KeyMapping['4']);
        }

        [Fact]
        public void T9_HasCorrectKey5Mapping()
        {
            Assert.Equal(new[] { 'J', 'K', 'L' }, KeypadConfiguration.T9.KeyMapping['5']);
        }

        [Fact]
        public void T9_HasCorrectKey6Mapping()
        {
            Assert.Equal(new[] { 'M', 'N', 'O' }, KeypadConfiguration.T9.KeyMapping['6']);
        }

        [Fact]
        public void T9_HasCorrectKey7Mapping()
        {
            Assert.Equal(new[] { 'P', 'Q', 'R', 'S' }, KeypadConfiguration.T9.KeyMapping['7']);
        }

        [Fact]
        public void T9_HasCorrectKey8Mapping()
        {
            Assert.Equal(new[] { 'T', 'U', 'V' }, KeypadConfiguration.T9.KeyMapping['8']);
        }

        [Fact]
        public void T9_HasCorrectKey9Mapping()
        {
            Assert.Equal(new[] { 'W', 'X', 'Y', 'Z' }, KeypadConfiguration.T9.KeyMapping['9']);
        }

        [Fact]
        public void T9_HasDefaultSeparatorChar()
        {
            Assert.Equal(' ', KeypadConfiguration.T9.SeparatorChar);
        }

        [Fact]
        public void T9_HasDefaultBackspaceChar()
        {
            Assert.Equal('*', KeypadConfiguration.T9.BackspaceChar);
        }

        [Fact]
        public void T9_HasDefaultEndMarker()
        {
            Assert.Equal('#', KeypadConfiguration.T9.EndMarker);
        }

        [Fact]
        public void T9_IsSingleton_ReturnsSameInstance()
        {
            var instance1 = KeypadConfiguration.T9;
            var instance2 = KeypadConfiguration.T9;
            Assert.Same(instance1, instance2);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidMapping_CreatesConfiguration()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping);

            Assert.NotNull(config);
            Assert.Equal(mapping, config.KeyMapping);
        }

        [Fact]
        public void Constructor_WithMultipleKeys_CreatesConfiguration()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } },
                { '3', new[] { 'C', 'D' } },
                { '4', new[] { 'E', 'F' } }
            };

            var config = new KeypadConfiguration(mapping);

            Assert.Equal(3, config.KeyMapping.Count);
        }

        [Fact]
        public void Constructor_WithNullMapping_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new KeypadConfiguration(null));
            Assert.Equal("keyMapping", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithEmptyMapping_ThrowsArgumentException()
        {
            var mapping = new Dictionary<char, char[]>();
            var ex = Assert.Throws<ArgumentException>(() => new KeypadConfiguration(mapping));
            Assert.Contains("cannot be empty", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullValueInMapping_ThrowsArgumentException()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', null }
            };

            var ex = Assert.Throws<ArgumentException>(() => new KeypadConfiguration(mapping));
            Assert.Contains("cannot be null or empty", ex.Message);
        }

        [Fact]
        public void Constructor_WithEmptyArrayInMapping_ThrowsArgumentException()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new char[] { } }
            };

            var ex = Assert.Throws<ArgumentException>(() => new KeypadConfiguration(mapping));
            Assert.Contains("cannot be null or empty", ex.Message);
        }

        #endregion

        #region Property Initialization Tests

        [Fact]
        public void SeparatorChar_CanBeCustomized()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping)
            {
                SeparatorChar = '-'
            };

            Assert.Equal('-', config.SeparatorChar);
        }

        [Fact]
        public void BackspaceChar_CanBeCustomized()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping)
            {
                BackspaceChar = 'X'
            };

            Assert.Equal('X', config.BackspaceChar);
        }

        [Fact]
        public void EndMarker_CanBeCustomized()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping)
            {
                EndMarker = '!'
            };

            Assert.Equal('!', config.EndMarker);
        }

        [Fact]
        public void AllControlChars_CanBeCustomizedTogether()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping)
            {
                SeparatorChar = '|',
                BackspaceChar = '<',
                EndMarker = '>'
            };

            Assert.Equal('|', config.SeparatorChar);
            Assert.Equal('<', config.BackspaceChar);
            Assert.Equal('>', config.EndMarker);
        }

        #endregion

        #region IsValidKeypadDigit Tests

        [Theory]
        [InlineData('2', true)]
        [InlineData('3', true)]
        [InlineData('4', true)]
        [InlineData('5', true)]
        [InlineData('6', true)]
        [InlineData('7', true)]
        [InlineData('8', true)]
        [InlineData('9', true)]
        public void IsValidKeypadDigit_WithStandardKeys_ReturnsTrue(char digit, bool expected)
        {
            var config = KeypadConfiguration.T9;
            Assert.Equal(expected, config.IsValidKeypadDigit(digit));
        }

        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        [InlineData('a')]
        [InlineData('Z')]
        [InlineData(' ')]
        [InlineData('*')]
        [InlineData('#')]
        public void IsValidKeypadDigit_WithInvalidKeys_ReturnsFalse(char digit)
        {
            var config = KeypadConfiguration.T9;
            Assert.False(config.IsValidKeypadDigit(digit));
        }

        [Fact]
        public void IsValidKeypadDigit_WithCustomMapping_ReturnsCorrectly()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { 'A', new[] { '1', '2' } },
                { 'B', new[] { '3', '4' } }
            };

            var config = new KeypadConfiguration(mapping);

            Assert.True(config.IsValidKeypadDigit('A'));
            Assert.True(config.IsValidKeypadDigit('B'));
            Assert.False(config.IsValidKeypadDigit('C'));
            Assert.False(config.IsValidKeypadDigit('2'));
        }

        #endregion

        #region IsControlChar Tests

        [Fact]
        public void IsControlChar_WithSeparator_ReturnsTrue()
        {
            var config = KeypadConfiguration.T9;
            Assert.True(config.IsControlChar(' '));
        }

        [Fact]
        public void IsControlChar_WithBackspace_ReturnsTrue()
        {
            var config = KeypadConfiguration.T9;
            Assert.True(config.IsControlChar('*'));
        }

        [Fact]
        public void IsControlChar_WithEndMarker_ReturnsTrue()
        {
            var config = KeypadConfiguration.T9;
            Assert.True(config.IsControlChar('#'));
        }

        [Theory]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('a')]
        [InlineData('Z')]
        public void IsControlChar_WithNonControlChars_ReturnsFalse(char c)
        {
            var config = KeypadConfiguration.T9;
            Assert.False(config.IsControlChar(c));
        }

        [Fact]
        public void IsControlChar_WithCustomControlChars_ReturnsCorrectly()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping)
            {
                SeparatorChar = '|',
                BackspaceChar = '<',
                EndMarker = '>'
            };

            Assert.True(config.IsControlChar('|'));
            Assert.True(config.IsControlChar('<'));
            Assert.True(config.IsControlChar('>'));
            Assert.False(config.IsControlChar(' '));
            Assert.False(config.IsControlChar('*'));
            Assert.False(config.IsControlChar('#'));
        }

        #endregion

        #region KeyMapping Immutability Tests

        [Fact]
        public void KeyMapping_IsReadOnly()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping);

            Assert.IsAssignableFrom<IReadOnlyDictionary<char, char[]>>(config.KeyMapping);
        }

        #endregion
    }

    public class KeypadExceptionTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithMessage_CreatesException()
        {
            var ex = new KeypadException("Test message");
            Assert.Equal("Test message", ex.Message);
            Assert.Null(ex.InvalidCharacter);
        }

        [Fact]
        public void Constructor_WithMessageAndChar_CreatesException()
        {
            var ex = new KeypadException("Test message", 'x');
            Assert.Equal("Test message", ex.Message);
            Assert.Equal('x', ex.InvalidCharacter);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_CreatesException()
        {
            var innerEx = new Exception("Inner exception");
            var ex = new KeypadException("Test message", innerEx);
            
            Assert.Equal("Test message", ex.Message);
            Assert.Same(innerEx, ex.InnerException);
            Assert.Null(ex.InvalidCharacter);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void InvalidCharacter_WhenNotSet_IsNull()
        {
            var ex = new KeypadException("Test");
            Assert.Null(ex.InvalidCharacter);
        }

        [Fact]
        public void InvalidCharacter_WhenSet_HasValue()
        {
            var ex = new KeypadException("Test", '5');
            Assert.NotNull(ex.InvalidCharacter);
            Assert.Equal('5', ex.InvalidCharacter.Value);
        }

        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        [InlineData('a')]
        [InlineData('Z')]
        [InlineData('!')]
        [InlineData('@')]
        public void InvalidCharacter_CanStoreAnyChar(char c)
        {
            var ex = new KeypadException("Test", c);
            Assert.Equal(c, ex.InvalidCharacter);
        }

        #endregion

        #region Exception Behavior Tests

        /*
        [Fact]
        public void Exception_CanBeThrown()
        {
            Assert.Throws<KeypadException>(() => throw new KeypadException("Test"));
        }
        */

        [Fact]
        public void Exception_CanBeCaught()
        {
            try
            {
                throw new KeypadException("Test", 'x');
            }
            catch (KeypadException ex)
            {
                Assert.Equal("Test", ex.Message);
                Assert.Equal('x', ex.InvalidCharacter);
            }
        }

        [Fact]
        public void Exception_IsException()
        {
            var ex = new KeypadException("Test");
            Assert.IsAssignableFrom<Exception>(ex);
        }

        #endregion
    }
}
/*
using System;
using System.Collections.Generic;
using Xunit;
using OldPhone.Core;

namespace OldPhone.Tests.Core
{
    public class KeypadConfigurationTests
    {
        [Fact]
        public void T9_HasCorrectDefaultMapping()
        {
            var config = KeypadConfiguration.T9;
            
            Assert.NotNull(config.KeyMapping);
            Assert.Equal(8, config.KeyMapping.Count);
            Assert.Equal(new[] { 'A', 'B', 'C' }, config.KeyMapping['2']);
        }

        [Fact]
        public void Constructor_WithValidMapping_CreatesConfiguration()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping);

            Assert.NotNull(config);
            Assert.Equal(mapping, config.KeyMapping);
        }

        [Fact]
        public void Constructor_WithNullMapping_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new KeypadConfiguration(null));
        }

        [Fact]
        public void Constructor_WithEmptyMapping_ThrowsArgumentException()
        {
            var mapping = new Dictionary<char, char[]>();
            Assert.Throws<ArgumentException>(() => new KeypadConfiguration(mapping));
        }

        [Fact]
        public void IsValidKeypadDigit_WithValidDigit_ReturnsTrue()
        {
            var config = KeypadConfiguration.T9;
            Assert.True(config.IsValidKeypadDigit('2'));
        }

        [Fact]
        public void IsValidKeypadDigit_WithInvalidDigit_ReturnsFalse()
        {
            var config = KeypadConfiguration.T9;
            Assert.False(config.IsValidKeypadDigit('0'));
        }

        [Fact]
        public void IsControlChar_WithSeparator_ReturnsTrue()
        {
            var config = KeypadConfiguration.T9;
            Assert.True(config.IsControlChar(' '));
        }

        [Fact]
        public void CustomConfiguration_CanModifyControlChars()
        {
            var mapping = new Dictionary<char, char[]>
            {
                { '2', new[] { 'A', 'B' } }
            };

            var config = new KeypadConfiguration(mapping)
            {
                SeparatorChar = '-',
                BackspaceChar = 'X',
                EndMarker = '!'
            };

            Assert.Equal('-', config.SeparatorChar);
            Assert.Equal('X', config.BackspaceChar);
            Assert.Equal('!', config.EndMarker);
        }
    }
}
*/