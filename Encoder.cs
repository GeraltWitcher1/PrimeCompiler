using Prime.AST; // Assuming this is where your AST classes are defined
using System;
using System.IO;

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
            else if (currentLevel - entityLevel <= 6)
                return Machine.LBr + currentLevel - entityLevel;
            else
            {
                Console.WriteLine("Accessing across too many levels");
                return Machine.L6r;
            }
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

        // Implement IAstVisitor methods
        public void Visit(ProgramNode node)
        {
            // Implementation logic
        }

        public void Visit(FunctionDeclarationNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(StatementNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(AssignmentExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(BinaryExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(IdentifierNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(LiteralNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(TypeNode node)
        {
            throw new NotImplementedException();
        }

        // ... Implement other methods from the IAstVisitor interface
    }

    // Assuming Instruction is a class with properties Op, N, R, D, and a Write method

}
