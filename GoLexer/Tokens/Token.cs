namespace GoLexer.Tokens;

//
// Summary:
//     Represents a single token with its type, textual value, and source position.
// Parameters:
//     Type: «Var», «Dot», «LeftParen», ...
//     Lexeme: «var», «.», «(», ...
//     Line: «2», ...
//     Column: «16», ...
public record Token(TokenType Type, string Lexeme, int Line, int Column)
{
    //
    // Returns:
    //     «[Var] 'var' at Line: 2, Column: 16».
    public override string ToString() =>
        $"[{Type}] '{Lexeme}' at Line: {Line}, Column: {Column}";
}
