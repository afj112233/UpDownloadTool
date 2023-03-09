using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class FBDInstructionCollection
    {
        private void Init()
        {

            _instrs["ABS"] = CommonInstruction.FABS;
            _instrs["ALM"] = CommonInstruction.ALM;
            _instrs["SCL"] = CommonInstruction.SCL;
            _instrs["PIDE"] = CommonInstruction.PIDE;
            _instrs["IMC"] = CommonInstruction.IMC;
            _instrs["CC"] = CommonInstruction.CC;
            _instrs["MMC"] = CommonInstruction.MMC;
            _instrs["RMPS"] = CommonInstruction.RMPS;
            _instrs["POSP"] = CommonInstruction.POSP;
            _instrs["SRTP"] = CommonInstruction.SRTP;
            _instrs["LDLG"] = CommonInstruction.LDLG;
            _instrs["FGEN"] = CommonInstruction.FGEN;
            _instrs["TOT"] = CommonInstruction.TOT;
            _instrs["DEDT"] = CommonInstruction.DEDT;
            _instrs["D2SD"] = CommonInstruction.D2SD;
            _instrs["D3SD"] = CommonInstruction.D3SD;

            _instrs["PMUL"] = CommonInstruction.PMUL;
            _instrs["SCRV"] = CommonInstruction.SCRV;
            _instrs["PI"] = CommonInstruction.PI;
            _instrs["INTG"] = CommonInstruction.INTG;
            _instrs["SOC"] = CommonInstruction.SOC;
            _instrs["UPDN"] = CommonInstruction.UPDN;

            _instrs["HPF"] = CommonInstruction.HPF;
            _instrs["LPF"] = CommonInstruction.LPF;
            _instrs["NTCH"] = CommonInstruction.NTCH;
            _instrs["LDL2"] = CommonInstruction.LDL2;
            _instrs["DERV"] = CommonInstruction.DERV;

            _instrs["SEL"] = CommonInstruction.SEL;
            _instrs["ESEL"] = CommonInstruction.ESEL;
            _instrs["SSUM"] = CommonInstruction.SSUM;
            _instrs["SNEG"] = CommonInstruction.SNEG;
            _instrs["MUX"] = CommonInstruction.MUX;
            _instrs["HLL"] = CommonInstruction.HLL;
            _instrs["RLIM"] = CommonInstruction.RLIM;

            _instrs["MAVE"] = CommonInstruction.MAVE;
            _instrs["MSTD"] = CommonInstruction.MSTD;
            _instrs["MINC"] = CommonInstruction.MINC;
            _instrs["MAXC"] = CommonInstruction.MAXC;

            _instrs["ALMD"] = CommonInstruction.FALMD;
            _instrs["ALMA"] = CommonInstruction.FALMA;
 
            _instrs["OSRI"] = CommonInstruction.OSRI;
            _instrs["OSFI"] = CommonInstruction.OSFI;

            _instrs["TONR"] = CommonInstruction.TONR;
            _instrs["TOFR"] = CommonInstruction.TOFR;
            _instrs["RTOR"] = CommonInstruction.RTOR;
            _instrs["CTUD"] = CommonInstruction.CTUD;

            _instrs["LIM"] = CommonInstruction.FLIM;
            _instrs["MEQ"] = CommonInstruction.FMEQ;
            _instrs["EQU"] = CommonInstruction.FEQU;
            _instrs["NEQ"] = CommonInstruction.FNEQ;
            _instrs["LES"] = CommonInstruction.FLES;
            _instrs["GRT"] = CommonInstruction.FGRT;
            _instrs["LEQ"] = CommonInstruction.FLEQ;
            _instrs["GEQ"] = CommonInstruction.FGEQ;

            _instrs["ADD"] = CommonInstruction.FADD;
            _instrs["SUB"] = CommonInstruction.FSUB;
            _instrs["MUL"] = CommonInstruction.FMUL;
            _instrs["DIV"] = CommonInstruction.FDIV;
            _instrs["MOD"] = CommonInstruction.FMOD;
            _instrs["SQR"] = CommonInstruction.FSQR;
            _instrs["NEG"] = CommonInstruction.FNEG;
            _instrs["ABS"] = CommonInstruction.FABS;

            _instrs["MVMT"] = CommonInstruction.MVMT;
            _instrs["AND"] = CommonInstruction.FAND;
            _instrs["OR"] = CommonInstruction.FOR;
            _instrs["XOR"] = CommonInstruction.FXOR;
            _instrs["NOT"] = CommonInstruction.FNOT;
            _instrs["BTDT"] = CommonInstruction.BTDT;
            _instrs["BAND"] = CommonInstruction.BAND;
            _instrs["BOR"] = CommonInstruction.BOR;
            _instrs["BXOR"] = CommonInstruction.BXOR;
            _instrs["BNOT"] = CommonInstruction.BNOT;
            _instrs["DFF"] = CommonInstruction.DFF;
            _instrs["JKFF"] = CommonInstruction.JKFF;
            _instrs["SETD"] = CommonInstruction.SETD;
            _instrs["RESD"] = CommonInstruction.RESD;

            /*
            _instrs["JSR"] = CommonInstruction.JSR;
            _instrs["RET"] = CommonInstruction.RET;
            _instrs["SBR"] = CommonInstruction.SBR;
            */

            _instrs["HMIBC"] = CommonInstruction.HMIBC;

            _instrs["SIN"] = CommonInstruction.FSIN;
            _instrs["COS"] = CommonInstruction.FCOS;
            _instrs["TAN"] = CommonInstruction.FTAN;
            _instrs["ASN"] = CommonInstruction.FASIN;
            _instrs["ACS"] = CommonInstruction.FACOS;
            _instrs["ATN"] = CommonInstruction.FATAN;

            _instrs["LN"] = CommonInstruction.FLN;
            _instrs["LOG"] = CommonInstruction.FLOG;
            _instrs["XPY"] = CommonInstruction.FXPY;

            _instrs["DEG"] = CommonInstruction.FDEG;
            _instrs["RAD"] = CommonInstruction.FRAD;
            _instrs["TOD"] = CommonInstruction.FTOD;
            _instrs["FRD"] = CommonInstruction.FFRD;
            _instrs["TRN"] = CommonInstruction.FTRN;

        }
    }
}
