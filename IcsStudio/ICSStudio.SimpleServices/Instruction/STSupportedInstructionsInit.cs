using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class STSupportedInstructions
    {
        public void Init()
        {

            _instrs["ALMD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ALMD"));
            _instrs["ALMA"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ALMA"));
            _instrs["LN"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("LN"));
            _instrs["LOGF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("LOGF"));
            _instrs["LOWER"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("LOWER"));
            _instrs["UPPER"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("UPPER"));
            _instrs["STOD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("STOD"));
            _instrs["DTOS"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("DTOS"));
            _instrs["STOR"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("STOR"));
            _instrs["RTOS"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("RTOS"));
            _instrs["FIND"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("FIND"));
            _instrs["INSERT"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("INSERT"));
            _instrs["MID"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MID"));
            _instrs["CONCAT"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("CONCAT"));
            _instrs["DELETE"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("DELETE"));
            _instrs["OSRI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("OSRI"));
            _instrs["OSFI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("OSFI"));
            _instrs["INTG"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("INTG"));
            _instrs["PI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("PI"));
            _instrs["PMUL"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("PMUL"));
            _instrs["SCRV"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SCRV"));
            _instrs["SOC"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SOC"));
            _instrs["UPDN"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("UPDN"));
            _instrs["DERV"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("DERV"));
            _instrs["HPF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("HPF"));
            _instrs["LPF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("LPF"));
            _instrs["NTCH"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("NTCH"));
            _instrs["LDL2"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("LDL2"));
            _instrs["SQRTF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SQRTF"));
            _instrs["SQRTI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SQRTI"));
            _instrs["ABSF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ABSF"));
            _instrs["ABSI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ABSI"));
            _instrs["MVMT"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MVMT"));
            _instrs["BTDT"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("BTDT"));
            _instrs["SWPBW"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SWPBW"));
            _instrs["SWPBD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SWPBD"));
            _instrs["DEGF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("DEGF"));
            _instrs["DEGI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("DEGI"));
            _instrs["RADF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("RADF"));
            _instrs["RADI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("RADI"));
            _instrs["TRUNCF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("TRUNCF"));
            _instrs["TRUNCI"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("TRUNCI"));
            _instrs["MAW"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAW"));
            _instrs["MDW"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MDW"));
            _instrs["MAR"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAR"));
            _instrs["MDR"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MDR"));
            _instrs["MAOC"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAOC"));
            _instrs["MDDOC"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MDDOC"));
            _instrs["MGS"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGS"));
            _instrs["MGSD"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGSD"));
            _instrs["MGSR"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGSR"));
            _instrs["MGSP"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MGSP"));
            _instrs["MAS"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAS"));
            _instrs["MAH"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAH"));
            _instrs["MAJ"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAJ"));
            _instrs["MAM"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAM"));
            _instrs["MAG"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAG"));
            _instrs["MCD"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MCD"));
            _instrs["MRP"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MRP"));
            _instrs["MCCP"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MCCP"));
            _instrs["MCSV"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MCSV"));
            _instrs["MAPC"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAPC"));
            _instrs["MATC"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MATC"));
            _instrs["MDAC"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MDAC"));
            _instrs["MSO"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MSO"));
            _instrs["MSF"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MSF"));
            _instrs["MASD"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MASD"));
            _instrs["MASR"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MASR"));
            _instrs["MAFR"] = new STTokenInstruction(_parentController.RTInstructionCollection.FindInstruction("MAFR"));
            _instrs["DFF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("DFF"));
            _instrs["JKFF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("JKFF"));
            _instrs["RESD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("RESD"));
            _instrs["SETD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SETD"));
            _instrs["TONR"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("TONR"));
            _instrs["TOFR"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("TOFR"));
            _instrs["RTOR"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("RTOR"));
            _instrs["CTUD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("CTUD"));
            _instrs["SINF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SINF"));
            _instrs["ATANF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ATANF"));
            _instrs["COSF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("COSF"));
            _instrs["TANF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("TANF"));
            _instrs["ASINF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ASINF"));
            _instrs["ACOSF"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ACOSF"));
            _instrs["ESEL"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("ESEL"));
            _instrs["HLL"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("HLL"));
            _instrs["MUX"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MUX"));
            _instrs["RLIM"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("RLIM"));
            _instrs["SEL"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SEL"));
            _instrs["SNEG"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SNEG"));
            _instrs["SSUM"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("SSUM"));
            _instrs["MAXC"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MAXC"));
            _instrs["MINC"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MINC"));
            _instrs["MAVE"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MAVE"));
            _instrs["MSTD"] = new STSimpleInstruction(_parentController.RTInstructionCollection.FindInstruction("MSTD"));
        }
    }
}
