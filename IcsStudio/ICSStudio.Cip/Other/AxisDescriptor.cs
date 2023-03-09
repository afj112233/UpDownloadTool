using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;

namespace ICSStudio.Cip.Other
{
    public class AxisDescriptor
    {
        private static readonly Dictionary<ushort, string> CIPErrorCodes;
        private readonly CIPAxis _cipAxis;

        static AxisDescriptor()
        {
            // motion-rm003, page 318
            CIPErrorCodes = new Dictionary<ushort, string>
            {
                { 0x00, "Success" },
                { 0x01, "Connection failure." },
                { 0x02, "Resource unavailable." },
                { 0x03, "Invalid value in object specific data parameter of a service request." },
                { 0x04, "IOI segment error." },
                { 0x05, "IOI destination unknown." },
                { 0x06, "Partial transfer." },
                { 0x07, "Connection lost." },
                { 0x08, "Unimplemented service." },
                { 0x09, "Invalid attribute value." },
                { 0x0A, "Attribute list error." },
                { 0x0B, "Already in requested mode/state." },
                { 0x0C, "Object can not perform service in its current mode/state." },
                { 0x0D, "Object already exists." },
                { 0x0E, "Attribute value not settable." },
                { 0x0F, "Access permission does not allow service." },
                { 0x10, "Device's mode/state does not allow object to perform service." },
                { 0x11, "Reply data too large." },
                { 0x12, "Fragmentation of a primitive value." },
                { 0x13, "Not enough data." },
                { 0x14, "Undefined attribute." },
                { 0x15, "Too much data." },
                { 0x16, "Object does not exist." },
                { 0x17, "Service fragmentation sequence not currently in progress." },
                { 0x18, "No stored attribute data." },
                { 0x19, "Store operation failure." },
                { 0x1A, "Bridging failure, request packet too large for network." },
                { 0x1B, "Bridging failure, response packet too large for network." },
                { 0x1C, "Missing attribute list entry data." },
                { 0x1D, "Invalid attribute value list." },
                { 0x1E, "Embedded service error." },
                { 0x1F, "Connection Related Failure." },
                { 0x20, "Invalid Parameter." },
                { 0x21, "Write–once value or medium already written." },
                { 0x22, "Invalid Reply Received." },
                { 0x23, "CST not coordinated." },
                { 0x24, "Connection Scheduling Error." },
                { 0x25, "Key Failure in IOI." },
                { 0x26, "IOI Size Invalid." },
                { 0x27, "Unexpected attribute in list." },
                { 0x28, "DNet Invalid Member ID." },
                { 0x29, "DNet Member not settable." },

            };
        }

        public AxisDescriptor(CIPAxis cipAxis)
        {
            _cipAxis = cipAxis;
        }

