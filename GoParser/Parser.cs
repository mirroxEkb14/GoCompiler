using GoLexer;

namespace GoParser;

public class Parser(List<Token> tokens)
{
    private readonly SymbolTable _symbolTable = new SymbolTable();
    private readonly List<Token> _tokens = tokens;
    private int _current = 0;

    public void Parse()
    {
        while (!IsAtEnd())
            ParseStatement();
    }

    private void ParseStatement()
    {
        if (Match(TokenType.Var))
            ParseVariableDeclaration();
        else if (Match(TokenType.If))
            ParseIfStatement();
        else if (Match(TokenType.For))
            ParseForStatement();
        else if (Check(TokenType.Identifier))
            ParseFunctionCall();
        else
            throw new Exception($"Unexpected token '{Peek().Lexeme}' at Line: {Peek().Line}, Column: {Peek().Column}");
    }

    private void ParseVariableDeclaration()
    {
        if (!Match(TokenType.Identifier))
            throw new Exception($"Expected identifier after 'var' at Line: {Peek().Line}, Column: {Peek().Column}");

        string variableName = Previous().Lexeme;

        if (!Match(TokenType.Int, TokenType.Float32, TokenType.String, TokenType.Bool))
            throw new Exception($"Expected type after variable name '{variableName}' at Line: {Peek().Line}, Column: {Peek().Column}");

        string variableType = Previous().Lexeme;
        object? variableValue = null;

        if (Match(TokenType.Assign))
        {
            if (Match(TokenType.Integer, TokenType.Float, TokenType.StringLiteral))
                variableValue = Previous().Lexeme;
            else
                throw new Exception($"Expected value after '=' at Line: {Peek().Line}, Column: {Peek().Column}");
        }

        _symbolTable.AddVariable(variableName, variableType, variableValue);
        Console.WriteLine($"Parsed variable: Name = {variableName}, Type = {variableType}, Value = {variableValue}");
    }

    private void ParseIfStatement()
    {
        Console.WriteLine("Parsing 'if' statement");

        ParseCondition();

        if (!Match(TokenType.LeftBrace))
            throw new Exception($"Expected '{{' after condition at Line: {Peek().Line}, Column: {Peek().Column}");

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
            ParseStatement();

        if (!Match(TokenType.RightBrace))
            throw new Exception($"Expected '}}' after 'if' block at Line: {Peek().Line}, Column: {Peek().Column}");

        if (Match(TokenType.Else))
        {
            Console.WriteLine("Parsing 'else' block");

            if (!Match(TokenType.LeftBrace))
                throw new Exception($"Expected '{{' after 'else' at Line: {Peek().Line}, Column: {Peek().Column}");

            while (!Check(TokenType.RightBrace) && !IsAtEnd())
                ParseStatement();

            if (!Match(TokenType.RightBrace))
                throw new Exception($"Expected '}}' after 'else' block at Line: {Peek().Line}, Column: {Peek().Column}");
        }

        Console.WriteLine("Finished parsing 'if' statement");
    }

