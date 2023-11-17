namespace Prime.Tokens;

public static class TokenFacts
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {"int", TokenType.Type},
        {"bool", TokenType.Type},
        {"char", TokenType.Type},
        {"array", TokenType.Array},
        {"func", TokenType.Func},
        {"return", TokenType.Return},
        {"if", TokenType.If},
        {"else", TokenType.Else},
        {"for", TokenType.For},
    };

    public static Token GetKeywordToken(string lexeme, int position)
    {
        return Keywords.TryGetValue(lexeme, out var keyword) ? new Token(keyword, lexeme, position) : new Token(TokenType.Error, "", position);
    }

    public static bool IsKeyword(string lexeme)
    {
        return Keywords.ContainsKey(lexeme);
    }

    public static bool IsOperator(char current, char lookahead)
    {
        return (current, lookahead) switch
        {
            ('+', _) => true,
            ('-', '>' ) => false,
            ('-', _) => true,
            ('*', _) => true,
            ('/', _) => true,
            ('=', _) => true,
            ('<', _) => true,
            ('>', _) => true,
            (':', _) => true,
            _ => false
        };
    }

    public static TokenType GetOperatorLevel(string spelling)
    {
        return spelling switch
        {
            "&" or "|" => TokenType.OperatorL1,
            "==" or "!=" or "<=" or ">=" or "<" or ">" => TokenType.OperatorL2,
            "+" or "-" => TokenType.OperatorL3,
            "*" or "/" or "%" => TokenType.OperatorL4,
            ":=" or "=" => TokenType.AssignOperator,
            _ => TokenType.Operator
        };
    }

}