using System;

namespace OldPhone
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string result = PhonePad.DecodeKeypadInput("8 88777444666*664#");
                Console.WriteLine($"Decoded message: {result}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}