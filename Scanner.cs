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

    public Token GetNextToken()
    {
        if (_position >= _input.Length)
        {
            return new Token(TokenType.EndOfFile, "\0", _position); // End of input
        }

        if (char.IsLetter(Current))
        {
            // Identifier or Keyword
            var lexeme = ReadIdentifierOrKeyword();
            return TokenFacts.IsKeyword(lexeme)
                ? TokenFacts.GetKeywordToken(lexeme, _position)
                : new Token(TokenType.Identifier, lexeme, _position);
        }

        if (char.IsDigit(Current))
        {
            // Integer
            var lexeme = ReadInteger();
            return new Token(TokenType.Integer, lexeme, _position);
        }

        if (char.IsWhiteSpace(Current))
        {
            // Whitespace
            _position++;
            return GetNextToken();
        }
        if (TokenFacts.IsOperator(Current, Lookahead))
        {
            // Operator
            string spelling = ReadOperator();
            var tokenType = TokenFacts.GetOperatorLevel(spelling);
            return new Token(tokenType, spelling, _position);
        }

        var token = ReadToken(Current);
        
        return token;
    }

    private Token ReadToken(char current)
    {
        var token = current switch
        {
            ';' => new Token(TokenType.Semicolon, ";", _position),
            '(' => new Token(TokenType.LeftParen, "(", _position),
            ')' => new Token(TokenType.RightParen, ")", _position),
            '{' => new Token(TokenType.LeftCurly, "{", _position),
            '}' => new Token(TokenType.RightCurly, "}", _position),
            '[' => new Token(TokenType.LeftBracket, "[", _position),
            ']' => new Token(TokenType.RightBracket, "]", _position),
            ',' => new Token(TokenType.Comma, ",", _position),
            '-' when Lookahead == '>' => new Token(TokenType.Arrow, "->", _position),
            '\0' => new Token(TokenType.EndOfFile, "\0", _position),
            _ => new Token(TokenType.Error, Current.ToString(), _position)
        };
        _position += token.Spelling.Length;
        return token;
    }
    
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
        var token =  Current switch
        {
            '=' when Lookahead == '=' => "==",
            '!' when Lookahead == '=' => "!=",
            '<' when Lookahead == '=' => "<=",
            '>' when Lookahead == '=' => ">=",
            '*' when Lookahead == '=' => "*=",
            '/' when Lookahead == '=' => "/=",
            '+' when Lookahead == '=' => "+=",
            '-' when Lookahead == '=' => "-=",
            ':' when Lookahead == '=' => ":=",
            _ => Current.ToString()
        };
        _position += token.Length;
        return token;
    }
}