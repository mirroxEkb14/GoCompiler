using GoLexer;
using GoParser;

namespace GoCompilerDemo;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            string sourceCode = @"
            var x int = 10
            var y float32 = 5.5
            var cond string = ""Hello, World!""
            if x > 10 && y <= 5 {
                fmt.Print(""Condition is true"")
            } else {
                fmt.Print(""Condition is false"")
            }
            ";

            var lexer = new Lexer(sourceCode);
            var tokens = lexer.ScanTokens();
            foreach (var token in tokens)
                Console.WriteLine($"[{token.Type}] '{token.Lexeme}' at Line: {token.Line}, Column: {token.Column}");

            //var parser = new Parser(tokens);
            //parser.Parse();
        }
        catch (LexerException le)
        {
            Console.WriteLine($"Lexer Error: {le.Message}");
        }
    }
}
