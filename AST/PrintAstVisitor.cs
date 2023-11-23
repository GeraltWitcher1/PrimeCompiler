namespace Prime.AST;

public class PrintAstVisitor : IAstVisitor
{
    private int _indentLevel = 0;
    private void PrintIndented(string text)
    {
        Console.WriteLine(new string(' ', _indentLevel * 2) + text);
    }
    private void IncreaseIndent() => _indentLevel++;
    private void DecreaseIndent() => _indentLevel--;
    public object? Visit(ProgramNode node, object? arg = null)
    {
        PrintIndented("Program");
        IncreaseIndent();
        foreach (var func in node.FunctionDeclarations)
        {
            func.Accept(this);
        }
        DecreaseIndent();
        return null;
    }
    public object? Visit(FunctionDeclarationNode node, object? arg = null)
    {
        PrintIndented($"Function: {node.Name}");
        IncreaseIndent();
        foreach (var param in node.Parameters)
        {
            param.Accept(this);
        }
        node.ReturnType?.Accept(this);
        foreach (var statement in node.Statements)
        {
            statement.Accept(this);
        }
        DecreaseIndent();
        return null;
    }
    public object? Visit(ParameterNode node, object? arg = null)
    {
        PrintIndented($"Parameter: {node.Name} : {node.Type.TypeName}");
        return null;
    }
    public object? Visit(VariableDeclarationNode node, object? arg = null)
    {
        PrintIndented($"Variable Declaration: {node.Identifier} : {node.Type.TypeName}");
        IncreaseIndent();
        node.Initializer?.Accept(this);
        DecreaseIndent();
        return null;
    }
    public object? Visit(IfStatementNode node, object? arg = null)
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
        return null;
    }
    public object? Visit(ForLoopNode node, object? arg = null)
    {
        PrintIndented($"For Loop: {node.Identifier}");
        IncreaseIndent();
        node.Initializer?.Accept(this);
        node.Condition?.Accept(this);
        node.Increment?.Accept(this);
        node.Statements.ForEach(statement => statement.Accept(this));
        DecreaseIndent();
        return null;
    }
    public object? Visit(ReturnStatementNode node, object? arg = null)
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
        return null;
    }
    public object? Visit(ExpressionStatementNode node, object? arg = null)
    {
        PrintIndented("Expression Statement");
        IncreaseIndent();
        PrintIndented("Expression:");
        IncreaseIndent();
        node.Expression?.Accept(this);
        DecreaseIndent();
        DecreaseIndent();
        return null;
    }
    public object? Visit(AssignmentExpressionNode node, object? arg = null)
    {
        PrintIndented($"Assignment Expression: {node.Identifier.Name}");
        IncreaseIndent();
        node.RightHandSide?.Accept(this);
        DecreaseIndent();
        return null;
    }
    public object? Visit(BinaryExpressionNode node, object? arg = null)
    {
        PrintIndented($"Binary Expression: Operator {node.OperatorSpelling}");
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
        return null;
    }
    public object? Visit(FunctionCallNode node, object? param = null)
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
        return null;
    }
    public object? Visit(IdentifierNode node, object? arg = null)
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
        return null;
    }
    public object? Visit(LiteralNode node, object? arg = null)
    {
        PrintIndented($"Literal: {node.Value}");
        return null;
    }
    public object? Visit(TypeNode node, object? arg = null)
    {
        PrintIndented($"Type: {node.TypeName}");
        return null;
    }
 
         
}