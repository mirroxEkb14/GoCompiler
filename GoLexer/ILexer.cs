using GoLexer.Tokens;

namespace GoLexer;

//
// Summary:
//     Defines a set of basic operations that a lexer must implement (a contract).
interface ILexer
{
    //
    // Summary:
    //     Scans the entire source code and converts it into a list of tokens.
    // Returns:
    //     A list of tokens.
    List<Token> ScanTokens();
}
