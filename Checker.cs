using Prime.AST;

namespace Prime
{
    public class Checker : IAstVisitor
    {
        private SymbolTable _symbolTable = new();
        private Stack<Scope> _scopes = new();

        public Checker()
        {
            _scopes.Push(new Scope()); // Global scope
        }

        public void Visit(ProgramNode node, object? arg = null)
        {
            foreach (var func in node.FunctionDeclarations)
            {
                func.Accept(this);
            }
        }

        public void Visit(FunctionDeclarationNode node, object? arg = null)
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

        public void Visit(ParameterNode node, object? arg = null)
        {
            if (!_scopes.Peek().TryAddVariable(node.Name, node))
            {
                throw new Exception($"Parameter '{node.Name}' is already defined in this scope.");
            }
        }

        public void Visit(VariableDeclarationNode node, object? arg = null)
        {
            if (!_scopes.Peek().TryAddVariable(node.Identifier, node))
            {
                throw new Exception($"Variable '{node.Identifier}' is already defined in this scope.");
            }

            node.Initializer?.Accept(this);
        }

        public void Visit(IfStatementNode node, object? arg = null)
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

        public void Visit(ForLoopNode node, object? arg = null)
        {
            _scopes.Push(new Scope()); // For loop scope

            if (!_scopes.Peek().TryAddVariable(node.Identifier, node))
            {
                throw new Exception($"Variable '{node.Identifier}' is already defined in this scope.");
            }

            node.Initializer?.Accept(this);
            node.Condition?.Accept(this);
            node.Increment?.Accept(this);
            node.Statements.ForEach(statement => statement.Accept(this));
            _scopes.Pop();
        }

        public void Visit(ReturnStatementNode node, object? arg = null)
        {
            node.ReturnValue?.Accept(this);
        }

        public void Visit(ExpressionStatementNode node, object? arg = null)
        {
            node.Expression?.Accept(this);
        }

        public void Visit(AssignmentExpressionNode node, object? arg = null)
        {
            node.Identifier?.Accept(this);

            // Check if the variable on the left-hand side of the assignment is declared
            if (!IsVariableDeclaredInScope(node.Identifier.Name))
            {
                throw new Exception(
                    $"Variable '{node.Identifier.Name}' is not defined in the current or any outer scope.");
            }

            // Now check the right-hand side expression
            node.RightHandSide?.Accept(this);
        }

        private bool IsVariableDeclaredInScope(string variableName, object? arg = null)
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
        
        public void Visit(BinaryExpressionNode node, object? arg = null)
        {
            node.Left?.Accept(this);
            node.Right?.Accept(this);
        }

        public void Visit(FunctionCallNode node, object? param = null)
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

        public void Visit(IdentifierNode node, object? arg = null)
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

        private bool IsIdentifierDefinedInScope(string identifierName, object? arg = null)
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


        public void Visit(LiteralNode node, object? arg = null)
        {
            // Literals typically don't require context checks
        }

        public void Visit(TypeNode node, object? arg = null)
        {
            // Type checking logic
        }
        
        
        private class SymbolTable
        {
            private Dictionary<string, FunctionDeclarationNode> _functions = new();

            private HashSet<string> _variables = new();

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
                if (_functions.TryGetValue(name, out var function1))
                {
                    function = function1;
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
            private Dictionary<string, object> _variables = new();

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