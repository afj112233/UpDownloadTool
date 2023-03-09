using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties
{
    public class Checking
    {
        public static int CheckModuleName(IController controller, string moduleName)
        {
            int result = 0;
            string warningReason = string.Empty;

            if (string.IsNullOrEmpty(moduleName))
            {
                result = -1;
                warningReason = "Invalid Name.";
            }

            if (result == 0)
            {
                Debug.Assert(moduleName != null, nameof(moduleName) + " != null");

                if (moduleName.Length > 40 || moduleName.EndsWith("_") ||
                    moduleName.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    result = -1;
                    warningReason = "Invalid Name.";
                }
            }

            if (result == 0)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                // ReSharper disable once AssignNullToNotNullAttribute
                if (!regex.IsMatch(moduleName))
                {
                    result = -1;
                    warningReason = "Invalid Name.";
                }
            }

            // key word
            if (result == 0)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or",
                    "ABS","SQRT",
                    "LOG","LN",
                    "DEG","RAD","TRN",
                    "ACS","ASN","ATN","COS","SIN","TAN"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                    {
                        result = -1;
                        warningReason = "Invalid Name.";
                    }
                }
            }

            if (result == 0)
            {
                var deviceModule = controller.DeviceModules[moduleName];
                if (deviceModule != null)
                {
                    if (string.Equals(deviceModule.Name, moduleName, StringComparison.OrdinalIgnoreCase))
                    {
                        result = -1;
                        warningReason = "Duplicate Name.";
                    }
                }

                //if (deviceModule != null)
                //{
                //    result = -1;
                //    warningReason = "Duplicate Name.";
                //}
            }

            if (result < 0)
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier(warningReason), "ICS Studio",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return result;
        }

        public static int CheckIPAddress(string ipAddress, IController controller, IDeviceModule originalMotionDrive)
        {
            const string pattern =
                "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\." +
                "([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\." +
                "([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\." +
                "([01]?\\d\\d?|2[0-4]\\d|25[0-5])$";

            int result = 0;
            string warningReason = string.Empty;

            Regex regex = new Regex(pattern);
            if (string.IsNullOrWhiteSpace(ipAddress) || !regex.IsMatch(ipAddress))
            {
                result = -1;
                warningReason = "Invalid IP Address.";
            }

            Debug.Assert(controller != null && originalMotionDrive != null);
            if (result != -1)
            {
                Debug.Assert(ipAddress != null, nameof(ipAddress) + " != null");

                foreach (var deviceModule in controller.DeviceModules)
                {
                    if (result == -1 || deviceModule.Name == "Local" || deviceModule.Name == "Discrete_IO") continue;
                    if (deviceModule is CommunicationsAdapter)
                    {
                        if (ipAddress.Equals(
                                ((CommunicationsAdapter)deviceModule).GetFirstPort(PortType.Ethernet)?.Address,
                                StringComparison.OrdinalIgnoreCase) && deviceModule != originalMotionDrive)
                        {
                            result = -1;
                            warningReason = "Duplicate IP Address.";
                        }
                    }
                    else
                    {
                        if (ipAddress.Equals(((DeviceModule)deviceModule).Ports[0]?.Address,
                                StringComparison.OrdinalIgnoreCase) && deviceModule != originalMotionDrive)
                        {
                            result = -1;
                            warningReason = "Duplicate IP Address.";
                        }
                    }

                }
            }

            if (result < 0)
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier(warningReason), "ICS Studio",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return result;
        }

        public static int CheckModuleAddress(IController controller, string address)
        {
            //TODO(gjc): add code here
            return 0;
        }
    }
}
