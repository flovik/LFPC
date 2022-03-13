using System;

namespace lexer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(@"D:\UTM\Anul 2\Semestrul 4\LFPC\Laborator3\lexer\sample.txt");
            var lexer = new Lexer(text);
            var tokens = lexer.Tokenizer();
            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.Type} {token.Value}");
            }
        }
    }
}