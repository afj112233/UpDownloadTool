using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Linq;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Utils;

namespace ICSStudio.FileConverter.L5XToJson.Objects
{
    public class MessageParameters
    {
        public static readonly string[] StringParameters =
        {
            "RemoteElement", "ConnectionPath",
            "DHPlusDestinationLink", "DHPlusDestinationNode",
            "LocalElement", "DestinationTag",
        };

        public static readonly string[] EnumParameters =
        {
            "MessageType", "ConnectedFlag", "CommTypeCode"
        };

        public static readonly string[] BoolParameters =
        {
            "RequestedLengthSpecified", "ConnectedFlagSpecified", "CommTypeCodeSpecified",
            "TargetObjectSpecified", "ChannelSpecified", "DHPlusSourceLinkSpecified",
            "RackSpecified", "GroupSpecified", "SlotSpecified",
            "LocalIndexSpecified", "RemoteIndexSpecified", "LargePacketUsageSpecified",
            "LargePacketUsage", "CacheConnections"
        };

        public static string[] IntParameters =
        {
            "Channel", "Rack", "Group", "Slot",
            "RequestedLength", "DHPlusSourceLink",
            "TargetObject", "LocalIndex", "RemoteIndex",
            "ServiceCode", "ObjectType", "AttributeNumber"
        };

        public static JObject ToJObject(XmlElement parameters)
        {
            JObject messageParameters = new JObject();

            List<string> attributeNames = new List<string>();
            foreach (XmlAttribute attribute in parameters.Attributes)
            {
                attributeNames.Add(attribute.Name);
            }

            foreach (var attributeName in attributeNames)
            {
                messageParameters.Add(attributeName,
                    GetValue(attributeName, parameters.Attributes[attributeName].Value));
            }

            //
            if (!messageParameters.ContainsKey("UnconnectedTimeout"))
            {
                messageParameters.Add("UnconnectedTimeout", 30000000);
            }

            if (!messageParameters.ContainsKey("ConnectionRate"))
            {
                messageParameters.Add("ConnectionRate", 7500000);
            }

            if (!messageParameters.ContainsKey("TimeoutMultiplier"))
            {
                messageParameters.Add("TimeoutMultiplier", 0);
            }

            return messageParameters;
        }

        private static int EnumParser(string name, string value)
        {
            switch (name)
            {
                case "MessageType":
                    return (int) EnumUtils.Parse<MessageTypeEnum>(value);
                case "ConnectedFlag": // 1,2
                    return int.Parse(value);
                case "CommTypeCode": // MsgDF1FlagEnum, 0,1,2,3,4,5,6
                    return int.Parse(value);
                default:
                    throw new ApplicationException($"add parse for {name}:{value}");
            }
        }

        private static JToken GetValue(string name, string value)
        {
            try
            {
                if (StringParameters.Contains(name))
                    return value;

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

                if (BoolParameters.Contains(name))
                    return bool.Parse(value);

                if (EnumParameters.Contains(name))
                {
                    return EnumParser(name, value);
                }

                throw new NotImplementedException(name);
            }
            catch (Exception)
            {
                Debug.WriteLine($"{name},{value} failed!");
                throw;
            }
        }
    }
}
