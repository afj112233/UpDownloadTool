using System;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.Other
{
    public static class MotionDirectCommandHelper
    {
        public static IMessageRouterRequest QueryCommandRequest(
            ushort classId, int instanceId, MotionDirectCommand command)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.QueryCommand,
                RequestPath = new PaddedEPath(classId, instanceId),
                RequestData = BitConverter.GetBytes((uint) command)
            };

            return request;
        }

        #region Motion Group

        public static IMessageRouterRequest MGS(int instanceId, uint stopMode)
        {
            var parameters = new byte[4 * 2];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(stopMode), 0, parameters, 4 * 1, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.MotionGroup, instanceId, MotionDirectCommand.MGS,
                parameters);
        }

        public static IMessageRouterRequest MGSD(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.MotionGroup, instanceId, MotionDirectCommand.MGSD,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MGSR(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.MotionGroup, instanceId, MotionDirectCommand.MGSR,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MGSP(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.MotionGroup, instanceId, MotionDirectCommand.MGSP,
                BitConverter.GetBytes(instanceId));
        }

        #endregion

        #region Axis

        #region Motion State

        public static IMessageRouterRequest MSO(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MSO,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MSF(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MSF,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MASD(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MASD,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MASR(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MASR,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MDO(int instanceId, float driveOutput, uint driveUnits)
        {
            var parameters = new byte[4 * 3];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 0, 4);
            Array.Copy(BitConverter.GetBytes(driveOutput), 0, parameters, 4, 4);
            Array.Copy(BitConverter.GetBytes(driveUnits), 0, parameters, 8, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MASR, parameters);
        }

        public static IMessageRouterRequest MDF(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MDF,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MDS(int instanceId, float speed, uint speedUnits)
        {
            var parameters = new byte[4 * 3];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 0, 4);
            Array.Copy(BitConverter.GetBytes(speed), 0, parameters, 4, 4);
            Array.Copy(BitConverter.GetBytes(speedUnits), 0, parameters, 8, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MDS, parameters);
        }

        public static IMessageRouterRequest MAFR(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAFR,
                BitConverter.GetBytes(instanceId));
        }

        #endregion

        #region Motion Move

        public static IMessageRouterRequest MAS(
            int instanceId,
            uint stopType, float decelRate,
            float decelJerk, uint jerkUnits,
            uint decelUnits, uint changeDecel, uint changeDecelJerk)
        {
            var parameters = new byte[4 * 6];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 0, 4);
            Array.Copy(BitConverter.GetBytes(stopType), 0, parameters, 4, 4);
            Array.Copy(BitConverter.GetBytes(decelRate), 0, parameters, 8, 4);
            Array.Copy(BitConverter.GetBytes(decelJerk), 0, parameters, 12, 4);
            Array.Copy(BitConverter.GetBytes(jerkUnits), 0, parameters, 16, 4);

            // Bits Config
            uint bitsConfig = 0;
            bitsConfig |= decelUnits;
            bitsConfig |= changeDecel << 1;
            bitsConfig |= changeDecelJerk << 2;

            Array.Copy(BitConverter.GetBytes(bitsConfig), 0, parameters, 20, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAS, parameters);
        }

        public static IMessageRouterRequest MAH(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAH,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MAJ(
            int instanceId,
            uint direction,
            float speed, uint speedUnits,
            float accelRate, uint accelUnits,
            float decelRate, uint decelUnits,
            uint profile,
            float accelJerk, float decelJerk,
            uint jerkUnits,
            uint merge, uint mergeSpeed,
            float lockPosition, uint lockDirection)
        {
            var parameters = new byte[4 * 16];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(direction), 0, parameters, 4 * 1, 4);
            Array.Copy(BitConverter.GetBytes(speed), 0, parameters, 4 * 2, 4);
            Array.Copy(BitConverter.GetBytes(speedUnits), 0, parameters, 4 * 3, 4);
            Array.Copy(BitConverter.GetBytes(accelRate), 0, parameters, 4 * 4, 4);
            Array.Copy(BitConverter.GetBytes(accelUnits), 0, parameters, 4 * 5, 4);
            Array.Copy(BitConverter.GetBytes(decelRate), 0, parameters, 4 * 6, 4);
            Array.Copy(BitConverter.GetBytes(decelUnits), 0, parameters, 4 * 7, 4);
            Array.Copy(BitConverter.GetBytes(profile), 0, parameters, 4 * 8, 4);
            Array.Copy(BitConverter.GetBytes(accelJerk), 0, parameters, 4 * 9, 4);
            Array.Copy(BitConverter.GetBytes(decelJerk), 0, parameters, 4 * 10, 4);
            Array.Copy(BitConverter.GetBytes(jerkUnits), 0, parameters, 4 * 11, 4);
            Array.Copy(BitConverter.GetBytes(merge), 0, parameters, 4 * 12, 4);
            Array.Copy(BitConverter.GetBytes(mergeSpeed), 0, parameters, 4 * 13, 4);
            Array.Copy(BitConverter.GetBytes(lockPosition), 0, parameters, 4 * 14, 4);
            Array.Copy(BitConverter.GetBytes(lockDirection), 0, parameters, 4 * 15, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAJ, parameters);
        }

        public static IMessageRouterRequest MAM(
            int instanceId,
            uint moveType,
            float position, float speed, uint speedUnits,
            float accelRate, uint accelUnits,
            float decelRate, uint decelUnits,
            uint profile,
            float accelJerk, float decelJerk, uint jerkUnits,
            uint merge, uint mergeSpeed,
            float lockPosition, uint lockDirection,
            float eventDistance = 0, float calculatedData = 0)
        {
            var parameters = new byte[4 * 19];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(moveType), 0, parameters, 4 * 1, 4);
            Array.Copy(BitConverter.GetBytes(position), 0, parameters, 4 * 2, 4);
            Array.Copy(BitConverter.GetBytes(speed), 0, parameters, 4 * 3, 4);
            Array.Copy(BitConverter.GetBytes(speedUnits), 0, parameters, 4 * 4, 4);
            Array.Copy(BitConverter.GetBytes(accelRate), 0, parameters, 4 * 5, 4);
            Array.Copy(BitConverter.GetBytes(accelUnits), 0, parameters, 4 * 6, 4);
            Array.Copy(BitConverter.GetBytes(decelRate), 0, parameters, 4 * 7, 4);
            Array.Copy(BitConverter.GetBytes(decelUnits), 0, parameters, 4 * 8, 4);
            Array.Copy(BitConverter.GetBytes(profile), 0, parameters, 4 * 9, 4);
            Array.Copy(BitConverter.GetBytes(accelJerk), 0, parameters, 4 * 10, 4);
            Array.Copy(BitConverter.GetBytes(decelJerk), 0, parameters, 4 * 11, 4);
            Array.Copy(BitConverter.GetBytes(jerkUnits), 0, parameters, 4 * 12, 4);
            Array.Copy(BitConverter.GetBytes(merge), 0, parameters, 4 * 13, 4);
            Array.Copy(BitConverter.GetBytes(mergeSpeed), 0, parameters, 4 * 14, 4);
            Array.Copy(BitConverter.GetBytes(lockPosition), 0, parameters, 4 * 15, 4);
            Array.Copy(BitConverter.GetBytes(lockDirection), 0, parameters, 4 * 16, 4);

            // real
            Array.Copy(BitConverter.GetBytes(eventDistance), 0, parameters, 4 * 17, 4);
            Array.Copy(BitConverter.GetBytes(calculatedData), 0, parameters, 4 * 18, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAM, parameters);
        }

        public static IMessageRouterRequest MAG(
            int instanceId,
            int masterAxisId,
            uint direction, float ratio,
            int slaveCounts, int masterCounts,
            float accelRate,
            uint masterReference, uint ratioFormat, uint clutch, uint accelUnits)
        {
            var parameters = new byte[4 * 8];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(masterAxisId), 0, parameters, 4 * 1, 4);
            Array.Copy(BitConverter.GetBytes(direction), 0, parameters, 4 * 2, 4);
            Array.Copy(BitConverter.GetBytes(ratio), 0, parameters, 4 * 3, 4);
            Array.Copy(BitConverter.GetBytes(slaveCounts), 0, parameters, 4 * 4, 4);
            Array.Copy(BitConverter.GetBytes(masterCounts), 0, parameters, 4 * 5, 4);
            Array.Copy(BitConverter.GetBytes(accelRate), 0, parameters, 4 * 6, 4);
            // Bits Config
            uint bitsConfig = 0;
            bitsConfig |= masterReference;
            bitsConfig |= ratioFormat << 1;
            bitsConfig |= clutch << 2;
            bitsConfig |= accelUnits << 3;

            Array.Copy(BitConverter.GetBytes(bitsConfig), 0, parameters, 4 * 7, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAG, parameters);
        }

        public static IMessageRouterRequest MCD(
            int instanceId,
            uint motionType, uint changeSpeed, float speed,
            uint changeAccel, float accelRate,
            uint changeDecel, float decelRate,
            uint changeAccelJerk, float accelJerk,
            uint changeDecelJerk, float decelJerk,
            uint speedUnits, uint accelUnits,
            uint decelUnits, uint jerkUnits)
        {
            var parameters = new byte[4 * 16];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(motionType), 0, parameters, 4 * 1, 4);
            Array.Copy(BitConverter.GetBytes(changeSpeed), 0, parameters, 4 * 2, 4);
            Array.Copy(BitConverter.GetBytes(speed), 0, parameters, 4 * 3, 4);
            Array.Copy(BitConverter.GetBytes(changeAccel), 0, parameters, 4 * 4, 4);
            Array.Copy(BitConverter.GetBytes(accelRate), 0, parameters, 4 * 5, 4);
            Array.Copy(BitConverter.GetBytes(changeDecel), 0, parameters, 4 * 6, 4);
            Array.Copy(BitConverter.GetBytes(decelRate), 0, parameters, 4 * 7, 4);
            Array.Copy(BitConverter.GetBytes(changeAccelJerk), 0, parameters, 4 * 8, 4);
            Array.Copy(BitConverter.GetBytes(accelJerk), 0, parameters, 4 * 9, 4);
            Array.Copy(BitConverter.GetBytes(changeDecelJerk), 0, parameters, 4 * 10, 4);
            Array.Copy(BitConverter.GetBytes(decelJerk), 0, parameters, 4 * 11, 4);
            Array.Copy(BitConverter.GetBytes(speedUnits), 0, parameters, 4 * 12, 4);
            Array.Copy(BitConverter.GetBytes(accelUnits), 0, parameters, 4 * 13, 4);
            Array.Copy(BitConverter.GetBytes(decelUnits), 0, parameters, 4 * 14, 4);
            Array.Copy(BitConverter.GetBytes(jerkUnits), 0, parameters, 4 * 15, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MCD, parameters);
        }

        public static IMessageRouterRequest MRP(
            int instanceId, float position, uint type, uint positionSelect)
        {
            var parameters = new byte[4 * 3];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(position), 0, parameters, 4 * 1, 4);

            // Bits Config
            uint bitsConfig = 0;
            bitsConfig |= type;
            bitsConfig |= positionSelect << 1;

            Array.Copy(BitConverter.GetBytes(bitsConfig), 0, parameters, 4 * 2, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MRP, parameters);
        }

        #endregion

        #region Motion Event

        public static IMessageRouterRequest MAW(int instanceId, float position, uint triggerCondition)
        {
            var parameters = new byte[4 * 3];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(position), 0, parameters, 4 * 1, 4);
            Array.Copy(BitConverter.GetBytes(triggerCondition), 0, parameters, 4 * 2, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAW, parameters);
        }

        public static IMessageRouterRequest MDW(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MDW,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MAR(
            int instanceId,
            uint inputNumber, uint triggerCondition,
            float minPosition, float maxPosition,
            uint windowedRegistration)
        {
            var parameters = new byte[4 * 6];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(inputNumber), 0, parameters, 4 * 1, 4);
            Array.Copy(BitConverter.GetBytes(triggerCondition), 0, parameters, 4 * 2, 4);
            Array.Copy(BitConverter.GetBytes(minPosition), 0, parameters, 4 * 3, 4);
            Array.Copy(BitConverter.GetBytes(maxPosition), 0, parameters, 4 * 4, 4);
            Array.Copy(BitConverter.GetBytes(windowedRegistration), 0, parameters, 4 * 5, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MAR, parameters);
        }

        public static IMessageRouterRequest MDR(
            int instanceId, uint inputNumber)
        {
            var parameters = new byte[4 * 2];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(inputNumber), 0, parameters, 4 * 1, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.MDR, parameters);
        }

        #endregion

        public static IMessageRouterRequest Autotune(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.AutoTune,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest HookupTest(int instanceId, uint testType = 0)
        {
            var parameters = new byte[4 * 2];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 4 * 0, 4);
            Array.Copy(BitConverter.GetBytes(testType), 0, parameters, 4 * 1, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.HookupTest, parameters);
        }

        #endregion
        
        private static IMessageRouterRequest ExecuteCommandRequest(
            ushort classId, int instanceId, MotionDirectCommand command, byte[] parameters)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.ExecuteCommand,
                RequestPath = new PaddedEPath(classId, instanceId),
                RequestData = null
            };

            if (parameters == null)
            {
                request.RequestData = BitConverter.GetBytes((uint) command);
            }
            else
            {
                request.RequestData = new byte[4 + parameters.Length];

                Array.Copy(BitConverter.GetBytes((uint) command), request.RequestData, 4);

                Array.Copy(parameters, 0, request.RequestData, 4, parameters.Length);
            }

            return request;
        }
    }
}
