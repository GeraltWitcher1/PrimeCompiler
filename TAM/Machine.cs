    namespace Prime
    {
        public static class Machine
        {
            public const int MaxRoutineLevel = 7;

            // Operation codes
            public const int LoadOp = 0,
                             LoadAOp = 1,
                             LoadIOp = 2,
                             LoadLOp = 3,
                             StoreOp = 4,
                             StoreIOp = 5,
                             CallOp = 6,
                             CallIOp = 7,
                             ReturnOp = 8,
                             PushOp = 10,
                             PopOp = 11,
                             JumpOp = 12,
                             JumpIOp = 13,
                             JumpIfOp = 14,
                             HaltOp = 15;

            // Code store
            public static Instruction[] Code = new Instruction[1024];

            // Code store registers
            public const int CB = 0,
                             PB = 1024,  // upper bound of code array + 1
                             PT = 1052;  // PB + 28

            // Register numbers
            public const int CBr = 0,
                             CTr = 1,
                             PBr = 2,
                             PTr = 3,
                             SBr = 4,
                             STr = 5,
                             HBr = 6,
                             HTr = 7,
                             LBr = 8,
                             L1r = LBr + 1,
                             L2r = LBr + 2,
                             L3r = LBr + 3,
                             L4r = LBr + 4,
                             L5r = LBr + 5,
                             L6r = LBr + 6,
                             CPr = 15;

            // Data representation
            public const int BooleanSize = 1,
                             CharacterSize = 1,
                             IntegerSize = 1,
                             AddressSize = 1,
                             ClosureSize = 2 * AddressSize,
                             LinkDataSize = 3 * AddressSize,
                             FalseRep = 0,
                             TrueRep = 1,
                             MaxintRep = 32767;

            // Addresses of primitive routines
            public const int IdDisplacement = 1,
                             NotDisplacement = 2,
                             AndDisplacement = 3,
                             OrDisplacement = 4,
                             SuccDisplacement = 5,
                             PredDisplacement = 6,
                             NegDisplacement = 7,
                             AddDisplacement = 8,
                             SubDisplacement = 9,
                             MultDisplacement = 10,
                             DivDisplacement = 11,
                             ModDisplacement = 12,
                             LtDisplacement = 13,
                             LeDisplacement = 14,
                             GeDisplacement = 15,
                             GtDisplacement = 16,
                             EqDisplacement = 17,
                             NeDisplacement = 18,
                             EolDisplacement = 19,
                             EofDisplacement = 20,
                             GetDisplacement = 21,
                             PutDisplacement = 22,
                             GeteolDisplacement = 23,
                             PuteolDisplacement = 24,
                             GetintDisplacement = 25,
                             PutintDisplacement = 26,
                             NewDisplacement = 27,
                             DisposeDisplacement = 28;
        }
    }
