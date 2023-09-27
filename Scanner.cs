using Prime.Tokens;

namespace Prime;

public class Scanner
{
    private readonly string _input;
    private int _position;

    public Scanner(string input)
    {
        _input = input;
        _position = 0;
    }

    private char Current => Peek(0);
    private char Lookahead => Peek(1);

    private char Peek(int offset)
    {
        var index = _position + offset;

        return index >= _input.Length ? '\0' : _input[index];
    }

    public Token? GetNextToken()
    {
        if (_position >= _input.Length)
        {
            return null; // End of input
        }

        if (char.IsLetter(Current))
        {
            // Identifier or Keyword
            var lexeme = ReadIdentifierOrKeyword();
            return TokenFacts.IsKeyword(lexeme)
                ? TokenFacts.GetKeywordToken(lexeme)
                : new Token(TokenType.Identifier, lexeme);
        }

        if (char.IsDigit(Current))
        {
            // Integer
            var lexeme = ReadInteger();
            return new Token(TokenType.Integer, lexeme);
        }

        if (char.IsWhiteSpace(Current))
        {
            // Whitespace
            _position++;
            return GetNextToken();
        }

        if (TokenFacts.IsOperator(Current))
        {
            // Operator
            string spelling = ReadOperator();
            return new Token(TokenType.Operator, spelling);
        }

        _position++;
        return GetNextToken();
    }


    // Helper methods for reading specific token types
    private string ReadIdentifierOrKeyword()
    {
        int start = _position;
        while (_position < _input.Length && char.IsLetterOrDigit(Current))
        {
            _position++;
        }

        return _input.Substring(start, _position - start);
    }

    private string ReadInteger()
    {
        int start = _position;
        while (_position < _input.Length && char.IsDigit(Current))
        {
            _position++;
        }

        return _input.Substring(start, _position - start);
    }

    private string ReadOperator()
    {
        // Implement operator recognition logic here
        // For simplicity, we'll assume operators are single characters
        return _input[_position++].ToString();
    }
}