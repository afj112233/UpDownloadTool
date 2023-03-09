using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    partial class SnippetLexer
    {
        private void InstrAst(ASTInstr instrStmt, SnippetInfo snippetInfo, bool isNotInAssign, ref bool isAccepted)
        {

            if (!string.IsNullOrEmpty(instrStmt.Error))
            {
                DrawFile(instrStmt, snippetInfo.Parent, instrStmt.Error);
                snippetInfo.IsCurrent = false;
            }

            var aoi = (_controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(instrStmt.name);
            var instr = aoi?.instr ??
                        ((Controller)_controller).STInstructionCollection.FindInstruction(instrStmt.name);

            if (instr == null)
            {
                DrawFile(instrStmt, snippetInfo.Parent,
                    "Unknown instruction or function.Instruction is not known or not defined.");
                snippetInfo.IsCurrent = false;
                return;
            }

            if (_routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                if ("jsr".Equals(instr.Name, StringComparison.OrdinalIgnoreCase))
                {
                    DrawFile(instrStmt, snippetInfo.Parent,
                        $"{instr.Name}:Invalid instruction reference.Instruction not supported in Add-On Instruction or by controller's major revision.");
                    snippetInfo.IsCurrent = false;
                }
            }

            ASTNodeList param = instrStmt.param_list;
            try
            {
                param = instr.ParseSTParameters(instrStmt.param_list);
            }
            catch (Exception e)
            {
                DrawFile(instrStmt, snippetInfo.Parent, e.Message);
                snippetInfo.IsCurrent = false;
            }

            ASTNodeList aoiParamList = new ASTNodeList();
            aoiParamList.AddNodes(instrStmt.param_list);
            if (!isAccepted)
            {
                isAccepted = true;
                try
                {
                    instrStmt.Accept(_typeChecker);
                }
                catch (Exception e)
                {
                    if (!e.Message.EndsWith("is undefined."))
                    {
                        snippetInfo.ErrorInfo = e.Message;
                        snippetInfo.IsCurrent = false;
                        if (!instrStmt.param_list.nodes.Exists(n => n is ASTError))
                        {
                            DrawFile(instrStmt, snippetInfo.Parent, e.Message);
                        }
                    }

                    isAccepted = false;
                }
            }

            //检查一些特别的指令
            if (instr.Name == "ABS" || instr.Name == "LN" || instr.Name == "ACOS" || instr.Name == "ASIN" ||
                instr.Name == "ATAN" || instr.Name == "COS" || instr.Name == "SIN" || instr.Name == "TAN" ||
                instr.Name == "LOG" || instr.Name == "DEG" || instr.Name == "RAD" || instr.Name == "TRUNC" ||
                instr.Name == "SQRT")
            {
                if (isNotInAssign)
                {
                    DrawFile(instrStmt, snippetInfo.Parent, $"';':Unexpected.");
                }
            }
            else
            {
                if (!isNotInAssign)
                {
                    DrawFile(instrStmt, snippetInfo.Parent, $"Instruction not allowed in expression.");
                }
            }

            //if (snippetInfo.IsCurrent)  
            {
                try
                {

                    #region special instr

                    if (instrStmt.name.Equals("jsr", StringComparison.OrdinalIgnoreCase))
                    {
                        if (instrStmt.param_list.nodes.Count == 1)
                        {
                            var astName = instrStmt.param_list.nodes[0] as ASTName;
                            if (astName != null)
                            {
                                var routineName = ObtainValue.GetAstName(astName);
                                var routine = _parentProgram.Routines[routineName];
                                if (routine != null)
                                {
                                    if (routine.IsMainRoutine)
                                    {
                                        DrawFile(instrStmt, snippetInfo.Parent, "Jumping to Main Routine not allowed.");
                                        snippetInfo.IsCurrent = false;
                                        goto End;

                                    }
                                    else
                                    {
                                        if (!_onlyTextMarker)
                                        {
                                            var routineVar = new VariableInfo(astName, routine.Name, _routine,
                                                    astName.ContextStart, snippetInfo.Parent,
                                                    _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                        astName.ContextStart))
                                            { IsRoutine = true, IsUseForJSR = true,IsDisplay = false};
                                            snippetInfo.AddVariable(routineVar);
                                        }

                                        goto End;
                                    }
                                }
                                else
                                {
                                    var errorRoutine = new VariableInfo(astName, routineName, _routine,
                                            astName.ContextStart, snippetInfo.Parent,
                                            _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                astName.ContextStart))
                                        { IsRoutine = true, IsUseForJSR = true ,IsUnknown = true, IsDisplay = false };
                                    snippetInfo.AddVariable(errorRoutine);
                                }
                            }
                        }

                        DrawFile(instrStmt, snippetInfo.Parent, "Error arguments for 'JSR'");
                        snippetInfo.IsCurrent = false;
                        return;
                    }

                    if (instrStmt.name.Equals("event", StringComparison.OrdinalIgnoreCase))
                    {
                        if (instrStmt.param_list.nodes.Count == 1)
                        {
                            var astName = instrStmt.param_list.nodes[0] as ASTName;
                            if (astName != null)
                            {
                                var task = _controller.Tasks[ObtainValue.GetAstName(astName)];
                                if (task != null)
                                {
                                    if (!_onlyTextMarker)
                                    {
                                        var routineVar = new VariableInfo(astName, task.Name, _routine,
                                                astName.ContextStart, snippetInfo.Parent,
                                                _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                    astName.ContextStart))
                                        { IsRoutine = true };
                                        snippetInfo.AddVariable(routineVar);
                                    }

                                    goto End;
                                }
                            }
                        }

                        DrawFile(instrStmt, snippetInfo.Parent, "Error arguments for 'EVENT'");
                        snippetInfo.IsCurrent = false;
                        goto End;
                    }

                    #endregion

                    bool isGSVorSSV = "SSV".Equals(instr.Name, StringComparison.OrdinalIgnoreCase) ||
                                      "GSV".Equals(instr.Name, StringComparison.OrdinalIgnoreCase);
                    var paramsList = instr.GetParameterInfo();
                    var realCount = param.nodes.Count;
                    if ((!isGSVorSSV && realCount == paramsList.Count) || (isGSVorSSV && realCount == 4))
                    {

                    }
                    else
                    {
                        snippetInfo.IsCurrent = false;
                        if (paramsList.Count < realCount)
                        {
                            DrawFile(instrStmt, snippetInfo.Parent,
                                $"'{instrStmt.name}':Too many arguments found for instruction.");
                        }
                        else
                        {
                            DrawFile(instrStmt, snippetInfo.Parent,
                                $"'{instrStmt.name}':More arguments are expected for instruction.");
                        }
                    }

                    #region Parse

                    var aoiInstr = instr as AoiDefinition.AOIInstruction;
                    foreach (var node1 in param.nodes)
                    {
                        ASTNode node = node1;
                        {
                            if (isGSVorSSV && param.nodes.IndexOf(node1) == 1)
                            {
                                if (instrStmt.name.ToLower() == "gsv")
                                {
                                    var className = ((param.nodes[0] as ASTInteger)?.value) ?? null;
                                    if(className!=null)
                                    {
                                        var classEnum = (InstrEnum.ClassName) (byte) className;
                                        if (classEnum == InstrEnum.ClassName.Controller ||
                                            classEnum == InstrEnum.ClassName.ControllerDevice ||
                                            classEnum == InstrEnum.ClassName.CST ||
                                            classEnum == InstrEnum.ClassName.FaultLog ||
                                            classEnum == InstrEnum.ClassName.TimeSynchronize ||
                                            classEnum == InstrEnum.ClassName.WallClockTime)
                                        {
                                            if (!(node is ASTEmpty))
                                            {
                                                DrawFile(instrStmt, snippetInfo.CodeText,
                                                    "GSV,Parameter 2:Argument must be empty.");
                                                continue;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var className = ((param.nodes[0] as ASTInteger)?.value) ?? null;
                                    if (className != null)
                                    {
                                        var classEnum = (InstrEnum.SSVClassName)(byte)className;
                                        if (classEnum == InstrEnum.SSVClassName.Controller ||
                                            classEnum == InstrEnum.SSVClassName.FaultLog ||
                                            classEnum == InstrEnum.SSVClassName.TimeSynchronize ||
                                            classEnum == InstrEnum.SSVClassName.WallClockTime)
                                        {
                                            if (!(node is ASTEmpty))
                                            {
                                                DrawFile(instrStmt, snippetInfo.CodeText,
                                                    "GSV,Parameter 2:Argument must be empty.");
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var astTypeConv = node as ASTTypeConv;
                        if (astTypeConv != null)
                        {
                            node = astTypeConv.expr;
                        }

                        var astError = node as ASTError;
                        if (astError != null)
                        {
                            if (astError.IsEnum)
                            {
                                var enums = new List<string>();
                                foreach (var x in Enum.GetValues(astError.EnumType))
                                {
                                    var attribute =
                                        Attribute.GetCustomAttribute(x.GetType().GetField(x.ToString()),
                                            typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                                    enums.Add(attribute?.Value ?? x.ToString());
                                }
                                var errorEnum=new VariableInfo(astError,ObtainValue.GetAstName(astError.ErrorObject as ASTName),_routine, snippetInfo.Offset + astError.ContextStart, snippetInfo.Parent,
                                    _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                        snippetInfo.Offset + astError.ContextStart))
                                {
                                    IsUnknown = true,Enums = enums
                                };
                                snippetInfo.AddVariable(errorEnum);
                            }
                            DrawFile(astError, snippetInfo.Parent, astError.Error, Colors.Red);
                            continue;
                        }

                        var astNameValue = node as ASTNameValue;
                        var astName = node as ASTName;
                        if (astNameValue != null)
                        {
                            astName = astNameValue.name;
                        }

                        if (astName != null)
                        {
                            var index = param.nodes.IndexOf(node1);
                            CheckConstantInAoiInstr(aoiInstr, astName, index, snippetInfo.Parent);
                            if (!isGSVorSSV)
                            {
                                if (index < paramsList.Count)
                                    astName.ExpectDataType = paramsList[index].Item2;
                                else
                                {
                                    var subAccepted = isAccepted;
                                    ConvertNodeToVariable(astName, snippetInfo, isNotInAssign, ref subAccepted,null);
                                    DrawFile(instrStmt, snippetInfo.Parent,
                                        "Too many arguments found for instruction.");
                                    continue;
                                }
                            }

                            var name = ObtainValue.GetAstName(astName);
                            if (isGSVorSSV && index == 1)
                            {
                                if (instrStmt.name.Equals("gsv", StringComparison.OrdinalIgnoreCase))
                                {
                                    var className = ((param.nodes[0] as ASTInteger)?.value) ?? null;
                                    if (className != null)
                                    {
                                        var classEnum = (InstrEnum.ClassName)(byte)className;
                                        switch (classEnum)
                                        {
                                            case InstrEnum.ClassName.AddOnInstructionDefinition:
                                                if (((AoiDefinitionCollection)_controller.AOIDefinitionCollection)
                                                    .Find(name) == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Add-On Instruction could not be found.");
                                                    continue;
                                                }

                                                if (!_onlyTextMarker)
                                                {
                                                    var aoiVar = new VariableInfo(astName, name, _routine,
                                                            astName.ContextStart, snippetInfo.Parent,
                                                            _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                                astName.ContextStart))
                                                    { IsDisplay = false, IsAOI = true };
                                                    snippetInfo.AddVariable(aoiVar);
                                                }

                                                continue;
                                            case InstrEnum.ClassName.Axis:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(AXIS_CIP_DRIVE.Inst, AXIS_VIRTUAL.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                var instance = param.nodes.Count > 3
                                                    ? ((param.nodes[2] as ASTInteger)?.value) ?? null
                                                    : null;
                                                if (instance != null)
                                                {
                                                    var tag = ObtainValue.NameToTag(name, TransformTable,
                                                            _parentProgram)
                                                        ?.Item1 as Tag;
                                                    if (!IsSupportInSetAxis(tag, (int)instance, true))
                                                    {
                                                        DrawFile(instrStmt, snippetInfo.Parent,
                                                            $"GSV,Parameter 2,\'{name}\':Invalid reference to unknown attribute");
                                                        continue;
                                                    }
                                                }

                                                break;
                                            case InstrEnum.ClassName.Controller:
                                                break;
                                            case InstrEnum.ClassName.ControllerDevice:
                                                break;
                                            case InstrEnum.ClassName.CoordinateSystem:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(COORDINATE_SYSTEM.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                break;
                                            case InstrEnum.ClassName.CST:
                                                break;
                                            case InstrEnum.ClassName.DataLog:
                                                //TODO(ZYL):Datalog
                                                continue;
                                            case InstrEnum.ClassName.DF1:
                                                break;
                                            case InstrEnum.ClassName.FaultLog:
                                                break;
                                            case InstrEnum.ClassName.Message:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(MESSAGE.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                break;
                                            case InstrEnum.ClassName.Module:
                                                if (_controller.DeviceModules[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Module could not be found.");
                                                    continue;
                                                }

                                                var moduleVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsModule = true, IsDisplay = false };
                                                snippetInfo.AddVariable(moduleVar);
                                                continue;
                                            case InstrEnum.ClassName.MotionGroup:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(MOTION_GROUP.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                break;
                                            case InstrEnum.ClassName.Program:
                                                if (_controller.Programs[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"'{name}':Program could not be found.");
                                                    continue;
                                                }

                                                var programVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsProgram = true, IsDisplay = false };
                                                snippetInfo.AddVariable(programVar);
                                                continue;
                                            case InstrEnum.ClassName.Routine:
                                                if (_parentProgram.Routines[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"'{name}':Routine could not be found.");
                                                    continue;
                                                }

                                                var routineVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsRoutine = true, IsDisplay = false };
                                                snippetInfo.AddVariable(routineVar);
                                                continue;
                                            case InstrEnum.ClassName.SerialPort:
                                                break;
                                            case InstrEnum.ClassName.Task:
                                                if (_parentProgram.ParentController.Tasks[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Task not found.");
                                                    continue;
                                                }

                                                var taskVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsTask = true, IsDisplay = false };
                                                snippetInfo.AddVariable(taskVar);
                                                continue;
                                            case InstrEnum.ClassName.TimeSynchronize:
                                                break;
                                            case InstrEnum.ClassName.WallClockTime:
                                                break;
                                        }

                                        var subAccepted = isAccepted;
                                        ConvertNodeToVariable(astName, snippetInfo, isNotInAssign, ref subAccepted);
                                    }
                                }

                                if (instrStmt.name.Equals("ssv", StringComparison.OrdinalIgnoreCase))
                                {
                                    var className = ((param.nodes[0] as ASTInteger)?.value) ?? null;
                                    if (className != null)
                                    {
                                        var classEnum = (InstrEnum.SSVClassName)(int)className;
                                        switch (classEnum)
                                        {
                                            case InstrEnum.SSVClassName.Axis:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(AXIS_CIP_DRIVE.Inst, AXIS_VIRTUAL.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                var instance = param.nodes.Count > 3
                                                    ? ((param.nodes[2] as ASTInteger)?.value) ?? null
                                                    : null;
                                                if (instance != null)
                                                {
                                                    var tag = ObtainValue.NameToTag(name, TransformTable,
                                                            _parentProgram)
                                                        ?.Item1 as Tag;
                                                    if (!IsSupportInSetAxis(tag, (int)instance, false))
                                                    {
                                                        DrawFile(instrStmt, snippetInfo.Parent,
                                                            $"SSV,Parameter 2,\'{name}\':Invalid reference to unknown attribute");
                                                        continue;
                                                    }
                                                }

                                                break;
                                            case InstrEnum.SSVClassName.Controller:
                                                break;
                                            case InstrEnum.SSVClassName.CoordinateSystem:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(COORDINATE_SYSTEM.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                break;
                                            case InstrEnum.SSVClassName.FaultLog:
                                                break;
                                            case InstrEnum.SSVClassName.Message:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(MESSAGE.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                break;
                                            case InstrEnum.SSVClassName.Module:
                                                if (_controller.DeviceModules[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Module could not be found.");
                                                    continue;
                                                }

                                                var moduleVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsModule = true, IsDisplay = false };
                                                snippetInfo.AddVariable(moduleVar);
                                                continue;
                                            case InstrEnum.SSVClassName.MotionGroup:
                                                if (!VerifyInstrParam(name,
                                                    new ExpectType(MOTION_GROUP.Inst)))
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Invalid data type.");
                                                    continue;
                                                }

                                                break;
                                            case InstrEnum.SSVClassName.Program:
                                                if (_controller.Programs[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"'{name}':Program could not be found.");
                                                    continue;
                                                }

                                                var programVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsProgram = true, IsDisplay = false };
                                                snippetInfo.AddVariable(programVar);
                                                continue;
                                            case InstrEnum.SSVClassName.Routine:
                                                if (_parentProgram.Routines[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"'{name}':Routine could not be found.");
                                                    continue;
                                                }

                                                var routineVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsRoutine = true, IsDisplay = false };
                                                snippetInfo.AddVariable(routineVar);
                                                continue;
                                            case InstrEnum.SSVClassName.Task:
                                                if (_parentProgram.ParentController.Tasks[name] == null)
                                                {
                                                    DrawFile(astName, snippetInfo.Parent,
                                                        $"{name}:Task not found.");
                                                    continue;
                                                }

                                                var taskVar = new VariableInfo(astName, name, _routine,
                                                        astName.ContextStart, snippetInfo.Parent,
                                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                                            astName.ContextStart))
                                                { IsTask = true, IsDisplay = false };
                                                snippetInfo.AddVariable(taskVar);
                                                continue;
                                            case InstrEnum.SSVClassName.TimeSynchronize:
                                                break;
                                            case InstrEnum.SSVClassName.WallClockTime:
                                                break;
                                        }

                                        var subAccepted = isAccepted;
                                        ConvertNodeToVariable(astName, snippetInfo, isNotInAssign, ref subAccepted);
                                    }
                                }

                                continue;
                            }

                            var subAccepted1 = isAccepted;
                            ConvertNodeToVariable(astName, snippetInfo, isNotInAssign, ref subAccepted1);
                            if (isGSVorSSV && index == 3&&(!astName.IsMarked))
                            {
                                var @class = param.nodes[0] as ASTInteger;
                                var attribute = param.nodes[2] as ASTInteger;
                                if (@class != null && attribute != null)
                                {
                                    if (instrStmt.name.Equals("gsv", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (!GsvDestParamCheck(
                                            (InstrEnum.ClassName) (int) @class.value,
                                            (int) attribute.value, astName?.Expr?.type))
                                        {
                                            DrawFile(instrStmt, snippetInfo.Parent,
                                                $"GSV, Parameter 4,'{name}':Invalid data type.Argument must match parameter data type.");
                                        }
                                    }
                                    else
                                    {
                                        if (!SsvDestParamCheck(
                                            (InstrEnum.SSVClassName) (int) @class.value, (int) attribute.value,
                                            astName?.Expr?.type))
                                        {
                                            DrawFile(instrStmt, snippetInfo.Parent,
                                                $"SSV, Parameter 4,'{name}':Invalid data type.Argument must match parameter data type.");
                                        }
                                    }

                                    if ("ssv".Equals(instr.Name,StringComparison.OrdinalIgnoreCase) &&(InstrEnum.ClassName) (int) @class.value == InstrEnum.ClassName.WallClockTime &&
                                        ((int) InstrEnum.WallClockTime.DateTime == (int) attribute.value ||
                                         (int) InstrEnum.WallClockTime.LocalDateTime == (int) attribute.value))
                                    {
                                        if (astName.Tag != null)
                                        {
                                            if (!CheckDatetimeInSSV(astName))
                                            {
                                                DrawFile(astName, snippetInfo.Parent,
                                                    $"SSV, Parameter 4,'{name}':Year limit to 0-2037.");
                                            }
                                        }
                                    }
                                }

                            }

                            continue;
                        }

                        var paramIndex = param.nodes.IndexOf(node1);

                        var targetParam = paramsList.Count > paramIndex ? paramsList[paramIndex] : null;

                        var astInstr = node as ASTInstr;
                        if (astInstr != null)
                        {
                            var subAccepted = isAccepted;
                            InstrAst(astInstr, snippetInfo, false, ref subAccepted);
                            continue;
                        }

                        var astInt = node as ASTInteger;
                        if (astInt != null)
                        {
                            if (!isGSVorSSV)
                            {
                                if (!DataTypeExtend.IsMatched(DINT.Inst, targetParam?.Item2))
                                {
                                    if ((targetParam.Item2 as ExpectType)?.ExpectTypes.Any(inf => inf is ZeroType) ??
                                        false)
                                    {
                                        var integer = node as ASTInteger;
                                        if (integer?.value == 0)
                                        {
                                            continue;
                                        }
                                    }

                                    if (targetParam.Item2.IsBool)
                                    {
                                        if (astInt.value == 0 || astInt.value == 1)
                                        {
                                            continue;
                                        }
                                    }

                                    if (targetParam.Item2 is ZeroType)
                                    {
                                        var integer = node as ASTInteger;
                                        if (integer?.value == 0)
                                        {
                                            continue;
                                        }
                                    }

                                    DrawFile(instrStmt, snippetInfo.Parent,
                                        $"Parameter {paramIndex + 1}:Invalid data type.Argument must match parameter data type.");
                                }
                            }

                            var index = param.nodes.IndexOf(node1);
                            var interrelated = "";
                            if (isGSVorSSV)
                            {
                                if (instrStmt.name.Equals("gsv", StringComparison.OrdinalIgnoreCase))
                                {
                                    var className = ((param.nodes[0] as ASTInteger)?.value) ?? null;
                                    if (className != null)
                                    {
                                        interrelated = ((InstrEnum.ClassName)(int)className).ToString();
                                    }
                                }

                                if (instrStmt.name.Equals("ssv", StringComparison.OrdinalIgnoreCase))
                                {
                                    var className = ((param.nodes[0] as ASTInteger)?.value) ?? null;
                                    if (className != null)
                                    {
                                        interrelated = ((InstrEnum.SSVClassName)(int)className).ToString();
                                    }
                                }
                            }

                            if (astInt.IsEnum)
                            {
                                var enumName = Enum.ToObject(astInt.EnumType, (int)astInt.value);
                                var attr = Attribute.GetCustomAttribute(
                                    enumName.GetType().GetField(enumName.ToString()),
                                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                                var name = attr == null ? enumName.ToString() : attr.Value.Replace(" ", "");


                                var enums = GetInstrEnum(instrStmt.name, index, interrelated);
                                var enumInfo = new VariableInfo(astInt, name, _routine, astInt.ContextStart,
                                        snippetInfo.Parent,
                                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(astInt.ContextStart))
                                { IsDisplay = false };
                                enumInfo.Enums = enums;
                                snippetInfo.AddVariable(enumInfo);
                                continue;
                            }
                            else
                            {
                                var enumType = GetInstrEnumInfo(instrStmt.name, index, interrelated)?.Item2;
                                if (enumType != null)
                                {
                                    if (!Enum.IsDefined(enumType, (int)astInt.value))
                                    {
                                        DrawFile(instrStmt, snippetInfo.Parent,
                                            $"Parameter {index + 1}:Immediate value out of range.");
                                    }
                                }

                                var subAccepted = isAccepted;
                                ConvertNodeToVariable(astInt, snippetInfo, isNotInAssign, ref subAccepted);
                                continue;
                            }
                        }

                        var astFloat = node as ASTFloat;
                        if (astFloat != null)
                        {
                            if (!isGSVorSSV)
                            {
                                if (!DataTypeExtend.IsMatched(REAL.Inst, targetParam?.Item2))
                                {
                                    DrawFile(instrStmt, snippetInfo.Parent,
                                        $"Parameter {paramIndex + 1}:Invalid data type.Argument must match parameter data type.");
                                }
                            }

                            var subAccepted = isAccepted;
                            ConvertNodeToVariable(astFloat, snippetInfo, isNotInAssign, ref subAccepted);
                            continue;
                        }

                        var astBinOp = node as ASTBinOp;
                        if (astBinOp != null)
                        {
                            if (astBinOp.type == null|| !isAccepted)
                            {
                                astBinOp.Accept(_typeChecker);
                            }

                            if (!DataTypeExtend.IsMatched(astBinOp.type,
                                paramsList[param.nodes.IndexOf(node1)].Item2, aoiInstr != null))
                            {
                                DrawFile(astBinOp, snippetInfo.Parent, "Invalid expression.");
                            }

                            var subAccepted = isAccepted;
                            ConvertNodeToVariable(astBinOp, snippetInfo, isNotInAssign, ref subAccepted);
                            continue;
                        }

                        var astCall = node as ASTCall;
                        if (astCall != null)
                        {
                            var instrTmp = new ASTInstr(astCall.name, astCall.param_list)
                            {
                                ContextStart = astCall.ContextStart,
                                ContextEnd = astCall.ContextEnd,
                                Error = astCall.Error
                            };
                            var subAccepted = isAccepted;
                            InstrAst(instrTmp, snippetInfo, isNotInAssign, ref subAccepted);
                            continue;
                        }

                        var astUnaryOp = node as ASTUnaryOp;
                        if (astUnaryOp != null)
                        {
                            var subAccepted = isAccepted;
                            ConvertNodeToVariable(astUnaryOp, snippetInfo, isNotInAssign, ref subAccepted);
                            continue;
                        }

                        var astEmpty = node as ASTEmpty;
                        if (astEmpty != null)
                        {
                            continue;
                        }

                        var astNameAttr = node as ASTNameAddr;
                        if (astNameAttr != null)
                        {
                            var subAccepted = isAccepted;
                            ConvertNodeToVariable(astNameAttr, snippetInfo, isNotInAssign, ref subAccepted);
                            continue;
                        }

                        Debug.Assert(false, $"parse {node.GetType().FullName}");
                        //DrawFile(node,snippetInfo.Parent,$"Unexpected");
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    DrawFile(instrStmt, snippetInfo.Parent, e.Message);
                    return;
                }
            }

            //if (snippetInfo.IsCurrent)
            End:
            {
                if (_onlyTextMarker) return;
                var instrVariable = new VariableInfo(instrStmt, instr.Name, _routine, instrStmt.ContextStart,
                    snippetInfo.Parent,
                    _textMarkerService.TextDocument.GetLocationNotVerifyAccess(instrStmt.ContextStart), true)
                {
                    Parameters = instrStmt.param_list,
                    IsDisplay = !snippetInfo.HiddenVariable,
                    IsAOI = instr is AoiDefinition.AOIInstruction,
                    IsJSR = "jsr".Equals(instr.Name, StringComparison.OrdinalIgnoreCase)
                };
                if (!snippetInfo.IsCurrent)
                {
                    instrVariable.IsDisplay = false;
                }

                if (snippetInfo.IsCurrent && !_isInitial && aoi != null)
                {
                    instrVariable.IsAOI = true;
                    if (!ObtainValue.HasVariableInDim(instrStmt.param_list.nodes[0]))
                    {
                        var aoiDataReference =
                            new AoiDataReference(aoi, _routine,
                                new ASTInstr(instrStmt.name, aoiParamList)
                                { ContextStart = instrStmt.ContextStart, ContextEnd = instrStmt.ContextEnd },_routine.CurrentOnlineEditType);
                        if (_routine.ParentCollection.ParentProgram is AoiDefinition)
                        {
                            aoiDataReference.InnerAoiDefinition =
                                _routine.ParentCollection.ParentProgram as AoiDefinition;
                        }

                        SetLocation(aoiDataReference, aoi, (_routine.ParentCollection.ParentProgram is AoiDefinition));
                    }
                }

                snippetInfo.AddVariable(instrVariable);
            }
        }

        //d3限制2038以内
        private bool CheckDatetimeInSSV(ASTName astName)
        {
            Tag tag = null;
            var value = ObtainValue.GetTagValue(ObtainValue.GetAstName(astName), _parentProgram, TransformTable,
                ref tag,false,true);
            if (value != null)
            {
                int year ;
                var result = int.TryParse(value, out year);
                if (result)
                {
                    return year >= 0 && year < 2038;
                }
            }
            return false;
        }

        // ReSharper disable once InconsistentNaming
        private bool IsSupportInSetAxis(Tag axis, int instance, bool isGSV)
        {
            if (axis.DataWrapper is AxisVirtual)
            {
                if (isGSV)
                {
                    var gsvInstance = (InstrEnum.Axis)instance;
                    switch (gsvInstance)
                    {
                        case InstrEnum.Axis.TorqueOffset:
                        case InstrEnum.Axis.TorqueTrim:
                        case InstrEnum.Axis.TorqueLimitPositive:
                        case InstrEnum.Axis.TorqueLimitNegative:
                        case InstrEnum.Axis.PositionTrim:
                        case InstrEnum.Axis.VelocityTrim:
                        case InstrEnum.Axis.InhibitAxis:
                        case InstrEnum.Axis.MotionPolarity:
                        case InstrEnum.Axis.HomeReturnSpeed:
                        case InstrEnum.Axis.PositionLoopBandwidth:
                        case InstrEnum.Axis.PositionIntegratorBandwidth:
                        case InstrEnum.Axis.PositionErrorTolerance:
                        case InstrEnum.Axis.VelocityLoopBandwidth:
                        case InstrEnum.Axis.VelocityIntegratorBandwidth:
                        case InstrEnum.Axis.VelocityErrorTolerance:
                        case InstrEnum.Axis.MotorOverspeedUserLimit:
                        case InstrEnum.Axis.RotaryMotorMaxSpeed:
                        case InstrEnum.Axis.PositionScalingNumerator:
                        case InstrEnum.Axis.PositionScalingDenominator:
                        case InstrEnum.Axis.FeedbackMode:
                            return false;
                        default:
                            return true;
                    }
                }
                else
                {
                    var gsvInstance = (InstrEnum.SSVAxis)instance;
                    switch (gsvInstance)
                    {
                        case InstrEnum.SSVAxis.TorqueOffset:
                        case InstrEnum.SSVAxis.TorqueTrim:
                        case InstrEnum.SSVAxis.TorqueLimitPositive:
                        case InstrEnum.SSVAxis.TorqueLimitNegative:
                        case InstrEnum.SSVAxis.PositionTrim:
                        case InstrEnum.SSVAxis.VelocityTrim:
                        case InstrEnum.SSVAxis.InhibitAxis:
                        case InstrEnum.SSVAxis.MotionPolarity:
                        case InstrEnum.SSVAxis.HomeReturnSpeed:
                        case InstrEnum.SSVAxis.PositionLoopBandwidth:
                        case InstrEnum.SSVAxis.PositionIntegratorBandwidth:
                        case InstrEnum.SSVAxis.PositionErrorTolerance:
                        case InstrEnum.SSVAxis.VelocityLoopBandwidth:
                        case InstrEnum.SSVAxis.VelocityIntegratorBandwidth:
                        case InstrEnum.SSVAxis.VelocityErrorTolerance:
                        case InstrEnum.SSVAxis.MotorOverspeedUserLimit:
                        case InstrEnum.SSVAxis.RotaryMotorMaxSpeed:
                        case InstrEnum.SSVAxis.PositionScalingNumerator:
                        case InstrEnum.SSVAxis.PositionScalingDenominator:
                        case InstrEnum.SSVAxis.FeedbackMode:
                            return false;
                        default:
                            return true;
                    }
                }
            }

            return true;
        }

        private bool GsvDestParamCheck(InstrEnum.ClassName className, int attr, IDataType destTypeInfo)
        {
            if (destTypeInfo == null) return false;
            if ("Axis".Equals(className.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                switch ((InstrEnum.Axis)attr)
                {
                    case InstrEnum.Axis.WatchPosition:
                        if (!destTypeInfo.IsReal) return false;
                        break;
                }
            }

            return GsvOrSsvCommonCheck(className.ToString(), attr, destTypeInfo);
        }

        private bool GsvOrSsvCommonCheck(string className, int attr, IDataType destTypeInfo)
        {
            if (destTypeInfo == null) return false;
            if ("Module".Equals(className, StringComparison.OrdinalIgnoreCase))
            {
                if (destTypeInfo is ArrayType) return false;
                switch ((InstrEnum.Module)attr)
                {
                    case InstrEnum.Module.FaultInfo:
                    case InstrEnum.Module.INSTANCE:
                        if (!(destTypeInfo is DINT)) return false;
                        break;

                    case InstrEnum.Module.Path:
                        if (!(destTypeInfo.IsNumber ||
                              destTypeInfo.FamilyType == FamilyType.StringFamily)) return false;
                        break;
                    default:
                        if (!(destTypeInfo is DINT || destTypeInfo is INT)) return false;
                        break;
                }
            }
            else if ("Axis".Equals(className, StringComparison.OrdinalIgnoreCase))
            {
                switch ((InstrEnum.Axis)attr)
                {
                    //Integer
                    case InstrEnum.Axis.MotionPolarity:
                    case InstrEnum.Axis.ActuatorDiameterUnit:
                    case InstrEnum.Axis.ActuatorLeadUnit:
                    case InstrEnum.Axis.ActuatorType:
                    case InstrEnum.Axis.AxisConfiguration:
                    case InstrEnum.Axis.AxisConfigurationState:
                    case InstrEnum.Axis.AxisState:
                    case InstrEnum.Axis.AxisUpdateSchedule:
                    case InstrEnum.Axis.CIPAxisAlarmLogReset:
                    case InstrEnum.Axis.CIPAxisFaultLogReset:
                    case InstrEnum.Axis.ControlMethod:
                    case InstrEnum.Axis.ControlMode:
                    case InstrEnum.Axis.Feedback1Type:
                    case InstrEnum.Axis.Feedback1Unit:
                    case InstrEnum.Axis.FeedbackConfiguration:
                    case InstrEnum.Axis.FeedbackMode:
                    case InstrEnum.Axis.HomeDirection:
                    case InstrEnum.Axis.HomeMode:
                    case InstrEnum.Axis.HomeSequence:
                    case InstrEnum.Axis.HookupTestCommutationPolarity:
                    case InstrEnum.Axis.HookupTestFeedbackChannel:
                    case InstrEnum.Axis.HookupTestStatus:
                    case InstrEnum.Axis.InhibitAxis:
                    case InstrEnum.Axis.LoadType:
                    case InstrEnum.Axis.ModuleChannel:
                    case InstrEnum.Axis.MotionScalingConfiguration:
                    case InstrEnum.Axis.MotionUnit:
                    case InstrEnum.Axis.MotorDataSource:
                    case InstrEnum.Axis.MotorTestStatus:
                    case InstrEnum.Axis.MotorType:
                    case InstrEnum.Axis.MotorUnit:
                    case InstrEnum.Axis.PositionIntegratorControl:
                    case InstrEnum.Axis.ProgrammedStopMode:
                    case InstrEnum.Axis.VelocityIntegratorControl:
                    case InstrEnum.Axis.TuningSelect:
                    case InstrEnum.Axis.TuningDirection:
                    case InstrEnum.Axis.TravelMode:
                    case InstrEnum.Axis.StoppingAction:
                    case InstrEnum.Axis.SoftTravelLimitChecking:
                    case InstrEnum.Axis.ScalingSource:
                        if (!destTypeInfo.IsInteger) return false;
                        break;

                    //real
                    case InstrEnum.Axis.AccelerationFeedback:
                    case InstrEnum.Axis.AccelerationFeedforwardCommand:
                    case InstrEnum.Axis.AccelerationFeedforwardGain:
                    case InstrEnum.Axis.ActualAcceleration:
                    case InstrEnum.Axis.ActualPosition:
                    case InstrEnum.Axis.ActualVelocity:
                    case InstrEnum.Axis.ActuatorDiameter:
                    case InstrEnum.Axis.ActuatorLead:
                    case InstrEnum.Axis.AverageVelocity:
                    case InstrEnum.Axis.AverageVelocityTimebase:
                    case InstrEnum.Axis.BacklashReversalOffset:
                    case InstrEnum.Axis.CommandAcceleration:
                    case InstrEnum.Axis.CommandPosition:
                    case InstrEnum.Axis.CommandVelocity:
                    case InstrEnum.Axis.ConversionConstant:
                    case InstrEnum.Axis.CurrentCommand:
                    case InstrEnum.Axis.DampingFactor:
                    case InstrEnum.Axis.DCBusVoltage:
                    case InstrEnum.Axis.DriveModelTimeConstant:
                    case InstrEnum.Axis.HomeOffset:
                    case InstrEnum.Axis.HomePosition:
                    case InstrEnum.Axis.HomeReturnSpeed:
                    case InstrEnum.Axis.HomeSpeed:
                    case InstrEnum.Axis.HookupTestCommutationOffset:
                    case InstrEnum.Axis.HookupTestDistance:
                    case InstrEnum.Axis.InterpolatedActualPosition:
                    case InstrEnum.Axis.InterpolatedCommandPosition:
                    case InstrEnum.Axis.InverterCapacity:
                    case InstrEnum.Axis.LoadInertiaRatio:
                    case InstrEnum.Axis.LoadRatio:
                    case InstrEnum.Axis.MasterOffset:
                    case InstrEnum.Axis.MasterPositionFilterBandwidth:
                    case InstrEnum.Axis.MaximumAcceleration:
                    case InstrEnum.Axis.MaximumAccelerationJerk:
                    case InstrEnum.Axis.MaximumDeceleration:
                    case InstrEnum.Axis.MaximumDecelerationJerk:
                    case InstrEnum.Axis.MaximumSpeed:
                    case InstrEnum.Axis.PositionTrim:
                    case InstrEnum.Axis.MotorCapacity:
                    case InstrEnum.Axis.MotorTestBusOvervoltageSpeed:
                    case InstrEnum.Axis.MotorTestCommutationOffsetComp:
                    case InstrEnum.Axis.MotorTestCounterEMF:
                    case InstrEnum.Axis.MotorTestInductance:
                    case InstrEnum.Axis.MotorTestLdFluxSaturation:
                    case InstrEnum.Axis.MotorTestLqInductance:
                    case InstrEnum.Axis.MotorTestResistance:
                    case InstrEnum.Axis.OutputCurrent:
                    case InstrEnum.Axis.OutputPower:
                    case InstrEnum.Axis.OutputVoltage:
                    case InstrEnum.Axis.PlannerCommandPositionFractional:
                    case InstrEnum.Axis.PositionError:
                    case InstrEnum.Axis.PositionErrorTolerance:
                    case InstrEnum.Axis.PositionIntegratorBandwidth:
                    case InstrEnum.Axis.PositionIntegratorOutput:
                    case InstrEnum.Axis.PositionLockTolerance:
                    case InstrEnum.Axis.PositionLoopBandwidth:
                    case InstrEnum.Axis.PositionLoopOutput:
                    case InstrEnum.Axis.PositionReference:
                    case InstrEnum.Axis.PositionScalingDenominator:
                    case InstrEnum.Axis.PositionScalingNumerator:
                    case InstrEnum.Axis.PositionServoBandwidth:
                    case InstrEnum.Axis.PositionUnwindDenominator:
                    case InstrEnum.Axis.PositionUnwindNumerator:
                    case InstrEnum.Axis.Registration1NegativeEdgePosition:
                    case InstrEnum.Axis.Registration1Position:
                    case InstrEnum.Axis.Registration1PositiveEdgePosition:
                    case InstrEnum.Axis.Registration2NegativeEdgePosition:
                    case InstrEnum.Axis.Registration2Position:
                    case InstrEnum.Axis.Registration2PositiveEdgePosition:
                    case InstrEnum.Axis.VelocityTrim:
                    case InstrEnum.Axis.VelocityStandstillWindow:
                    case InstrEnum.Axis.VelocityServoBandwidth:
                    case InstrEnum.Axis.VelocityReference:
                    case InstrEnum.Axis.VelocityOffset:
                    case InstrEnum.Axis.VelocityLoopOutput:
                    case InstrEnum.Axis.VelocityLoopBandwidth:
                    case InstrEnum.Axis.VelocityIntegratorOutput:
                    case InstrEnum.Axis.VelocityIntegratorBandwidth:
                    case InstrEnum.Axis.VelocityFeedforwardGain:
                    case InstrEnum.Axis.VelocityFeedforwardCommand:
                    case InstrEnum.Axis.VelocityFeedback:
                    case InstrEnum.Axis.VelocityError:
                    case InstrEnum.Axis.TuningTravelLimit:
                    case InstrEnum.Axis.TuningTorque:
                    case InstrEnum.Axis.TuningSpeed:
                    case InstrEnum.Axis.TuneLoadOffset:
                    case InstrEnum.Axis.TuneInertiaMass:
                    case InstrEnum.Axis.TuneFriction:
                    case InstrEnum.Axis.TuneDecelerationTime:
                    case InstrEnum.Axis.TuneDeceleration:
                    case InstrEnum.Axis.TuneAccelerationTime:
                    case InstrEnum.Axis.TuneAcceleration:
                    case InstrEnum.Axis.TravelRange:
                    case InstrEnum.Axis.TorqueTrim:
                    case InstrEnum.Axis.TorqueReferenceLimited:
                    case InstrEnum.Axis.TorqueReferenceFiltered:
                    case InstrEnum.Axis.TorqueReference:
                    case InstrEnum.Axis.TorqueOffset:
                    case InstrEnum.Axis.TorqueLimitPositive:
                    case InstrEnum.Axis.TorqueLimitNegative:
                    case InstrEnum.Axis.SystemInertia:
                    case InstrEnum.Axis.SystemBandwidth:
                    case InstrEnum.Axis.StrobeMasterOffset:
                    case InstrEnum.Axis.StrobeCommandPosition:
                    case InstrEnum.Axis.StrobeActualPosition:
                    case InstrEnum.Axis.StoppingTorque:
                    case InstrEnum.Axis.StartMasterOffset:
                    case InstrEnum.Axis.StartCommandPosition:
                    case InstrEnum.Axis.StartActualPosition:
                    case InstrEnum.Axis.SoftTravelLimitPositive:
                    case InstrEnum.Axis.SoftTravelLimitNegative:
                        if (!destTypeInfo.IsReal) return false;
                        break;

                    //int and dint
                    case InstrEnum.Axis.AttributeErrorCode:
                    case InstrEnum.Axis.AttributeErrorID:
                    case InstrEnum.Axis.CIPAPRFaults:
                    case InstrEnum.Axis.CIPAPRFaultsRA:
                    case InstrEnum.Axis.CIPAxisState:
                    case InstrEnum.Axis.CIPStartInhibits:
                    case InstrEnum.Axis.CIPStartInhibitsRA:
                    case InstrEnum.Axis.GainTuningConfigurationBits:
                    case InstrEnum.Axis.InterpolationTime:
                    case InstrEnum.Axis.MemoryUse:
                    case InstrEnum.Axis.Registration1Time:
                    case InstrEnum.Axis.Registration2Time:
                    case InstrEnum.Axis.RegistrationInputs:
                    case InstrEnum.Axis.TuneStatus:
                        if (!(destTypeInfo is INT || destTypeInfo is DINT)) return false;
                        break;

                    //dint
                    case InstrEnum.Axis.AxisEventBits:
                    case InstrEnum.Axis.AxisFaultBits:
                    case InstrEnum.Axis.AxisFeatures:
                    case InstrEnum.Axis.AxisID:
                    case InstrEnum.Axis.AxisInstance:
                    case InstrEnum.Axis.AxisStatusBits:
                    case InstrEnum.Axis.C2CConnectionInstance:
                    case InstrEnum.Axis.C2CMapInstance:
                    case InstrEnum.Axis.CIPAxisIOStatus:
                    case InstrEnum.Axis.CIPAxisIOStatusRA:
                    case InstrEnum.Axis.CIPAxisStatus:
                    case InstrEnum.Axis.CIPAxisStatusRA:
                    case InstrEnum.Axis.CIPInitializationFaults:
                    case InstrEnum.Axis.CIPInitializationFaultsRA:
                    case InstrEnum.Axis.CommandUpdateDelayOffset:
                    case InstrEnum.Axis.DynamicsConfigurationBits:
                    case InstrEnum.Axis.GroupInstance:
                    case InstrEnum.Axis.HomeConfigurationBits:
                    case InstrEnum.Axis.InterpolatedPositionConfiguration:
                    case InstrEnum.Axis.MapInstance:
                    case InstrEnum.Axis.MasterInputConfigurationBits:
                    case InstrEnum.Axis.ModuleAlarmBits:
                    case InstrEnum.Axis.ModuleClassCode:
                    case InstrEnum.Axis.ModuleFaultBits:
                    case InstrEnum.Axis.MotionAlarmBits:
                    case InstrEnum.Axis.MotionFaultBits:
                    case InstrEnum.Axis.MotionStatusBits:
                    case InstrEnum.Axis.MotionResolution:
                    case InstrEnum.Axis.OutputCamExecutionTargets:
                    case InstrEnum.Axis.OutputCamLockStatus:
                    case InstrEnum.Axis.OutputCamPendingStatus:
                    case InstrEnum.Axis.OutputCamStatus:
                    case InstrEnum.Axis.OutputCamTransitionStatus:
                    case InstrEnum.Axis.PlannerActualPosition:
                    case InstrEnum.Axis.PlannerCommandPositionInteger:
                    case InstrEnum.Axis.PositionUnwind:
                    case InstrEnum.Axis.Registration1NegativeEdgeTime:
                    case InstrEnum.Axis.Registration1PositiveEdgeTime:
                    case InstrEnum.Axis.Registration2NegativeEdgeTime:
                    case InstrEnum.Axis.Registration2PositiveEdgeTime:
                    case InstrEnum.Axis.TransmissionRatioOutput:
                    case InstrEnum.Axis.TransmissionRatioInput:
                        if (!(destTypeInfo is DINT)) return false;
                        break;
                        
                    //lint and dint
                    case InstrEnum.Axis.CIPAxisFaults:
                    case InstrEnum.Axis.CIPAxisFaultsRA:
                        if (!(destTypeInfo is LINT || destTypeInfo is DINT)) return false;
                        break;
                }
            }else if ("WallClockTime".Equals(className,StringComparison.OrdinalIgnoreCase))
            {
                switch ((InstrEnum.WallClockTime)attr)
                {
                    case InstrEnum.WallClockTime.DateTime:
                    case InstrEnum.WallClockTime.LocalDateTime:
                        if (!(destTypeInfo is DINT)) return false;
                        break;
                    default:
                        return true;
                }
            }

            return true;
        }

        private bool SsvDestParamCheck(InstrEnum.SSVClassName className, int attr, IDataType destTypeInfo)
        {
            return GsvOrSsvCommonCheck(className.ToString(), attr, destTypeInfo);
        }
    }
}
