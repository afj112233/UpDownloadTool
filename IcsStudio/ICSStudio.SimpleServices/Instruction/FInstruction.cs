using ICSStudio.SimpleServices.Compiler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ICSStudio.Gui.Utils;

namespace ICSStudio.SimpleServices.Instruction
{
    public abstract class FInstruction
    {
        protected FInstruction(string instrName)
        {
            InstrName = instrName;
        }

        public abstract ASTNodeList ParseSTParameters(ASTNodeList parameters);
        public abstract ASTNodeList ParseRLLParameters(List<string> parameters);
        public string InstrName { get; }

        public static ASTNode ParseExpr(string param)
        {
            return STASTGenVisitor.ParseExpr(param);
        }

        public static ASTNodeList ParseExprList(List<string> params_string)
        {
            var list = new ASTNodeList();
            foreach (var param in params_string)
            {
                list.AddNode(string.IsNullOrEmpty(param) ? new ASTEmpty() : ParseExpr(param));
            }

            return list;
        }

        public virtual Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            return null;
        }

        public virtual List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            return null;
        }

        protected virtual ASTNode STConvert<T>(ASTNode astNode, int index)
        {
            var node = astNode as ASTName;
            if (node != null)
            {
                var astNameItem = node.id_list.nodes[0] as ASTNameItem;
                string immediateStr = astNameItem?.id;
                if (node.id_list.nodes.Count == 1 && !string.IsNullOrEmpty(immediateStr))
                {
                    var flag = false;
                    var result = Enum.GetValues(typeof(T)).Cast<T>().ToList().Find(x =>
                    {
                        var attribute =
                            Attribute.GetCustomAttribute(x.GetType().GetField(x.ToString()),
                                typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                        if (x.ToString().Equals(immediateStr))
                        {
                            flag = true;
                            return flag;
                        }
                        flag = attribute?.Value.Equals(immediateStr, StringComparison.OrdinalIgnoreCase) ??
                               x.ToString().Equals(immediateStr, StringComparison.OrdinalIgnoreCase);

                        return flag;
                    });

                    if (flag)
                    {
                        var value = Enum.Parse(typeof(T), result.ToString());
                        return new ASTInteger((int)value)
                        {
                            IsEnum = true,
                            EnumType = typeof(T),
                            ContextStart = node.ContextStart,
                            ContextEnd = node.ContextEnd
                        };
                    }

                    return new ASTError(astNode)
                    {
                        ContextStart = astNode.ContextStart,
                        ContextEnd = astNode.ContextEnd,
                        IsEnum = true,
                        EnumType = typeof(T),
                        Error = $"Error enum '{immediateStr}' of {InstrName}"
                    };
                }

                Debug.Assert(false);
                return astNode;
            }

            var integer = astNode as ASTInteger;
            var immediate = integer?.value;
            if (immediate != null)
            {
                if (!Enum.IsDefined(typeof(T), (int)immediate))
                {
                    throw new InstructionException($"{InstrName},param {++index}:Immediate value out of range.");
                }

                integer.IsEnum = true;
                integer.EnumType = typeof(T);
            }

            if (astNode is ASTUnaryOp)
                throw new InstructionException($"{InstrName},param {++index}:Immediate value out of range.");
            return astNode;
        }

        protected virtual string RLLConvert<T>(string parameter)
        {
            var name = parameter;
            Debug.Assert(!string.IsNullOrEmpty(name));
            int immediate;
            var res = int.TryParse(name, out immediate);
            if (res)
            {
                if (Enum.IsDefined(typeof(T), immediate))
                {
                    return parameter;
                }
                throw new InstructionException("error enum.");
            }
            object result = null;
            var enums = Enum.GetValues(typeof(T));
            foreach (T @enum in enums)
            {
                var attribute =
                    Attribute.GetCustomAttribute(@enum.GetType().GetField(@enum.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null && attribute.Value.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    result = @enum;
                    break;
                }

                if (@enum.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    result = @enum;
                    break;
                }
            }

            if (result != null)
            {
                var value = Enum.Parse(typeof(T), result.ToString());
                return ((int)value).ToString();
            }
            else
            {
                Debug.Assert(false);
                throw new InstructionException("error enum.");
            }

            //return parameter;
        }

        protected List<string> EnumNames<T>()
        {
            var enumNames = new List<string>();
            var enums = Enum.GetValues(typeof(T));
            foreach (T @enum in enums)
            {
                var attribute =
                    Attribute.GetCustomAttribute(@enum.GetType().GetField(@enum.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null)
                    enumNames.Add(attribute.Value.Replace(" ", ""));
                else
                    enumNames.Add(@enum.ToString());
            }

            return enumNames;
        }

        protected List<string> EnumRLLNames<T>()
        {
            var enumNames = new List<string>();
            var enums = Enum.GetValues(typeof(T));
            foreach (T @enum in enums)
            {
                var attribute =
                    Attribute.GetCustomAttribute(@enum.GetType().GetField(@enum.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null)
                    enumNames.Add(attribute.Value);
                else
                    enumNames.Add(@enum.ToString());
            }

            return enumNames;

        }
    }

    public class CommonFInstruction : FInstruction
    {
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return ParseExprList(parameters);
        }

        public CommonFInstruction(string instrName) : base(instrName)
        {
        }
    }

    public class MAMFInstruction : FInstruction
    {
        public MAMFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 20)
            {
                parameters.nodes[5] = STConvert<InstrEnum.MAMSpeedUnit>(parameters.nodes[5], 5);
                parameters.nodes[7] = STConvert<InstrEnum.MAMAccelUnit>(parameters.nodes[7], 7);
                parameters.nodes[9] = STConvert<InstrEnum.MAMAccelUnit>(parameters.nodes[9], 9);
                parameters.nodes[10] = STConvert<InstrEnum.Profile>(parameters.nodes[10], 10);
                parameters.nodes[13] = STConvert<InstrEnum.MAMJerkUnit>(parameters.nodes[13], 13);
                parameters.nodes[14] = STConvert<InstrEnum.MergeDisable>(parameters.nodes[14], 14);
                parameters.nodes[15] = STConvert<InstrEnum.MCLMSpeed>(parameters.nodes[15], 15);
                parameters.nodes[17] = STConvert<InstrEnum.MCLMLockDirection>(parameters.nodes[17], 17);
            }
            else
            {
                throw new Exception("MAMFInstruction:parameter count should be 20");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count == 20)
            {
                parameters[5] = RLLConvert<InstrEnum.MAMSpeedUnit_RLL>(parameters[5]);
                parameters[7] = RLLConvert<InstrEnum.MAMAccelUnit_RLL>(parameters[7]);
                parameters[9] = RLLConvert<InstrEnum.MAMAccelUnit_RLL>(parameters[9]);
                parameters[10] = RLLConvert<InstrEnum.Profile_RLL>(parameters[10]);
                parameters[13] = RLLConvert<InstrEnum.MAMJerkUnit_RLL>(parameters[13]);
                parameters[14] = RLLConvert<InstrEnum.MergeDisable>(parameters[14]);
                parameters[15] = RLLConvert<InstrEnum.MCLMSpeed>(parameters[15]);
                parameters[17] = RLLConvert<InstrEnum.MCLMLockDirection_RLL>(parameters[17]);
            }
            else
            {
                throw new Exception("MAMFInstruction:parameter count should be 20");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 5:
                    //return typeof(InstrEnum.MAMSpeedUnit);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAMSpeedUnit>(),
                        typeof(InstrEnum.MAMSpeedUnit));
                case 7:
                    //return typeof(InstrEnum.MAMAccelUnit);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAMAccelUnit>(),
                        typeof(InstrEnum.MAMAccelUnit));
                case 9:
                    //return typeof(InstrEnum.MAMAccelUnit);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAMAccelUnit>(),
                        typeof(InstrEnum.MAMAccelUnit));
                case 10:
                    //return typeof(InstrEnum.Profile);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Profile>(), typeof(InstrEnum.Profile));
                case 13:
                    //return typeof(InstrEnum.MAMJerkUnit);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAMJerkUnit>(),
                        typeof(InstrEnum.MAMJerkUnit));
                case 14:
                    //return typeof(InstrEnum.MergeDisable);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MergeDisable>(),
                        typeof(InstrEnum.MergeDisable));
                case 15:
                    //return typeof(InstrEnum.MCLMSpeed);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMSpeed>(), typeof(InstrEnum.MCLMSpeed));
                case 17:
                    //return typeof(InstrEnum.MCLMLockDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMLockDirection>(),
                        typeof(InstrEnum.MCLMLockDirection));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 5:
                    return EnumRLLNames<InstrEnum.MAMSpeedUnit_RLL>();
                case 7:
                    return EnumRLLNames<InstrEnum.MAMAccelUnit_RLL>();
                case 9:
                    return EnumRLLNames<InstrEnum.MAMAccelUnit_RLL>();
                case 10:
                    return EnumRLLNames<InstrEnum.Profile_RLL>();
                case 13:
                    return EnumRLLNames<InstrEnum.MAMJerkUnit_RLL>();
                case 14:
                    return EnumRLLNames<InstrEnum.MergeDisable>();
                case 15:
                    return EnumRLLNames<InstrEnum.MCLMSpeed>();
                case 17:
                    return EnumRLLNames<InstrEnum.MCLMLockDirection>();
                default:
                    return null;
            }
        }
    }

    public class SFPFInstruction : FInstruction
    {
        public SFPFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 2)
            {
                parameters.nodes[1] = STConvert<InstrEnum.TargetState>(parameters.nodes[1], 1);
            }
            else
            {
                throw new Exception("SFPFInstruction:parameter count should be 2");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 2)
            {
                parameters[1] = RLLConvert<InstrEnum.TargetState>(parameters[1]);
            }
            else
            {
                throw new Exception("SFPFInstruction:parameter count should be 2");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    //return typeof(InstrEnum.TargetState);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.TargetState>(),
                        typeof(InstrEnum.TargetState));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    return EnumRLLNames<InstrEnum.TargetState>();
                default:
                    return null;
            }
        }
    }

    public class PXRQFInstruction : FInstruction
    {
        public PXRQFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[1] = STConvert<InstrEnum.RequestType>(parameters.nodes[1], 1);
            }
            else
            {
                throw new Exception("PXRQFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[1] = RLLConvert<InstrEnum.RequestType_RLL>(parameters[1]);
            }
            else
            {
                throw new Exception("PXRQFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    //return typeof(InstrEnum.RequestType);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.RequestType>(),
                        typeof(InstrEnum.RequestType));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    return EnumRLLNames<InstrEnum.RequestType_RLL>();
                default:
                    return null;
            }
        }
    }

    public class POVRFInstruction : FInstruction
    {
        public POVRFInstruction(string instrName) : base(instrName)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 2)
            {
                parameters.nodes[1] = STConvert<InstrEnum.Command>(parameters.nodes[1], 1);
            }
            else
            {
                throw new Exception("POVRFInstruction:parameter count should be 2");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 2)
            {
                parameters[1] = RLLConvert<InstrEnum.Command>(parameters[1]);
            }
            else
            {
                throw new Exception("POVRFInstruction:parameter count should be 2");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    //return typeof(InstrEnum.Command);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Command>(), typeof(InstrEnum.Command));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    return EnumRLLNames<InstrEnum.Command>();
                default:
                    return null;
            }

        }
    }

    public class SCMDFInstruction : FInstruction
    {
        public SCMDFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[1] = STConvert<InstrEnum.SCMDCommand>(parameters.nodes[1], 1);
            }
            else
            {
                throw new Exception("SCMDFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[1] = RLLConvert<InstrEnum.SCMDCommand>(parameters[1]);
            }
            else
            {
                throw new Exception("SCMDFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    //return typeof(InstrEnum.SCMDCommand);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SCMDCommand>(),
                        typeof(InstrEnum.SCMDCommand));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    return EnumRLLNames<InstrEnum.SCMDCommand>();
                default:
                    return null;
            }

        }
    }

    public class SOVRFInstruction : FInstruction
    {
        public SOVRFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[1] = STConvert<InstrEnum.Command>(parameters.nodes[1], 1);
            }
            else
            {
                throw new Exception("SOVRFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[1] = RLLConvert<InstrEnum.Command>(parameters[1]);
            }
            else
            {
                throw new Exception("SOVRFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    //return typeof(InstrEnum.Command);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Command>(), typeof(InstrEnum.Command));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    return EnumRLLNames<InstrEnum.Command>();
                default:
                    return null;
            }

        }
    }

    public class GSVFInstruction : FInstruction
    {
        public GSVFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                var classNode = STConvert<InstrEnum.ClassName>(parameters.nodes[0], 0);
                parameters.nodes[0] = classNode;
                if (!(classNode is ASTInteger))
                {
                    var astName = parameters.nodes[2] as ASTName;
                    if (astName != null)
                    {
                        astName.IsEnum = true;

                        return parameters;
                    }
                }
                var className = (InstrEnum.ClassName)(byte)((ASTInteger)classNode).value;
                switch (className)
                {
                    case InstrEnum.ClassName.AddOnInstructionDefinition:
                        parameters.nodes[2] = STConvert<InstrEnum.AddOnInstructionDefinition>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Axis:
                        parameters.nodes[2] = STConvert<InstrEnum.Axis>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Controller:
                        parameters.nodes[2] = STConvert<InstrEnum.Controller>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.ControllerDevice:
                        parameters.nodes[2] = STConvert<InstrEnum.ControllerDevice>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.CoordinateSystem:
                        parameters.nodes[2] = STConvert<InstrEnum.CoordinateSystem>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.CST:
                        parameters.nodes[2] = STConvert<InstrEnum.CST>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.DataLog:
                        parameters.nodes[2] = STConvert<InstrEnum.DataLog>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.DF1:
                        parameters.nodes[2] = STConvert<InstrEnum.DF1>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.FaultLog:
                        parameters.nodes[2] = STConvert<InstrEnum.FaultLog>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Message:
                        parameters.nodes[2] = STConvert<InstrEnum.Message>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Module:
                        parameters.nodes[2] = STConvert<InstrEnum.Module>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.MotionGroup:
                        parameters.nodes[2] = STConvert<InstrEnum.MotionGroup>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Program:
                        parameters.nodes[2] = STConvert<InstrEnum.Program>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Routine:
                        parameters.nodes[2] = STConvert<InstrEnum.Routine>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.SerialPort:
                        parameters.nodes[2] = STConvert<InstrEnum.SerialPort>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.Task:
                        parameters.nodes[2] = STConvert<InstrEnum.Task>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.TimeSynchronize:
                        parameters.nodes[2] = STConvert<InstrEnum.TimeSynchronize>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.ClassName.WallClockTime:
                        parameters.nodes[2] = STConvert<InstrEnum.WallClockTime>(parameters.nodes[2], 2);
                        break;
                }
            }
            else
            {
                throw new Exception("GSVFInstruction:parameter count should be 4");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[0] = RLLConvert<InstrEnum.ClassName>(parameters[0]);
                var className = (InstrEnum.ClassName)Enum.Parse(typeof(InstrEnum.ClassName), parameters[0]);
                switch (className)
                {
                    case InstrEnum.ClassName.AddOnInstructionDefinition:
                        parameters[2] = RLLConvert<InstrEnum.AddOnInstructionDefinition>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Axis:
                        parameters[2] = RLLConvert<InstrEnum.Axis>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Controller:
                        parameters[2] = RLLConvert<InstrEnum.Controller>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.ControllerDevice:
                        parameters[2] = RLLConvert<InstrEnum.ControllerDevice>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.CoordinateSystem:
                        parameters[2] = RLLConvert<InstrEnum.CoordinateSystem>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.CST:
                        parameters[2] = RLLConvert<InstrEnum.CST>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.DataLog:
                        parameters[2] = RLLConvert<InstrEnum.DataLog>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.DF1:
                        parameters[2] = RLLConvert<InstrEnum.DF1>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.FaultLog:
                        parameters[2] = RLLConvert<InstrEnum.FaultLog>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Message:
                        parameters[2] = RLLConvert<InstrEnum.Message>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Module:
                        parameters[2] = RLLConvert<InstrEnum.Module>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.MotionGroup:
                        parameters[2] = RLLConvert<InstrEnum.MotionGroup>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Program:
                        parameters[2] = RLLConvert<InstrEnum.Program>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Routine:
                        parameters[2] = RLLConvert<InstrEnum.Routine>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.SerialPort:
                        parameters[2] = RLLConvert<InstrEnum.SerialPort>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.Task:
                        parameters[2] = RLLConvert<InstrEnum.Task>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.TimeSynchronize:
                        parameters[2] = RLLConvert<InstrEnum.TimeSynchronize>(parameters[2]);
                        break;
                    case InstrEnum.ClassName.WallClockTime:
                        parameters[2] = RLLConvert<InstrEnum.WallClockTime>(parameters[2]);
                        break;
                }
            }
            else
            {
                throw new Exception("GSVFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 0:
                    //return typeof(InstrEnum.ClassName);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ClassName>(), typeof(InstrEnum.ClassName));
                case 2:
                    if (string.IsNullOrEmpty(interrelated))
                        return null;
                    var className = (InstrEnum.ClassName)Enum.Parse(typeof(InstrEnum.ClassName), interrelated);
                    switch (className)
                    {
                        case InstrEnum.ClassName.AddOnInstructionDefinition:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.AddOnInstructionDefinition>(),
                                typeof(InstrEnum.AddOnInstructionDefinition));
                        case InstrEnum.ClassName.Axis:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Axis>(), typeof(InstrEnum.Axis));
                        case InstrEnum.ClassName.Controller:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Controller>(),
                                typeof(InstrEnum.Controller));
                        case InstrEnum.ClassName.ControllerDevice:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ControllerDevice>(),
                                typeof(InstrEnum.ControllerDevice));
                        case InstrEnum.ClassName.CoordinateSystem:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.CoordinateSystem>(),
                                typeof(InstrEnum.CoordinateSystem));
                        case InstrEnum.ClassName.CST:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.CST>(), typeof(InstrEnum.CST));
                        case InstrEnum.ClassName.DataLog:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.DataLog>(),
                                typeof(InstrEnum.DataLog));
                        case InstrEnum.ClassName.DF1:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.DF1>(), typeof(InstrEnum.DF1));
                        case InstrEnum.ClassName.FaultLog:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.FaultLog>(),
                                typeof(InstrEnum.FaultLog));
                        case InstrEnum.ClassName.Message:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Message>(),
                                typeof(InstrEnum.Message));
                        case InstrEnum.ClassName.Module:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Module>(),
                                typeof(InstrEnum.Module));
                        case InstrEnum.ClassName.MotionGroup:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MotionGroup>(),
                                typeof(InstrEnum.MotionGroup));
                        case InstrEnum.ClassName.Program:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Program>(),
                                typeof(InstrEnum.Program));
                        case InstrEnum.ClassName.Routine:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Routine>(),
                                typeof(InstrEnum.Routine));
                        case InstrEnum.ClassName.SerialPort:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SerialPort>(),
                                typeof(InstrEnum.SerialPort));
                        case InstrEnum.ClassName.Task:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Task>(), typeof(InstrEnum.Task));
                        case InstrEnum.ClassName.TimeSynchronize:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.TimeSynchronize>(),
                                typeof(InstrEnum.TimeSynchronize));
                        case InstrEnum.ClassName.WallClockTime:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.WallClockTime>(),
                                typeof(InstrEnum.WallClockTime));
                    }

                    Debug.Assert(false);
                    return null;
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 0:
                    return EnumRLLNames<InstrEnum.ClassName>();
                case 2:
                    if (string.IsNullOrEmpty(interrelated))
                        return null;
                    var className = (InstrEnum.ClassName)Enum.Parse(typeof(InstrEnum.ClassName), interrelated);
                    switch (className)
                    {
                        case InstrEnum.ClassName.AddOnInstructionDefinition:
                            return EnumRLLNames<InstrEnum.AddOnInstructionDefinition>();
                        case InstrEnum.ClassName.Axis:
                            return EnumRLLNames<InstrEnum.Axis>();
                        case InstrEnum.ClassName.Controller:
                            return EnumRLLNames<InstrEnum.Controller>();
                        case InstrEnum.ClassName.ControllerDevice:
                            return EnumRLLNames<InstrEnum.ControllerDevice>();
                        case InstrEnum.ClassName.CoordinateSystem:
                            return EnumRLLNames<InstrEnum.CoordinateSystem>();
                        case InstrEnum.ClassName.CST:
                            return EnumRLLNames<InstrEnum.CST>();
                        case InstrEnum.ClassName.DataLog:
                            return EnumRLLNames<InstrEnum.DataLog>();
                        case InstrEnum.ClassName.DF1:
                            return EnumRLLNames<InstrEnum.DF1>();
                        case InstrEnum.ClassName.FaultLog:
                            return EnumRLLNames<InstrEnum.FaultLog>();
                        case InstrEnum.ClassName.Message:
                            return EnumRLLNames<InstrEnum.Message>();
                        case InstrEnum.ClassName.Module:
                            return EnumRLLNames<InstrEnum.Module>();
                        case InstrEnum.ClassName.MotionGroup:
                            return EnumRLLNames<InstrEnum.MotionGroup>();
                        case InstrEnum.ClassName.Program:
                            return EnumRLLNames<InstrEnum.Program>();
                        case InstrEnum.ClassName.Routine:
                            return EnumRLLNames<InstrEnum.Routine>();
                        case InstrEnum.ClassName.SerialPort:
                            return EnumRLLNames<InstrEnum.SerialPort>();
                        case InstrEnum.ClassName.Task:
                            return EnumRLLNames<InstrEnum.Task>();
                        case InstrEnum.ClassName.TimeSynchronize:
                            return EnumRLLNames<InstrEnum.TimeSynchronize>();
                        case InstrEnum.ClassName.WallClockTime:
                            return EnumRLLNames<InstrEnum.WallClockTime>();
                    }

                    Debug.Assert(false);
                    return null;
                default:
                    return null;
            }
        }
    }

    public class SSVFInstruction : FInstruction
    {
        public SSVFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                var classNode = STConvert<InstrEnum.SSVClassName>(parameters.nodes[0], 0);
                parameters.nodes[0] = classNode;
                if (!(classNode is ASTInteger))
                {
                    var astName = parameters.nodes[2] as ASTName;
                    if (astName != null)
                    {
                        astName.IsEnum = true;

                        return parameters;
                    }
                }
                var className = (InstrEnum.SSVClassName)(byte)((ASTInteger)classNode).value;
                switch (className)
                {
                    case InstrEnum.SSVClassName.Axis:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVAxis>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.Controller:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVController>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.CoordinateSystem:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVCoordinateSystem>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.FaultLog:
                        parameters.nodes[2] = STConvert<InstrEnum.FaultLog>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.Message:
                        parameters.nodes[2] = STConvert<InstrEnum.Message>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.Module:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVModule>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.MotionGroup:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVMotionGroup>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.Program:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVProgram>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.Routine:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVRoutine>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.Task:
                        parameters.nodes[2] = STConvert<InstrEnum.Task>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.TimeSynchronize:
                        parameters.nodes[2] = STConvert<InstrEnum.SSVTimeSynchronize>(parameters.nodes[2], 2);
                        break;
                    case InstrEnum.SSVClassName.WallClockTime:
                        parameters.nodes[2] = STConvert<InstrEnum.WallClockTime>(parameters.nodes[2], 2);
                        break;
                }
            }
            else
            {
                throw new Exception("SSVFInstruction:parameter count should be 4");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[0] = RLLConvert<InstrEnum.SSVClassName>(parameters[0]);
                var className = (InstrEnum.SSVClassName)Enum.Parse(typeof(InstrEnum.ClassName), parameters[0]);
                switch (className)
                {
                    case InstrEnum.SSVClassName.Axis:
                        parameters[2] = RLLConvert<InstrEnum.SSVAxis>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.Controller:
                        parameters[2] = RLLConvert<InstrEnum.SSVController>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.CoordinateSystem:
                        parameters[2] = RLLConvert<InstrEnum.SSVCoordinateSystem>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.FaultLog:
                        parameters[2] = RLLConvert<InstrEnum.FaultLog>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.Message:
                        parameters[2] = RLLConvert<InstrEnum.Message>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.Module:
                        parameters[2] = RLLConvert<InstrEnum.SSVModule>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.MotionGroup:
                        parameters[2] = RLLConvert<InstrEnum.SSVMotionGroup>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.Program:
                        parameters[2] = RLLConvert<InstrEnum.SSVProgram>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.Routine:
                        parameters[2] = RLLConvert<InstrEnum.SSVRoutine>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.Task:
                        parameters[2] = RLLConvert<InstrEnum.Task>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.TimeSynchronize:
                        parameters[2] = RLLConvert<InstrEnum.SSVTimeSynchronize>(parameters[2]);
                        break;
                    case InstrEnum.SSVClassName.WallClockTime:
                        parameters[2] = RLLConvert<InstrEnum.WallClockTime>(parameters[2]);
                        break;
                }
            }
            else
            {
                throw new Exception("SSVFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 0:
                    //return typeof(InstrEnum.SSVClassName);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVClassName>(),
                        typeof(InstrEnum.SSVClassName));
                case 2:
                    if (string.IsNullOrEmpty(interrelated))
                        return null;
                    var className = (InstrEnum.SSVClassName)Enum.Parse(typeof(InstrEnum.SSVClassName), interrelated);
                    switch (className)
                    {
                        case InstrEnum.SSVClassName.Axis:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVAxis>(),
                                typeof(InstrEnum.SSVAxis));
                        case InstrEnum.SSVClassName.Controller:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVController>(),
                                typeof(InstrEnum.SSVController));
                        case InstrEnum.SSVClassName.CoordinateSystem:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVCoordinateSystem>(),
                                typeof(InstrEnum.SSVCoordinateSystem));
                        case InstrEnum.SSVClassName.FaultLog:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.FaultLog>(),
                                typeof(InstrEnum.FaultLog));
                        case InstrEnum.SSVClassName.Message:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Message>(),
                                typeof(InstrEnum.Message));
                        case InstrEnum.SSVClassName.Module:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVModule>(),
                                typeof(InstrEnum.SSVModule));
                        case InstrEnum.SSVClassName.MotionGroup:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVMotionGroup>(),
                                typeof(InstrEnum.SSVMotionGroup));
                        case InstrEnum.SSVClassName.Program:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVProgram>(),
                                typeof(InstrEnum.SSVProgram));
                        case InstrEnum.SSVClassName.Routine:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVRoutine>(),
                                typeof(InstrEnum.SSVRoutine));
                        case InstrEnum.SSVClassName.Task:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Task>(), typeof(InstrEnum.Task));
                        case InstrEnum.SSVClassName.TimeSynchronize:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.SSVTimeSynchronize>(),
                                typeof(InstrEnum.SSVTimeSynchronize));
                        case InstrEnum.SSVClassName.WallClockTime:
                            return new Tuple<List<string>, Type>(EnumNames<InstrEnum.WallClockTime>(),
                                typeof(InstrEnum.WallClockTime));
                    }

                    return null;
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 0:
                    return EnumRLLNames<InstrEnum.SSVClassName>();
                case 2:
                    if (string.IsNullOrEmpty(interrelated))
                        return null;
                    var className = (InstrEnum.SSVClassName)Enum.Parse(typeof(InstrEnum.SSVClassName), interrelated);
                    switch (className)
                    {
                        case InstrEnum.SSVClassName.Axis:
                            return EnumRLLNames<InstrEnum.SSVAxis>();
                        case InstrEnum.SSVClassName.Controller:
                            return EnumRLLNames<InstrEnum.SSVController>();
                        case InstrEnum.SSVClassName.CoordinateSystem:
                            return EnumRLLNames<InstrEnum.SSVCoordinateSystem>();
                        case InstrEnum.SSVClassName.FaultLog:
                            return EnumRLLNames<InstrEnum.FaultLog>();
                        case InstrEnum.SSVClassName.Message:
                            return EnumRLLNames<InstrEnum.Message>();
                        case InstrEnum.SSVClassName.Module:
                            return EnumRLLNames<InstrEnum.SSVModule>();
                        case InstrEnum.SSVClassName.MotionGroup:
                            return EnumRLLNames<InstrEnum.SSVMotionGroup>();
                        case InstrEnum.SSVClassName.Program:
                            return EnumRLLNames<InstrEnum.SSVProgram>();
                        case InstrEnum.SSVClassName.Routine:
                            return EnumRLLNames<InstrEnum.SSVRoutine>();
                        case InstrEnum.SSVClassName.Task:
                            return EnumRLLNames<InstrEnum.Task>();
                        case InstrEnum.SSVClassName.TimeSynchronize:
                            return EnumRLLNames<InstrEnum.SSVTimeSynchronize>();
                        case InstrEnum.SSVClassName.WallClockTime:
                            return EnumRLLNames<InstrEnum.WallClockTime>();
                    }

                    return null;
                default:
                    return null;
            }
        }
    }

    public class MDOFInstruction : FInstruction
    {
        public MDOFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters.nodes[3] = STConvert<InstrEnum.DriveUnits>(parameters.nodes[3], 3);
            }
            else
            {
                throw new Exception("MDOFInstruction:parameter count should be 4");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[3] = RLLConvert<InstrEnum.DriveUnits>(parameters[3]);
            }
            else
            {
                throw new Exception("MDOFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    //return typeof(InstrEnum.DriveUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.DriveUnits>(),
                        typeof(InstrEnum.DriveUnits));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.DriveUnits>();
                default:
                    return null;
            }
        }
    }

    public class MDSFInstruction : FInstruction
    {
        public MDSFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters.nodes[3] = STConvert<InstrEnum.MDSSpeedUnits>(parameters.nodes[3], 3);
            }
            else
            {
                throw new Exception("MDSFInstruction:parameter count should be 4");
            }
            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[3] = RLLConvert<InstrEnum.MDSSpeedUnits_RLL>(parameters[3]);
            }
            else
            {
                throw new Exception("MDSFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MDSSpeedUnits_RLL);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MDSSpeedUnits_RLL>(),
                        typeof(InstrEnum.MDSSpeedUnits_RLL));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.MDSSpeedUnits_RLL>();
                default:
                    return null;
            }
        }

    }

    public class MASFInstruction : FInstruction
    {
        public MASFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 9)
            {
                parameters.nodes[2] = STConvert<InstrEnum.Stop>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.YesNo>(parameters.nodes[3], 3);
                parameters.nodes[5] = STConvert<InstrEnum.MASDecelUnits>(parameters.nodes[5], 5);
                parameters.nodes[6] = STConvert<InstrEnum.YesNo>(parameters.nodes[6], 6);
                parameters.nodes[8] = STConvert<InstrEnum.MASJerkUnits>(parameters.nodes[8], 8);
            }
            else
            {
                throw new Exception("MASFInstruction:parameter count should be 9");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 9)
            {
                parameters[2] = RLLConvert<InstrEnum.Stop_RLL>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.YesNo>(parameters[3]);
                parameters[5] = RLLConvert<InstrEnum.MASDecelUnits_RLL>(parameters[5]);
                parameters[6] = RLLConvert<InstrEnum.YesNo>(parameters[6]);
                parameters[8] = RLLConvert<InstrEnum.MASJerkUnits_RLL>(parameters[8]);
            }
            else
            {
                throw new Exception("MASFInstruction:parameter count should be 9");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.Stop);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Stop>(), typeof(InstrEnum.Stop));
                case 3:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 5:
                    //return typeof(InstrEnum.MASDecelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MASDecelUnits>(),
                        typeof(InstrEnum.MASDecelUnits));
                case 6:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 8:
                    //return typeof(InstrEnum.MASJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MASJerkUnits>(),
                        typeof(InstrEnum.MASJerkUnits));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.Stop_RLL>();
                case 3:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 5:
                    return EnumRLLNames<InstrEnum.MASDecelUnits_RLL>();
                case 6:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 8:
                    return EnumRLLNames<InstrEnum.MASJerkUnits_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MAJFInstruction : FInstruction
    {
        public MAJFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 17)
            {
                parameters.nodes[4] = STConvert<InstrEnum.MAJSpeedUnits>(parameters.nodes[4], 4);
                parameters.nodes[6] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[6], 6);
                parameters.nodes[8] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[8], 8);
                parameters.nodes[9] = STConvert<InstrEnum.Profile>(parameters.nodes[9], 9);
                parameters.nodes[12] = STConvert<InstrEnum.MAJJerkUnits>(parameters.nodes[12], 12);
                parameters.nodes[13] = STConvert<InstrEnum.MergeDisable>(parameters.nodes[13], 13);
                parameters.nodes[14] = STConvert<InstrEnum.MCLMSpeed>(parameters.nodes[14], 14);
                parameters.nodes[16] = STConvert<InstrEnum.MCLMLockDirection>(parameters.nodes[16], 16);
            }
            else
            {
                throw new Exception("MAJFInstruction:parameter count should be 17");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 17)
            {
                parameters[4] = RLLConvert<InstrEnum.MAJSpeedUnits_RLL>(parameters[4]);
                parameters[6] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[6]);
                parameters[8] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[8]);
                parameters[9] = RLLConvert<InstrEnum.Profile_RLL>(parameters[9]);
                parameters[12] = RLLConvert<InstrEnum.MAJJerkUnits_RLL>(parameters[12]);
                parameters[13] = RLLConvert<InstrEnum.MergeDisable>(parameters[13]);
                parameters[14] = RLLConvert<InstrEnum.MCLMSpeed>(parameters[14]);
                parameters[16] = RLLConvert<InstrEnum.MCLMLockDirection_RLL>(parameters[16]);
            }
            else
            {
                throw new Exception("MAJFInstruction:parameter count should be 17");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 4:
                    //return typeof(InstrEnum.MAJSpeedUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJSpeedUnits>(),
                        typeof(InstrEnum.MAJSpeedUnits));
                case 6:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 8:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 9:
                    //return typeof(InstrEnum.Profile);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Profile>(), typeof(InstrEnum.Profile));
                case 12:
                    //return typeof(InstrEnum.MAJJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJJerkUnits>(),
                        typeof(InstrEnum.MAJJerkUnits));
                case 13:
                    //return typeof(InstrEnum.MergeDisable);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MergeDisable>(),
                        typeof(InstrEnum.MergeDisable));
                case 14:
                    //return typeof(InstrEnum.MCLMSpeed);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMSpeed>(), typeof(InstrEnum.MCLMSpeed));
                case 16:
                    //return typeof(InstrEnum.MCLMLockDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMLockDirection>(),
                        typeof(InstrEnum.MCLMLockDirection));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 4:
                    return EnumRLLNames<InstrEnum.MAJSpeedUnits_RLL>();
                case 6:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 8:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 9:
                    return EnumRLLNames<InstrEnum.Profile_RLL>();
                case 12:
                    return EnumRLLNames<InstrEnum.MAJJerkUnits_RLL>();
                case 13:
                    return EnumRLLNames<InstrEnum.MergeDisable>();
                case 14:
                    return EnumRLLNames<InstrEnum.MCLMSpeed>();
                case 16:
                    return EnumRLLNames<InstrEnum.MCLMLockDirection_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MAGFInstruction : FInstruction
    {
        public MAGFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 12)
            {
                parameters.nodes[7] = STConvert<InstrEnum.MasterReference>(parameters.nodes[7], 7);
                parameters.nodes[8] = STConvert<InstrEnum.RatioFormat>(parameters.nodes[8], 8);
                parameters.nodes[9] = STConvert<InstrEnum.ClutchEnable>(parameters.nodes[9], 9);
                parameters.nodes[11] = STConvert<InstrEnum.MAGAccelUnits>(parameters.nodes[11], 11);
            }
            else
            {
                throw new Exception("MAGFInstruction:parameter count should be 12");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 12)
            {
                parameters[7] = RLLConvert<InstrEnum.MasterReference>(parameters[7]);
                parameters[8] = RLLConvert<InstrEnum.RatioFormat>(parameters[8]);
                parameters[9] = RLLConvert<InstrEnum.ClutchEnable>(parameters[9]);
                parameters[11] = RLLConvert<InstrEnum.MAGAccelUnits_RLL>(parameters[11]);
            }
            else
            {
                throw new Exception("MAGFInstruction:parameter count should be 12");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 7:
                    //return typeof(InstrEnum.MasterReference);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterReference>(),
                        typeof(InstrEnum.MasterReference));
                case 8:
                    //return typeof(InstrEnum.RatioFormat);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.RatioFormat>(),
                        typeof(InstrEnum.RatioFormat));
                case 9:
                    //return typeof(InstrEnum.MergeDisable);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MergeDisable>(),
                        typeof(InstrEnum.MergeDisable));
                case 11:
                    //return typeof(InstrEnum.MAGAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAGAccelUnits>(),
                        typeof(InstrEnum.MAGAccelUnits));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 7:
                    return EnumRLLNames<InstrEnum.MasterReference>();
                case 8:
                    return EnumRLLNames<InstrEnum.RatioFormat>();
                case 9:
                    return EnumRLLNames<InstrEnum.MergeDisable>();
                case 11:
                    return EnumRLLNames<InstrEnum.MAGAccelUnits_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MCDFInstruction : FInstruction
    {
        public MCDFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 17)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MCDMotion>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.YesNo>(parameters.nodes[3], 3);
                parameters.nodes[5] = STConvert<InstrEnum.YesNo>(parameters.nodes[5], 5);
                parameters.nodes[7] = STConvert<InstrEnum.YesNo>(parameters.nodes[7], 7);
                parameters.nodes[9] = STConvert<InstrEnum.YesNo>(parameters.nodes[9], 9);
                parameters.nodes[11] = STConvert<InstrEnum.YesNo>(parameters.nodes[11], 11);
                parameters.nodes[13] = STConvert<InstrEnum.MAJSpeedUnits>(parameters.nodes[13], 13);
                parameters.nodes[14] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[14], 14);
                parameters.nodes[15] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[15], 15);
                parameters.nodes[16] = STConvert<InstrEnum.MAJJerkUnits>(parameters.nodes[16], 16);
            }
            else
            {
                throw new Exception("MCDFInstruction:parameter count should be 17");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 17)
            {
                parameters[2] = RLLConvert<InstrEnum.MCDMotion>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.YesNo>(parameters[3]);
                parameters[5] = RLLConvert<InstrEnum.YesNo>(parameters[5]);
                parameters[7] = RLLConvert<InstrEnum.YesNo>(parameters[7]);
                parameters[9] = RLLConvert<InstrEnum.YesNo>(parameters[9]);
                parameters[11] = RLLConvert<InstrEnum.YesNo>(parameters[11]);
                parameters[13] = RLLConvert<InstrEnum.MAJSpeedUnits_RLL>(parameters[13]);
                parameters[14] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[14]);
                parameters[15] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[15]);
                parameters[16] = RLLConvert<InstrEnum.MAJJerkUnits_RLL>(parameters[16]);
            }
            else
            {
                throw new Exception("MCDFInstruction:parameter count should be 17");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MCDMotion);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCDMotion>(), typeof(InstrEnum.MCDMotion));
                case 3:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 5:
                    // return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 7:
                    // return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 9:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 11:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 13:
                    //return typeof(InstrEnum.MAJSpeedUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJSpeedUnits>(),
                        typeof(InstrEnum.MAJSpeedUnits));
                case 14:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 15:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 16:
                    //return typeof(InstrEnum.MAJJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJJerkUnits>(),
                        typeof(InstrEnum.MAJJerkUnits));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MCDMotion>();
                case 3:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 5:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 7:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 9:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 11:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 13:
                    return EnumRLLNames<InstrEnum.MAJSpeedUnits_RLL>();
                case 14:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 15:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 16:
                    return EnumRLLNames<InstrEnum.MAJJerkUnits_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MRPFInstruction : FInstruction
    {
        public MRPFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 5)
            {
                parameters.nodes[2] = STConvert<InstrEnum.Type>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.MasterReference>(parameters.nodes[3], 3);
            }
            else
            {
                throw new Exception("MRPFInstruction:parameter count should be 5");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 5)
            {
                parameters[2] = RLLConvert<InstrEnum.Type>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.MasterReference>(parameters[3]);
            }
            else
            {
                throw new Exception("MRPFInstruction:parameter count should be 5");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.Type);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Type>(), typeof(InstrEnum.Type));
                case 3:
                    //return typeof(InstrEnum.MasterReference);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterReference>(),
                        typeof(InstrEnum.MasterReference));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.Type>();
                case 3:
                    return EnumRLLNames<InstrEnum.MasterReference>();
                default:
                    return null;
            }
        }
    }

    public class MAPCFInstruction : FInstruction
    {
        public MAPCFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 13)
            {
                parameters.nodes[7] = STConvert<InstrEnum.ExecutionMode>(parameters.nodes[7], 7);
                parameters.nodes[8] = STConvert<InstrEnum.ExecutionSchedule>(parameters.nodes[8], 8);
                parameters.nodes[11] = STConvert<InstrEnum.MasterReference>(parameters.nodes[11], 11);
                parameters.nodes[12] = STConvert<InstrEnum.MasterDirection>(parameters.nodes[12], 12);
            }
            else
            {
                throw new Exception("MAPCFInstruction:parameter count should be 13");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 13)
            {
                parameters[7] = RLLConvert<InstrEnum.ExecutionMode>(parameters[7]);
                parameters[8] = RLLConvert<InstrEnum.ExecutionSchedule_RLL>(parameters[8]);
                parameters[11] = RLLConvert<InstrEnum.MasterReference>(parameters[11]);
                parameters[12] = RLLConvert<InstrEnum.MasterDirection_RLL>(parameters[12]);
            }
            else
            {
                throw new Exception("MAPCFInstruction:parameter count should be 13");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 7:
                    //return typeof(InstrEnum.ExecutionMode);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ExecutionMode>(),
                        typeof(InstrEnum.ExecutionMode));
                case 8:
                    //return typeof(InstrEnum.ExecutionSchedule);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ExecutionSchedule>(),
                        typeof(InstrEnum.ExecutionSchedule));
                case 11:
                    //return typeof(InstrEnum.MasterReference);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterReference>(),
                        typeof(InstrEnum.MasterReference));
                case 12:
                    //return typeof(InstrEnum.MasterDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterDirection>(),
                        typeof(InstrEnum.MasterDirection));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 7:
                    return EnumRLLNames<InstrEnum.ExecutionMode>();
                case 8:
                    return EnumRLLNames<InstrEnum.ExecutionSchedule_RLL>();
                case 11:
                    return EnumRLLNames<InstrEnum.MasterReference>();
                case 12:
                    return EnumRLLNames<InstrEnum.MasterDirection_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MDRFInstruction : FInstruction
    {
        public MDRFInstruction(string instrName) : base(instrName)
        {
        }
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[2] = STConvert<InstrEnum.InputNumber>(parameters.nodes[2], 2);
            }
            else
            {
                throw new Exception("MDRFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[2] = RLLConvert<InstrEnum.InputNumber>(parameters[2]);
            }
            else
            {
                throw new Exception("MDRFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.InputNumber>(),
                        typeof(InstrEnum.InputNumber));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.InputNumber>();
                default:
                    return null;
            }
        }
    }

    public class MATCFInstruction : FInstruction
    {
        public MATCFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 11)
            {
                parameters.nodes[6] = STConvert<InstrEnum.MATCExecutionMode>(parameters.nodes[6], 6);
                parameters.nodes[7] = STConvert<InstrEnum.MATCExecutionSchedule>(parameters.nodes[7], 7);
                parameters.nodes[9] = STConvert<InstrEnum.MCLMLockDirection>(parameters.nodes[9], 9);
                parameters.nodes[10] = STConvert<InstrEnum.InstructionMode>(parameters.nodes[10], 10);
            }
            else
            {
                throw new Exception("MATCFInstruction:parameter count should be 11");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 11)
            {
                parameters[6] = RLLConvert<InstrEnum.MATCExecutionMode>(parameters[6]);
                parameters[7] = RLLConvert<InstrEnum.MATCExecutionSchedule>(parameters[7]);
                parameters[9] = RLLConvert<InstrEnum.MCLMLockDirection_RLL>(parameters[9]);
                parameters[10] = RLLConvert<InstrEnum.InstructionMode_RLL>(parameters[10]);
            }
            else
            {
                throw new Exception("MATCFInstruction:parameter count should be 11");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 6:
                    //return typeof(InstrEnum.MATCExecutionMode);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MATCExecutionMode>(),
                        typeof(InstrEnum.MATCExecutionMode));
                case 7:
                    //return typeof(InstrEnum.MATCExecutionSchedule);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MATCExecutionSchedule>(),
                        typeof(InstrEnum.MATCExecutionSchedule));
                case 9:
                    //return typeof(InstrEnum.MCLMLockDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMLockDirection>(),
                        typeof(InstrEnum.MCLMLockDirection));
                case 10:
                    //return typeof(InstrEnum.InstructionMode);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.InstructionMode>(),
                        typeof(InstrEnum.InstructionMode));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 6:
                    return EnumRLLNames<InstrEnum.MATCExecutionMode>();
                case 7:
                    return EnumRLLNames<InstrEnum.MATCExecutionSchedule>();
                case 9:
                    return EnumRLLNames<InstrEnum.MCLMLockDirection_RLL>();
                case 10:
                    return EnumRLLNames<InstrEnum.InstructionMode_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MDACFInstruction : FInstruction
    {
        public MDACFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 5)
            {
                parameters.nodes[3] = STConvert<InstrEnum.MDACMotion>(parameters.nodes[3], 3);
                parameters.nodes[4] = STConvert<InstrEnum.MasterReference>(parameters.nodes[4], 4);
            }
            else
            {
                throw new Exception("MDACFInstruction:parameter count should be 5");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 5)
            {
                parameters[3] = RLLConvert<InstrEnum.MDACMotion_RLL>(parameters[3]);
                parameters[4] = RLLConvert<InstrEnum.MasterReference>(parameters[4]);
            }
            else
            {
                throw new Exception("MDACFInstruction:parameter count should be 5");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    //return typeof(InstrEnum.MDACMotion);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MDACMotion>(),
                        typeof(InstrEnum.MDACMotion));
                case 4:
                    //return typeof(InstrEnum.MasterReference);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterReference>(),
                        typeof(InstrEnum.MasterReference));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.MDACMotion_RLL>();
                case 4:
                    return EnumRLLNames<InstrEnum.MasterReference>();
                default:
                    return null;
            }
        }
    }

    public class MGSFInstruction : FInstruction
    {
        public MGSFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MGSStop>(parameters.nodes[2], 2);
            }
            else
            {
                throw new Exception("MGSFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[2] = RLLConvert<InstrEnum.MGSStop_RLL>(parameters[2]);
            }
            else
            {
                throw new Exception("MGSFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MGSStop);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MGSStop>(), typeof(InstrEnum.MGSStop));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MGSStop_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MAWFInstruction : FInstruction
    {
        public MAWFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MAWTriggerConditiononce>(parameters.nodes[2], 2);
            }
            else
            {
                throw new Exception("MAWFInstruction:parameter count should be 4");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[2] = RLLConvert<InstrEnum.MAWTriggerConditiononce>(parameters[2]);
            }
            else
            {
                throw new Exception("MAWFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MAWTriggerConditiononce);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAWTriggerConditiononce>(),
                        typeof(InstrEnum.MAWTriggerConditiononce));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MAWTriggerConditiononce>();
                default:
                    return null;
            }
        }
    }

    public class MARFInstruction : FInstruction
    {
        public MARFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 7)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MARTriggerCondition>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.MergeDisable>(parameters.nodes[3], 3);
                parameters.nodes[6] = STConvert<InstrEnum.InputNumber>(parameters.nodes[6], 6);
            }
            else
            {
                throw new Exception("MARFInstruction:parameter count should be 7");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 7)
            {
                parameters[2] = RLLConvert<InstrEnum.MARTriggerCondition>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.MergeDisable>(parameters[3]);
                parameters[6] = RLLConvert<InstrEnum.InputNumber>(parameters[6]);
            }
            else
            {
                throw new Exception("MARFInstruction:parameter count should be 7");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MARTriggerCondition);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MARTriggerCondition>(),
                        typeof(InstrEnum.MARTriggerCondition));
                case 3:
                    //return typeof(InstrEnum.MergeDisable);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MergeDisable>(),
                        typeof(InstrEnum.MergeDisable));
                case 6:
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.InputNumber>(),
                        typeof(InstrEnum.InputNumber));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MARTriggerCondition>();
                case 3:
                    return EnumRLLNames<InstrEnum.MergeDisable>();
                case 6:
                    return EnumRLLNames<InstrEnum.InputNumber>();
                default:
                    return null;
            }
        }
    }

    public class MAOCFInstruction : FInstruction
    {
        public MAOCFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 14)
            {
                parameters.nodes[9] = STConvert<InstrEnum.ExecutionMode>(parameters.nodes[9], 9);
                parameters.nodes[10] = STConvert<InstrEnum.ExecutionSchedule>(parameters.nodes[10], 10);
                parameters.nodes[13] = STConvert<InstrEnum.MasterReference>(parameters.nodes[13], 13);
            }
            else
            {
                throw new Exception("MAOCFInstruction:parameter count should be 14");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 14)
            {
                parameters[9] = RLLConvert<InstrEnum.ExecutionMode>(parameters[9]);
                parameters[10] = RLLConvert<InstrEnum.ExecutionSchedule_RLL>(parameters[10]);
                parameters[13] = RLLConvert<InstrEnum.MasterReference>(parameters[13]);
            }
            else
            {
                throw new Exception("MAOCFInstruction:parameter count should be 14");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 9:
                    //return typeof(InstrEnum.ExecutionMode);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ExecutionMode>(),
                        typeof(InstrEnum.ExecutionMode));
                case 10:
                    //return typeof(InstrEnum.ExecutionSchedule);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ExecutionSchedule>(),
                        typeof(InstrEnum.ExecutionSchedule));
                case 13:
                    //return typeof(InstrEnum.MasterReference);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterReference>(),
                        typeof(InstrEnum.MasterReference));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 9:
                    return EnumRLLNames<InstrEnum.ExecutionMode>();
                case 10:
                    return EnumRLLNames<InstrEnum.ExecutionSchedule_RLL>();
                case 13:
                    return EnumRLLNames<InstrEnum.MasterReference>();
                default:
                    return null;
            }
        }
    }

    public class MDOCFInstruction : FInstruction
    {
        public MDOCFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters.nodes[3] = STConvert<InstrEnum.DisarmType>(parameters.nodes[3], 3);
            }
            else
            {
                throw new Exception("MDOCFInstruction:parameter count should be 4");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[3] = RLLConvert<InstrEnum.DisarmType>(parameters[3]);
            }
            else
            {
                throw new Exception("MDOCFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    //return typeof(InstrEnum.DisarmType);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.DisarmType>(),
                        typeof(InstrEnum.DisarmType));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.DisarmType>();
                default:
                    return null;
            }
        }
    }

    public class MAHDFInstruction : FInstruction
    {
        public MAHDFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters.nodes[2] = STConvert<InstrEnum.DiagnosticTest>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.ObservedDirection>(parameters.nodes[3], 3);
            }
            else
            {
                throw new Exception("MAHDFInstruction:parameter count should be 4");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 4)
            {
                parameters[2] = RLLConvert<InstrEnum.DiagnosticTest>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.ObservedDirection>(parameters[3]);
            }
            else
            {
                throw new Exception("MAHDFInstruction:parameter count should be 4");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.DiagnosticTest);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.DiagnosticTest>(),
                        typeof(InstrEnum.DiagnosticTest));
                case 3:
                    //return typeof(InstrEnum.ObservedDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.ObservedDirection>(),
                        typeof(InstrEnum.ObservedDirection));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.DiagnosticTest>();
                case 3:
                    return EnumRLLNames<InstrEnum.ObservedDirection>();
                default:
                    return null;
            }
        }
    }

    public class MRHDFInstruction : FInstruction
    {
        public MRHDFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MRHDDiagnosticTest>(parameters.nodes[2], 2);
            }
            else
            {
                throw new Exception("MRHDFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[2] = RLLConvert<InstrEnum.MRHDDiagnosticTest>(parameters[2]);
            }
            else
            {
                throw new Exception("MRHDFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MRHDDiagnosticTest);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MRHDDiagnosticTest>(),
                        typeof(InstrEnum.MRHDDiagnosticTest));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MRHDDiagnosticTest>();
                default:
                    return null;
            }
        }
    }

    public class MCSFInstruction : FInstruction
    {
        public MCSFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 9)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MCSStop>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.YesNo>(parameters.nodes[3], 3);
                parameters.nodes[5] = STConvert<InstrEnum.MASDecelUnits>(parameters.nodes[5], 5);
                parameters.nodes[6] = STConvert<InstrEnum.YesNo>(parameters.nodes[6], 6);
                parameters.nodes[8] = STConvert<InstrEnum.MASJerkUnits>(parameters.nodes[8], 8);
            }
            else
            {
                throw new Exception("MCSFInstruction:parameter count should be 9");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 9)
            {
                parameters[2] = RLLConvert<InstrEnum.MCSStop_RLL>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.YesNo>(parameters[3]);
                parameters[5] = RLLConvert<InstrEnum.MASDecelUnits_RLL>(parameters[5]);
                parameters[6] = RLLConvert<InstrEnum.YesNo>(parameters[6]);
                parameters[8] = RLLConvert<InstrEnum.MASJerkUnits_RLL>(parameters[8]);
            }
            else
            {
                throw new Exception("MCSFInstruction:parameter count should be 9");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MCSStop);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCSStop>(), typeof(InstrEnum.MCSStop));
                case 3:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 5:
                    //return typeof(InstrEnum.MASDecelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MASDecelUnits>(),
                        typeof(InstrEnum.MASDecelUnits));
                case 6:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 8:
                    //return typeof(InstrEnum.MASJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MASJerkUnits>(),
                        typeof(InstrEnum.MASJerkUnits));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MCSStop_RLL>();
                case 3:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 5:
                    return EnumRLLNames<InstrEnum.MASDecelUnits_RLL>();
                case 6:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 8:
                    return EnumRLLNames<InstrEnum.MASJerkUnits_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MCLMFInstruction : FInstruction
    {
        public MCLMFInstruction(string instrName) : base(instrName)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 22)
            {
                parameters.nodes[5] = STConvert<InstrEnum.MCLMSpeedUnits>(parameters.nodes[5], 5);
                parameters.nodes[7] = STConvert<InstrEnum.MCLMAccelUnits>(parameters.nodes[7], 7);
                parameters.nodes[9] = STConvert<InstrEnum.MCLMAccelUnits>(parameters.nodes[9], 9);
                parameters.nodes[10] = STConvert<InstrEnum.Profile>(parameters.nodes[10], 10);
                parameters.nodes[13] = STConvert<InstrEnum.MCLMJerkUnits>(parameters.nodes[13], 13);
                parameters.nodes[15] = STConvert<InstrEnum.MCLMMerge>(parameters.nodes[15], 15);
                parameters.nodes[16] = STConvert<InstrEnum.MCLMSpeed>(parameters.nodes[16], 16);
                parameters.nodes[19] = STConvert<InstrEnum.MCLMLockDirection>(parameters.nodes[19], 19);
            }
            else
            {
                throw new Exception("MCLMFInstruction:parameter count should be 22");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 22)
            {
                parameters[5] = RLLConvert<InstrEnum.MCLMSpeedUnits_RLL>(parameters[5]);
                parameters[7] = RLLConvert<InstrEnum.MCLMAccelUnits_RLL>(parameters[7]);
                parameters[9] = RLLConvert<InstrEnum.MCLMAccelUnits_RLL>(parameters[9]);
                parameters[10] = RLLConvert<InstrEnum.Profile_RLL>(parameters[10]);
                parameters[13] = RLLConvert<InstrEnum.MCLMJerkUnits_RLL>(parameters[13]);
                parameters[15] = RLLConvert<InstrEnum.MCLMMerge_RLL>(parameters[15]);
                parameters[16] = RLLConvert<InstrEnum.MCLMSpeed>(parameters[16]);
                parameters[19] = RLLConvert<InstrEnum.MCLMLockDirection_RLL>(parameters[19]);
            }
            else
            {
                throw new Exception("MCLMFInstruction:parameter count should be 22");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 5:
                    //return typeof(InstrEnum.MCLMSpeedUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMSpeedUnits>(),
                        typeof(InstrEnum.MCLMSpeedUnits));
                case 7:
                    //return typeof(InstrEnum.MCLMAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMAccelUnits>(),
                        typeof(InstrEnum.MCLMAccelUnits));
                case 9:
                    //return typeof(InstrEnum.MCLMAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMAccelUnits>(),
                        typeof(InstrEnum.MCLMAccelUnits));
                case 10:
                    //return typeof(InstrEnum.Profile);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Profile>(), typeof(InstrEnum.Profile));
                case 13:
                    //return typeof(InstrEnum.MCLMJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMJerkUnits>(),
                        typeof(InstrEnum.MCLMJerkUnits));
                case 15:
                    //return typeof(InstrEnum.MCLMMerge);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMMerge>(), typeof(InstrEnum.MCLMMerge));
                case 16:
                    //return typeof(InstrEnum.MCLMSpeed);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMSpeed>(), typeof(InstrEnum.MCLMSpeed));
                case 19:
                    //return typeof(InstrEnum.MCLMLockDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMLockDirection>(),
                        typeof(InstrEnum.MCLMLockDirection));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 5:
                    return EnumRLLNames<InstrEnum.MCLMSpeedUnits_RLL>();
                case 7:
                    return EnumRLLNames<InstrEnum.MCLMAccelUnits_RLL>();
                case 9:
                    return EnumRLLNames<InstrEnum.MCLMAccelUnits_RLL>();
                case 10:
                    return EnumRLLNames<InstrEnum.Profile_RLL>();
                case 13:
                    return EnumRLLNames<InstrEnum.MCLMJerkUnits_RLL>();
                case 15:
                    return EnumRLLNames<InstrEnum.MCLMMerge_RLL>();
                case 16:
                    return EnumRLLNames<InstrEnum.MCLMSpeed>();
                case 19:
                    return EnumRLLNames<InstrEnum.MCLMLockDirection_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MCCMFInstruction : FInstruction
    {
        public MCCMFInstruction(string instrName) : base(instrName)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 25)
            {
                parameters.nodes[8] = STConvert<InstrEnum.MAJSpeedUnits>(parameters.nodes[8], 8);
                parameters.nodes[10] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[10], 10);
                parameters.nodes[12] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[12], 12);
                parameters.nodes[13] = STConvert<InstrEnum.Profile>(parameters.nodes[13], 13);
                parameters.nodes[16] = STConvert<InstrEnum.MCLMJerkUnits>(parameters.nodes[16], 16);
                parameters.nodes[18] = STConvert<InstrEnum.MCLMMerge>(parameters.nodes[18], 18);
                parameters.nodes[19] = STConvert<InstrEnum.MCLMSpeed>(parameters.nodes[19], 19);
                parameters.nodes[22] = STConvert<InstrEnum.MCLMLockDirection>(parameters.nodes[22], 22);
            }
            else
            {
                throw new Exception("MCCMFInstruction:parameter count should be 26");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 25)
            {
                parameters[8] = RLLConvert<InstrEnum.MAJSpeedUnits_RLL>(parameters[8]);
                parameters[10] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[10]);
                parameters[12] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[12]);
                parameters[13] = RLLConvert<InstrEnum.Profile_RLL>(parameters[13]);
                parameters[16] = RLLConvert<InstrEnum.MCLMJerkUnits_RLL>(parameters[16]);
                parameters[18] = RLLConvert<InstrEnum.MCLMMerge_RLL>(parameters[18]);
                parameters[19] = RLLConvert<InstrEnum.MCLMSpeed>(parameters[19]);
                parameters[22] = RLLConvert<InstrEnum.MCLMLockDirection_RLL>(parameters[22]);
            }
            else
            {
                throw new Exception("MCCMFInstruction:parameter count should be 26");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 8:
                    //return typeof(InstrEnum.MAJSpeedUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJSpeedUnits>(),
                        typeof(InstrEnum.MAJSpeedUnits));
                case 10:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 12:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 13:
                    //return typeof(InstrEnum.Profile);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Profile>(), typeof(InstrEnum.Profile));
                case 16:
                    //return typeof(InstrEnum.MCLMJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMJerkUnits>(),
                        typeof(InstrEnum.MCLMJerkUnits));
                case 18:
                    //return typeof(InstrEnum.MCLMMerge);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMMerge>(), typeof(InstrEnum.MCLMMerge));
                case 19:
                    //return typeof(InstrEnum.MCLMSpeed);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMSpeed>(), typeof(InstrEnum.MCLMSpeed));
                case 22:
                    //return typeof(InstrEnum.MCLMLockDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCLMLockDirection>(),
                        typeof(InstrEnum.MCLMLockDirection));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 8:
                    return EnumRLLNames<InstrEnum.MAJSpeedUnits_RLL>();
                case 10:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 12:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 13:
                    return EnumRLLNames<InstrEnum.Profile_RLL>();
                case 16:
                    return EnumRLLNames<InstrEnum.MCLMJerkUnits_RLL>();
                case 18:
                    return EnumRLLNames<InstrEnum.MCLMMerge_RLL>();
                case 19:
                    return EnumRLLNames<InstrEnum.MCLMSpeed>();
                case 22:
                    return EnumRLLNames<InstrEnum.MCLMLockDirection_RLL>();
                default:
                    return null;
            }
        }

    }

    public class MCCDFInstruction : FInstruction
    {
        public MCCDFInstruction(string instrName) : base(instrName)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 18)
            {
                parameters.nodes[2] = STConvert<InstrEnum.MCCDMotion>(parameters.nodes[2], 2);
                parameters.nodes[3] = STConvert<InstrEnum.YesNo>(parameters.nodes[3], 3);
                parameters.nodes[5] = STConvert<InstrEnum.MAJSpeedUnits>(parameters.nodes[5], 5);
                parameters.nodes[6] = STConvert<InstrEnum.YesNo>(parameters.nodes[6], 6);
                parameters.nodes[8] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[8], 8);
                parameters.nodes[9] = STConvert<InstrEnum.YesNo>(parameters.nodes[9], 9);
                parameters.nodes[11] = STConvert<InstrEnum.MAJAccelUnits>(parameters.nodes[11], 11);
                parameters.nodes[12] = STConvert<InstrEnum.YesNo>(parameters.nodes[12], 12);
                parameters.nodes[14] = STConvert<InstrEnum.YesNo>(parameters.nodes[14], 14);
                parameters.nodes[16] = STConvert<InstrEnum.MAJJerkUnits>(parameters.nodes[16], 16);
                parameters.nodes[17] = STConvert<InstrEnum.MCCDScoped>(parameters.nodes[17], 17);
            }
            else
            {
                throw new Exception("MCCDFInstruction:parameter count should be 18");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 18)
            {
                parameters[2] = RLLConvert<InstrEnum.MCCDMotion_RLL>(parameters[2]);
                parameters[3] = RLLConvert<InstrEnum.YesNo>(parameters[3]);
                parameters[5] = RLLConvert<InstrEnum.MAJSpeedUnits_RLL>(parameters[5]);
                parameters[6] = RLLConvert<InstrEnum.YesNo>(parameters[6]);
                parameters[8] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[8]);
                parameters[9] = RLLConvert<InstrEnum.YesNo>(parameters[9]);
                parameters[11] = RLLConvert<InstrEnum.MAJAccelUnits_RLL>(parameters[11]);
                parameters[12] = RLLConvert<InstrEnum.YesNo>(parameters[12]);
                parameters[14] = RLLConvert<InstrEnum.YesNo>(parameters[14]);
                parameters[16] = RLLConvert<InstrEnum.MAJJerkUnits_RLL>(parameters[16]);
                parameters[17] = RLLConvert<InstrEnum.MCCDScoped_RLL>(parameters[17]);
            }
            else
            {
                throw new Exception("MCCDFInstruction:parameter count should be 18");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    //return typeof(InstrEnum.MCCDMotion);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCCDMotion>(),
                        typeof(InstrEnum.MCCDMotion));
                case 3:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 5:
                    //return typeof(InstrEnum.MAJSpeedUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJSpeedUnits>(),
                        typeof(InstrEnum.MAJSpeedUnits));
                case 6:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 8:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 9:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 11:
                    //return typeof(InstrEnum.MAJAccelUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJAccelUnits>(),
                        typeof(InstrEnum.MAJAccelUnits));
                case 12:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 14:
                    //return typeof(InstrEnum.YesNo);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.YesNo>(), typeof(InstrEnum.YesNo));
                case 16:
                    //return typeof(InstrEnum.MAJJerkUnits);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MAJJerkUnits>(),
                        typeof(InstrEnum.MAJJerkUnits));
                case 17:
                    //return typeof(InstrEnum.MCCDScoped);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MCCDScoped>(),
                        typeof(InstrEnum.MCCDScoped));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 2:
                    return EnumRLLNames<InstrEnum.MCCDMotion_RLL>();
                case 3:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 5:
                    return EnumRLLNames<InstrEnum.MAJSpeedUnits_RLL>();
                case 6:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 8:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 9:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 11:
                    return EnumRLLNames<InstrEnum.MAJAccelUnits_RLL>();
                case 12:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 14:
                    return EnumRLLNames<InstrEnum.YesNo>();
                case 16:
                    return EnumRLLNames<InstrEnum.MAJJerkUnits_RLL>();
                case 17:
                    return EnumRLLNames<InstrEnum.MCCDScoped_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MCTPFInstruction : FInstruction
    {
        public MCTPFInstruction(string instrName) : base(instrName)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 8)
            {
                parameters.nodes[5] = STConvert<InstrEnum.TransformDirection>(parameters.nodes[5], 5);
            }
            else
            {
                throw new Exception("MCTPFInstruction:parameter count should be 8");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 8)
            {
                parameters[5] = RLLConvert<InstrEnum.TransformDirection_RLL>(parameters[5]);
            }
            else
            {
                throw new Exception("MCTPFInstruction:parameter count should be 8");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 5:
                    //return typeof(InstrEnum.TransformDirection);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.TransformDirection>(),
                        typeof(InstrEnum.TransformDirection));
                default:
                    return null;
            }
        }

        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 5:
                    return EnumRLLNames<InstrEnum.TransformDirection_RLL>();
                default:
                    return null;
            }
        }
    }

    public class MDCCFInstruction : FInstruction
    {
        public MDCCFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 5)
            {
                parameters.nodes[3] = STConvert<InstrEnum.MasterReference>(parameters.nodes[3], 3);
            }
            else
            {
                throw new Exception("MDCCFInstruction:parameter count should be 5");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 5)
            {
                parameters[3] = RLLConvert<InstrEnum.MasterReference>(parameters[3]);
            }
            else
            {
                throw new Exception("MDCCFInstruction:parameter count should be 5");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    //return typeof(InstrEnum.MasterReference);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.MasterReference>(),
                        typeof(InstrEnum.MasterReference));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.MasterReference>();
                default:
                    return null;
            }
        }
    }

    public class SWPBFInstruction : FInstruction
    {
        public SWPBFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters.nodes[1] = STConvert<InstrEnum.OrderMode>(parameters.nodes[1], 1);
            }
            else
            {
                throw new Exception("SWPBFInstruction:parameter count should be 3");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 3)
            {
                parameters[1] = RLLConvert<InstrEnum.RLLOrderMode>(parameters[1]);
            }
            else
            {
                throw new Exception("SWPBFInstruction:parameter count should be 3");
            }

            return ParseExprList(parameters);
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    //return typeof(InstrEnum.OrderMode);
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.OrderMode>(), typeof(InstrEnum.OrderMode));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 1:
                    return EnumRLLNames<InstrEnum.OrderMode>();
                default:
                    return null;
            }
        }
    }

    #region FileMisc

    public class FALFInstruction : FInstruction
    {
        public FALFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            throw new NotImplementedException();
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            throw new NotImplementedException();
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Mode>(), typeof(InstrEnum.Mode));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.Mode>();
                default:
                    return null;
            }
        }
    }

    public class FSCFInstruction : FInstruction
    {
        public FSCFInstruction(string instrName) : base(instrName)
        {
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            throw new NotImplementedException();
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            throw new NotImplementedException();
        }

        public override Tuple<List<string>, Type> GetInstrEnumType(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return new Tuple<List<string>, Type>(EnumNames<InstrEnum.Mode>(), typeof(InstrEnum.Mode));
                default:
                    return null;
            }
        }
        public override List<string> GetRLLInstrEnumNames(int posInParameters, string interrelated = "")
        {
            switch (posInParameters)
            {
                case 3:
                    return EnumRLLNames<InstrEnum.Mode>();
                default:
                    return null;
            }
        }
    }

    #endregion

    //列举所有含有枚举的指令
    public class AllFInstructions
    {
        public static FInstruction MAM = new MAMFInstruction("MAM");
        public static FInstruction SFP = new SFPFInstruction("SFP");
        public static FInstruction PXRQ = new PXRQFInstruction("PXRQ");
        public static FInstruction POVR = new POVRFInstruction("POVR");
        public static FInstruction SCMD = new SCMDFInstruction("SCMD");
        public static FInstruction SOVR = new SOVRFInstruction("SOVR");
        public static FInstruction GSV = new GSVFInstruction("GSV");
        public static FInstruction SSV = new SSVFInstruction("SSV");
        public static FInstruction MDO = new MDOFInstruction("MDO");
        public static FInstruction MDS = new MDSFInstruction("MDS");
        public static FInstruction MAS = new MASFInstruction("MAS");
        public static FInstruction MAJ = new MAJFInstruction("MAJ");
        public static FInstruction MAG = new MAGFInstruction("MAG");
        public static FInstruction MCD = new MCDFInstruction("MCD");
        public static FInstruction MRP = new MRPFInstruction("MRP");
        public static FInstruction MAPC = new MAPCFInstruction("MAPC");
        public static FInstruction MATC = new MATCFInstruction("MATC");
        public static FInstruction MDAC = new MDACFInstruction("MDAC");
        public static FInstruction MGS = new MGSFInstruction("MGS");
        public static FInstruction MAW = new MAWFInstruction("MAW");
        public static FInstruction MAR = new MARFInstruction("MAR");
        public static FInstruction MAOC = new MAOCFInstruction("MAOC");
        public static FInstruction MDOC = new MDOCFInstruction("MDOC");
        public static FInstruction MAHD = new MAHDFInstruction("MAHD");
        public static FInstruction MRHD = new MRHDFInstruction("MRHD");
        public static FInstruction MCS = new MCSFInstruction("MCS");
        public static FInstruction MCLM = new MCLMFInstruction("MCLM");
        public static FInstruction MCCM = new MCCMFInstruction("MCCM");
        public static FInstruction MCCD = new MCCDFInstruction("MCCD");
        public static FInstruction MCTP = new MCTPFInstruction("MCTP");
        public static FInstruction MDCC = new MDCCFInstruction("MDCC");
        public static FInstruction SWPB = new SWPBFInstruction("SWPB");
        public static FInstruction FAL = new FALFInstruction("FAL");
        public static FInstruction FSC = new FALFInstruction("FSC");
        public static FInstruction MDR = new MDRFInstruction("MDR");
    }
}
