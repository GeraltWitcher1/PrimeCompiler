using Prime.AST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime
{
    public class Checker : IAstVisitor
    {
        private SymbolTable _symbolTable = new SymbolTable();
        private Stack<Scope> _scopes = new Stack<Scope>();

        public Checker()
        {
            _scopes.Push(new Scope()); // Global scope
        }

        public void Visit(ProgramNode node)
        {
            foreach (var func in node.FunctionDeclarations)
            {
                func.Accept(this);
            }
        }

        public void Visit(FunctionDeclarationNode node)
        {
            if (!_symbolTable.TryAddFunction(node.Name, node))
            {
                throw new Exception($"Function '{node.Name}' is already defined.");
            }

            _scopes.Push(new Scope()); // Function scope
            foreach (var param in node.Parameters)
            {
                param.Accept(this);
            }
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
            _scopes.Pop();
        }

        public void Visit(ParameterNode node)
        {
            if (!_scopes.Peek().TryAddVariable(node.Name, node))
            {
                throw new Exception($"Parameter '{node.Name}' is already defined in this scope.");
            }
        }

        public void Visit(VariableDeclarationNode node)
        {
            if (!_scopes.Peek().TryAddVariable(node.Identifier, node))
            {
                throw new Exception($"Variable '{node.Identifier}' is already defined in this scope.");
            }
            node.Initializer?.Accept(this);
        }

        public void Visit(IfStatementNode node)
        {
            node.Condition?.Accept(this);
            _scopes.Push(new Scope()); // If branch scope
            foreach (var statement in node.IfBranch)
            {
                statement.Accept(this);
            }
            _scopes.Pop();
            if (node.ElseBranch != null)
            {
                _scopes.Push(new Scope()); // Else branch scope
                foreach (var statement in node.ElseBranch)
                {
                    statement.Accept(this);
                }
                _scopes.Pop();
            }
        }

        public void Visit(ForLoopNode node)
        {
            _scopes.Push(new Scope()); // For loop scope

            if(!_scopes.Peek().TryAddVariable(node.Identifier, node))
    {
                throw new Exception($"Variable '{node.Identifier}' is already defined in this scope.");
            }

            node.Initializer?.Accept(this);
            node.Condition?.Accept(this);
            node.Increment?.Accept(this);
            node.Statements.ForEach(statement => statement.Accept(this));
            _scopes.Pop();
        }

        public void Visit(ReturnStatementNode node)
        {
            node.ReturnValue?.Accept(this);
        }

        public void Visit(ExpressionStatementNode node)
        {
            node.Expression?.Accept(this);
        }

        public void Visit(AssignmentExpressionNode node)
        {
            node.Identifier?.Accept(this);
            if (!_scopes.Peek().IsVariableDefined(node.Identifier.Name))
            {
                throw new Exception($"Variable '{node.Identifier.Name}' is not defined.");
            }
            node.RightHandSide?.Accept(this);
        }

        public void Visit(BinaryExpressionNode node)
        {
            node.Left?.Accept(this);
            node.Right?.Accept(this);
        }

        public void Visit(IdentifierNode node)
        {
            if (!_symbolTable.IsNameDefined(node.Name))
            {
                throw new Exception($"Identifier '{node.Name}' is not defined.");
            }
            node.Indices.ForEach(index => index.Accept(this));
            node.Arguments?.ForEach(arg => arg.Accept(this));
        }

        public void Visit(LiteralNode node)
        {
            // Literals typically don't require context checks
        }

        public void Visit(TypeNode node)
        {
            // Type checking logic
        }

        public void Visit(StatementNode node)
        {
           
        }

        public void Visit(ExpressionNode node)
        {
            
        }

        // Other Visit methods...

        private class SymbolTable
        {
            private Dictionary<string, FunctionDeclarationNode> _functions = new Dictionary<string, FunctionDeclarationNode>();
            private HashSet<string> _variables = new HashSet<string>();

            public bool TryAddFunction(string name, FunctionDeclarationNode function)
            {
                if (_functions.ContainsKey(name))
                {
                    return false;
                }
                _functions[name] = function;
                return true;
            }

            public bool IsNameDefined(string name)
            {
                return _functions.ContainsKey(name) || _variables.Contains(name);
            }

            public void AddVariable(string name)
            {
                _variables.Add(name);
            }
        }

        private class Scope
        {
            private Dictionary<string, object> _variables = new Dictionary<string, object>();

            public bool TryAddVariable(string name, object variable)
            {
                if (_variables.ContainsKey(name))
                {
                    return false;
                }
                _variables[name] = variable;
                return true;
            }

            public bool IsVariableDefined(string name)
            {
                return _variables.ContainsKey(name);
            }
        }
    }
}