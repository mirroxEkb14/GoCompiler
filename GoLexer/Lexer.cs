using System.Text;

namespace GoLexer;

//
// Summary:
//     Is responsible for tokenizing the source code into a sequence of tokens that can be consumed by the parser.
public class Lexer(string sourceCode)
{
    private readonly string _source = sourceCode;
    private int _current = 0;
    private int _line = 1;
    private int _column = 1;

    private static readonly Func<char, bool> _isLetterOrDigitOrUnderscore =
        ch => char.IsLetterOrDigit(ch) || ch == '_';
    private static readonly Func<char, bool> _isLetterOrUnderscore =
        ch => char.IsLetter(ch) || ch == '_';
    private static readonly Func<char, bool> _isDigit =
        char.IsDigit;
    private static readonly Func<char, bool> _isString =
        ch => ch == '"';

    //
    // Summary:
    //     Contains keywords (key) recognized by the lexer and their corresponding token types (value).
    private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "var", TokenType.Var },
            { "func", TokenType.Func },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "for", TokenType.For },
            { "return", TokenType.Return },
            { "range", TokenType.Range },
            { "true", TokenType.True },
            { "false", TokenType.False },
            { "int", TokenType.Int },
            { "float32", TokenType.Float32 },
            { "string", TokenType.String },
            { "bool", TokenType.Bool },
            { "fmt.Print", TokenType.Print }
        };

    public List<Token> ScanTokens()
    {
        var tokens = new List<Token>();
        while (!IsAtEnd())
        {
            int start = _current;
            char c = Advance();

            if (HandleWhitespace(c))
                continue;

            if (_isLetterOrUnderscore(c))
                HandleKeywordOrIdentifier(tokens);
            else if (_isDigit(c))
                HandleNumber(tokens);
            else if (_isString(c))
                HandleString(tokens);
            else
                tokens.Add(MatchOperatorOrUnknown(c));
        }
        tokens.Add(new Token(TokenType.EndOfFile, "", _line, _column));
        return tokens;
    }

    #region «ScanTokens()» Method Helpers
    //
    // Summary:
    //     Reads the entire identifier.
    //     Checks if the identifier is a keyword (var, func, if etc.).
    //     If the identifier is found in the «Keywords» dictionary, adds the corresponding keyword token to the list.
    //     Otherwise, treats it as a user-defined identifier and creates a token of type «Identifier» (x, myVar etc.).
    // Parameters:
    //     tokens: The list of tokens to add to.
    private void HandleKeywordOrIdentifier(List<Token> tokens)
    {
        string identifier = ReadWhile(_isLetterOrDigitOrUnderscore);
        if (Keywords.TryGetValue(identifier, out TokenType keyword))
            tokens.Add(new Token(keyword, identifier, _line, _column));
        else
            tokens.Add(new Token(TokenType.Identifier, identifier, _line, _column));
    }

    //
    // Summary:
    //     Reads the integer part of the number ('123' from the input of '123.456').
    //     Check for a decimal point.
    //     If found, consumes the '.'.
    //     Reads the fractional part of the number.
    //     Ensures there are digits after the '.', and if the fractional part is empty, throws an exception
    //         (e.g., '123.').
    //     Combines the integer and fractional parts (e.g., "123" + "." + "456" = "123.456").
    //     If it’s an integer, adds a token of type «Integer» for input without a '.'.
    private void HandleNumber(List<Token> tokens)
    {
        string number = ReadWhile(_isDigit);
        if (Peek() == '.')
        {
            Advance();
            string fractionalPart = ReadWhile(_isDigit);
            if (fractionalPart.Length == 0)
                throw new LexerException("Malformed float", _line, _column);
            number += fractionalPart;
            tokens.Add(new Token(TokenType.Float, number, _line, _column));
            return;
        }
        tokens.Add(new Token(TokenType.Integer, number, _line, _column));
    }

    //
    // Summary:
    //     Extracts the string content.
    //     Adds a token to the list.
    private void HandleString(List<Token> tokens)
    {
        string str = ReadString();
        tokens.Add(new Token(TokenType.StringLiteral, str, _line, _column));
    }
    #endregion

    #region Private Helper Methods
    //
    // Summary:
    //     Reads characters while the specified condition is met.
    //     For instance, the «var myVariable = 10» source code is processed. The «condition» is set to the
    //         «_isLetterOrDigitOrUnderscore» delegate. The lexer reads the characters «var» and stops at the
    //         whitespace character (' ' between the 'var' and 'myVariable'). The method returns the 'var'.
    // Returns:
    //     The string of characters read.
    private string ReadWhile(Func<char, bool> condition)
    {
        var result = new StringBuilder();
        result.Append(CurrentChar());
        while (!IsAtEnd() && condition(Peek()))
            result.Append(Advance());

        return result.ToString();
    }

    //
    // Summary:
    //     Reads until the closing quote or the end of the source (from the "Hello, World!" to 'Hello, World!').
    //     If the end of the source is reached without a closing quote, throws an error.
    //     Consumes the closing quote.
    // Returns:
    //     The string literal without the enclosing quotes.
    private string ReadString()
    {
        var result = new StringBuilder();
        while (!IsAtEnd() && Peek() != '"')
            result.Append(Advance());

        if (IsAtEnd())
            throw new LexerException("Unterminated string literal", _line, _column);

        Advance();
        return result.ToString();
    }

    //
    // Summary:
    //     Matches a single or multi-character operator or returns an unknown token if unmatched.
    // Parameters:
    //     c: The current character to match.
    // Returns:
    //     A token representing the matched operator or unknown token.
    private Token MatchOperatorOrUnknown(char c)
    {
        switch (c)
        {
            case '+': return new Token(TokenType.Plus, c.ToString(), _line, _column);
            case '-': return new Token(TokenType.Minus, c.ToString(), _line, _column);
            case '*': return new Token(TokenType.Multiply, c.ToString(), _line, _column);
            case '/': return new Token(TokenType.Divide, c.ToString(), _line, _column);
            case '%': return new Token(TokenType.Modulo, c.ToString(), _line, _column);
            case '=':
                if (Peek() == '=')
                {
                    Advance();
                    return new Token(TokenType.Equal, "==", _line, _column);
                }
                return new Token(TokenType.Assign, "=", _line, _column);
            case '!':
                if (Peek() == '=')
                {
                    Advance();
                    return new Token(TokenType.NotEqual, "!=", _line, _column);
                }
                break;
            case '<':
                if (Peek() == '=')
                {
                    Advance();
                    return new Token(TokenType.LessOrEqual, "<=", _line, _column);
                }
                return new Token(TokenType.Less, "<", _line, _column);
            case '>':
                if (Peek() == '=')
                {
                    Advance();
                    return new Token(TokenType.GreaterOrEqual, ">=", _line, _column);
                }
                return new Token(TokenType.Greater, ">", _line, _column);
            case '&':
                if (Peek() == '&')
                {
                    Advance();
                    return new Token(TokenType.And, "&&", _line, _column);
                }
                break;
            case '|':
                if (Peek() == '|')
                {
                    Advance();
                    return new Token(TokenType.Or, "||", _line, _column);
                }
                break;
            case ':':
                if (Peek() == '=')
                {
                    Advance();
                    return new Token(TokenType.ShortAssign, ":=", _line, _column);
                }
                break;
            case '{': return new Token(TokenType.LeftBrace, c.ToString(), _line, _column);
            case '}': return new Token(TokenType.RightBrace, c.ToString(), _line, _column);
            case '(': return new Token(TokenType.LeftParen, c.ToString(), _line, _column);
            case ')': return new Token(TokenType.RightParen, c.ToString(), _line, _column);
            case '.': return new Token(TokenType.Dot, c.ToString(), _line, _column);
            case ',': return new Token(TokenType.Comma, c.ToString(), _line, _column);
        }
        return new Token(TokenType.Unknown, c.ToString(), _line, _column);
    }

    //
    // Summary:
    //     «.IsWhiteSpace()» evaluates to «true» if 'c' is a whitespace character (a space ' ', a tab '\t',
    //         or a newline '\n').
    //     Handles newlines specifically: (i) increments the '_line' counter to indicate a new line
    //         has started in the source code, (ii) resets '_column' to 1 since the column count restarts at
    //         the beginning of a new line.
    // Returns:
    //     «True» if the character is whitespace; otherwise, «false».
    //     In case of a «true», in the «ScanTokens()» method the lexer will 'continue' to the next character
    //         (meaning skips the remaining code in the loop and moves to the next iteration, effectively
    //         ignoring the current whitespace character).
    private bool HandleWhitespace(char c)
    {
        if (char.IsWhiteSpace(c))
        {
            if (c == '\n')
            {
                _line++;
                _column = 1;
            }
            return true;
        }
        return false;
    }

    //
    // Returns:
    //     The current character before advancing.
    //     Handles tabs appropriately by treating them as multiple spaces (assumes a tab width of 4 spaces).
    private char Advance()
    {
        _column++;
        return _source[_current++];
    }

    //
    // Returns:
    //     The next character in the source code or null character ('\0') if at the end of the source code.
    private char Peek() => IsAtEnd() ? '\0' : _source[_current];

    //
    // Returns:
    //     The current character in the source code.
    private char CurrentChar() => _source[_current - 1];

    //
    // Returns:
    //     «True» if the lexer has reached the end of the source code; otherwise, «false».
    private bool IsAtEnd() => _current >= _source.Length;
    #endregion
}
