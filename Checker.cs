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

            // Check if the variable on the left-hand side of the assignment is declared
            if (!IsVariableDeclaredInScope(node.Identifier.Name))
            {
                throw new Exception($"Variable '{node.Identifier.Name}' is not defined in the current or any outer scope.");
            }

            // Now check the right-hand side expression
            node.RightHandSide?.Accept(this);
        }

        private bool IsVariableDeclaredInScope(string variableName)
        {
            foreach (var scope in _scopes)
            {
                if (scope.IsVariableDefined(variableName))
                {
                    return true;
                }
            }
            return false;
        }


        public void Visit(BinaryExpressionNode node)
        {
            node.Left?.Accept(this);
            node.Right?.Accept(this);
        }

        public void Visit(FunctionCallNode node)
        {
            if (!_symbolTable.TryGetFunctionByName(node.FunctionName, out var function))
            {
                throw new Exception($"Function '{node.FunctionName}' is not defined.");
            }
            if (node.Arguments.Count != function!.Parameters.Count)
            {
                throw new Exception($"Function '{node.FunctionName}' is called with incorrect number of arguments.");
            }

            // Optionally, check the types of arguments

            foreach (var arg in node.Arguments)
            {
                arg.Accept(this);
            }
        }

        public void Visit(IdentifierNode node)
        {
            if (!IsVariableDeclaredInScope(node.Name))
            {
                throw new Exception($"Variable '{node.Name}' is not defined in the current or any outer scope.");
            }

            foreach (var index in node.Indices)
            {
                index.Accept(this); // Check indices if it's an array access
            }
        }

        private bool IsIdentifierDefinedInScope(string identifierName)
        {
            // Check in the current scope and all outer scopes
            foreach (var scope in _scopes)
            {
                if (scope.IsVariableDefined(identifierName))
                {
                    return true;
                }
            }
            // Additionally, check for function names at the global scope
            return _symbolTable.IsNameDefined(identifierName);
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
                    throw new Exception("Not Recognized");
            }

        }

        public void Visit(ExpressionNode node)
        {

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
                    throw new Exception("Not Recognized");
            }

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
            
            public bool TryGetFunctionByName(string name,out FunctionDeclarationNode? function)
            {
                if (_functions.ContainsKey(name))
                {
                    function = _functions[name];
                    return true;
                }
                function = null;
                return false;
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