using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using ICSStudio.Cip;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Notification;
using Newtonsoft.Json;

namespace ICSStudio.IoConfigTest
{
    class Program
    {
        // ReSharper disable once InconsistentNaming
        private const string KL5XFile = "l18_adapter_slot1.L5X";
        private const string KJsonFile = "l18_adapter_slot1.json";

        private const string KAdapterFolder = @"adapter\";
        private const string KDiscreteIOFolder = @"discrete_io\";

        static void Main()
        {
            CreateAdapterFiles();

            CreateDiscreteIOFiles();
        }

        private static void CreateDiscreteIOFiles()
        {
            Converter.L5XToJson(KL5XFile, KJsonFile);
            var controller = Controller.Open(KJsonFile);

            CommunicationsAdapter adapter = controller.DeviceModules["adapter"] as CommunicationsAdapter;

            var allConnectionConfigID = adapter?.Profiles.GetConnectionConfigIDListByMajor(adapter.Major);
            if (allConnectionConfigID == null)
                return;

            foreach (uint connectionConfigID in allConnectionConfigID)
            {
                controller = Controller.Open(KJsonFile);
                adapter = controller.DeviceModules["adapter"] as CommunicationsAdapter;
                if (adapter != null)
                {
                    //string connection =
                    //    adapter.Profiles.GetConnectionStringByConfigID(connectionConfigID, adapter.Major);
                    adapter.ChangeConnectionConfigID(connectionConfigID);

                    // add io module
                    adapter.ChassisSize = 10;

                    AddDiscreteIO(controller, adapter, PortType.PointIO, "1734-IB4");
                    AddDiscreteIO(controller, adapter, PortType.PointIO, "1734-IB8");
                    AddDiscreteIO(controller, adapter, PortType.PointIO, "1734-OB4");
                    AddDiscreteIO(controller, adapter, PortType.PointIO, "1734-OB8");
                    AddDiscreteIO(controller, adapter, PortType.PointIO, "ICD-IB16");
                    AddDiscreteIO(controller, adapter, PortType.PointIO, "ICD-OB16");

                    SaveAllDiscreteIODataType(controller.DeviceModules.OfType<DiscreteIO>().ToList(),
                        KDiscreteIOFolder);
                }
            }
        }

        private static void SaveAllDiscreteIODataType(List<DiscreteIO> discreteIOList, string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (DiscreteIO discreteIO in discreteIOList)
            {
                if (discreteIO.ConfigTag != null)
                {
                    SaveDataType(discreteIO.ConfigTag.DataTypeInfo.DataType as ModuleDefinedDataType, folder);
                }

                if (discreteIO.InputTag != null)
                {
                    SaveDataType(discreteIO.InputTag.DataTypeInfo.DataType as ModuleDefinedDataType, folder);
                }

                if (discreteIO.OutputTag != null)
                {
                    SaveDataType(discreteIO.OutputTag.DataTypeInfo.DataType as ModuleDefinedDataType, folder);
                }
            }

        }

        private static void AddDiscreteIO(Controller controller, CommunicationsAdapter adapter, PortType portType,
            string catalogNumber)
        {
            DeviceModuleFactory factory = new DeviceModuleFactory();
            var discreteIO =
                factory.Create(CipDeviceType.GeneralPurposeDiscreteIO, catalogNumber) as DiscreteIO;

            if (discreteIO == null)
                return;

            discreteIO.ParentModule = adapter;
            var parentPort = adapter.GetFirstPort(portType);
            discreteIO.ParentModPortId = parentPort.Id;

            // slot
            discreteIO.Slot = GetMinUnusedSlot(controller, discreteIO.ParentModule, portType);

            // connection
            uint connectionConfigID = GetMatchConnectionConfigID(discreteIO);
            discreteIO.ChangeConnectionConfigID(connectionConfigID);

            DeviceModuleCollection deviceModules = controller.DeviceModules as DeviceModuleCollection;
            Contract.Assert(deviceModules != null);

            discreteIO.ParentController = controller;
            deviceModules.AddDeviceModule(discreteIO);

            discreteIO.RebuildDeviceTag();

            if (discreteIO.IsEnhancedRack)
            {
                adapter.RebuildDeviceTag();
            }

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }

        private static void CreateAdapterFiles()
        {
            Converter.L5XToJson(KL5XFile, KJsonFile);
            var controller = Controller.Open(KJsonFile);

            CommunicationsAdapter adapter = controller.DeviceModules["adapter"] as CommunicationsAdapter;

            if (adapter != null)
            {
                for (int slot = 1; slot <= 64; slot++)
                {
                    adapter.ChassisSize = slot;

                    SaveAdapterDataType(adapter, KAdapterFolder);
                }
            }

        }

        private static void SaveAdapterDataType(CommunicationsAdapter adapter, string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (adapter.ConfigTag != null)
            {
                SaveDataType(adapter.ConfigTag.DataTypeInfo.DataType as ModuleDefinedDataType, folder);
            }

            if (adapter.InputTag != null)
            {
                SaveDataType(adapter.InputTag.DataTypeInfo.DataType as ModuleDefinedDataType, folder);
            }

            if (adapter.OutputTag != null)
            {
                SaveDataType(adapter.OutputTag.DataTypeInfo.DataType as ModuleDefinedDataType, folder);
            }
        }

        private static void SaveDataType(ModuleDefinedDataType dataType, string folder)
        {
            if (dataType == null)
                return;

            string file = $"{folder}{dataType.Name}.json";
            file = file.Replace(":", "_");

            using (var sw = File.CreateText(file))
            using (var jw = new JsonTextWriter(sw))
            {
                Console.WriteLine(file);
                jw.Formatting = Formatting.Indented;
                dataType.ConvertToJObject().WriteTo(jw);
            }
        }

        private static int GetMinUnusedSlot(IController controller, IDeviceModule parentModule, PortType portType)
        {
            List<int> usedSlotList = new List<int>();

            foreach (var module in controller.DeviceModules)
            {
                DeviceModule deviceModule = module as DeviceModule;
                if (deviceModule != null && deviceModule != parentModule && deviceModule.ParentModule == parentModule)
                {
                    var port = deviceModule.GetFirstPort(portType);
                    if (port != null && port.Upstream)
                    {
                        usedSlotList.Add(int.Parse(port.Address));
                    }
                }
            }

            usedSlotList.Sort();

            int minSlot = 1;

            foreach (var slot in usedSlotList)
            {
                if (minSlot < slot)
                    break;

                minSlot++;
            }

            return minSlot;
        }

        private static uint GetMatchConnectionConfigID(DiscreteIO discreteIO)
        {
            var allConnectionConfigID = discreteIO.Profiles.GetConnectionConfigIDListByMajor(discreteIO.Major);

            if (discreteIO.ParentModule is LocalModule)
                return allConnectionConfigID[0];

            CommunicationsAdapter adapter =
                discreteIO.ParentModule as CommunicationsAdapter;
            if (adapter != null)
            {
                //uint parentConnectionConfigID = adapter.ExtendedProperties.Public.ConfigID;

                //uint mask = parentConnectionConfigID & (uint) (DIOConnectionTypeMask.ListenOnlyRack
                //                                               | DIOConnectionTypeMask.Rack
                //                                               | DIOConnectionTypeMask.EnhancedRack);

                //foreach (var i in allConnectionConfigID)
                //{
                //    if ((i & mask) > 0)
                //        return i;
                //}
            }

            return allConnectionConfigID[0];
        }
    }
}
