namespace Prime.Tokens;

public static class TokenFacts
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {"int", TokenType.Keyword},
        {"bool", TokenType.Keyword},
        {"char", TokenType.Keyword},
        {"array", TokenType.Keyword},
        {"func", TokenType.Keyword},
        {"return", TokenType.Keyword},
        {"if", TokenType.Keyword},
        {"else", TokenType.Keyword},
        {"for", TokenType.Keyword},
        {"main", TokenType.Keyword},
    };

    public static Token GetKeywordToken(string lexeme)
    {
        return Keywords.TryGetValue(lexeme, out var keyword) ? new Token(keyword, lexeme) : new Token(TokenType.Error, "");
    }

    public static bool IsKeyword(string lexeme)
    {
        return Keywords.ContainsKey(lexeme);
    }

    public static bool IsOperator(char current)
    {
        return current switch
        {
            '+' => true,
            '-' => true,
            '*' => true,
            '/' => true,
            '=' => true,
            '<' => true,
            '>' => true,
            ':' => true,
            _ => false
        };
    }
}