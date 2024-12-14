namespace GoLexer;

//
// Summary:
//     Handles lexer errors with additional context about the error's location.
public class LexerException(
    string message,
    int line,
    int column) : Exception($"{message} (Line: {line}, Column: {column})")
{
    public int Line { get; } = line;
    public int Column { get; } = column;
}
