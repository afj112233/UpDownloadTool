using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.DeviceModule
{
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    public class LocalModule : DeviceModule
    {
        public LocalModule(IController controller, string catalogNumber) : base(controller)
        {
            Name = "Local";
            ParentModule = this;

            PropertyChangedEventManager.AddHandler(controller, OnPropertyChanged, "");

            // TODO(gjc): need edit here
            // load default port setting

            if (catalogNumber == "1769-L18ERM-BB1B"
                || catalogNumber == "ICC-B010ERM"
               )
            {
                var port = new Port
                {
                    Address = "0",
                    Id = 1,
                    Type = PortType.PointIO,
                    Upstream = false,
                    Bus = new Bus { Size = 2 }
                };

                Ports.Add(port);

                port = new Port
                {
                    Id = 2,
                    Type = PortType.Ethernet,
                    Upstream = false
                };

                Ports.Add(port);
            }
            else if (catalogNumber == "ICC-P0100ERM"|| catalogNumber == "ICC-P010ERM"|| catalogNumber == "ICC-P020ERM"|| catalogNumber == "ICC-T0100ERM" )
            {
                var port = new Port
                {
                    Address = "0",
                    Id = 1,
                    Type = PortType.PointIO,
                    Upstream = false,
                    Bus = new Bus { Size = 2 }
                };

                Ports.Add(port);

                if (controller.EtherNetIPMode != "A1/A2: Dual-IP")
                {
                    Ports.Add(new Port
                    {
                        Id = 2,
                        Type = PortType.Ethernet,
                        Upstream = false
                    });
                }
                else
                {
                    Ports.Add(new Port
                    {
                        Id = 3,
                        Type = PortType.Ethernet,
                        Upstream = false
                    });

                    Ports.Add(new Port
                    {
                        Id = 4,
                        Type = PortType.Ethernet,
                        Upstream = false
                    });
                }
            }
            else if (catalogNumber == "1769-L36ERM")
            {
                var port = new Port
                {
                    Address = "0",
                    Id = 1,
                    Type = PortType.Compact,
                    Upstream = false,
                    Bus = new Bus { Size = 31 }
                };

                Ports.Add(port);

                port = new Port
                {
                    Id = 2,
                    Type = PortType.Ethernet,
                    Upstream = false
                };

                Ports.Add(port);
            }
            else if (catalogNumber == "1769-L33ERM")
            {
                var port = new Port
                {
                    Address = "0",
                    Id = 1,
                    Type = PortType.Compact,
                    Upstream = false,
                    Bus = new Bus { Size = 17 }
                };

                Ports.Add(port);

                port = new Port
                {
                    Id = 2,
                    Type = PortType.Ethernet,
                    Upstream = false
                };

                Ports.Add(port);
            }
            else if (catalogNumber == "5069-L306ERM"
                     || catalogNumber == "5069-L310ERM"
                     || catalogNumber == "5069-L320ERM"
                     || catalogNumber == "5069-L330ERM"
                     || catalogNumber == "5069-L340ERM"
                     || catalogNumber == "5069-L350ERM"
                     || catalogNumber == "5069-L380ERM"
                     || catalogNumber == "5069-L3100ERM")
            {
                //TODO(gjc): need check later
                if (controller.EtherNetIPMode != "A1/A2: Dual-IP")
                {
                    var port = new Port
                    {
                        Id = 2,
                        Type = PortType.Ethernet,
                        Upstream = false
                    };

                    Ports.Add(port);
                }
                else
                {
                    Ports.Add(new Port
                    {
                        Id = 3,
                        Type = PortType.Ethernet,
                        Upstream = false
                    });

                    Ports.Add(new Port
                    {
                        Id = 4,
                        Type = PortType.Ethernet,
                        Upstream = false
                    });
                }

            }
            else if (catalogNumber == "1756-L81E"
                     || catalogNumber == "1756-L82E"
                     || catalogNumber == "1756-L83E"
                     || catalogNumber == "1756-L84E"
                     || catalogNumber == "1756-L85E")
            {
                var port = new Port
                {
                    Address = "0",
                    Id = 1,
                    Type = PortType.ICP,
                    Upstream = false,
                    Bus = new Bus { Size = 10 }
                };

                Ports.Add(port);

                port = new Port
                {
                    Id = 2,
                    Type = PortType.Ethernet,
                    Upstream = false
                };

                Ports.Add(port);
            }
            else
            {
                throw new NotSupportedException($"Not support  {catalogNumber}!");
            }

            //1769 - L18ERM

            //1769 - L36ERM

            //1756 - L81E
            //1756 - L82E
            //1756 - L83E
            //1756 - L84E
            //1756 - L85E

            //5069 - L306ERM
            //5069 - L3100ERM
            //5069 - L310ERM
            //5069 - L320ERM
            //5069 - L330ERM
            //5069 - L340ERM
            //5069 - L350ERM
            //5069 - L380ERM

            //else
            //{
            //    var port = new Port()
            //    {
            //        Id = 1,
            //        Type = PortType.Ethernet,
            //        Upstream = false
            //    };

            //    Ports.Add(port);
            //}
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                RaisePropertyChanged("DisplayText");
            }
        }
    }
}
