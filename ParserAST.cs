using Prime.AST;
using Prime.Tokens; // Import your AST namespace

namespace Prime
{
    public class ParserAST
    {
        private readonly Scanner _scanner;
        private Token _currentToken;

        public ParserAST(Scanner scanner)
        {
            _scanner = scanner;
            _currentToken = _scanner.GetNextToken();
        }

        public ProgramNode ParseProgram()
        {
            var program = new ProgramNode();
            ParseFunctionDeclarations(program);

            if (_currentToken.Type != TokenType.EndOfFile)
                throw new ParseException("Cannot define non functions at outer scope", _currentToken.Position);

            return program;
        }

        private Token Consume(TokenType expectedType, string? expectedSpelling = null)
        {
            if (_currentToken.Type != expectedType ||
                (expectedSpelling != null && _currentToken.Spelling != expectedSpelling))
            {
                var expected = expectedSpelling == null ? expectedType.ToString() : $"'{expectedSpelling}'";
                var actual = _currentToken.Type == TokenType.EndOfFile ? "end of file" : $"'{_currentToken.Spelling}'";
                throw new ParseException($"Syntax error: expected {expected} but found {actual}", _currentToken.Position);
            }

            var consumedToken = _currentToken;
            _currentToken = _scanner.GetNextToken();
            return consumedToken;
        }

        private bool MatchAndConsume(TokenType expectedType)
        {
            if (_currentToken.Type != expectedType)
                return false;
            Consume(expectedType);
            return true;
        }

        private void ParseFunctionDeclarations(ProgramNode program)
        {
            while (_currentToken.Type == TokenType.Func)
            {
                var functionDeclaration = new FunctionDeclarationNode();
                Consume(TokenType.Func);

                functionDeclaration.Name = Consume(TokenType.Identifier).Spelling;

               
                Consume(TokenType.LeftParen);

                if (_currentToken.Type == TokenType.Type)
                {
                    ParseParameters(functionDeclaration);
                }

                Consume(TokenType.RightParen);

                if (_currentToken.Type == TokenType.Arrow)
                {
                    Consume(TokenType.Arrow);
                    functionDeclaration.ReturnType = ParseType();
                }

                Consume(TokenType.LeftCurly);
                functionDeclaration.Statements = ParseStatements();
                Consume(TokenType.RightCurly);

                program.FunctionDeclarations.Add(functionDeclaration);
            }
        }

        private List<StatementNode> ParseStatements()
        {
            var statements = new List<StatementNode>();

            while (_currentToken.Type != TokenType.RightCurly)
            {
                switch (_currentToken.Type)
                {
                    case TokenType.Type:
                        statements.Add(ParseVariableDeclaration());
                        break;
                    case TokenType.If:
                        statements.Add(ParseIfStatement());
                        break;
                    case TokenType.For:
                        statements.Add(ParseForLoop());
                        break;
                    case TokenType.Return:
                        statements.Add(ParseReturnStatement());
                        break;
                    default:
                        statements.Add(ParseExpressionStatement());
                        break;
                }
            }

            return statements;
        }

        private VariableDeclarationNode ParseVariableDeclaration()
        {
            var variableDeclaration = new VariableDeclarationNode();
            variableDeclaration.Type = ParseType();
            variableDeclaration.Identifier = Consume(TokenType.Identifier).Spelling;
            Consume(TokenType.AssignOperator);
            variableDeclaration.Initializer = ParseExpression();
            Consume(TokenType.Semicolon);

            return variableDeclaration;
        }

        private IfStatementNode ParseIfStatement()
        {
            var ifStatement = new IfStatementNode();
            Consume(TokenType.If);
            ifStatement.Condition = ParseExpression();
            Consume(TokenType.LeftCurly);
            ifStatement.IfBranch = ParseStatements();
            Consume(TokenType.RightCurly);

            if (MatchAndConsume(TokenType.Else))
            {
                Consume(TokenType.Else);
                Consume(TokenType.LeftCurly);
                ifStatement.ElseBranch = ParseStatements();
                Consume(TokenType.RightCurly);
            }

            return ifStatement;
        }

        private ForLoopNode ParseForLoop()
        {
            var forLoop = new ForLoopNode();
            Consume(TokenType.For);
            Consume(TokenType.Type);
            forLoop.Identifier = Consume(TokenType.Identifier).Spelling;
            Consume(TokenType.AssignOperator);
            forLoop.Initializer = ParseExpression();
            Consume(TokenType.Semicolon);
            forLoop.Condition = ParseExpression();
            Consume(TokenType.Semicolon);
            forLoop.Increment = ParseExpression();
            Consume(TokenType.LeftCurly);
            forLoop.Statements = ParseStatements();
            Consume(TokenType.RightCurly);

            return forLoop;
        }

