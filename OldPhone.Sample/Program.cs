using System;
using System.Collections.Generic;
using OldPhone;
using OldPhone.Core;
using OldPhone.Decoders;

namespace OldPhone.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== OldPhone T9 Decoder Demo ===\n");

            // Example 1: Basic usage with facade
            BasicExample();

            Console.WriteLine();

            // Example 2: Using the interface directly
            InterfaceExample();

            Console.WriteLine();

            // Example 3: Custom configuration
            CustomConfigExample();

            Console.WriteLine();

            // Example 4: Error handling
            ErrorHandlingExample();

            Console.WriteLine();

            // Example 5: Interactive mode
            if (args.Length == 0)
            {
                InteractiveMode();
            }
        }

        static void BasicExample()
        {
            Console.WriteLine("--- Example 1: Basic Usage ---");
            
            var inputs = new[]
            {
                "44 444#",
                "44 33555 555666#",
                "8 88777444666*664#"
            };

            foreach (var input in inputs)
            {
                string result = PhonePad.Decode(input);
                Console.WriteLine($"Input:  {input}");
                Console.WriteLine($"Output: {result}\n");
            }
        }

        static void InterfaceExample()
        {
            Console.WriteLine("--- Example 2: Using IKeypadDecoder Interface ---");

            // Create a decoder instance
            IKeypadDecoder decoder = new T9Decoder();

            var testCases = new Dictionary<string, string>
            {
                { "222#", "C" },
                { "2 22 222#", "ABC" },
                { "222*#", "" },
                { "7777#", "S" }
            };

            foreach (var testCase in testCases)
            {
                string result = decoder.Decode(testCase.Key);
                bool passed = result == testCase.Value;
                Console.WriteLine($"Input: {testCase.Key,-20} Expected: {testCase.Value,-10} Got: {result,-10} {(passed ? "✓" : "✗")}");
            }
        }

        static void CustomConfigExample()
        {
            Console.WriteLine("--- Example 3: Custom Configuration ---");

            // Create a simple numeric keypad (keys map to digits)
            var numericMapping = new Dictionary<char, char[]>
            {
                { '2', new[] { '2' } },
                { '3', new[] { '3' } },
                { '4', new[] { '4' } },
                { '5', new[] { '5' } },
                { '6', new[] { '6' } },
                { '7', new[] { '7' } },
                { '8', new[] { '8' } },
                { '9', new[] { '9' } }
            };

            var config = new KeypadConfiguration(numericMapping)
            {
                SeparatorChar = '-',
                BackspaceChar = 'X',
                EndMarker = '!'
            };

            var decoder = new T9Decoder(config);

            Console.WriteLine("Custom keypad that outputs numbers:");
            Console.WriteLine("Separator: '-', Backspace: 'X', End: '!'");
            
            string input = "2-3-4-5!";
            string result = decoder.Decode(input);
            Console.WriteLine($"Input:  {input}");
            Console.WriteLine($"Output: {result}");
        }

        static void ErrorHandlingExample()
        {
            Console.WriteLine("--- Example 4: Error Handling ---");

            var invalidInputs = new[]
            {
                ("0#", "Invalid digit (0)"),
                ("1#", "Invalid digit (1)"),
                (null, "Null input")
            };

            foreach (var (input, description) in invalidInputs)
            {
                Console.Write($"Testing {description}... ");
                try
                {
                    PhonePad.Decode(input);
                    Console.WriteLine("❌ Should have thrown exception");
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("✓ Caught ArgumentNullException");
                }
                catch (KeypadException ex)
                {
                    Console.WriteLine($"✓ Caught KeypadException: {ex.Message}");
                    if (ex.InvalidCharacter.HasValue)
                    {
                        Console.WriteLine($"  Invalid character: '{ex.InvalidCharacter}'");
                    }
                }
            }
        }

        static void InteractiveMode()
        {
            Console.WriteLine("--- Interactive Mode ---");
            Console.WriteLine("Enter keypad sequences to decode (or 'quit' to exit)");
            Console.WriteLine("Example: 44 33555 555666#");
            Console.WriteLine();

            var decoder = new T9Decoder();

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                try
                {
                    string result = decoder.Decode(input);
                    Console.WriteLine($"Decoded: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Goodbye!");
        }
    }
}