    private void ParseForStatement()
    {
        if (!Match(TokenType.For))
            throw new Exception($"Expected 'for' at Line: {Peek().Line}, Column: {Peek().Column}");

        Console.WriteLine("Parsing 'for' statement");

        if (Match(TokenType.Identifier))
        {
            if (!Match(TokenType.Assign))
                throw new Exception($"Expected ':=' after identifier in 'for' range at Line: {Peek().Line}, Column: {Peek().Column}");

            if (!Match(TokenType.Range))
                throw new Exception($"Expected 'range' after ':=' in 'for' range at Line: {Peek().Line}, Column: {Peek().Column}");

            if (!Match(TokenType.Identifier))
                throw new Exception($"Expected iterable identifier after 'range' in 'for' range at Line: {Peek().Line}, Column: {Peek().Column}");
        }
        else if (Match(TokenType.Semicolon))
        {
            ParseExpression();
            if (!Match(TokenType.Semicolon))
                throw new Exception($"Expected ';' after initialization in 'for' at Line: {Peek().Line}, Column: {Peek().Column}");

            ParseCondition();
            if (!Match(TokenType.Semicolon))
                throw new Exception($"Expected ';' after condition in 'for' at Line: {Peek().Line}, Column: {Peek().Column}");

            ParseExpression();
        }
        else
        {
            throw new Exception($"Invalid 'for' statement at Line: {Peek().Line}, Column: {Peek().Column}");
        }

        if (!Match(TokenType.LeftBrace))
            throw new Exception($"Expected '{{' after 'for' condition at Line: {Peek().Line}, Column: {Peek().Column}");

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
            ParseStatement();

        if (!Match(TokenType.RightBrace))
            throw new Exception($"Expected '}}' after 'for' block at Line: {Peek().Line}, Column: {Peek().Column}");

        Console.WriteLine("Finished parsing 'for' statement");
    }

    private void ParseFunctionCall()
    {
        if (!Match(TokenType.Identifier))
            throw new Exception($"Expected function name at Line: {Peek().Line}, Column: {Peek().Column}");

        string functionName = Previous().Lexeme;

        if (!Match(TokenType.Dot))
            throw new Exception($"Expected '.' after identifier '{functionName}' at Line: {Peek().Line}, Column: {Peek().Column}");

        if (!Match(TokenType.Identifier))
            throw new Exception($"Expected method name after '.' at Line: {Peek().Line}, Column: {Peek().Column}");

        string methodName = Previous().Lexeme;

        if (!Match(TokenType.LeftParen))
            throw new Exception($"Expected '(' after method name '{methodName}' at Line: {Peek().Line}, Column: {Peek().Column}");

        while (!Check(TokenType.RightParen) && !IsAtEnd())
        {
            ParseExpression();
            if (!Match(TokenType.Comma)) break;
        }

        if (!Match(TokenType.RightParen))
            throw new Exception($"Expected ')' after function arguments at Line: {Peek().Line}, Column: {Peek().Column}");

        Console.WriteLine($"Parsed function call: {functionName}.{methodName}");
    }

    private void ParseExpression()
    {
        if (Match(TokenType.Integer, TokenType.Float, TokenType.StringLiteral, TokenType.Identifier))
            Console.WriteLine($"Parsed expression: {Previous().Lexeme}");
        else
            throw new Exception($"Expected expression at Line: {Peek().Line}, Column: {Peek().Column}");
    }

    private void ParseCondition()
    {
        ParseComparison();

        while (Match(TokenType.And, TokenType.Or))
        {
            Token logicalOperator = Previous();
            Console.WriteLine($"Parsed logical operator: {logicalOperator.Lexeme}");

            ParseComparison();
        }
    }

    private void ParseComparison()
    {
        ParseArithmeticMember();

        if (Match(TokenType.Equal, TokenType.NotEqual, TokenType.Less, TokenType.LessOrEqual, TokenType.Greater, TokenType.GreaterOrEqual))
        {
            Token comparisonOperator = Previous();
            Console.WriteLine($"Parsed comparison operator: {comparisonOperator.Lexeme}");

            ParseArithmeticMember();
        }
        else
        {
            throw new Exception($"Expected comparison operator at Line: {Peek().Line}, Column: {Peek().Column}");
        }
    }

    private void ParseArithmeticMember()
    {
        if (Match(TokenType.Identifier, TokenType.Integer, TokenType.Float, TokenType.StringLiteral))
        {
            Token member = Previous();
            Console.WriteLine($"Parsed arithmetic member: {member.Lexeme}");
        }
        else
        {
            throw new Exception($"Expected identifier or value at Line: {Peek().Line}, Column: {Peek().Column}");
        }
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EndOfFile;

    private Token Peek() => _tokens[_current];
    private Token Previous() => _tokens[_current - 1];
}
