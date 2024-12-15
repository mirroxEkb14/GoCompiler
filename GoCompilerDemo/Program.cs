using GoLexer;
using GoParser;

namespace GoCompilerDemo;

internal class Program
{
    static readonly string _filePath = "Resources/source.go";

    static void Main(string[] args)
    {
        try
        {
            string sourceCode = SourceReader.ReadSourceCode(_filePath);
            SourceReader.WriteLine($"{sourceCode}\n");

            var lexer = new Lexer(sourceCode);
            var tokens = lexer.ScanTokens();
            foreach (var token in tokens)
                SourceReader.WriteLine($"[{token.Type}] '{token.Lexeme}' at Line: {token.Line}, Column: {token.Column}");
        }
        catch (FileNotFoundException)
        {
            SourceReader.WriteLine(SourceReader.FileNotFoundExceptionMessage);
        }
        catch (EmptySourceException ese)
        {
            SourceReader.WriteLine(ese.Message);
        }
        catch (LexerException le)
        {
            SourceReader.WriteLine($"Lexer Error: {le.Message}");
        }
    }
}
