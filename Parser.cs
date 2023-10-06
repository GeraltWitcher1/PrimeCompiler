using Prime.Tokens;

namespace Prime;

public class Parser
{
    private readonly Scanner _scanner;
    private Token _currentToken;

    public Parser(Scanner scanner)
    {
        _scanner = scanner;
        _currentToken = _scanner.GetNextToken();
    }

    public void ParseProgram()
    {
        ParseFunctionDeclarations();
        ParseMainFunction();
        
        if(_currentToken.Type != TokenType.EndOfFile )
            Console.WriteLine("Tokens found after end of program");
    }

    private void Consume(TokenType expectedType, string? expectedSpelling = null)
    {
        if (_currentToken.Type != expectedType || (expectedSpelling != null && _currentToken.Spelling != expectedSpelling))
        {
            var expected = expectedSpelling == null ? expectedType.ToString() : $"'{expectedSpelling}'";
            var actual = _currentToken.Type == TokenType.EndOfFile ? "end of file" : $"'{_currentToken.Spelling}'";
            throw new ParseException($"Syntax error: expected {expected} but found {actual}", _currentToken.LineNumber);
        }

        var token = _currentToken;
        _currentToken = _scanner.GetNextToken();
    }
    
    private bool Match(TokenType expectedType)
    {
        if (_currentToken.Type != expectedType) 
            return false;
        _currentToken = _scanner.GetNextToken();
        return true;
    }
    
    private void ParseFunctionDeclarations()
    {
        while (_currentToken.Type == TokenType.Func)
        {
            // Consume 'func' token
            Consume(TokenType.Func);

            // Parse function name (Identifier)
            Consume(TokenType.Identifier);

            // Check for parameters
            if (_currentToken.Type == TokenType.LeftParen)
            {
                Consume(TokenType.LeftParen);

                // Parse parameter list if it exists
                if (_currentToken.Type == TokenType.Type)
                {
                    ParseParameters();
                }

                // Consume closing parenthesis
                Consume(TokenType.RightParen);
            }

            // Check for return type (-> Type)
            if (_currentToken.Type == TokenType.Operator)
            {
                Consume(TokenType.Operator, "->");
                Consume(TokenType.Type);
            }

            // Consume opening curly brace '{'
            Consume(TokenType.LeftCurly);

            // Parse statements within the function body
            ParseStatements();

            // Consume closing curly brace '}'
            Consume(TokenType.RightCurly);
        }
    }

    private void ParseStatements()
{
    while (_currentToken.Type != TokenType.RightCurly)
    {
        switch (_currentToken.Type)
        {
            case TokenType.Type:
                // Parse variable declaration statement
                Consume(TokenType.Type);
                Consume(TokenType.Identifier);
                Consume(TokenType.Operator, ":=");
                ParseExpression();
                Consume(TokenType.Semicolon);
                break;
            case TokenType.If:
            {
                // Parse if statement
                Consume(TokenType.If);
                ParseExpression();
                Consume(TokenType.LeftCurly);
                ParseStatements();
                Consume(TokenType.RightCurly);
                if (Match(TokenType.Else))
                {
                    Consume(TokenType.Else);
                    Consume(TokenType.LeftCurly);
                    ParseStatements();
                    Consume(TokenType.RightCurly);
                }

                break;
            }
            case TokenType.For:
                // Parse for loop statement
                Consume(TokenType.For);
                Consume(TokenType.Type);
                Consume(TokenType.Identifier);
                Consume(TokenType.Operator, ":=");
                ParseExpression();
                Consume(TokenType.Semicolon);
                ParseExpression();
                Consume(TokenType.Semicolon);
                ParseExpression();
                Consume(TokenType.LeftCurly);
                ParseStatements();
                Consume(TokenType.RightCurly);
                break;
            case TokenType.Return:
            {
                // Parse return statement
                Consume(TokenType.Return);
                if (_currentToken.Type != TokenType.Semicolon)
                {
                    ParseExpression();
                }
                Consume(TokenType.Semicolon);
                break;
            }
            default:
                // Parse expression statement
                ParseExpression();
                Consume(TokenType.Semicolon);
                break;
        }
    }
}
    
    private void ParseExpression()
    {
        throw new NotImplementedException();
    }

    private void ParseParameters()
    {
        throw new NotImplementedException();
    }

    private void ParseMainFunction()
    {
        throw new NotImplementedException();
    }
}

internal class ParseException : Exception
{
    public ParseException(string s, int currentTokenLineNumber)
    {
        //implement this
        
    }
}