        public string AxisState
        {
            get
            {
                if (_cipAxis == null)
                    return string.Empty;

                var axisState = (AxisStateType)Convert.ToByte(_cipAxis.AxisState);
                try
                {
                    return TypeDescriptor.GetConverter(axisState).ConvertToString(axisState);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return string.Empty;
            }
        }

        public string CIPAxisState
        {
            get
            {
                if (_cipAxis == null)
                    return string.Empty;

                var cipAxisState = (CIPAxisStateType)Convert.ToByte(_cipAxis.CIPAxisState);
                try
                {
                    return TypeDescriptor.GetConverter(cipAxisState).ConvertToString(cipAxisState);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// axis fault + cip axis fault + motion fault(or other fault)
        /// </summary>
        public string AxisFault
        {
            get
            {
                if (_cipAxis == null)
                    return string.Empty;

                //Axis Fault Bits
                var axisFaultBits = Convert.ToUInt32(_cipAxis.AxisFaultBits);

                List<string> mainFaultList = new List<string>();
                List<string> subFaultList = new List<string>();

                var axisFaultBitmap = (AxisFaultBitmap)axisFaultBits;

                if ((axisFaultBitmap & AxisFaultBitmap.PhysicalAxisFault) != 0)
                {
                    mainFaultList.Add("PhysicalAxisFault");

                    // CIP Axis Faults
                    var cipAxisFaults = Convert.ToUInt64(_cipAxis.CIPAxisFaults);
                    var exceptionList = GetAllCipAxisException(cipAxisFaults);
                    if (exceptionList.Count > 0)
                        subFaultList.AddRange(exceptionList);

                    // CIP Axis Faults RA
                    var cipAxisFaultsRA = Convert.ToUInt64(_cipAxis.CIPAxisFaultsRA);
                    var exceptionRAList = GetAllCipAxisExceptionRA(cipAxisFaultsRA);
                    if (exceptionRAList.Count > 0)
                        subFaultList.AddRange(exceptionRAList);

                }

                if ((axisFaultBitmap & AxisFaultBitmap.ModuleFault) != 0)
                {
                    mainFaultList.Add("ModuleFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.ConfigurationFault) != 0)
                {
                    mainFaultList.Add("ConfigFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.GroupFault) != 0)
                {
                    mainFaultList.Add("GroupFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.MotionFault) != 0)
                {
                    mainFaultList.Add("MotionFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.GuardFault) != 0)
                {
                    mainFaultList.Add("GuardFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.InitializationFault) != 0)
                {
                    mainFaultList.Add("InitializationFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.APRFault) != 0)
                {
                    mainFaultList.Add("APRFault");
                }

                if ((axisFaultBitmap & AxisFaultBitmap.SafetyFault) != 0)
                {
                    mainFaultList.Add("SafetyFault");
                }

                //
                string result = string.Empty;

                if (axisFaultBits == 0)
                    result = "No Faults";

                if (mainFaultList.Count > 0)
                {
                    result = string.Join(",", mainFaultList);
                }

                if (subFaultList.Count > 0)
                {
                    result = result + "," + string.Join(",", subFaultList.ToArray());
                }

                return result;

            }
        }

        // Module Faults, Module Fault Bits
        public string ModuleFaults
        {
            get
            {
                var moduleFaultBits = (ModuleFaultBitmap)Convert.ToUInt32(_cipAxis.ModuleFaultBits);

                if (moduleFaultBits == 0)
                {
                    var axisFaultBitmap = (AxisFaultBitmap)Convert.ToUInt32(_cipAxis.AxisFaultBits);

                    if ((axisFaultBitmap & AxisFaultBitmap.ModuleFault) == 0)
                        return "No Faults";
                }

                return moduleFaultBits.ToString();
            }
        }

        // Group Fault, 对应的是Motion Groups下面该轴对应的分组的组的Group Fault(暂时没完善先显示No Faults)
        public string GroupFault
        {
            get
            {
                var axisFaultBitmap = (AxisFaultBitmap)Convert.ToUInt32(_cipAxis.AxisFaultBits);

                if ((axisFaultBitmap & AxisFaultBitmap.GroupFault) == 0)
                    return "No Faults";

                //TODO(gjc): add code here
                return "TODO: Add Group Fault";
            }
        }

        // Motion Fault, Motion Fault Bits
        public string MotionFault
        {
            get
            {
                var motionFaultBits = Convert.ToUInt32(_cipAxis.MotionFaultBits);
                var axisFaultBitmap = (AxisFaultBitmap)Convert.ToUInt32(_cipAxis.AxisFaultBits);

                if (motionFaultBits == 0)
                {
                    if ((axisFaultBitmap & AxisFaultBitmap.MotionFault) == 0)
                        return "No Faults";
                }

                return string.Join(",", GetAllMotionFault(motionFaultBits));
            }
        }

        // Initialization Fault, CIP Initialization Faults
        public string InitializationFault
        {
            get
            {
                var axisFaultBitmap = (AxisFaultBitmap)Convert.ToUInt32(_cipAxis.AxisFaultBits);

                if ((axisFaultBitmap & AxisFaultBitmap.InitializationFault) == 0)
                    return "No Faults";

                //TODO(TLM):暂时拿到的_cipAxis.CIPInitializationFaults,16进制显示是5A5A5A5A，是没有被赋值的数据，需要底层赋值后继续更改
                var initializationFaults =
                    (CIPInitializationFaultBitmap)Convert.ToUInt32(_cipAxis.CIPInitializationFaults);

                return initializationFaults.ToString();

                //TODO(gjc): add CIPInitializationFaultsRA???
            }
        }

        public string APRFault
        {
            get
            {
                var axisFaultBitmap = (AxisFaultBitmap)Convert.ToUInt32(_cipAxis.AxisFaultBits);

                var aprFaults = (CIPAPRFaultBitmap)Convert.ToUInt16(_cipAxis.CIPAPRFaults);
                if ((int)aprFaults == 0)
                {
                    if ((axisFaultBitmap & AxisFaultBitmap.APRFault) == 0)
                        return "No Faults";
                }

                return aprFaults.ToString();
                //TODO(gjc): add CIPAPRFaultsRA???
            }
        }

        // Attribute Error, Attribute Error Code + Attribute Error ID
        public string AttributeError
        {
            get
            {
                if (_cipAxis == null)
                    return string.Empty;

                //拿到的attributeErrorCode和attributeErrorID全都为0，可能存在问题，暂时不做变动，返回No Faults
                var attributeErrorCode = Convert.ToUInt16(_cipAxis.AttributeErrorCode);
                var attributeErrorID = Convert.ToUInt16(_cipAxis.AttributeErrorID);
                if (attributeErrorCode == 0)
                    return "No Faults";

                var attributeMap = CipAttributeHelper.GetAttributeMap(typeof(CIPAxis));

                if (attributeMap.ContainsKey(attributeErrorID) && CIPErrorCodes.ContainsKey(attributeErrorCode))
                {
                    return $"{attributeMap[attributeErrorID]}-{CIPErrorCodes[attributeErrorCode]}";

                }

                return $"Unknown attribute error:{attributeErrorID},{attributeErrorCode}";
            }
        }

        // Guard Fault, Guard Status + Guard Faults
        public string GuardFault
        {
            get
            {
                var axisFaultBitmap = (AxisFaultBitmap)Convert.ToUInt32(_cipAxis.AxisFaultBits);
                var guardFaults = Convert.ToUInt32(_cipAxis.GuardFaults);
                if ((axisFaultBitmap & AxisFaultBitmap.GuardFault) == 0)
                    return "No Faults";

                //TODO(TLM):暂时拿到的_cipAxis.GuardFaults16进制显示是5A5A5A5A，是没有被赋值的数据，需要底层赋值后继续更改
                return string.Join(",", GetAllGuardFault(guardFaults));
            }
        }

        private List<string> GetAllGuardFault(uint guardFaults)
        {
            List<string> exceptionList = new List<string>();

            var allGuardException = Enum.GetValues(typeof(GuardFaultBitmap));
            foreach (var exception in allGuardException)
            {
                var guardException = (GuardFaultBitmap)exception;
                var guardExceptionValue = (byte)guardException;

                if ((guardFaults & (guardExceptionValue)) != 0)
                {
                    var guardFaultID = Math.Log(guardExceptionValue, 2);
                    if (guardFaultID != 1) //SF01为SafetyCoreFault
                        exceptionList.Add(EnumHelper.GetEnumMember(guardException) + $"(SF{guardFaultID:00})");
                    else
                        exceptionList.Add(EnumHelper.GetEnumMember(guardException));
                }
            }

            return exceptionList;
        }

        public string StartInhibited
        {
            get
            {
                if (_cipAxis == null)
                    return string.Empty;

                var cipStartInhibits = Convert.ToUInt16(_cipAxis.CIPStartInhibits);
                var startInhibited = (StandardStartInhibitBitmap)cipStartInhibits;

                if (startInhibited == 0)
                {
                    if (cipStartInhibits == 0)
                        return "Not Inhibited";
                }

                return startInhibited.ToString();
            }
        }

        public string MotorCatalog
        {
            get
            {
                if (_cipAxis == null)
                    return "<none>";

                return _cipAxis.MotorCatalogNumber.GetString();
            }
        }

        // ReSharper disable once UnusedMember.Local
        private string GetFirstCipAxisException(ulong cipAxisFaults)
        {
            // CIPStandardException
            var allCIPStandardException = Enum.GetValues(typeof(CIPStandardException));
            foreach (var exception in allCIPStandardException)
            {
                var standardException = (CIPStandardException)exception;
                var standardExceptionValue = (byte)standardException;

                if ((cipAxisFaults & ((ulong)1 << standardExceptionValue)) != 0)
                    return EnumHelper.GetEnumMember(standardException)+ $"(F{Convert.ToUInt32(standardException)})";
            }

            return "Something is wrong!";
        }

        private List<string> GetAllCipAxisException(ulong cipAxisFaults)
        {
            List<string> exceptionList = new List<string>();

            // CIPStandardException
            var allCIPStandardException = Enum.GetValues(typeof(CIPStandardException));
            foreach (var exception in allCIPStandardException)
            {
                var standardException = (CIPStandardException)exception;
                var standardExceptionValue = (byte)standardException;

                if ((cipAxisFaults & ((ulong)1 << standardExceptionValue)) != 0)
                {
                    exceptionList.Add(EnumHelper.GetEnumMember(standardException) + $"(F{Convert.ToUInt32(standardException)})");
                }

            }
            return exceptionList;
        }

        private List<string> GetAllCipAxisExceptionRA(ulong cipAxisFaultsRA)
        {
            List<string> exceptionList = new List<string>();

            // CIPStandardExceptionRA
            var allCIPStandardException = Enum.GetValues(typeof(CIPStandardExceptionRA));
            foreach (var exception in allCIPStandardException)
            {
                var standardException = (CIPStandardExceptionRA)exception;
                var standardExceptionValue = (byte)standardException;

                if ((cipAxisFaultsRA & ((ulong)1 << standardExceptionValue)) != 0)
                {
                    if (Convert.ToUInt32(standardException) == 26)
                        exceptionList.Add(EnumHelper.GetEnumMember(standardException) + $"(FC{Convert.ToUInt32(standardException)})");
                    else
                        exceptionList.Add(EnumHelper.GetEnumMember(standardException));
                }

            }

            return exceptionList;
        }

        private List<string> GetAllMotionFault(uint motionFaultBits)
        {
            List<string> exceptionList = new List<string>();

            var allMotionException = Enum.GetValues(typeof(MotionException));
            foreach (var exception in allMotionException)
            {
                var motionException = (MotionException)exception;
                var motionExceptionValue = (byte)motionException;

                if ((motionFaultBits & ((uint)1 << motionExceptionValue)) != 0)
                {
                    exceptionList.Add(EnumHelper.GetEnumMember(motionException));
                }
            }

            return exceptionList;
        }
    }
}
