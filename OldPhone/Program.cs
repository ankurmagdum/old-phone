using System;

namespace OldPhone
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string result = PhonePad.DecodeKeypadInput("222 2 3***#");
                Console.WriteLine($"Decoded message: {result}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}