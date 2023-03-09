using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using MessagePack;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public enum ElectronicKeyingType
    {
        [EnumMember(Value = "Exact Match")] ExactMatch,

        [EnumMember(Value = "Compatible Module")]
        CompatibleModule,

        [EnumMember(Value = "Disable Keying")] Disabled
    }

    public enum ConnectionType
    {
        [EnumMember(Value = "Motion")] Motion,
    }

    [SuppressMessage("ReSharper", "UseNameofExpression")]
    public class DeviceModule : IDeviceModule
    {
        private string _name;
        private string _description;

        private bool _inhibited;
        private int _major;
        private int _minor;
        private bool _majorFault;

        private short _entryStatus;
        private short _faultCode;
        private int _faultInfo;

        public DeviceModule(IController controller)
        {
            ParentController = controller;
            Uid = Guid.NewGuid().GetHashCode();

            Type = DeviceType.NullType;
        }

        protected JToken ConvertBase64Data(JToken data)
        {
            if (data != null && data.Type == JTokenType.String)
            {
                string s = (string)data;

                if (s != null)
                {
                    var bytes = Convert.FromBase64String(s);

                    string json = MessagePackSerializer.ToJson(bytes);

                    data = JToken.Parse(json);
                    return data;
                }
                
            }

            return data;
        }

        public IDeviceModuleCollection ParentCollection { get; set; }
        public DeviceType Type { get; set; }

        public string DisplayText
        {
            get
            {
                string name = Name ?? string.Empty;

                if (name.Equals("Local"))
                {
                    return $"{CatalogNumber} {ParentController.Name}";
                }

                return $"{CatalogNumber} {name}";
            }
        }

        public IEnumerable<string> ModulePath { get; set; }
        public string CatalogNumber { get; set; }
        public string IconPath { get; set; }

        public bool Inhibited
        {
            get { return _inhibited; }
            set
            {
                _inhibited = value;
                RaisePropertyChanged();
            }
        }

        public int Major
        {
            get { return _major; }
            set
            {
                _major = value;
                RaisePropertyChanged();
            }
        }

        public int Minor
        {
            get { return _minor; }
            set
            {
                _minor = value;
                RaisePropertyChanged();
            }
        }

        public bool MajorFault
        {
            get { return _majorFault; }
            set
            {
                _majorFault = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                OldName = _name;
                _name = value;
                RaisePropertyChanged();
                RaisePropertyChanged("DisplayText");
            }
        }

        public string OldName { private set; get; }

        public List<Port> Ports { get; } = new List<Port>();

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
        }

        public int InstanceNumber { get; set; }
        public bool IsSafety { get; set; }
        public bool IsTypeLess { get; set; }

        public bool IsDescriptionDefaultLocale()
        {
            throw new NotImplementedException();
        }

        public Language[] GetDescriptionTranslations()
        {
            throw new NotImplementedException();
        }

        public int ParentModPortId { get; set; }
        public IDeviceModule ParentModule { get; set; }
        public string ParentModuleName { get; set; }
        public int ProductCode { get; set; }
        public int ProductType { get; set; }
        public int Vendor { get; set; }

        public ElectronicKeyingType EKey { get; set; }
        public ConnectionType Connection { get; set; }

        public ExtendedProperties ExtendedProperties { get; set; }

        public Tag ConfigTag { get; protected set; }
        public Tag InputTag { get; protected set; }
        public Tag OutputTag { get; protected set; }

        public virtual JObject ConvertToJObject()
        {
            JObject module = new JObject
            {
                { "CatalogNumber", CatalogNumber }, { "Inhibited", Inhibited },
                { "Major", Major }, { "MajorFault", MajorFault },
                { "Minor", Minor }, { "Name", Name },
                { "ParentModPortId", ParentModPortId }, { "ParentModule", ParentModule.Name },
                { "ProductCode", ProductCode }, { "ProductType", ProductType },
                { "Vendor", Vendor }, { "EKey", EKey.ToString() }
            };

            // Ports
            JArray ports = new JArray();
            foreach (var port in Ports)
            {
                ports.Add(port.ConvertToJObject());
            }

            module.Add("Ports", ports);
            module.Add("Description", Description);
            return module;
        }

        public virtual void RebuildDeviceTag()
        {
            
        }

        protected void MarkReferenceTagRoutine()
        {
            if (ParentController?.Programs == null)
                return;

            if (ParentController.Programs.Count == 0)
                return;

            foreach (var parentControllerProgram in ParentController.Programs)
            {
                foreach (var routine in parentControllerProgram.Routines)
                {
                    var st = routine as STRoutine;
                    if (st != null)
                    {
                        if (st.GetAllReferenceTags().Any(t => t == InputTag || t == ConfigTag || t == OutputTag))
                        {
                            st.IsError = true;
                        }
                    }
                }
            }
        }

        public virtual void RemoveDeviceTag()
        {

        }

        public virtual void PostLoadJson()
        {

        }

        public void Dispose()
        {
        }

        public IController ParentController { get; set; }
        public int Uid { get; }

        public bool IsVerified { get; set; }
        public bool IsDeleted { get; set; }
        public int ParentProgramUid { get; set; }
        public int ParentRoutineUid { get; set; }

        public short EntryStatus
        {
            get { return _entryStatus; }
            set
            {
                if (_entryStatus != value)
                {
                    _entryStatus = value;
                    RaisePropertyChanged();
                }
            }
        }

        public short FaultCode
        {
            get { return _faultCode; }
            set
            {
                if (_faultCode != value)
                {
                    _faultCode = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int FaultInfo
        {
            get { return _faultInfo; }
            set
            {
                if (_faultInfo != value)
                {
                    _faultInfo = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void BeginTransactionSet()
        {
            throw new NotImplementedException();
        }

        public void EndTransactionSet()
        {
            throw new NotImplementedException();
        }

        public void CancelTransactionSet()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            foreach (string propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        public Port GetFirstPort(PortType portType)
        {
            foreach (var port in Ports)
            {
                if (port.Type == portType)
                    return port;
            }

            return null;
        }

        public List<Port> GetPorts(PortType portType)
        {
            List<Port> ports = new List<Port>();

            foreach (var port in Ports)
            {
                if (port.Type == portType)
                    ports.Add(port);
            }

            return ports.Count > 0 ? ports : null;
        }

        public Port GetPortById(int portId)
        {
            foreach (var port in Ports)
            {
                if (port.Id == portId)
                    return port;
            }

            return null;
        }
    }

    public class ExtendedProperties
    {
        public Dictionary<string, string> Public { get; set; }
    }
}
