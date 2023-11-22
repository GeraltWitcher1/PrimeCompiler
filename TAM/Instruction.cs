namespace Prime
{
    public class Instruction
    {
        // Assuming the OpCode, Length, and Operand types are represented by 'int' in C#.

        public int Op; // OpCode
        public int R;  // RegisterNumber
        public int N;  // Length
        public int D;  // Operand

        public Instruction()
        {
            Op = 0;
            R = 0;
            N = 0;
            D = 0;
        }

        public void Write(BinaryWriter output)
        {
            output.Write(Op);
            output.Write(R);
            output.Write(N);
            output.Write(D);
        }

        public static Instruction Read(BinaryReader input)
        {
            try
            {
                return new Instruction
                {
                    Op = input.ReadInt32(),
                    R = input.ReadInt32(),
                    N = input.ReadInt32(),
                    D = input.ReadInt32()
                };
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }
    }
}
