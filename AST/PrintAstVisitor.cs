namespace Prime.AST
{
    public class PrintAstVisitor : IAstVisitor
    {
        private int _indentLevel = 0;

        private void PrintIndented(string text)
        {
            Console.WriteLine(new string(' ', _indentLevel * 2) + text);
        }

        private void IncreaseIndent() => _indentLevel++;
        private void DecreaseIndent() => _indentLevel--;

        public void Visit(ProgramNode node)
        {
            PrintIndented("Program");
            IncreaseIndent();
            foreach (var func in node.FunctionDeclarations)
            {
                func.Accept(this);
            }
            DecreaseIndent();
        }

        public void Visit(FunctionDeclarationNode node)
        {
            PrintIndented($"Function: {node.Name}");
            IncreaseIndent();
            foreach (var param in node.Parameters)
            {
                param.Accept(this);
            }
            if (node.ReturnType != null)
            {
                node.ReturnType.Accept(this);
            }
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
            DecreaseIndent();
        }

        public void Visit(ParameterNode node)
        {
            PrintIndented($"Parameter: {node.Name} : {node.Type.TypeName}");
        }

        public void Visit(VariableDeclarationNode node)
        {
            PrintIndented($"Variable Declaration: {node.Identifier} : {node.Type.TypeName}");
            IncreaseIndent();
            node.Initializer?.Accept(this);
            DecreaseIndent();
        }

        public void Visit(IfStatementNode node)
        {
            PrintIndented("If Statement");
            IncreaseIndent();
            node.Condition?.Accept(this);
            PrintIndented("If Branch");
            IncreaseIndent();
            node.IfBranch.ForEach(statement => statement.Accept(this));
            DecreaseIndent();
            if (node.ElseBranch != null)
            {
                PrintIndented("Else Branch");
                IncreaseIndent();
                node.ElseBranch.ForEach(statement => statement.Accept(this));
                DecreaseIndent();
            }
            DecreaseIndent();
        }

        public void Visit(ForLoopNode node)
        {
            PrintIndented($"For Loop: {node.Identifier}");
            IncreaseIndent();
            node.Initializer?.Accept(this);
            node.Condition?.Accept(this);
            node.Increment?.Accept(this);
            node.Statements.ForEach(statement => statement.Accept(this));
            DecreaseIndent();
        }

        public void Visit(ReturnStatementNode node)
        {
            PrintIndented("Return Statement");
            IncreaseIndent();
            if (node.ReturnValue != null)
            {
                PrintIndented("Return Value:");
                IncreaseIndent();
                node.ReturnValue.Accept(this);
                DecreaseIndent();
            }
            else
            {
                PrintIndented("No return value");
            }
            DecreaseIndent();
        }

        public void Visit(ExpressionStatementNode node)
        {
            PrintIndented("Expression Statement");
            IncreaseIndent();
            PrintIndented("Expression:");
            IncreaseIndent();
            node.Expression?.Accept(this);
            DecreaseIndent();
            DecreaseIndent();
        }

        public void Visit(AssignmentExpressionNode node)
        {
            PrintIndented($"Assignment: {node.Identifier.Name}");
            IncreaseIndent();
            node.RightHandSide?.Accept(this);
            DecreaseIndent();
        }

        public void Visit(BinaryExpressionNode node)
        {
            PrintIndented($"Binary Expression: Operator {node.Operator}");
            IncreaseIndent();
            PrintIndented("Left:");
            IncreaseIndent();
            node.Left?.Accept(this);
            DecreaseIndent();
            PrintIndented("Right:");
            IncreaseIndent();
            node.Right?.Accept(this);
            DecreaseIndent();
            DecreaseIndent();
        }

        public void Visit(FunctionCallNode node)
        {
            PrintIndented($"Function Call: {node.FunctionName}");
            IncreaseIndent();
            PrintIndented("Arguments:");
            IncreaseIndent();
            foreach (var arg in node.Arguments)
            {
                arg.Accept(this);
            }
            DecreaseIndent();
            DecreaseIndent();
        }

        public void Visit(IdentifierNode node)
        {
            PrintIndented($"Identifier: {node.Name}");
            if (node.Indices.Any())
            {
                IncreaseIndent();
                foreach (var index in node.Indices)
                {
                    PrintIndented("Index:");
                    IncreaseIndent();
                    index.Accept(this);
                    DecreaseIndent();
                }
                DecreaseIndent();
            }
        }

        public void Visit(LiteralNode node)
        {
            PrintIndented($"Literal: {node.Value}");
        }

        public void Visit(TypeNode node)
        {
            PrintIndented($"Type: {node.TypeName}");
        }

        public void Visit(StatementNode node)
        {
            PrintIndented($"Statement Node: {node.GetType().Name}");
            IncreaseIndent();

            switch (node)
            {
                case VariableDeclarationNode variableDeclarationNode:
                    Visit(variableDeclarationNode);
                    break;
                case IfStatementNode ifStatementNode:
                    Visit(ifStatementNode);
                    break;
                case ForLoopNode forLoopNode:
                    Visit(forLoopNode);
                    break;
                case ReturnStatementNode returnStatementNode:
                    Visit(returnStatementNode);
                    break;
                case ExpressionStatementNode expressionStatementNode:
                    Visit(expressionStatementNode);
                    break;
                // Add cases for other StatementNode subclasses
                default:
                    PrintIndented("Unknown statement type");
                    break;
            }

            DecreaseIndent();
        }

        public void Visit(ExpressionNode node)
        {
            PrintIndented($"Expression Node: {node.GetType().Name}");
            IncreaseIndent();

            switch (node)
            {
                case AssignmentExpressionNode assignmentExpressionNode:
                    Visit(assignmentExpressionNode);
                    break;
                case BinaryExpressionNode binaryExpressionNode:
                    Visit(binaryExpressionNode);
                    break;
                case IdentifierNode identifierNode:
                    Visit(identifierNode);
                    break;
                case LiteralNode literalNode:
                    Visit(literalNode);
                    break;
                case FunctionCallNode functionCallNode:
                    Visit(functionCallNode);
                    break;
                // Add cases for other ExpressionNode subclasses
                default:
                    PrintIndented("Unknown expression type");
                    break;
            }

            DecreaseIndent();
        }


    }
}
