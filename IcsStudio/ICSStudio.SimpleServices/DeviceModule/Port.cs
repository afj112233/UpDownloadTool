using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Gui.Annotations;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public enum PortType
    {
        Ethernet = 1,
        PointIO = 2,
        Compact = 3, //1769 Bus
        ICP = 4,
    }

    public class Port : INotifyPropertyChanged
    {
        private string _address;

        public string Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Id { get; set; }
        public PortType Type { get; set; }
        public bool Upstream { get; set; }

        public Bus Bus { get; set; }

        public JObject ConvertToJObject()
        {
            JObject port = new JObject();

            if (!(Type == PortType.Ethernet && !Upstream))
                port.Add("Address", Address);

            port.Add("Id", Id);
            port.Add("Type", Type.ToString());
            port.Add("Upstream", Upstream);

            if (Upstream == false)
            {
                if (Bus == null)
                    Bus = new Bus();

                port.Add("Bus", JToken.FromObject(Bus));
            }


            return port;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