        private ReturnStatementNode ParseReturnStatement()
        {
            var returnStatement = new ReturnStatementNode();
            Consume(TokenType.Return);

            if (!MatchAndConsume(TokenType.Semicolon))
            {
                returnStatement.ReturnValue = ParseExpression();
            }

            Consume(TokenType.Semicolon);

            return returnStatement;
        }

        private ExpressionStatementNode ParseExpressionStatement()
        {
            var expressionStatement = new ExpressionStatementNode();
            expressionStatement.Expression = ParseExpression();
            Consume(TokenType.Semicolon);

            return expressionStatement;
        }

        private ExpressionNode ParseExpression()
        {
            var expression = ParseExpression1();

            if (MatchAndConsume(TokenType.AssignOperator))
            {
                var assignmentExpression = new AssignmentExpressionNode
                {
                    Identifier = expression as IdentifierNode,
                    RightHandSide = ParseExpression1()
                };
                return assignmentExpression;
            }

            return expression;
        }

        private ExpressionNode ParseExpression1()
        {
            var expression = ParseExpression2();

            while (MatchAndConsume(TokenType.OperatorL1))
            {
                var binaryExpression = new BinaryExpressionNode
                {
                    Left = expression,
                    Operator = _currentToken.Type,
                    Right = ParseExpression2()
                };
                expression = binaryExpression;
            }

            return expression;
        }

        private ExpressionNode ParseExpression2()
        {
            var expression = ParseExpression3();

            while (MatchAndConsume(TokenType.OperatorL2))
            {
                var binaryExpression = new BinaryExpressionNode
                {
                    Left = expression,
                    Operator = _currentToken.Type,
                    Right = ParseExpression3()
                };
                expression = binaryExpression;
            }

            return expression;
        }

        private ExpressionNode ParseExpression3()
        {
            var expression = ParseExpression4();

            while (MatchAndConsume(TokenType.OperatorL3))
            {
                var binaryExpression = new BinaryExpressionNode
                {
                    Left = expression,
                    Operator = _currentToken.Type,
                    Right = ParseExpression4()
                };
                expression = binaryExpression;
            }

            return expression;
        }

        private ExpressionNode ParseExpression4()
        {
            var expression = ParsePrimary();

            while (MatchAndConsume(TokenType.OperatorL4))
            {
                var binaryExpression = new BinaryExpressionNode
                {
                    Left = expression,
                    Operator = _currentToken.Type,
                    Right = ParsePrimary()
                };
                expression = binaryExpression;
            }

            return expression;
        }

        private ExpressionNode ParsePrimary()
        {
            switch (_currentToken.Type)
            {
                case TokenType.LeftParen:
                    Consume(TokenType.LeftParen);
                    var expression = ParseExpression1();
                    Consume(TokenType.RightParen);
                    return expression;
                case TokenType.Identifier:
                    return ParseIdentifier();
                case TokenType.Integer:
                    return new LiteralNode { Value = int.Parse(Consume(TokenType.Integer).Spelling) };
                default:
                    throw new ParseException($"Unexpected token {_currentToken.Type}", _currentToken.Position);
            }
        }

        private IdentifierNode ParseIdentifier()
        {
            var identifier = new IdentifierNode { Name = Consume(TokenType.Identifier).Spelling };

            if (MatchAndConsume(TokenType.LeftBracket)) //array access
            {
                identifier.Indices.Add(ParseExpression());
                Consume(TokenType.RightBracket);
            }
            else if (MatchAndConsume(TokenType.LeftParen)) //function call
            {
                identifier.Arguments = ParseArguments();
                Consume(TokenType.RightParen);
            }

            return identifier;
        }

        private List<ExpressionNode> ParseArguments()
        {
            var arguments = new List<ExpressionNode>();

            if (MatchAndConsume(TokenType.RightParen))
                return arguments;

            do
            {
                arguments.Add(ParseExpression());
            } while (MatchAndConsume(TokenType.Comma));

            return arguments;
        }

        private void ParseParameters(FunctionDeclarationNode functionDeclaration)
        {
            if (MatchAndConsume(TokenType.RightParen))
                return;

            do
            {
                var parameter = new ParameterNode
                {
                    Type = ParseType(),
                    Name = Consume(TokenType.Identifier).Spelling
                };
                functionDeclaration.Parameters.Add(parameter);
            } while (MatchAndConsume(TokenType.Comma));
        }

        private TypeNode ParseType()
        {
            return new TypeNode { TypeName = Consume(TokenType.Type).Spelling };
        }

    }

    internal class ParseException : Exception
    {
        public ParseException(string message, int currentTokenLineNumber)
            : base($"Parse error at position {currentTokenLineNumber}: {message}")
        {
        }
    }
}
