using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ICSStudio.Cip.Objects;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.L5XToJson.Objects
{
    public class AxisVirtualParameters
    {
        public static string[] StringParameters = {"MotionGroup", "PositionUnits",};

        public static string[] EnumParameters =
            {"RotaryAxis", "HomeMode", "HomeDirection", "HomeSequence", "ProgrammedStopMode", "AxisUpdateSchedule"};

        public static string[] IntParameters =
        {
            "OutputCamExecutionTargets", "PositionUnwind", "HomeConfigurationBits", "MasterInputConfigurationBits",
            "DynamicsConfigurationBits", "InterpolatedPositionConfiguration"
        };

        public static string[] FloatParameters =
        {
            "ConversionConstant", "AverageVelocityTimebase", "HomePosition", "HomeOffset", "MaximumSpeed",
            "MaximumAcceleration", "MaximumDeceleration", "MasterPositionFilterBandwidth", "MaximumAccelerationJerk",
            "MaximumDecelerationJerk"
        };
        public static JObject ToJObject(XmlElement parameters)
        {
            JObject axisVirtualParameters = new JObject();

            List<string> attributeNames = new List<string>();
            foreach (XmlAttribute attribute in parameters.Attributes)
            {
                attributeNames.Add(attribute.Name);
            }

            attributeNames.Sort();

            foreach (var attributeName in attributeNames)
            {
                axisVirtualParameters.Add(attributeName,GetValue(attributeName, parameters.Attributes[attributeName].Value));
            }

            return axisVirtualParameters;
        }
        private static JToken GetValue(string name, string value)
        {
            try
            {
                if (StringParameters.Contains(name))
                    return value;
                if (FloatParameters.Contains(name))
                    return float.Parse(value);
                if (IntParameters.Contains(name))
                {
                    if (value.StartsWith("16#"))
                    {
                        value = value.Replace("16#", "");
                        value = value.Replace("_", "");
                        return Convert.ToInt32(value, 16);
                    }

                    uint result;
                    if (uint.TryParse(value, out result))
                        return result;

                    return int.Parse(value);
                }
                
                if (EnumParameters.Contains(name))
                {
                    return EnumParser(name, value);
                }
              
                throw new ApplicationException(name);
            }
            catch (Exception)
            {
                Console.WriteLine($"{name},{value} failed!");
                throw;
            }
        }

        private static int EnumParser(string name, string value)
        {
            switch (name)
            {
                case "AxisUpdateSchedule":
                    return (int)EnumUtils.Parse<AxisUpdateScheduleType>(value);
             
                case "HomeDirection":
                    {
                        switch (value)
                        {
                            case "Uni-directional Forward":
                                return 0;
                            case "Bi-directional Forward":
                                return 1;
                            case "Uni-directional Reverse":
                                return 2;
                            case "Bi-directional Reverse":
                                return 3;
                            default:
                                throw new ApplicationException($"Parse {name}:{value} failed!");
                        }
                    }
                case "HomeMode":
                    return (int)EnumUtils.Parse<HomeModeType>(value);
                case "HomeSequence":
                    return (int)EnumUtils.Parse<HomeSequenceType>(value);
              
                case "ProgrammedStopMode":
                    return (int)EnumUtils.Parse<ProgrammedStopModeType>(value);
                case "RotaryAxis":
                    return (int) EnumUtils.Parse<SimpleServices.DataWrapper.AxisVirtualParameters.PositioningMode>(value);

                default:
                    throw new ApplicationException($"add parse for {name}:{value}");
            }
        }
    }
}
