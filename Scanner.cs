namespace Prime;

public class Scanner
{
    private readonly string input;
    private int position;

    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        { "int", TokenType.Keyword },
        { "bool", TokenType.Keyword },
        { "char", TokenType.Keyword },
        { "array", TokenType.Keyword },
        { "func", TokenType.Keyword },
        { "return", TokenType.Keyword },
        { "if", TokenType.Keyword },
        { "else", TokenType.Keyword },
        { "for", TokenType.Keyword },
        { "main", TokenType.Keyword },
    };

    public Scanner(string input)
    {
        this.input = input;
        position = 0;
    }

    public Token? GetNextToken()
    {
        if (position >= input.Length)
        {
            return null; // End of input
        }

        char currentChar = input[position];

        if (char.IsLetter(currentChar) || currentChar == '_')
        {
            // Identifier or Keyword
            string lexeme = ReadIdentifierOrKeyword();
            if (Keywords.TryGetValue(lexeme, out var keyword))
            {
                return new Token(keyword, lexeme);
            }
            return new Token(TokenType.Identifier, lexeme);
        }
        else if (char.IsDigit(currentChar))
        {
            // Integer
            string lexeme = ReadInteger();
            return new Token(TokenType.Integer, lexeme);
        }
        else
        {
            // Handle other token types as needed
            
            // Operator
            string lexeme = ReadOperator();
            return new Token(TokenType.Operator, lexeme);
        }

        // If no token is recognized, move to the next character and try again
        position++;
        return GetNextToken();
    }

    // Helper methods for reading specific token types
    private string ReadIdentifierOrKeyword()
    {
        int start = position;
        while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
        {
            position++;
        }
        return input.Substring(start, position - start);
    }

    private string ReadInteger()
    {
        int start = position;
        while (position < input.Length && char.IsDigit(input[position]))
        {
            position++;
        }
        return input.Substring(start, position - start);
    }

    private string ReadOperator()
    {
        // Implement operator recognition logic here
        // For simplicity, we'll assume operators are single characters
        return input[position++].ToString();
    }

    // Add more helper methods for other token types if needed
}