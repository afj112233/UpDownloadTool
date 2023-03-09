using System.Collections.Generic;
using ICSStudio.DeviceProfiles.Common;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class Feedback
    {
        public List<FeedbackPort> Ports { get; set; }
        public List<FeedbackDevicePort> DevicePorts { get; set; }
        public List<FeedbackDevice> Devices { get; set; }

        public FeedbackDevice GetDeviceByPortNumber(int number)
        {
            // number -> cardtype -> device

            if (Devices == null)
                return null;

            int cardType = GetCardTypeByPortNumber(number);
            if (cardType < 0)
                return null;

            return GetDeviceByCardType(cardType);
        }

        private FeedbackDevice GetDeviceByCardType(int cardType)
        {
            if (Devices == null)
                return null;

            foreach (var device in Devices)
            {
                if (device.CardType == cardType)
                    return device;
            }

            return null;
        }

        private int GetCardTypeByPortNumber(int number)
        {
            if (Devices == null)
                return -1;

            foreach (var devicePort in DevicePorts)
            {
                if (devicePort.Number == number)
                    return devicePort.DefaultCardType;
            }

            return -1;
        }
    }

    public class FeedbackPort
    {
        public int Number { get; set; }
        public bool Hidden { get; set; }
        public List<Description> Description { get; set; }
    }

    public class FeedbackDevicePort
    {
        public int Number { get; set; }
        public int FeedbackPortNumber { get; set; }
        public int DefaultCardType { get; set; }
    }

    public class FeedbackDevice
    {
        public int CardType { get; set; }
        public int Channels { get; set; }
        public string CatalogNumber { get; set; }

        public List<SupportedValue<string>> FeedbackTypes { get; set; }

        public int RegistrationInputs { get; set; }

        public List<string> AxisFeatures { get; set; }

        public List<string> HomeSequence { get; set; }
    }
}
