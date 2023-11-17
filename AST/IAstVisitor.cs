using Prime.AST;

namespace Prime.AST
{
    public interface IAstVisitor
    {
        void Visit(ProgramNode node);
        void Visit(FunctionDeclarationNode node);
        void Visit(ParameterNode node);
        void Visit(StatementNode node);
        void Visit(ExpressionNode node);
        void Visit(AssignmentExpressionNode node);
        void Visit(BinaryExpressionNode node);
        void Visit(IdentifierNode node);
        void Visit(LiteralNode node);
        void Visit(TypeNode node);
    }
}