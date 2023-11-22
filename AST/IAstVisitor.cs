namespace Prime.AST
{
    public interface IAstVisitor
    {
        void Visit(ProgramNode node, object? arg = null);
        void Visit(FunctionDeclarationNode node, object? arg = null);
        void Visit(ParameterNode node, object? arg = null);
        void Visit(VariableDeclarationNode node, object? arg = null);
        void Visit(IfStatementNode node, object? arg = null);
        void Visit(ForLoopNode node, object? arg = null);
        void Visit(ReturnStatementNode node, object? arg = null);
        void Visit(ExpressionStatementNode node, object? arg = null);
        void Visit(AssignmentExpressionNode node, object? arg = null);
        void Visit(BinaryExpressionNode node, object? arg = null);
        void Visit(FunctionCallNode node, object? arg = null);
        void Visit(IdentifierNode node, object? arg = null);
        void Visit(LiteralNode node, object? arg = null);
        void Visit(TypeNode node, object? arg = null);
    }
}