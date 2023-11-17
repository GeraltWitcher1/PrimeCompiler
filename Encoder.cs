using Prime.AST;
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

        // Add more methods for handling specific AST nodes
        // ...

        public void Visit(ProgramNode node)
        {
            _currentLevel = 0;

            // Process the program node
            // ...

            Emit(Machine.HaltOp, 0, 0, 0);
        }

        public void Visit(FunctionDeclarationNode node)
        {
            // Handle function declaration
            // ...

            _currentLevel++;
            // Process function parameters and body
            _currentLevel--;
        }

        // Implement other Visit methods for handling different AST nodes
        // ...

        public void SaveTargetProgram(string fileName)
        {
            try
            {
                using (var outStream = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    for (int i = Machine.CB; i < _nextAddress; i++)
                    {
                        Machine.Code[i].Write(outStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trouble writing " + fileName + ": " + ex.Message);
            }
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

        // Additional helper methods as needed
        // ...
    }
}
