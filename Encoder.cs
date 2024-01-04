using System.Drawing;
using Prime.AST;

namespace Prime
{
    public class Encoder : IAstVisitor
    {
        private int _nextAddress = Machine.CB;
        private int _currentLevel;

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
                using var outStream = new BinaryWriter(new FileStream(fileName, FileMode.Create));
                for (var i = Machine.CB; i < _nextAddress; i++)
                    Machine.Code[i].Write(outStream);

                Console.WriteLine("Compiled!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Trouble writing {fileName}: {ex.Message}");
            }
        }

        public object? Visit(ProgramNode node, object? arg = null)
        {
            _currentLevel = 0;
            
            int before = _nextAddress;
            Emit( Machine.JumpOp, 0, Machine.CB, 0 );
            var size = HandleFunctionDeclarations(node.FunctionDeclarations, new Address());
            var mainFunction = node.FunctionDeclarations.First(f => f.Name == "main");
            Patch(before, mainFunction.Address.Displacement);
            if(size> 0)
                Emit( Machine.PushOp, 0, 0, size );

            //TODO if this doesn't work add main node type
            
            Emit( Machine.HaltOp, 0, 0, 0 );
            return null;
        }

        private int HandleFunctionDeclarations(List<FunctionDeclarationNode> functionDeclarations, Address adr)
        {
            int startDisplacement = adr.Displacement;
            foreach (var function in functionDeclarations)
            {
                adr = (Address) function.Accept(this, adr)!;
            }

            int size = adr.Displacement - startDisplacement;
            return size;
        }

        public object? Visit(FunctionDeclarationNode node, object? arg = null)
        {
            node.Address = new Address(_currentLevel, _nextAddress);
            if (arg is null)
            {
                throw new Exception("Address required");
            }


            _currentLevel++;
            
            var adr = new Address((Address) arg);

            // Emit(Machine.CallOp, 0, Machine.PBr, Machine.PBr + 1);

            int size = HandleParams(node.Parameters, adr);
            var newAdd = new Address(adr, size);
            HandleParams(node.Parameters, newAdd);

            // Visit the function's body
            
            var returnStatement = node.Statements.FirstOrDefault(st => st is ReturnStatementNode);
            if (returnStatement is not null)
            {
                var returnStatementNode = (ReturnStatementNode) returnStatement;
                returnStatementNode.ParameterSize = size;
            }
            foreach (var statement in node.Statements)
            {
                statement.Accept(this, new Address( adr, Machine.LinkDataSize ) );
            }

            // Emit(Machine.ReturnOp, 1, 0, size);
            _currentLevel--;
            return arg;
        }

        private int HandleParams(List<ParameterNode> parameters, Address adr)
        {
            int startDisplacement = adr.Displacement;
            foreach (var parameter in parameters)
            {
                adr = (Address) parameter.Accept(this, adr)!;
            }

            int size = adr.Displacement - startDisplacement;
            return size;
        }


        public object? Visit(ParameterNode node, object? arg = null)
        {
            return new Address((Address) arg, 1);
        }

        public object? Visit(ExpressionStatementNode node, object? arg = null)
        {
            return node.Expression.Accept(this, arg);
        }

        public object? Visit(AssignmentExpressionNode node, object? arg = null)
        {
            var adr = (Address) node.Identifier.Accept(this)!;
            node.RightHandSide.Accept(this, true);
            var register = DisplayRegister(_currentLevel, adr.Level);
            Emit(Machine.StoreOp, 1, register, adr.Displacement);
            var valueNeeded = (bool) (arg ?? false);
            if (valueNeeded)
                Emit(Machine.LoadOp, 1, register, adr.Displacement);
            return null;
        }

        public object? Visit(BinaryExpressionNode node, object? arg = null)
        {
            node.Left.Accept(this, arg);
            node.Right.Accept(this, arg);
            // var op = "+";
            var op = node.OperatorSpelling;
            var valueNeeded = arg is bool;
            if (valueNeeded)
            {
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
            
            return null;
        }

        public object? Visit(FunctionCallNode node, object? arg = null)
        {
            bool valueNeeded = (bool) (arg ?? false);

            foreach (var argument in node.Arguments)
            {
                argument.Accept(this, null);
            }
            
            var adr = node.FunctionDeclarationNode.Address;
            int register = DisplayRegister( _currentLevel, adr.Level );
		
            Emit( Machine.CallOp, register, Machine.CB, adr.Displacement );
		
            if( !valueNeeded )
                Emit( Machine.PopOp, 0, 0, 1 );
		
            return null;
        }

        public object? Visit(VariableDeclarationNode node, object? arg = null)
        {
            var add = (Address) arg!;
            
            node.Address = add;
            node.Initializer?.Accept(this, true);
            var register = DisplayRegister(_currentLevel, add.Level);
            Emit(Machine.StoreOp, 1, register, add.Displacement);

            var valueNeeded = arg is bool;

            if (valueNeeded)
                Emit(Machine.LoadOp, 1, register, add.Displacement);
            return new Address(add, 1);
        }

        public object? Visit(IfStatementNode node, object? arg = null)
        {
            node.Condition.Accept(this);

            int jumpIfAddress = _nextAddress++;
            Emit(Machine.JumpIfOp, 0, 0, 0);

            foreach (var statement in node.IfBranch)
            {
                statement.Accept(this);
            }

            Patch(jumpIfAddress, _nextAddress);

            if (node.ElseBranch == null) return null;
            {
                foreach (var statement in node.ElseBranch)
                {
                    statement.Accept(this);
                }
            }
            return null;
        }

        public object? Visit(ForLoopNode node, object? arg = null)
        {
            throw new NotImplementedException();
        }

        public object? Visit(ReturnStatementNode node, object? arg = null)
        {
            // Visit the return value and push it onto the stack
            node.ReturnValue?.Accept(this, true);

            // Emit the RETURN instruction
            Emit(Machine.ReturnOp, 1, 0, 0);

            return null;
        }


        // Implement methods for other statement nodes and expression nodes...

        public object? Visit(IdentifierNode node, object? arg = null)
        {
            return null;
        }

        public object? Visit(LiteralNode node, object? arg = null)
        {
            var valueNeeded = (bool) (arg ?? false);
            if (node.Value is int val && valueNeeded)
            {
                Emit(Machine.LoadLOp, 1, 0, val);
            }
            else
            {
                // Handle other literal types
            }

            return null;
        }

        public object? Visit(TypeNode node, object? arg = null)
        {
            // Handle type nodes if necessary
            return null;
        }

        // ... Implement other methods from the IAstVisitor interface
    }

    // Assuming Instruction is a class with properties Op, N, R, D, and a Write method
}