using System;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.Other
{
    public static class MotionGeneratorHelper
    {
        public static IMessageRouterRequest QueryCommandRequest(
            ushort classId, int instanceId, MotionGeneratorCommand command)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.QueryCommand,
                RequestPath = new PaddedEPath(classId, instanceId),
                RequestData = BitConverter.GetBytes((uint) command)
            };

            return request;
        }

        #region Axis

        #region Motion State

        public static IMessageRouterRequest MSO(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MSO,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MSF(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MSF,
                BitConverter.GetBytes(instanceId));
        }

        public static IMessageRouterRequest MDS(int instanceId, float speed, uint speedUnits)
        {
            var parameters = new byte[4 * 3];

            Array.Copy(BitConverter.GetBytes(instanceId), 0, parameters, 0, 4);
            Array.Copy(BitConverter.GetBytes(speed), 0, parameters, 4, 4);
            Array.Copy(BitConverter.GetBytes(speedUnits), 0, parameters, 8, 4);

            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MDS, parameters);
        }

        public static IMessageRouterRequest MAFR(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MAFR,
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
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MAS, parameters);
        }

        public static IMessageRouterRequest MAH(int instanceId)
        {
            return ExecuteCommandRequest(
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MAH,
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
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MAJ, parameters);
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
                (ushort) CipObjectClassCode.Axis, instanceId, MotionGeneratorCommand.MAM, parameters);
        }

        #endregion

        #endregion

        private static IMessageRouterRequest ExecuteCommandRequest(
            ushort classId, int instanceId, MotionGeneratorCommand command, byte[] parameters)
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
