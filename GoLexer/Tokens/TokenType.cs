using System.ComponentModel;
using System.Reflection;

namespace GoLexer.Tokens;

//
// Summary:
//     Represents the different types of tokens that can be identified in the source code.
//     Contains: keywords, types, boolean literals, arithmetic operators, comparison operators,
//         logical operators, assignment operators, delimiters, literals and special tokens.
public enum TokenType
{
    [Description("var")]
    Var,

    [Description("func")]
    Func,
    [Description("return")]
    Return,

    [Description("if")]
    If,
    [Description("else")]
    Else,

    [Description("for")]
    For,
    [Description("range")]
    Range,

    [Description("fmt.Print")]
    Print,
    [Description("len")]
    Len,

    [Description("int")]
    Int,
    [Description("float32")]
    Float32,
    [Description("string")]
    String,
    [Description("bool")]
    Bool,

    [Description("true")]
    True,
    [Description("false")]
    False,

    [Description("+")]
    Plus,
    [Description("-")]
    Minus,
    [Description("*")]
    Multiply,
    [Description("/")]
    Divide,
    [Description("%")]
    Modulo,

    [Description("==")]
    Equal,
    [Description("!=")]
    NotEqual,
    [Description(">")]
    Greater,
    [Description(">=")]
    GreaterOrEqual,
    [Description("<")]
    Less,
    [Description("<=")]
    LessOrEqual,

    [Description("&&")]
    And,
    [Description("||")]
    Or,
    [Description("!")]
    Not,

    [Description("&")]
    LogicalAnd,
    [Description("|")]
    LogicalOr,

    [Description("=")]
    Assign,
    [Description("=:")]
    ShortAssign,

    [Description("++")]
    Increment,
    [Description("--")]
    Decrement,

    [Description("(")]
    LeftParen,
    [Description(")")]
    RightParen,
    [Description("{")]
    LeftBrace,
    [Description("}")]
    RightBrace,
    [Description("[")]
    LeftBracket,
    [Description("]")]
    RightBracket,

    [Description(",")]
    Comma,
    [Description(";")]
    Semicolon,
    [Description(":")]
    Colon,
    [Description(".")]
    Dot,

    [Description("identifier")]
    Identifier,
    [Description("integer")]
    Integer,
    [Description("float")]
    Float,
    [Description("\"")]
    StringLiteral,

    [Description("")]
    EndOfFile,
    [Description("\n")]
    NewLine,
    [Description("_")]
    Underscore,
    [Description("\0")]
    Null,
    [Description("unknown")]
    Unknown
}

//
// Summary:
//     Provides extension methods for the «TokenType» enumeration.
public static class EnumExtensions
{
    //
    // Summary:
    //     Uses reflection to get the «Description» attribute as string for the specified enum value.
    public static string GetString(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    //
    // Summary:
    //     Gets the first character of the string representation of the specified enum value
    //         (for instance, used in delegates in «Lexer»).
    public static char GetChar(this TokenType tokenType) =>
        tokenType.GetString()[0];

    //
    // Returns:
    //     "fmt" for «TokenType.Print».
    public static string GetFormatPackageName(this TokenType tokenType) =>
        tokenType == TokenType.Print ? "fmt" : throw new ArgumentException($"Shoudl be called upon «TokenType.Print»");

    //
    // Returns:
    //     "Print" for «TokenType.Print».
    public static string GetPrintFunctionName(this TokenType tokenType) =>
        tokenType == TokenType.Print ? "Print" : throw new ArgumentException($"Shoudl be called upon «TokenType.Print»");
}
