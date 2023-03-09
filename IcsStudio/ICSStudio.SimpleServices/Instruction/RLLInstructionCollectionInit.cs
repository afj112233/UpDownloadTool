using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class RLLInstructionCollection
    {
        private void Init()
        {

            _instrs["ALMD"] = CommonInstruction.RALMD;
            _instrs["ALMA"] = CommonInstruction.ALMA;

            _instrs["XIC"] = CommonInstruction.XIC;
            _instrs["XIO"] = CommonInstruction.XIO;
            _instrs["OTE"] = CommonInstruction.OTE;
            _instrs["OTL"] = CommonInstruction.OTL;
            _instrs["OTU"] = CommonInstruction.OTU;
            _instrs["ONS"] = CommonInstruction.ONS;
            _instrs["OSR"] = CommonInstruction.OSR;
            _instrs["OSF"] = CommonInstruction.OSF;

           
            _instrs["TON"] = CommonInstruction.TON;
            _instrs["TOF"] = CommonInstruction.TOF;
            _instrs["RTO"] = CommonInstruction.RTO;
            _instrs["CTU"] = CommonInstruction.CTU;
            _instrs["CTD"] = CommonInstruction.CTD;
            _instrs["RES"] = CommonInstruction.RES;

            _instrs["MSG"] = CommonInstruction.MSG;
            _instrs["GSV"] = CommonInstruction.GSV;
            _instrs["SSV"] = CommonInstruction.SSV;
            _instrs["IOT"] = CommonInstruction.IOT;

            _instrs["CMP"] = CommonInstruction.RCMP;
            _instrs["LIM"] = CommonInstruction.LIM;
            _instrs["MEQ"] = CommonInstruction.RMEQ;
            _instrs["EQU"] = CommonInstruction.EQU;
            _instrs["NEQ"] = CommonInstruction.RNEQ;
            _instrs["LES"] = CommonInstruction.RLES;
            _instrs["GRT"] = CommonInstruction.RGRT;
            _instrs["LEQ"] = CommonInstruction.RLEQ;
            _instrs["GEQ"] = CommonInstruction.RGEQ;
            _instrs["GRT"] = CommonInstruction.RGRT;

            _instrs["CPT"] = CommonInstruction.RCPT;
            _instrs["ADD"] = CommonInstruction.RADD;
            _instrs["SUB"] = CommonInstruction.RSUB;
            _instrs["MUL"] = CommonInstruction.RMUL;
            _instrs["DIV"] = CommonInstruction.RDIV;
            _instrs["MOD"] = CommonInstruction.RMOD;
            _instrs["SQR"] = CommonInstruction.RSQR;
            _instrs["NEG"] = CommonInstruction.RNEG;
            _instrs["ABS"] = CommonInstruction.RABS;

            _instrs["MOV"] = CommonInstruction.RMOV;
            _instrs["MVM"] = CommonInstruction.RMVM;
            _instrs["AND"] = CommonInstruction.RAND;
            _instrs["OR"] =  CommonInstruction.ROR;
            _instrs["XOR"] = CommonInstruction.RXOR;
            _instrs["NOT"] = CommonInstruction.RNOT;
            _instrs["SWPB"] = CommonInstruction.SWPB;
            _instrs["CLR"] = CommonInstruction.RCLR;
            _instrs["BTD"] = CommonInstruction.RBTD;

            _instrs["FAL"] = CommonInstruction.RFAL;
            _instrs["FSC"] = CommonInstruction.RFSC;
            _instrs["COP"] = CommonInstruction.COP;
            _instrs["FLL"] = CommonInstruction.RFLL;
            _instrs["AVE"] = CommonInstruction.RAVE;
            _instrs["SRT"] = CommonInstruction.RSRT;
            _instrs["STD"] = CommonInstruction.RSTD;
            _instrs["SIZE"] = CommonInstruction.SIZE;
            _instrs["CPS"] = CommonInstruction.CPS;

            _instrs["BSL"] = CommonInstruction.RBSL;
            _instrs["BSR"] = CommonInstruction.RBSR;
            _instrs["FFL"] = CommonInstruction.RFFL;
            _instrs["FFU"] = CommonInstruction.RFFU;
            _instrs["LFL"] = CommonInstruction.RLFL;
            _instrs["LFU"] = CommonInstruction.RLFU;

            _instrs["SQI"] = CommonInstruction.RSQI;
            _instrs["SQO"] = CommonInstruction.RSQO;
            _instrs["SQL"] = CommonInstruction.RSQL;

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

            _instrs["JMP"] = CommonInstruction.RJMP;
            _instrs["LBL"] = CommonInstruction.RLBL;
            _instrs["JSR"] = CommonInstruction.JSR;
            _instrs["JXR"] = CommonInstruction.RJXR;
            _instrs["RET"] = CommonInstruction.RET;
            _instrs["SBR"] = CommonInstruction.SBR;
            _instrs["TND"] = CommonInstruction.TND;
            _instrs["MCR"] = CommonInstruction.RMCR;
            _instrs["UID"] = CommonInstruction.UID;
            _instrs["UIE"] = CommonInstruction.UIE;
            _instrs["SFR"] = CommonInstruction.SFR;
            _instrs["SFP"] = CommonInstruction.SFP;
            _instrs["EVENT"] = CommonInstruction.EVENT;
            _instrs["EOT"] = CommonInstruction.EOT;
            _instrs["AFI"] = CommonInstruction.AFI;
            _instrs["NOP"] = CommonInstruction.RNOP;

            _instrs["FOR"] = CommonInstruction.RFOR;
            _instrs["BRK"] = CommonInstruction.RBRK;

            _instrs["FBC"] = CommonInstruction.RFBC;
            _instrs["DDT"] = CommonInstruction.RDDT;
            _instrs["DTR"] = CommonInstruction.RDTR;
            _instrs["PID"] = CommonInstruction.PID;

            _instrs["HMIBC"] = CommonInstruction.HMIBC;

            _instrs["SIN"] = CommonInstruction.RSIN;
            _instrs["COS"] = CommonInstruction.RCOS;
            _instrs["TAN"] = CommonInstruction.RTAN;
            _instrs["ASN"] = CommonInstruction.RASIN;
            _instrs["ACS"] = CommonInstruction.RACOS;
            _instrs["ATN"] = CommonInstruction.RATAN;

            _instrs["LN"] = CommonInstruction.RLN;
            _instrs["LOG"] = CommonInstruction.RLOG;
            _instrs["XPY"] = CommonInstruction.RXPY;

            _instrs["DEG"] = CommonInstruction.RDEG;
            _instrs["RAD"] = CommonInstruction.RRAD;
            _instrs["TOD"] = CommonInstruction.RTOD;
            _instrs["FRD"] = CommonInstruction.RFRD;
            _instrs["TRN"] = CommonInstruction.RTRN;

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
            _instrs["CONCAT"] = CommonInstruction.RCONCAT;
            _instrs["MID"] = CommonInstruction.MID;
            _instrs["DELETE"] = CommonInstruction.DELETE;

            _instrs["DTOS"] = CommonInstruction.DTOS;
            _instrs["STOD"] = CommonInstruction.STOD;
            _instrs["RTOS"] = CommonInstruction.RTOS;
            _instrs["STOR"] = CommonInstruction.STOR;
            _instrs["UPPER"] = CommonInstruction.UPPER;
            _instrs["LOWER"] = CommonInstruction.LOWER;


            /*
            _instrs["EQU"] = (CommonInstruction.EQU);
            _instrs["OTU"] = CommonInstruction.OTU;

            _instrs["MSO"] = (CommonInstruction.MSO);
            _instrs["MSF"] = (CommonInstruction.MSF);
            _instrs["MASD"] = CommonInstruction.MASD;
            _instrs["MASR"] = CommonInstruction.MASR;
            _instrs["MAFR"] = CommonInstruction.MAFR;
            _instrs["MAS"] = CommonInstruction.MAS;

            _instrs["MAM"] = CommonInstruction.MAM;
            _instrs["MAJ"] = CommonInstruction.MAJ;
            _instrs["MATC"] = CommonInstruction.MATC;
            */

            /*
            _instrs["XIO"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("XIO"));
            _instrs["ONS"] = new RLLONSInstruction(_parentController.RTInstructionCollection.FindInstruction("ONS"));
            //_instrs["OSF"] = new RLLOSFInstruction(_parentController.RTInstructionCollection.FindInstruction("OSF"));
            //_instrs["OSR"] = new RLLOSRInstruction(_parentController.RTInstructionCollection.FindInstruction("OSR"));
            //_instrs["OTE"] = new RLLOTEInstruction(_parentController.RTInstructionCollection.FindInstruction("OTE"));
            //_instrs["OTL"] = new RLLOTLInstruction(_parentController.RTInstructionCollection.FindInstruction("OTL"));
            //_instrs["OTU"] = new RLLOTUInstruction(_parentController.RTInstructionCollection.FindInstruction("OTU"));
            _instrs["MAW"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAW"));
            _instrs["MDW"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MDW"));
            _instrs["MAR"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAR"));
            _instrs["MDR"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MDR"));
            _instrs["MAOC"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAOC"));
            _instrs["MGS"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGS"));
            _instrs["MGSD"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGSD"));
            _instrs["MGSR"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGSR"));
            _instrs["MGSP"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGSP"));
            _instrs["MAS"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAS"));
            _instrs["MAH"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAH"));
            _instrs["MAJ"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAJ"));
            _instrs["MAM"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAM"));
            _instrs["MAG"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAG"));
            _instrs["MCD"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MCD"));
            _instrs["MRP"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MRP"));
            _instrs["MCCP"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MCCP"));
            _instrs["MCSV"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MCSV"));
            _instrs["MAPC"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAPC"));
            _instrs["MATC"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MATC"));
            _instrs["MDAC"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MDAC"));
            _instrs["MSO"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MSO"));
            _instrs["MSF"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MSF"));
            _instrs["MASD"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MASD"));
            _instrs["MASR"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MASR"));
            _instrs["MAFR"] = new RLLTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAFR"));
        */
        }
    }
}
