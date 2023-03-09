using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class STInstructionCollection
    {
        private void Init()
        {
            _instrs["ALM"] = CommonInstruction.ALM;
            _instrs["SCL"] = CommonInstruction.SCL;
            _instrs["PID"] = CommonInstruction.PID;
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
            _instrs["ESEL"] = CommonInstruction.ESEL;
            _instrs["SSUM"] = CommonInstruction.SSUM;
            _instrs["SNEG"] = CommonInstruction.SNEG;
            _instrs["HLL"] = CommonInstruction.HLL;
            _instrs["RLIM"] = CommonInstruction.RLIM;
            _instrs["MAVE"] = CommonInstruction.MAVE;
            _instrs["MSTD"] = CommonInstruction.MSTD;
            _instrs["MINC"] = CommonInstruction.MINC;
            _instrs["MAXC"] = CommonInstruction.MAXC;
            _instrs["ALMD"] = CommonInstruction.ALMD;
            _instrs["ALMA"] = CommonInstruction.ALMA;
            _instrs["OSRI"] = CommonInstruction.OSRI;
            _instrs["OSFI"] = CommonInstruction.OSFI;
            _instrs["TONR"] = CommonInstruction.TONR;
            _instrs["TOFR"] = CommonInstruction.TOFR;
            _instrs["RTOR"] = CommonInstruction.RTOR;
            _instrs["CTUD"] = CommonInstruction.CTUD;
            _instrs["SQRT"] = CommonInstruction.SQRT;
            _instrs["ABS"] = CommonInstruction.ABS;
            _instrs["MVMT"] = CommonInstruction.MVMT;
            _instrs["SWPB"] = CommonInstruction.SWPB;
            _instrs["BTDT"] = CommonInstruction.BTDT;
            _instrs["DFF"] = CommonInstruction.DFF;
            _instrs["JKFF"] = CommonInstruction.JKFF;
            _instrs["SETD"] = CommonInstruction.SETD;
            _instrs["RESD"] = CommonInstruction.RESD;
            _instrs["HMIBC"] = CommonInstruction.HMIBC;
            _instrs["SIN"] = CommonInstruction.SIN;
            _instrs["COS"] = CommonInstruction.COS;
            _instrs["TAN"] = CommonInstruction.TAN;
            _instrs["ASIN"] = CommonInstruction.ASIN;
            _instrs["ACOS"] = CommonInstruction.ACOS;
            _instrs["ATAN"] = CommonInstruction.ATAN;
            _instrs["LN"] = CommonInstruction.LN;
            _instrs["LOG"] = CommonInstruction.LOG;
            _instrs["DEG"] = CommonInstruction.DEG;
            _instrs["RAD"] = CommonInstruction.RAD;
            _instrs["TRUNC"] = CommonInstruction.TRUNC;
            _instrs["COP"] = CommonInstruction.COP;
            _instrs["SRT"] = CommonInstruction.SRT;
            _instrs["SIZE"] = CommonInstruction.SIZE;
            _instrs["CPS"] = CommonInstruction.CPS;
            _instrs["PSC"] = CommonInstruction.PSC;
            _instrs["PFL"] = CommonInstruction.PFL;
            _instrs["PCMD"] = CommonInstruction.PCMD;
            _instrs["PCLF"] = CommonInstruction.PCLF;
            _instrs["PXRQ"] = CommonInstruction.PXRQ;
            _instrs["PPD"] = CommonInstruction.PPD;
            _instrs["PRNP"] = CommonInstruction.PRNP;
            _instrs["PATT"] = CommonInstruction.PATT;
            _instrs["PDET"] = CommonInstruction.PDET;
            _instrs["POVR"] = CommonInstruction.POVR;
            _instrs["SATT"] = CommonInstruction.SATT;
            _instrs["SDET"] = CommonInstruction.SDET;
            _instrs["SCMD"] = CommonInstruction.SCMD;
            _instrs["SCLF"] = CommonInstruction.SCLF;
            _instrs["SOVR"] = CommonInstruction.SOVR;
            _instrs["SASI"] = CommonInstruction.SASI;
            _instrs["JSR"] = CommonInstruction.JSR;
            _instrs["RET"] = CommonInstruction.RET;
            _instrs["SBR"] = CommonInstruction.SBR;
            _instrs["TND"] = CommonInstruction.TND;
            _instrs["UID"] = CommonInstruction.UID;
            _instrs["UIE"] = CommonInstruction.UIE;
            _instrs["SFR"] = CommonInstruction.SFR;
            _instrs["SFP"] = CommonInstruction.SFP;
            _instrs["EOT"] = CommonInstruction.EOT;
            _instrs["EVENT"] = CommonInstruction.EVENT;
            _instrs["MSG"] = CommonInstruction.MSG;
            _instrs["GSV"] = CommonInstruction.GSV;
            _instrs["SSV"] = CommonInstruction.SSV;
            _instrs["IOT"] = CommonInstruction.IOT;

            _instrs["MSO"] = CommonInstruction.MSO;
            _instrs["MSF"] = CommonInstruction.MSF;
            _instrs["MASD"] = CommonInstruction.MASD;
            _instrs["MASR"] = CommonInstruction.MASR;
            _instrs["MDO"] = CommonInstruction.MDO;
            _instrs["MDF"] = CommonInstruction.MDF;
            _instrs["MDS"] = CommonInstruction.MDS;
            _instrs["MAFR"] = CommonInstruction.MAFR;

            _instrs["MAS"] = CommonInstruction.MAS;
            _instrs["MAH"] = CommonInstruction.MAH;
            _instrs["MAJ"] = CommonInstruction.MAJ;
            _instrs["MAM"] = CommonInstruction.MAM;
            _instrs["MAG"] = CommonInstruction.MAG;
            _instrs["MCD"] = CommonInstruction.MCD;
            _instrs["MRP"] = CommonInstruction.MRP;
            _instrs["MCCP"] = CommonInstruction.MCCP;
            _instrs["MCSV"] = CommonInstruction.MCSV;
            _instrs["MAPC"] = CommonInstruction.MAPC;
            _instrs["MATC"] = CommonInstruction.MATC;
            _instrs["MDAC"] = CommonInstruction.MDAC;

            _instrs["MGS"] = CommonInstruction.MGS;
            _instrs["MGSD"] = CommonInstruction.MGSD;
            _instrs["MGSR"] = CommonInstruction.MGSR;
            _instrs["MGSP"] = CommonInstruction.MGSP;

            _instrs["MAW"] = CommonInstruction.MAW;
            _instrs["MDW"] = CommonInstruction.MDW;
            _instrs["MAR"] = CommonInstruction.MAR;
            _instrs["MDR"] = CommonInstruction.MDR;
            _instrs["MAOC"] = CommonInstruction.MAOC;
            _instrs["MDOC"] = CommonInstruction.MDOC;
            _instrs["MAAT"] = CommonInstruction.MAAT;
            _instrs["MRAT"] = CommonInstruction.MRAT;
            _instrs["MAHD"] = CommonInstruction.MAHD;
            _instrs["MRHD"] = CommonInstruction.MRHD;

            _instrs["MCS"] = CommonInstruction.MCS;
            _instrs["MCLM"] = CommonInstruction.MCLM;
            _instrs["MCCM"] = CommonInstruction.MCCM;
            _instrs["MCCD"] = CommonInstruction.MCCD;
            _instrs["MCT"] = CommonInstruction.MCT;
            _instrs["MCTP"] = CommonInstruction.MCTP;
            _instrs["MCSD"] = CommonInstruction.MCSD;
            _instrs["MCSR"] = CommonInstruction.MCSR;
            _instrs["MDCC"] = CommonInstruction.MDCC;

            _instrs["FIND"] = CommonInstruction.FIND;
            _instrs["INSERT"] = CommonInstruction.INSERT;
            _instrs["CONCAT"] = CommonInstruction.CONCAT;
            _instrs["MID"] = CommonInstruction.MID;
            _instrs["DELETE"] = CommonInstruction.DELETE;

            _instrs["DTOS"] = CommonInstruction.DTOS;
            _instrs["STOD"] = CommonInstruction.STOD;
            _instrs["RTOS"] = CommonInstruction.RTOS;
            _instrs["STOR"] = CommonInstruction.STOR;
            _instrs["UPPER"] = CommonInstruction.UPPER;
            _instrs["LOWER"] = CommonInstruction.LOWER;

        }
    }
}
