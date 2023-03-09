using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class InstrEnum
    {
        public enum MCLMSpeedUnits
        {
            [EnumMember(Value = "UnitsperSec")]
            UnitsperSec = 0,
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "UnitsperMasterUnit")]
            UnitsperMasterUnit = 4,
        }

        public enum MCLMSpeedUnits_RLL
        {
            [EnumMember(Value = "Units per sec")]
            UnitsperSec = 0,
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per MasterUnit")]
            UnitsperMasterUnit = 4
        }

        public enum MCLMAccelUnits
        {
            [EnumMember(Value = "UnitsperSec2")]
            UnitsperSec = 0,
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "UnitsperMasterUnit2")]
            UnitsperMasterUnit = 4,
        }

        public enum MCLMAccelUnits_RLL
        {
            [EnumMember(Value = "Units per sec2")]
            UnitsperSec = 0,
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per MasterUnit2")]
            UnitsperMasterUnit = 4,
        }

        public enum Profile
        {
            [EnumMember(Value = "Trapezoidal")]
            Trapezoidal = 0,
            [EnumMember(Value = "SCurve")]
            SCurve = 1,
        }

        public enum Profile_RLL
        {
            [EnumMember(Value = "Trapezoidal")]
            Trapezoidal = 0,
            [EnumMember(Value = "S-Curve")]
            SCurve = 1,
        }

        public enum MCLMJerkUnits
        {
            [EnumMember(Value = "UnitsperSec3")]
            UnitsperSec3 = 0,
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "%ofTime")]
            OfTime = 2,
            [EnumMember(Value = "UnitsperMasterUnit3")]
            UnitsperMasterUnit = 4,
            [EnumMember(Value = "%ofTimeMasterDriven")]
            OfTimeMasterDriven = 6,
        }

        public enum MCLMJerkUnits_RLL
        {
            [EnumMember(Value = "Units per sec3")]
            UnitsperSec3 = 0,
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "% of Time")]
            OfTime = 2,
            [EnumMember(Value = "Units per MasterUnit3")]
            UnitsperMasterUnit = 4,
            [EnumMember(Value = "% of Time-Master Driven")]
            OfTimeMasterDriven = 6,
        }

        public enum MCLMMerge
        {
            [EnumMember(Value = "Disabled")]
            Disabled = 0,
            [EnumMember(Value = "CoordinatedMotion")]
            CoordinatedMotion = 1,
            [EnumMember(Value = "AllMotion")]
            AllMotion = 2,
        }

        public enum MCLMMerge_RLL
        {
            [EnumMember(Value = "Disabled")]
            Disabled = 0,
            [EnumMember(Value = "Coordinated Motion")]
            CoordinatedMotion = 1,
            [EnumMember(Value = "All Motion")]
            AllMotion = 2,
        }

        public enum MCLMSpeed
        {
            [EnumMember(Value = "Programmed")]
            Programmed = 0,
            [EnumMember(Value = "Current")]
            Current = 1,
        }

        public enum MCLMLockDirection
        {
            [EnumMember(Value = "None")]
            None = 0,
            [EnumMember(Value = "ImmediateForwardOnly")]
            ImmediateForwardOnly = 1,
            [EnumMember(Value = "ImmediateReverseOnly")]
            ImmediateReverseOnly = 2,
            [EnumMember(Value = "PositionForwardOnly")]
            PositionForwardOnly = 3,
            [EnumMember(Value = "PositionReverseOnly")]
            PositionReverseOnly = 4,
        }

        public enum MCLMLockDirection_RLL
        {
            [EnumMember(Value = "None")]
            None = 0,
            [EnumMember(Value = "Immediate Forward Only")]
            ImmediateForwardOnly = 1,
            [EnumMember(Value = "Immediate Reverse Only")]
            ImmediateReverseOnly = 2,
            [EnumMember(Value = "Position Forward Only")]
            PositionForwardOnly = 3,
            [EnumMember(Value = "Position Reverse Only")]
            PositionReverseOnly = 4,
        }

        public enum MAMSpeedUnit
        {
            [EnumMember(Value = "Unitspersec")]
            Unitspersec = 0,
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,
            [EnumMember(Value = "UnitsperMasterUnit")]
            UnitsperMasterUnit = 4,
            [EnumMember(Value = "MasterUnits")]
            MasterUnits = 7,
        }

        public enum MAMSpeedUnit_RLL
        {
            [EnumMember(Value = "Units per sec")]
            Unitspersec = 0,
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,
            [EnumMember(Value = "Units per MasterUnit")]
            UnitsperMasterUnit = 4,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7,
        }

        public enum MAMAccelUnit
        {
            [EnumMember(Value = "Unitspersec2")]
            Unitspersec2 = 0,
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,
            [EnumMember(Value = "UnitsperMasterUnit2")]
            UnitsperMasterUnit2 = 4,
            [EnumMember(Value = "MasterUnits")]
            MasterUnits = 7,
        }

        public enum MAMAccelUnit_RLL
        {
            [EnumMember(Value = "Units per sec2")]
            Unitspersec2 = 0,
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,
            [EnumMember(Value = "Units per MasterUnit2")]
            UnitsperMasterUnit2 = 4,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7,
        }

        public enum MAMJerkUnit
        {
            [EnumMember(Value = "Unitspersec3")]
            Unitspersec = 0,
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "%ofTime")]
            OfTime = 2,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,
            [EnumMember(Value = "UnitsperMasterUnit3")]
            UnitsperMasterUnit3 = 4,
            [EnumMember(Value = "%ofTimeMasterDriven")]
            OfTimeMasterDriven = 6,
            [EnumMember(Value = "MasterUnits")]
            MasterUnits = 7,
        }

        public enum MAMJerkUnit_RLL
        {
            [EnumMember(Value = "Units per sec3")]
            Unitspersec = 0,
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "% of Time")]
            OfTime = 2,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,
            [EnumMember(Value = "Units per MasterUnit3")]
            UnitsperMasterUnit3 = 4,
            [EnumMember(Value = "% of Time-Master Driven")]
            OfTimeMasterDriven = 6,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7,
        }

        public enum YesNo
        {
            [EnumMember(Value = "No")]
            No = 0,
            [EnumMember(Value = "Yes")]
            Yes = 1,
        }

        public enum TargetState
        {
            [EnumMember(Value = "Execute")]
            Execute = 0,
            [EnumMember(Value = "Paused")]
            Paused = 1,
        }

        public enum RequestType
        {
            [EnumMember(Value = "AcquireResources")]
            AcquireResources = 0,
            [EnumMember(Value = "CancelMessageToLinkedPhase")]
            CancelMessageToLinkedPhase = 1,
            [EnumMember(Value = "ClearMessageToOperator")]
            ClearMessageToOperator = 2,
            [EnumMember(Value = "DownloadBatchData")]
            DownloadBatchData = 3,
            [EnumMember(Value = "DownloadContainerBindingPriority")]
            DownloadContainerBindingPriority = 4,
            [EnumMember(Value = "DownloadInputParameters")]
            DownloadInputParameters = 5,
            [EnumMember(Value = "DownloadInputParametersSubset")]
            DownloadInputParametersSubset = 6,
            [EnumMember(Value = "DownloadMaterialTrackDatabaseData")]
            DownloadMaterialTrackDatabaseData = 7,
            [EnumMember(Value = "DownloadMaterialTrackDataContainerInUse")]
            DownloadMaterialTrackDataContainerInUse = 8,
            [EnumMember(Value = "DownloadOutputParameterLimits")]
            DownloadOutputParameterLimits = 9,
            [EnumMember(Value = "DownloadSufficientMaterial")]
            DownloadSufficientMaterial = 10,
            [EnumMember(Value = "GenerateESignature")]
            GenerateESignature = 11,
            [EnumMember(Value = "ReceiveMessageFromLinkedPhase")]
            ReceiveMessageFromLinkedPhase = 12,
            [EnumMember(Value = "ReleaseResources")]
            ReleaseResources = 13,
            [EnumMember(Value = "SendMessageToLinkedPhase")]
            SendMessageToLinkedPhase = 14,
            [EnumMember(Value = "SendMessageToLinkedPhaseAndWait")]
            SendMessageToLinkedPhaseAndWait = 15,
            [EnumMember(Value = "SendMessageToOperator")]
            SendMessageToOperaor = 16,
            [EnumMember(Value = "UploadContainerBindingPriority")]
            UploadContainerBindingPriority = 17,
            [EnumMember(Value = "UploadMaterialTrackDatabaseData")]
            UploadMaterialTrackDatabaseData = 18,
            [EnumMember(Value = "UploadMaterialTrackDataContainerInUse")]
            UploadMaterialTrackDataContainerInUse = 19,
            [EnumMember(Value = "UploadOutputParameters")]
            UploadOutputParameters = 20,
            [EnumMember(Value = "UploadOutputParametersSubset")]
            UploadOutputParametersSubset = 21,
        }

        public enum RequestType_RLL
        {
            [EnumMember(Value = "Acquire Resources")]
            AcquireResources = 0,
            [EnumMember(Value = "Cancel Message To Linked Phase")]
            CancelMessageToLinkedPhase = 1,
            [EnumMember(Value = "Clear Message To Operator")]
            ClearMessageToOperator = 2,
            [EnumMember(Value = "Download Batch Data")]
            DownloadBatchData = 3,
            [EnumMember(Value = "Download Container Binding Priority")]
            DownloadContainerBindingPriority = 4,
            [EnumMember(Value = "Download Input Parameters")]
            DownloadInputParameters = 5,
            [EnumMember(Value = "Download Input Parameters Subset")]
            DownloadInputParametersSubset = 6,
            [EnumMember(Value = "Download Material Track Database Data")]
            DownloadMaterialTrackDatabaseData = 7,
            [EnumMember(Value = "Download Material Track Data Container In Use")]
            DownloadMaterialTrackDataContainerInUse = 8,
            [EnumMember(Value = "Download Output Parameter Limits")]
            DownloadOutputParameterLimits = 9,
            [EnumMember(Value = "Download Sufficient Material")]
            DownloadSufficientMaterial = 10,
            [EnumMember(Value = "Generate ESignature")]
            GenerateESignature = 11,
            [EnumMember(Value = "Receive Message From Linked Phase")]
            ReceiveMessageFromLinkedPhase = 12,
            [EnumMember(Value = "Release Resources")]
            ReleaseResources = 13,
            [EnumMember(Value = "Send Message To Linked Phase")]
            SendMessageToLinkedPhase = 14,
            [EnumMember(Value = "Send Message To Linked Phase And Wait")]
            SendMessageToLinkedPhaseAndWait = 15,
            [EnumMember(Value = "Send Message To Operator")]
            SendMessageToOperaor = 16,
            [EnumMember(Value = "Upload Container Binding Priority")]
            UploadContainerBindingPriority = 17,
            [EnumMember(Value = "Upload Material Track Database Data")]
            UploadMaterialTrackDatabaseData = 18,
            [EnumMember(Value = "Upload Material Track Data Container In Use")]
            UploadMaterialTrackDataContainerInUse = 19,
            [EnumMember(Value = "Upload Output Parameters")]
            UploadOutputParameters = 20,
            [EnumMember(Value = "Upload Output Parameters Subset")]
            UploadOutputParametersSubset = 21,
        }

        public enum Command
        {
            [EnumMember(Value = "Abort")]
            Abort = 0,
            [EnumMember(Value = "Stop")]
            Stop = 2,
            [EnumMember(Value = "Hold")]
            Hold = 4,
        }

        public enum SCMDCommand
        {
            [EnumMember(Value = "Abort")]
            Abort = 0,
            [EnumMember(Value = "Pause")]
            Pause = 1,
            [EnumMember(Value = "Stop")]
            Stop = 2,
            [EnumMember(Value = "AutoPhase")]
            AutoPhase = 3,
            [EnumMember(Value = "Hold")]
            Hold = 4,
            [EnumMember(Value = "Reset")]
            Reset = 5,
            [EnumMember(Value = "Restart")]
            Restart = 6,
            [EnumMember(Value = "Resume")]
            Resume = 7,
            [EnumMember(Value = "Start")]
            Start = 8,
        }

        public enum ClassName
        {
            [EnumMember(Value = "AddOnInstructionDefinition")]
            AddOnInstructionDefinition = 0,
            [EnumMember(Value = "Axis")]
            Axis,
            [EnumMember(Value = "Controller")]
            Controller,
            [EnumMember(Value = "ControllerDevice")]
            ControllerDevice,
            [EnumMember(Value = "CoordinateSystem")]
            CoordinateSystem,
            [EnumMember(Value = "CST")]
            CST,
            [EnumMember(Value = "DataLog")]
            DataLog,
            [EnumMember(Value = "DF1")]
            DF1,
            [EnumMember(Value = "FaultLog")]
            FaultLog,
            [EnumMember(Value = "Message")]
            Message,
            [EnumMember(Value = "Module")]
            Module,
            [EnumMember(Value = "MotionGroup")]
            MotionGroup,
            [EnumMember(Value = "Program")]
            Program,
            [EnumMember(Value = "Routine")]
            Routine,
            [EnumMember(Value = "SerialPort")]
            SerialPort,
            [EnumMember(Value = "Task")]
            Task,
            [EnumMember(Value = "TimeSynchronize")]
            TimeSynchronize,
            [EnumMember(Value = "WallClockTime")]
            WallClockTime,
        }

        public enum SSVClassName
        {
            [EnumMember(Value = "Axis")]
            Axis = 1,
            [EnumMember(Value = "Controller")]
            Controller = 2,
            [EnumMember(Value = "CoordinateSystem")]
            CoordinateSystem = 4,
            [EnumMember(Value = "FaultLog")]
            FaultLog = 8,
            [EnumMember(Value = "Message")]
            Message = 9,
            [EnumMember(Value = "Module")]
            Module = 10,
            [EnumMember(Value = "MotionGroup")]
            MotionGroup = 11,
            [EnumMember(Value = "Program")]
            Program = 12,
            [EnumMember(Value = "Routine")]
            Routine = 13,
            [EnumMember(Value = "Task")]
            Task = 15,
            [EnumMember(Value = "TimeSynchronize")]
            TimeSynchronize = 16,
            [EnumMember(Value = "WallClockTime")]
            WallClockTime = 17,
        }

        public enum DriveUnits
        {
            [EnumMember(Value = "Percent")]
            Percent = 0,
            [EnumMember(Value = "Volts")]
            Volts = 1,
        }

        public enum MDSSpeedUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec")]
            Unitspersec = 0,
        }

        public enum MDSSpeedUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum_R = 1,
            [EnumMember(Value = "Units per sec")]
            Unitspersec_R = 0,
        }

        public enum Stop
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "Jog")]
            Jog = 1,
            [EnumMember(Value = "Move")]
            Move = 2,
            [EnumMember(Value = "Gear")]
            Gear = 3,
            [EnumMember(Value = "Home")]
            Home = 4,
            [EnumMember(Value = "Tune")]
            Tune = 5,
            [EnumMember(Value = "Test")]
            Test = 6,
            [EnumMember(Value = "TimeCam")]
            TimeCam = 7,
            [EnumMember(Value = "PositionCam")]
            PositionCam = 8,
            [EnumMember(Value = "MasterOffsetMove")]
            MasterOffsetMove = 9,
            [EnumMember(Value = "DirectControl")]
            DirectControl = 10,
        }

        public enum Stop_RLL
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "Jog")]
            Jog = 1,
            [EnumMember(Value = "Move")]
            Move = 2,
            [EnumMember(Value = "Gear")]
            Gear = 3,
            [EnumMember(Value = "Home")]
            Home = 4,
            [EnumMember(Value = "Tune")]
            Tune = 5,
            [EnumMember(Value = "Test")]
            Test = 6,
            [EnumMember(Value = "Time Cam")]
            TimeCam = 7,
            [EnumMember(Value = "Position Cam")]
            PositionCam = 8,
            [EnumMember(Value = "Master Offset Move")]
            MasterOffsetMove = 9,
            [EnumMember(Value = "Direct Control")]
            DirectControl = 10,
        }

        public enum MASDecelUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec2")]
            Unitspersec2 = 0,
        }

        public enum MASDecelUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per sec2")]
            Unitspersec2 = 0,
        }

        public enum MASJerkUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec3")]
            Unitspersec3 = 0,
            [EnumMember(Value = "%ofTime")]
            OfTime = 2,
        }

        public enum MASJerkUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per sec3")]
            Unitspersec3 = 0,
            [EnumMember(Value = "% of Time")]
            OfTime = 2,
        }

        public enum MAJSpeedUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec")]
            Unitspersec = 0,
            [EnumMember(Value = "UnitsperMasterUnit")]
            UnitsperMasterUnit = 4,
        }

        public enum MAJSpeedUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per sec")]
            Unitspersec = 0,
            [EnumMember(Value = "Units per MasterUnit")]
            UnitsperMasterUnit = 4,
        }

        public enum MAJAccelUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec2")]
            Unitspersec2 = 0,
            [EnumMember(Value = "UnitsperMasterUnit2")]
            UnitsperMasterUnit2 = 4,
        }

        public enum MAJAccelUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per sec2")]
            Unitspersec2 = 0,
            [EnumMember(Value = "Units per MasterUnit2")]
            UnitsperMasterUnit2 = 4,
        }

        public enum MAJJerkUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec3")]
            Unitspersec3 = 0,
            [EnumMember(Value = "UnitsperMasterUnit3")]
            UnitsperMasterUnit3 = 4,
            [EnumMember(Value = "%ofTime")]
            OfTime = 2,
            [EnumMember(Value = "%ofTimeMasterDriven")]
            OfTimeMasterDriven = 6,
        }

        public enum MAJJerkUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per sec3")]
            Unitspersec3 = 0,
            [EnumMember(Value = "Units per MasterUnit3")]
            UnitsperMasterUnit3 = 4,
            [EnumMember(Value = "% of Time")]
            OfTime = 2,
            [EnumMember(Value = "% of Time-Master Driven")]
            OfTimeMasterDriven = 6,
        }

        public enum MergeDisable
        {
            [EnumMember(Value = "Enabled")]
            Enabled = 1,
            [EnumMember(Value = "Disabled")]
            Disabled = 0,
        }

        public enum ClutchEnable
        {
            [EnumMember(Value = "Enabled")]
            Enabled = 0,
            [EnumMember(Value = "Disabled")]
            Disabled = 1,
        }

        public enum MasterReference
        {
            [EnumMember(Value = "Actual")]
            Actual = 0,
            [EnumMember(Value = "Command")]
            Command = 1,
        }

        public enum RatioFormat
        {
            [EnumMember(Value = "real")]
            Real = 0,
            [EnumMember(Value = "Fraction_slave_master_counts")]
            Fraction_Slave_Master_Counts = 1,
        }

        public enum MAGAccelUnits
        {
            [EnumMember(Value = "%ofMaximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Unitspersec2")]
            Unitspersec2 = 0,
        }

        public enum MAGAccelUnits_RLL
        {
            [EnumMember(Value = "% of Maximum")]
            OfMaximum = 1,
            [EnumMember(Value = "Units per sec2")]
            Unitspersec2 = 0,
        }

        public enum MCDMotion
        {

            [EnumMember(Value = "Move")]
            Move = 1,
            [EnumMember(Value = "Jog")]
            Jog = 0,
        }

        public enum Type
        {
            [EnumMember(Value = "Absolute")]
            Absolute = 0,
            [EnumMember(Value = "Relative")]
            Relative = 1,
        }

        public enum ExecutionMode
        {
            [EnumMember(Value = "Once")]
            Once = 0,
            [EnumMember(Value = "Continuous")]
            Continuous = 1,
            [EnumMember(Value = "Persistent")]
            Persistent = 2,
        }

        public enum ExecutionSchedule
        {
            [EnumMember(Value = "Immediate")]
            Immediate = 0,
            [EnumMember(Value = "Pending")]
            Pending = 1,
            [EnumMember(Value = "ForwardOnly")]
            ForwardOnly = 2,
            [EnumMember(Value = "ReverseOnly")]
            ReverseOnly = 3,
            [EnumMember(Value = "Bidirectional")]
            Bidirectional = 4,
        }

        public enum ExecutionSchedule_RLL
        {
            [EnumMember(Value = "Immediate")]
            Immediate = 0,
            [EnumMember(Value = "Pending")]
            Pending = 1,
            [EnumMember(Value = "Forward Only")]
            ForwardOnly = 2,
            [EnumMember(Value = "Reverse Only")]
            ReverseOnly = 3,
            [EnumMember(Value = "Bi-Directional")]
            Bidirectional = 4,
        }

        public enum MasterDirection
        {
            [EnumMember(Value = "Bidirectional")]
            Bidirectional = 0,
            [EnumMember(Value = "ForwardOnly")]
            ForwardOnly = 1,
            [EnumMember(Value = "ReverseOnly")]
            ReverseOnly = 2,
        }

        public enum MasterDirection_RLL
        {
            [EnumMember(Value = "Bi-Directional")]
            Bidirectional = 0,
            [EnumMember(Value = "Forward Only")]
            ForwardOnly = 1,
            [EnumMember(Value = "Reverse Only")]
            ReverseOnly = 2,
        }

        public enum MATCExecutionMode
        {
            [EnumMember(Value = "Once")]
            Once = 0,
            [EnumMember(Value = "Continuous")]
            Continuous = 1,
        }

        public enum MATCExecutionSchedule
        {
            [EnumMember(Value = "Immediate")]
            Immediate = 0,
            [EnumMember(Value = "Pending")]
            Pending = 1,
        }

        public enum InstructionMode
        {
            [EnumMember(Value = "TimeDrivenMode")]
            TimeDrivenMode = 0,
            [EnumMember(Value = "MasterDrivenMode")]
            MasterDrivenMode = 1,
        }

        public enum InstructionMode_RLL
        {
            [EnumMember(Value = "Time Driven Mode")]
            TimeDrivenMode = 0,
            [EnumMember(Value = "Master Driven Mode")]
            MasterDrivenMode = 1,
        }

        public enum MDACMotion
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "Move")]
            Move = 1,
            [EnumMember(Value = "Jog")]
            Jog = 2,
            [EnumMember(Value = "TimeCam")]
            TimeCam = 3,
            [EnumMember(Value = "MasterOffsetMove")]
            MasterOffsetMove = 4,
        }

        public enum MDACMotion_RLL
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "Move")]
            Move = 1,
            [EnumMember(Value = "Jog")]
            Jog = 2,
            [EnumMember(Value = "Time Cam")]
            TimeCam = 3,
            [EnumMember(Value = "Master Offset Move")]
            MasterOffsetMove = 4,
        }

        public enum MGSStop
        {
            [EnumMember(Value = "Programmed")]
            Programmed = 0,
            [EnumMember(Value = "FastStop")]
            FastStop = 1,
            [EnumMember(Value = "FastDisable")]
            FastDisable = 2,
        }

        public enum MGSStop_RLL
        {
            [EnumMember(Value = "Programmed")]
            Programmed = 0,
            [EnumMember(Value = "Fast Stop")]
            FastStop = 1,
            [EnumMember(Value = "Fast Disable")]
            FastDisable = 2,
        }

        public enum MAWTriggerConditiononce
        {
            [EnumMember(Value = "Forward")]
            Forward = 0,
            [EnumMember(Value = "Reverse")]
            Reverse = 1,
        }

        public enum MARTriggerCondition
        {
            [EnumMember(Value = "Positive_Edge")]
            Positive_Edge = 0,
            [EnumMember(Value = "Negative_Edge")]
            Negative_Edge = 1,
        }

        public enum InputNumber
        {
            [EnumMember(Value = "1")]
            Num1 =1,
            [EnumMember(Value = "2")]
            Num2 =2
        }

        public enum DisarmType
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "Specific")]
            Specific = 1,
        }

        public enum DiagnosticTest
        {
            [EnumMember(Value = "Motor_Encoder")]
            Motor_Encoder = 0,
            [EnumMember(Value = "Encoder")]
            Encoder = 1,
            [EnumMember(Value = "Marker")]
            marker = 2,
        }

        public enum ObservedDirection
        {
            [EnumMember(Value = "Forward")]
            Forward = 0,
            [EnumMember(Value = "Reverse")]
            Reverse = 1,
        }

        public enum MRHDDiagnosticTest
        {
            [EnumMember(Value = "Motor_Encoder")]
            Motor_Encoder = 0,
            [EnumMember(Value = "Encoder")]
            Encoder = 1,
            [EnumMember(Value = "Marker")]
            marker = 2,
            [EnumMember(Value = "Commutation")]
            Commutation = 3,
        }

        public enum MCSStop
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "CoordinatedMove")]
            CoordinatedMove = 2,
            [EnumMember(Value = "CoordinatedTransform")]
            CoordinatedTransform = 3,
        }

        public enum MCSStop_RLL
        {
            [EnumMember(Value = "All")]
            All = 0,
            [EnumMember(Value = "Coordinated Move")]
            CoordinatedMove = 2,
            [EnumMember(Value = "Coordinated Transform")]
            CoordinatedTransform = 3,
        }

        public enum MCCDMotion
        {
            [EnumMember(Value = "CoordinatedMove")]
            CoordinatedMove = 1,
        }

        public enum MCCDMotion_RLL
        {
            [EnumMember(Value = "Coordinated Move")]
            CoordinatedMove = 1,
        }

        public enum MCCDScoped
        {
            [EnumMember(Value = "ActiveMotion")]
            ActiveMotion = 0,
            [EnumMember(Value = "ActiveAndPendingMotion")]
            ActiveAndPendingMotion = 1,
        }

        public enum MCCDScoped_RLL
        {
            [EnumMember(Value = "Active Motion")]
            ActiveMotion = 0,
            [EnumMember(Value = "Active and Pending Motion")]
            ActiveAndPendingMotion = 1,
        }

        public enum TransformDirection
        {
            [EnumMember(Value = "Forward")]
            Forward = 0,
            [EnumMember(Value = "Inverse")]
            Inverse = 1,
            [EnumMember(Value = "InverseLeftArm")]
            InverseLeftArm = 2,
            [EnumMember(Value = "InverseLftArmMirror")]
            InverseLftArmMirror = 3,
            [EnumMember(Value = "InverseRightArm")]
            InverseRightArm = 4,
            [EnumMember(Value = "InverseRightArmMirror")]
            InverseRightArmMirror = 5,
        }

        public enum TransformDirection_RLL
        {
            [EnumMember(Value = "Forward")]
            Forward = 0,
            [EnumMember(Value = "Inverse")]
            Inverse = 1,
            [EnumMember(Value = "Inverse Left Arm")]
            InverseLeftArm = 2,
            [EnumMember(Value = "Inverse Lft Arm Mirror")]
            InverseLftArmMirror = 3,
            [EnumMember(Value = "Inverse Right Arm")]
            InverseRightArm = 4,
            [EnumMember(Value = "Inverse Right Arm Mirror")]
            InverseRightArmMirror = 5,
        }

        public enum OrderMode
        {
            [EnumMember(Value = "HIGHLOW")]
            HIGHLOW = 2,
            [EnumMember(Value = "REVERSE")]
            REVERSE = 0,
            [EnumMember(Value = "WORD")]
            WORD = 1,
        }

        public enum RLLOrderMode
        {
            [EnumMember(Value = "HIGH/LOW")]
            HIGHLOW = 2,
            [EnumMember(Value = "REVERSE")]
            REVERSE = 0,
            [EnumMember(Value = "WORD")]
            WORD = 1,
        }

        public enum Mode
        {
            [EnumMember(Value = "ALL")]
            ALL = 0,
            [EnumMember(Value = "INC")]
            INC = 1,
        }
    }
}
