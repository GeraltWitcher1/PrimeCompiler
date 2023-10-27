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

        if (_currentToken.Type != TokenType.EndOfFile)
            throw new ParseException("Cannot define non functions at outer scope", _currentToken.Position);
    }

    private void Consume(TokenType expectedType, string? expectedSpelling = null)
    {
        if (_currentToken.Type != expectedType ||
            (expectedSpelling != null && _currentToken.Spelling != expectedSpelling))
        {
            var expected = expectedSpelling == null ? expectedType.ToString() : $"'{expectedSpelling}'";
            var actual = _currentToken.Type == TokenType.EndOfFile ? "end of file" : $"'{_currentToken.Spelling}'";
            throw new ParseException($"Syntax error: expected {expected} but found {actual}", _currentToken.Position);
        }

        _currentToken = _scanner.GetNextToken();
    }

    private bool MatchAndConsume(TokenType expectedType)
    {
        if (_currentToken.Type != expectedType)
            return false;
        Consume(expectedType);
        return true;
    }

    private void ParseFunctionDeclarations()
    {
        while (_currentToken.Type == TokenType.Func)
        {
            Consume(TokenType.Func);

            if (_currentToken.Type == TokenType.Main)
            {
                ParseMainFunction();
                return;
            }

            Consume(TokenType.Identifier);
            Consume(TokenType.LeftParen);

            if (_currentToken.Type == TokenType.Type)
            {
                ParseParameters();
            }

            Consume(TokenType.RightParen);
            
            if (_currentToken.Type == TokenType.Arrow)
            {
                Consume(TokenType.Arrow);
                Consume(TokenType.Type);
            }

            Consume(TokenType.LeftCurly);
            ParseStatements();
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
                    Consume(TokenType.Type);
                    Consume(TokenType.Identifier);
                    Consume(TokenType.AssignOperator);
                    ParseExpression();
                    Consume(TokenType.Semicolon);
                    break;
                case TokenType.If:
                {
                    Consume(TokenType.If);
                    ParseExpression();
                    Consume(TokenType.LeftCurly);
                    ParseStatements();
                    Consume(TokenType.RightCurly);
                    if (MatchAndConsume(TokenType.Else))
                    {
                        Consume(TokenType.Else);
                        Consume(TokenType.LeftCurly);
                        ParseStatements();
                        Consume(TokenType.RightCurly);
                    }

                    break;
                }
                case TokenType.For:
                    Consume(TokenType.For);
                    Consume(TokenType.Type);
                    Consume(TokenType.Identifier);
                    Consume(TokenType.AssignOperator);
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
                    Consume(TokenType.Return);
                    if (!MatchAndConsume(TokenType.Semicolon))
                    {
                        ParseExpression();
                    }
                    Consume(TokenType.Semicolon);
                    break;
                }
                default:
                    ParseExpression();
                    Consume(TokenType.Semicolon);
                    break;
            }
        }
    }

    private void ParseExpression()
    {
        ParseExpression1();

        if (MatchAndConsume(TokenType.AssignOperator))
        {
            ParseExpression1();
        }
    }

    private void ParseExpression1()
    {
        ParseExpression2();

        while (MatchAndConsume(TokenType.OperatorL1))
        {
            ParseExpression2();
        }
    }

    private void ParseExpression2()
    {
        ParseExpression3();

        while (MatchAndConsume(TokenType.OperatorL2))
        {
            ParseExpression3();
        }
    }

    private void ParseExpression3()
    {
        ParseExpression4();

        while (MatchAndConsume(TokenType.OperatorL3))
        {
            ParseExpression4();
        }
    }

    private void ParseExpression4()
    {
        ParsePrimary();

        while (MatchAndConsume(TokenType.OperatorL4))
        {
            ParsePrimary();
        }
    }

    private void ParsePrimary()
    {
        switch (_currentToken.Type)
        {
            case TokenType.LeftParen: // ( expression )
                Consume(TokenType.LeftParen);
                ParseExpression1();
                Consume(TokenType.RightParen);
                break;
            case TokenType.Identifier:
                ParseIdentifier();
                break;
            case TokenType.Integer:
                Consume(TokenType.Integer);
                break;
            default:
                throw new ParseException($"Unexpected token {_currentToken.Type}", _currentToken.Position);
        }
    }

    private void ParseIdentifier()
    {
        Consume(TokenType.Identifier);
        if (MatchAndConsume(TokenType.LeftBracket)) //array access
        {
            ParseExpression();
            Consume(TokenType.RightBracket);
        }
        else if (MatchAndConsume(TokenType.LeftParen)) //function call
        {
            ParseArguments();
            Consume(TokenType.RightParen);
        }
    }

    private void ParseArguments()
    {
        if (MatchAndConsume(TokenType.RightParen)) return;
        do
        {
            Consume(TokenType.Identifier);
        } while (MatchAndConsume(TokenType.Comma));
    }

    private void ParseParameters()
    {
        if (MatchAndConsume(TokenType.RightParen)) return;
        do
        {
            Consume(TokenType.Type);
            Consume(TokenType.Identifier);
        } while (MatchAndConsume(TokenType.Comma));
    }

    private void ParseMainFunction()
    {
        Consume(TokenType.Main);
        Consume(TokenType.LeftParen);
        Consume(TokenType.RightParen);
        Consume(TokenType.LeftCurly);
        ParseStatements();
        Consume(TokenType.RightCurly);
    }
}

internal class ParseException : Exception
{
    public ParseException(string message, int currentTokenLineNumber)
        : base($"Parse error at position {currentTokenLineNumber}: {message}")
    {
    }
}