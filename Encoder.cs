using Prime.AST;

namespace Prime
{
    public class Encoder : IAstVisitor
    {
        private int _nextAddress = Machine.CB;
        private int _currentLevel = 0;

        private void Emit(int op, int n, int r, int d)
        {
            if (n > 255)
            {
                Console.WriteLine("Operand too long");
                n = 255;
            }

            var instr = new Instruction
            {
                Op = op,
                N = n,
                R = r,
                D = d
            };

            if (_nextAddress >= Machine.PB)
                Console.WriteLine("Program too large");
            else
                Machine.Code[_nextAddress++] = instr;
        }

        private void Patch(int address, int d)
        {
            Machine.Code[address].D = d;
        }

        private int DisplayRegister(int currentLevel, int entityLevel)
        {
            if (entityLevel == 0)
                return Machine.SBr;
            if (currentLevel - entityLevel <= 6)
                return Machine.LBr + currentLevel - entityLevel;
            Console.WriteLine("Accessing across too many levels");
            return Machine.L6r;
        }

        public void SaveTargetProgram(string fileName)
        {
            try
            {
                using (var outStream = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    for (int i = Machine.CB; i < _nextAddress; i++)
                        Machine.Code[i].Write(outStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Trouble writing {fileName}: {ex.Message}");
            }
        }

        public void Visit(ProgramNode node, object? arg = null)
        {
            foreach (var functionDeclaration in node.FunctionDeclarations)
            {
                functionDeclaration.Accept(this);
            }
        }

        public void Visit(FunctionDeclarationNode node, object? arg = null)
        {
            node.Address = new Address(_currentLevel, _nextAddress);
            if (arg is null)
            {
                throw new Exception("Address required");
            }
            var adr = new Address( (Address) arg );

            int size = 1; //TODO replace with 
            
            // Emit(Machine.CallOp, 0, Machine.PBr, Machine.PBr + 1);
            _currentLevel++;

            // Parameters handling
            foreach (var parameter in node.Parameters)
            {
                parameter.Accept(this);
            }

            // Visit the function's body
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }

            Emit(Machine.ReturnOp, 0, 0, 0);
            _currentLevel--;
        }

        public void Visit(ParameterNode node, object? arg = null)
        {
            // Parameters are handled during function declaration
        }

        public void Visit(ExpressionStatementNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        public void Visit(AssignmentExpressionNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        public void Visit(BinaryExpressionNode node, object? arg = null)
        {
            node.Left.Accept(this, true);
            node.Right.Accept(this, true);
            var op = node.Operator.ToString();

            switch (op)
            {
                case "+":
                    Emit(Machine.CallOp, 0, Machine.PBr, Machine.AddDisplacement);
                    break;
                case "-":
                    Emit(Machine.CallOp, 0, Machine.PBr, Machine.SubDisplacement);
                    break;
                case "*":
                    Emit(Machine.CallOp, 0, Machine.PBr, Machine.MultDisplacement);
                    break;
                case "/":
                    Emit(Machine.CallOp, 0, Machine.PBr, Machine.DivDisplacement);
                    break;
                default:
                    // Handle other operators as needed
                    throw new NotImplementedException($"Operator {op} not implemented.");
            }
        }

        public void Visit(FunctionCallNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        public void Visit(VariableDeclarationNode node, object? arg = null)
        {
            if (node.Initializer != null)
            {
                node.Initializer.Accept(this);
            }
            else
            {
                // Handle the case where there's no initializer
                // Depending on your language, you may need different instructions here
            }

            Emit(Machine.StoreOp, _currentLevel - node.Address.Level, node.Address.Displacement, 0);
        }

        public void Visit(IfStatementNode node, object? arg = null)
        {
            node.Condition.Accept(this);

            int jumpIfAddress = _nextAddress++;
            Emit(Machine.JumpIfOp, 0, 0, 0);

            foreach (var statement in node.IfBranch)
            {
                statement.Accept(this);
            }

            Patch(jumpIfAddress, _nextAddress);

            if (node.ElseBranch == null) return;
            {
                foreach (var statement in node.ElseBranch)
                {
                    statement.Accept(this);
                }
            }
        }

        public void Visit(ForLoopNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatementNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        // Implement methods for other statement nodes and expression nodes...

        public void Visit(IdentifierNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        public void Visit(LiteralNode node, object? arg = null)
        {
            if (node.Value is int val)
            {
                Emit(Machine.LoadLOp, 0, 0, val);
            }
            else
            {
                // Handle other literal types
            }
        }

        public void Visit(TypeNode node, object? arg = null)
        {
            // Handle type nodes if necessary
        }

        // ... Implement other methods from the IAstVisitor interface
    }

    // Assuming Instruction is a class with properties Op, N, R, D, and a Write method
}