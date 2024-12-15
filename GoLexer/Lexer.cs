using System.Text;
using GoLexer.Tokens;

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
        ch => char.IsLetterOrDigit(ch) || ch == TokenType.Underscore.GetChar();
    private static readonly Func<char, bool> _isLetterOrDigit =
        ch => char.IsLetterOrDigit(ch);
    private static readonly Func<char, bool> _isLetterOrUnderscore =
        ch => char.IsLetter(ch) || ch == TokenType.Underscore.GetChar();
    private static readonly Func<char, bool> _isDigit =
        char.IsDigit;
    private static readonly Func<char, bool> _isString =
        ch => ch == TokenType.StringLiteral.GetChar();
    private static readonly Func<char, bool> _isLeftBracket =
        ch => ch == TokenType.LeftBracket.GetChar();
    private static readonly Func<char, bool> _isRightBracket =
        ch => ch == TokenType.RightBracket.GetChar();

    //
    // Summary:
    //     Contains keywords (key) recognized by the lexer and their corresponding token types (value).
    private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { TokenType.Var.GetString(), TokenType.Var },
            { TokenType.Func.GetString(), TokenType.Func },
            { TokenType.Return.GetString(), TokenType.Return },
            { TokenType.If.GetString(), TokenType.If },
            { TokenType.Else.GetString(), TokenType.Else },
            { TokenType.For.GetString(), TokenType.For },
            { TokenType.Range.GetString(), TokenType.Range },
            { TokenType.Print.GetString(), TokenType.Print },
            { TokenType.Len.GetString(), TokenType.Len },
            { TokenType.Int.GetString(), TokenType.Int },
            { TokenType.Float32.GetString(), TokenType.Float32 },
            { TokenType.String.GetString(), TokenType.String },
            { TokenType.Bool.GetString(), TokenType.Bool },
            { TokenType.True.GetString(), TokenType.True },
            { TokenType.False.GetString(), TokenType.False },
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
            else if (_isLeftBracket(c))
                tokens.Add(new Token(TokenType.LeftBracket, TokenType.LeftBracket.GetString(), _line, _column));
            else if (_isRightBracket(c))
                tokens.Add(new Token(TokenType.RightBracket, TokenType.RightBracket.GetString(), _line, _column));
            else
                tokens.Add(MatchOperatorOrUnknown(c));
        }
        tokens.Add(new Token(TokenType.EndOfFile, TokenType.EndOfFile.GetString(), _line, _column));
        return tokens;
    }

    #region The «MatchOperatorOrUnknown()» Method
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
            case var _ when c == TokenType.Plus.GetChar():
                return GetPlusToken(c);
            case var _ when c == TokenType.Minus.GetChar():
                return GetMinusToken(c);
            case var _ when c == TokenType.Multiply.GetChar():
                return CreateToken(TokenType.Multiply);
            case var _ when c == TokenType.Divide.GetChar():
                return CreateToken(TokenType.Divide);
            case var _ when c == TokenType.Modulo.GetChar():
                return CreateToken(TokenType.Modulo);
            case var _ when c == TokenType.Assign.GetChar():
                return GetEqualToken(c);
            case var _ when c == TokenType.NotEqual.GetChar():
                return GetNotEqualToken(c);
            case var _ when c == TokenType.Less.GetChar():
                return GetLessOrEqualsToken(c);
            case var _ when c == TokenType.Greater.GetChar():
                return GetGreaterOrEqualsToken(c);
            case var _ when c == TokenType.LogicalAnd.GetChar():
                return GetAndToken(c);
            case var _ when c == TokenType.LogicalOr.GetChar():
                return GetOrToken(c);
            case var _ when c == TokenType.Colon.GetChar():
                return GetShortAssignToken(c);
            case var _ when c == TokenType.Semicolon.GetChar():
                return CreateToken(TokenType.Semicolon);
            case var _ when c == TokenType.LeftBrace.GetChar():
                return CreateToken(TokenType.LeftBrace);
            case var _ when c == TokenType.RightBrace.GetChar():
                return CreateToken(TokenType.RightBrace);
            case var _ when c == TokenType.LeftParen.GetChar():
                return CreateToken(TokenType.LeftParen);
            case var _ when c == TokenType.RightParen.GetChar():
                return CreateToken(TokenType.RightParen);
            case var _ when c == TokenType.Dot.GetChar():
                return CreateToken(TokenType.Dot);
            case var _ when c == TokenType.Comma.GetChar():
                return CreateToken(TokenType.Comma);
        }
        return CreateToken(TokenType.Unknown);
    }

    private Token GetPlusToken(char c)
    {
        if (Peek() == TokenType.Plus.GetChar())
        {
            Advance();
            return CreateToken(TokenType.Increment);
        }
        return CreateToken(TokenType.Plus);
    }

    private Token GetMinusToken(char c)
    {
        if (Peek() == TokenType.Minus.GetChar())
        {
            Advance();
            return CreateToken(TokenType.Decrement);
        }
        return CreateToken(TokenType.Minus);
    }

    private Token GetEqualToken(char c)
    {
        if (Peek() == TokenType.Assign.GetChar())
        {
            Advance();
            return CreateToken(TokenType.Equal);
        }
        return CreateToken(TokenType.Assign);
    }

    private Token GetNotEqualToken(char c)
    {
        if (Peek() == TokenType.Assign.GetChar())
        {
            Advance();
            return CreateToken(TokenType.NotEqual);
        }
        return CreateToken(TokenType.Unknown);
    }

    private Token GetLessOrEqualsToken(char c)
    {
        if (Peek() == TokenType.Assign.GetChar())
        {
            Advance();
            return CreateToken(TokenType.LessOrEqual);
        }
        return CreateToken(TokenType.Less);
    }

    private Token GetGreaterOrEqualsToken(char c)
    {
        if (Peek() == TokenType.Assign.GetChar())
        {
            Advance();
            return CreateToken(TokenType.GreaterOrEqual);
        }
        return CreateToken(TokenType.Greater);
    }

    private Token GetAndToken(char c)
    {
        if (Peek() == TokenType.LogicalAnd.GetChar())
        {
            Advance();
            return CreateToken(TokenType.And);
        }
        return CreateToken(TokenType.Unknown);
    }

    private Token GetOrToken(char c)
    {
        if (Peek() == TokenType.LogicalOr.GetChar())
        {
            Advance();
            return CreateToken(TokenType.Or);
        }
        return CreateToken(TokenType.Unknown);
    }

    private Token GetShortAssignToken(char c)
    {
        if (Peek() == TokenType.Assign.GetChar())
        {
            Advance();
            return CreateToken(TokenType.ShortAssign);
        }
        return CreateToken(TokenType.Unknown);
    }

    private Token CreateToken(TokenType tokenType) =>
        new(tokenType, tokenType.GetString(), _line, _column);
    #endregion

    #region «ScanTokens()» Method Helpers
    //
    // Summary:
    //     Reads the entire identifier.
    //     Checks for the "fmt.Print", consumes the 't' and '.', reads the 'Print' part, and adds an appropriate token.
    //     Checks if the identifier is a keyword (var, func, if etc.).
    //     If the identifier is found in the «Keywords» dictionary, adds the corresponding keyword token to the list.
    //     Otherwise, treats it as a user-defined identifier and creates a token of type «Identifier» (x, myVar etc.).
    // Parameters:
    //     tokens: The list of tokens to add to.
    private void HandleKeywordOrIdentifier(List<Token> tokens)
    {
        string identifier = ReadWhile(_isLetterOrDigitOrUnderscore);

        if (identifier == TokenType.Print.GetFormatPackageName() && Peek() == TokenType.Dot.GetChar())
        {
            Advance();
            Advance();
            string nextPart = ReadWhile(_isLetterOrDigit);
            if (nextPart == TokenType.Print.GetPrintFunctionName())
            {
                tokens.Add(new Token(TokenType.Print, TokenType.Print.GetString(), _line, _column));
                return;
            }
        }

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
        if (Peek() == TokenType.Dot.GetChar())
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
        while (!IsAtEnd() && Peek() != TokenType.StringLiteral.GetChar())
            result.Append(Advance());

        if (IsAtEnd())
            throw new LexerException("Unterminated string literal", _line, _column);

        Advance();
        return result.ToString();
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
            if (c == TokenType.NewLine.GetChar())
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
    private char Peek() => IsAtEnd() ? TokenType.Null.GetChar() : _source[_current];

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
