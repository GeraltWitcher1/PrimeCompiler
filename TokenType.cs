namespace Prime;


public enum TokenType
{
    Identifier,
    Keyword,
    Integer,
    Operator,
    // Add more token types as needed
}

public class Token
{
    public TokenType Type { get; }
    public string Spelling { get; }

    public Token(TokenType type, string spelling)
    {
        Type = type;
        Spelling = spelling;
    }
}