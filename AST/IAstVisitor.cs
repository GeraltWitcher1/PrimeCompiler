namespace Prime.AST
{
    public interface IAstVisitor
    {
        object? Visit(ProgramNode node, object? arg = null);
        object? Visit(FunctionDeclarationNode node, object? arg = null);
        object? Visit(ParameterNode node, object? arg = null);
        object? Visit(VariableDeclarationNode node, object? arg = null);
        object? Visit(IfStatementNode node, object? arg = null);
        object? Visit(ForLoopNode node, object? arg = null);
        object? Visit(ReturnStatementNode node, object? arg = null);
        object? Visit(ExpressionStatementNode node, object? arg = null);
        object? Visit(AssignmentExpressionNode node, object? arg = null);
        object? Visit(BinaryExpressionNode node, object? arg = null);
        object? Visit(FunctionCallNode node, object? arg = null);
        object? Visit(IdentifierNode node, object? arg = null);
        object? Visit(LiteralNode node, object? arg = null);
        object? Visit(TypeNode node, object? arg = null);
    }
}