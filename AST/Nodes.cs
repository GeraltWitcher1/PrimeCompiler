using Prime.Tokens;

namespace Prime.AST
{
    public abstract class AstNode
    {
        public abstract object? Accept(IAstVisitor visitor, object? arg = null);
    }

    public class ProgramNode : AstNode
    {
        public List<FunctionDeclarationNode> FunctionDeclarations { get; set; } = new();

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class FunctionDeclarationNode : AstNode
    {
        public string Name { get; set; }
        public List<ParameterNode> Parameters { get; set; } = new();
        public TypeNode? ReturnType { get; set; }
        public List<StatementNode> Statements { get; set; } = new();
        public Address Address { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class ParameterNode : AstNode
    {
        public TypeNode Type { get; set; }
        public string Name { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public abstract class StatementNode : AstNode
    {
        // Additional properties/methods for statements...
    }


    public class ExpressionStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public abstract class ExpressionNode : AstNode
    {
        // Additional properties/methods for expressions...
    }

    public class AssignmentExpressionNode : ExpressionNode
    {
        public IdentifierNode Identifier { get; set; }
        public ExpressionNode RightHandSide { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public TokenType Operator { get; set; }
        public string OperatorSpelling { get; set; }
        public ExpressionNode Right { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class VariableDeclarationNode : StatementNode
    {
        public TypeNode Type { get; set; }
        public string Identifier { get; set; }
        public ExpressionNode? Initializer { get; set; }
        public Address Address { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class IfStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> IfBranch { get; set; } = new();
        public List<StatementNode>? ElseBranch { get; set; } = new();

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class ForLoopNode : StatementNode
    {
        public string Identifier { get; set; }
        public ExpressionNode Initializer { get; set; }
        public ExpressionNode Condition { get; set; }
        public ExpressionNode Increment { get; set; }
        public List<StatementNode> Statements { get; set; } = new();

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class ReturnStatementNode : StatementNode
    {
        public ExpressionNode? ReturnValue { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class IdentifierNode : ExpressionNode
    {
        public string Name { get; set; }
        public List<ExpressionNode> Indices { get; set; } = new();

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class FunctionCallNode : ExpressionNode
    {
        public string FunctionName { get; set; }
        public List<ExpressionNode> Arguments { get; set; } = new();
        
        public FunctionDeclarationNode FunctionDeclarationNode { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class LiteralNode : ExpressionNode
    {
        public object Value { get; set; }

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }

    public class TypeNode : AstNode
    {
        public string TypeName { get; set; } // E.g., "int", "bool", "char"

        public override object? Accept(IAstVisitor visitor, object? arg = null)
        {
            return visitor.Visit(this, arg);
        }
    }
}