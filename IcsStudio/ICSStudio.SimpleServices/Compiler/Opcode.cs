namespace ICSStudio.SimpleServices.Compiler
{
    public class Opcode
    {
        public enum Code {
            NOP = 0,
            BIPUSH = 1,
            SIPUSH = 2,
            POP = 3,
            DUP = 4,
            CLOAD = 5,
            LOAD_LOCAL = 6,
            STORE_LOCAL = 7,
            LOAD_INT8_BIT = 8,
            LOAD_INT16_BIT = 9,
            LOAD_INT32_BIT = 10,
            LOAD_INT64_BIT = 11,
            LOAD_INT8 = 12,
            LOAD_INT16 = 13,
            LOAD_INT32 = 14,
            LOAD_INT64 = 15,
            LOAD_FLOAT = 16,
            LOAD_DOUBLE = 17,
            STORE_INT8_BIT = 18,
            STORE_INT16_BIT = 19,
            STORE_INT32_BIT = 20,
            STORE_INT64_BIT = 21,
            STORE_INT8 = 22,
            STORE_INT16 = 23,
            STORE_INT32 = 24,
            STORE_INT64 = 25,
            STORE_FLOAT = 26,
            STORE_DOUBLE = 27,
            PBINC = 28,
            PSINC = 29,
            PADD = 30,
            IADD = 31,
            LADD = 32,
            FADD = 33,
            DADD = 34,
            ISUB = 35,
            LSUB = 36,
            FSUB = 37,
            DSUB = 38,
            IMUL = 39,
            LMUL = 40,
            FMUL = 41,
            DMUL = 42,
            IDIV = 43,
            LDIV = 44,
            FDIV = 45,
            DDIV = 46,
            IMOD = 47,
            FMOD = 48,
            ICMP = 49,
            LMOD = 50,
            DMOD = 51,
            LCMP = 52,
            FCMP = 53,
            DCMP = 54,
            IAND = 55,
            LAND = 56,
            IOR = 57,
            LOR = 58,
            INOT = 59,
            BNOT = 60,
            LNOT = 61,
            IXOR = 62,
            LXOR = 63,
            INEG = 64,
            LNEG = 65,
            FNEG = 66,
            DNEG = 67,
            I2B = 68,
            I2S = 69,
            B2I = 70,
            S2I = 71,
            I2L = 72,
            I2F = 73,
            I2D = 74,
            L2I = 75,
            L2F = 76,
            L2D = 77,
            F2I = 78,
            F2L = 79,
            F2D = 80,
            D2I = 81,
            D2L = 82,
            D2F = 83,
            CALL = 84,
            RET = 85,
            JEQ = 86,
            JNE = 87,
            JGE = 88,
            JGT = 89,
            JLE = 90,
            JLT = 91,
            JMP = 92,
            BREAKPOINT = 93,
            ECHECK = 94,
            THROW = 95,
            DUP_X1 = 96,
            SWAP = 97,
            CONST_PNULL = 98,
            CONST_I0 = 99,
            CONST_I1 = 100,
            CONST_L0 = 101,
            CONST_L1 = 102,
            CONST_F0 = 103,
            CONST_F1 = 104,
            CONST_D0 = 105,
            CONST_D1 = 106,
            CONST_M1 = 107,
            CHECK = 108,
            SWAP_X1 = 109,
            ISHL = 110,
            ISHR = 111,
            LSHL = 112,
            LSHR = 113,
            NUM_OPCODES = 114,
        };
        public const byte NOP = 0;
        public const byte BIPUSH = 1;
        public const byte SIPUSH = 2;
        public const byte POP = 3;
        public const byte DUP = 4;
        public const byte CLOAD = 5;
        public const byte LOAD_LOCAL = 6;
        public const byte STORE_LOCAL = 7;
        public const byte LOAD_INT8_BIT = 8;
        public const byte LOAD_INT16_BIT = 9;
        public const byte LOAD_INT32_BIT = 10;
        public const byte LOAD_INT64_BIT = 11;
        public const byte LOAD_INT8 = 12;
        public const byte LOAD_INT16 = 13;
        public const byte LOAD_INT32 = 14;
        public const byte LOAD_INT64 = 15;
        public const byte LOAD_FLOAT = 16;
        public const byte LOAD_DOUBLE = 17;
        public const byte STORE_INT8_BIT = 18;
        public const byte STORE_INT16_BIT = 19;
        public const byte STORE_INT32_BIT = 20;
        public const byte STORE_INT64_BIT = 21;
        public const byte STORE_INT8 = 22;
        public const byte STORE_INT16 = 23;
        public const byte STORE_INT32 = 24;
        public const byte STORE_INT64 = 25;
        public const byte STORE_FLOAT = 26;
        public const byte STORE_DOUBLE = 27;
        public const byte PBINC = 28;
        public const byte PSINC = 29;
        public const byte PADD = 30;
        public const byte IADD = 31;
        public const byte LADD = 32;
        public const byte FADD = 33;
        public const byte DADD = 34;
        public const byte ISUB = 35;
        public const byte LSUB = 36;
        public const byte FSUB = 37;
        public const byte DSUB = 38;
        public const byte IMUL = 39;
        public const byte LMUL = 40;
        public const byte FMUL = 41;
        public const byte DMUL = 42;
        public const byte IDIV = 43;
        public const byte LDIV = 44;
        public const byte FDIV = 45;
        public const byte DDIV = 46;
        public const byte IMOD = 47;
        public const byte FMOD = 48;
        public const byte ICMP = 49;
        public const byte LMOD = 50;
        public const byte DMOD = 51;
        public const byte LCMP = 52;
        public const byte FCMP = 53;
        public const byte DCMP = 54;
        public const byte IAND = 55;
        public const byte LAND = 56;
        public const byte IOR = 57;
        public const byte LOR = 58;
        public const byte INOT = 59;
        public const byte BNOT = 60;
        public const byte LNOT = 61;
        public const byte IXOR = 62;
        public const byte LXOR = 63;
        public const byte INEG = 64;
        public const byte LNEG = 65;
        public const byte FNEG = 66;
        public const byte DNEG = 67;
        public const byte I2B = 68;
        public const byte I2S = 69;
        public const byte B2I = 70;
        public const byte S2I = 71;
        public const byte I2L = 72;
        public const byte I2F = 73;
        public const byte I2D = 74;
        public const byte L2I = 75;
        public const byte L2F = 76;
        public const byte L2D = 77;
        public const byte F2I = 78;
        public const byte F2L = 79;
        public const byte F2D = 80;
        public const byte D2I = 81;
        public const byte D2L = 82;
        public const byte D2F = 83;
        public const byte CALL = 84;
        public const byte RET = 85;
        public const byte JEQ = 86;
        public const byte JNE = 87;
        public const byte JGE = 88;
        public const byte JGT = 89;
        public const byte JLE = 90;
        public const byte JLT = 91;
        public const byte JMP = 92;
        public const byte BREAKPOINT = 93;
        public const byte ECHECK = 94;
        public const byte THROW = 95;
        public const byte DUP_X1 = 96;
        public const byte SWAP = 97;
        public const byte CONST_PNULL = 98;
        public const byte CONST_I0 = 99;
        public const byte CONST_I1 = 100;
        public const byte CONST_L0 = 101;
        public const byte CONST_L1 = 102;
        public const byte CONST_F0 = 103;
        public const byte CONST_F1 = 104;
        public const byte CONST_D0 = 105;
        public const byte CONST_D1 = 106;
        public const byte CONST_M1 = 107;
        public const byte CHECK = 108;
        public const byte SWAP_X1 = 109;
        public const byte ISHL = 110;
        public const byte ISHR = 111;
        public const byte LSHL = 112;
        public const byte LSHR = 113;
        public const byte NUM_OPCODES = 114;
        public static int[] ARG_SIZE = {
            0,
            1,
            2,
            0,
            0,
            4,
            2,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2,
            1,
            4,
            4,
            4,
            4,
            4,
            4,
            4,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
        };
    }
}
