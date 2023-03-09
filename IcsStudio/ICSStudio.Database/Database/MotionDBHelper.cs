using System;
using System.Collections.Generic;
using System.Linq;
using ICSStudio.Database.Table.Motion;
using ICSStudio.Utils;

namespace ICSStudio.Database.Database
{
    public class MotionDbHelper : BaseDbHelper
    {
        public MotionDbHelper()
        {
            var dllPath = AssemblyUtils.AssemblyDirectory;
            ConnectionString = $@"Data Source={dllPath}\ModuleProfiles\motion.db";
        }

        public string ConnectionString { get; }

        public int GetMotionDriveTypeId(
            string catalogNo,
            int converterACInputVoltage,
            int converterACInputPhasing)
        {
            string sql =
                $"select DriveTypeID from Drive where CatalogNo = '{catalogNo}' and ConverterACInputVoltage = {converterACInputVoltage} and ConverterACInputPhasing = {converterACInputPhasing}";
            int typeId;

            try
            {
                typeId = DoQuery<int>(sql, ConnectionString).Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            return typeId;

        }

        public Drive GetMotionDrive(
            string catalogNo,
            int converterACInputVoltage,
            int converterACInputPhasing)
        {
            string sql =
                $"select * from Drive where CatalogNo = '{catalogNo}' and ConverterACInputVoltage = {converterACInputVoltage} and ConverterACInputPhasing = {converterACInputPhasing}";

            return DoQuery<Drive>(sql, ConnectionString).SingleOrDefault();
        }

        public List<Drive> GetMotionDrive(string catalogNo)
        {
            string sql =
                $"select * from Drive where CatalogNo = '{catalogNo}'";

            return DoQuery<Drive>(sql, ConnectionString).ToList();
        }

        public IEnumerable<MotorSearchView> GetSupportMotors(
            int driveTypeId, List<int> feedbackTypes, List<int> motorTypes)
        {
            if (feedbackTypes == null)
                return null;

            if (feedbackTypes.Count == 0)
                return null;

            string feedbackTypeSql = $"{feedbackTypes[0]}";
            for (int i = 1; i < feedbackTypes.Count; i++)
            {
                feedbackTypeSql += $",{feedbackTypes[i]}";
            }

            string motorTypeSql = $"{motorTypes[0]}";
            for (int i = 1; i < motorTypes.Count; i++)
            {
                motorTypeSql += $",{motorTypes[i]}";
            }

            string sql =
                $"select * from MotorSearchView where DriveTypeID = {driveTypeId} and CipID in ({feedbackTypeSql}) and MotorTypeID in ({motorTypeSql})";

            return DoQuery<MotorSearchView>(sql, ConnectionString);
        }

        public IEnumerable<MotorFamilySearchView> GetSupportMotorFamilies(int driveTypeId)
        {
            string sql = $"select * from MotorFamilySearchView where DriveTypeID = {driveTypeId}";
            return DoQuery<MotorFamilySearchView>(sql, ConnectionString);
        }

        public Motor GetBaseMotorParameters(int motorId)
        {
            string sql = $"select * from Motor where ID = {motorId}";
            return DoQuery<Motor>(sql, ConnectionString).SingleOrDefault();
        }

        // for test
        public IEnumerable<Motor> GetAllMotors()
        {
            string sql = "select * from Motor";
            return DoQuery<Motor>(sql, ConnectionString);
        }
        // end test


        public PMRotaryMotor GetPMRotaryMotorParameters(int motorId)
        {
            string sql = $"select * from PMRotaryMotor where MotorID = {motorId}";
            return DoQuery<PMRotaryMotor>(sql, ConnectionString).SingleOrDefault();
        }

        public FeedbackDeviceView GetFeedbackDeviceParameters(int feedbackDeviceId)
        {
            string sql = $"select * from FeedbackDeviceView where ID = {feedbackDeviceId}";
            return DoQuery<FeedbackDeviceView>(sql, ConnectionString).SingleOrDefault();
        }

        public int GetInterpolationFactor(int feedbackTypeId, int driveTypeId)
        {
            string sql =
                $"select InterpolationFactor from Interpolation where FeedbackTypeID = {feedbackTypeId} and DriveTypeID = {driveTypeId}";
            int interpolationFactor = DoQuery<int>(sql, ConnectionString).SingleOrDefault();

            if (interpolationFactor == 0)
                interpolationFactor = 1;

            return interpolationFactor;
        }

        public IEnumerable<ShuntView> GetSupportExternalShunt(int driveId)
        {
            string sql = $"select * from ShuntView where DriveID = {driveId}";
            return DoQuery<ShuntView>(sql, ConnectionString);
        }
    }

}
