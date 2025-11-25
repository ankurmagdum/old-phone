# OldPhone - T9 Keypad Decoder Library

A modern .NET library for decoding T9-style phone keypad input sequences into text.

## Features

- Standard T9 keypad decoding
- Configurable keypad mappings
- Support for backspace operations
- Extensible architecture via interfaces
- Comprehensive error handling
- Full XML documentation

## Installation

```bash
dotnet add package OldPhone
```

## Quick Start

### Basic Usage

```csharp
using OldPhone;

// Simple decoding with default T9 configuration
string result = PhonePad.Decode("44 444#");
Console.WriteLine(result);  // Output: "HI"

// More complex example
string message = PhonePad.Decode("8 88777444666*664#");
Console.WriteLine(message);  // Output: "TURING"
```

### Advanced Usage

#### Using the Decoder Interface

```csharp
using OldPhone.Core;
using OldPhone.Decoders;

// Create a decoder instance
IKeypadDecoder decoder = new T9Decoder();

try
{
    string result = decoder.Decode("44 33555 555666#");
    Console.WriteLine(result);  // Output: "HELLO"
}
catch (KeypadException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
    if (ex.InvalidCharacter.HasValue)
    {
        Console.WriteLine($"Problem character: {ex.InvalidCharacter}");
    }
}
```

#### Custom Keypad Configuration

```csharp
using OldPhone.Core;
using OldPhone.Decoders;

// Create custom keypad mapping
var customMapping = new Dictionary<char, char[]>
{
    { '2', new[] { 'A', 'B', 'C' } },
    { '3', new[] { 'D', 'E', 'F' } },
    { '4', new[] { '1', '2', '3' } }  // Numbers on key 4
};

var config = new KeypadConfiguration(customMapping)
{
    SeparatorChar = '-',      // Use dash instead of space
    BackspaceChar = 'X',      // Use X for backspace
    EndMarker = '!'           // Use ! as end marker
};

var decoder = new T9Decoder(config);
string result = decoder.Decode("4-44-444!");
Console.WriteLine(result);  // Output: "123"
```

#### Using the Facade Pattern

```csharp
using OldPhone;
using OldPhone.Core;

// Quick access via static facade
string result1 = PhonePad.Decode("222#");

// Custom decoder via facade
var config = KeypadConfiguration.T9;
var customDecoder = PhonePad.CreateDecoder(config);
string result2 = PhonePad.Decode("222#", customDecoder);
```

## Keypad Mapping

Standard T9 keypad layout:

```
2: ABC    3: DEF
4: GHI    5: JKL
6: MNO    7: PQRS
8: TUV    9: WXYZ
```

## Special Characters

- **Space (' ')**: Separates consecutive presses of the same key
- **Asterisk ('*')**: Backspace/delete last character
- **Hash ('#')**: End of input marker

## Examples

| Input | Output | Description |
|-------|--------|-------------|
| `2#` | A | Single press of key 2 |
| `22#` | B | Two presses of key 2 |
| `222#` | C | Three presses of key 2 |
| `44 444#` | HI | H then I (same key) |
| `222*#` | (empty) | C then backspace |
| `8 88777444666*664#` | TURING | Complex message |

## Architecture

### Project Structure

```
OldPhone/
├── Core/
│   ├── IKeypadDecoder.cs          # Decoder interface
│   ├── KeypadConfiguration.cs     # Configuration model
│   └── KeypadException.cs         # Custom exception
├── Decoders/
│   └── T9Decoder.cs               # T9 implementation
└── PhonePad.cs                    # Simple facade
```

### Design Principles

1. **Interface-based**: Easy to implement custom decoders
2. **Configuration over code**: Keypad mappings are configurable
3. **Fail-fast**: Invalid input throws meaningful exceptions
4. **Immutability**: Configuration objects are immutable
5. **Single Responsibility**: Each class has one clear purpose

## Extensibility

### Implementing a Custom Decoder

```csharp
using OldPhone.Core;

public class MyCustomDecoder : IKeypadDecoder
{
    public string Decode(string input)
    {
        // Your custom implementation
        return "decoded text";
    }
}

// Use it
IKeypadDecoder decoder = new MyCustomDecoder();
string result = decoder.Decode("some input");
```

### Adding New Keypad Types

```csharp
// International keypad with different layout
var internationalMapping = new Dictionary<char, char[]>
{
    { '2', new[] { 'A', 'Ä', 'B', 'C' } },  // German layout
    // ... more mappings
};

var config = new KeypadConfiguration(internationalMapping);
var decoder = new T9Decoder(config);
```

## Error Handling

```csharp
using OldPhone;
using OldPhone.Core;

try
{
    string result = PhonePad.Decode("0#");  // Invalid digit
}
catch (ArgumentNullException ex)
{
    // Input was null
    Console.WriteLine("Input cannot be null");
}
catch (KeypadException ex)
{
    // Invalid keypad input
    Console.WriteLine($"Invalid input: {ex.Message}");
    
    if (ex.InvalidCharacter.HasValue)
    {
        Console.WriteLine($"Character '{ex.InvalidCharacter}' is not valid");
    }
}
```

## Testing

Run the test suite:

```bash
dotnet test
```

Run with coverage:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Building

Build the library:

```bash
dotnet build
```

Create NuGet package:

```bash
dotnet pack --configuration Release
```
