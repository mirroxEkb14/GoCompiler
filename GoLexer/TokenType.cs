namespace GoLexer;

//
// Summary:
//     Represents the different types of tokens that can be identified in the source code.
//     Contains: keywords, types, boolean literals, arithmetic operators, comparison operators,
//         logical operators, assignment operators, delimiters, literals and special tokens.
public enum TokenType
{
    Var, Func, If, Else, For, Return, Range, Print, Len,

    Int, Float32, String, Bool,

    True, False,

    Plus, Minus, Multiply, Divide, Modulo,
    Equal, NotEqual, Greater, GreaterOrEqual, Less, LessOrEqual,
    And, Or, Not,
    Assign, ShortAssign,
    Increment, Decrement,

    LeftParen, RightParen, LeftBrace, RightBrace, LeftBracket, RightBracket,
    Comma, Semicolon, Dot,

    Identifier, Integer, Float, StringLiteral,

    EndOfFile, Unknown
}
