using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Descriptor;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public partial class MessageConfigurationViewModel
        : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Tag _tag;

        private bool? _dialogResult;
        private int _selectedIndex;

        private MessageTypeEnum _messageType;
        private CIPServiceTypeEnum _serviceType;

        private string _serviceCode;
        private string _instanceID;
        private string _classID;
        private string _attributeID;

        private string _sourceElement;
        private string _destinationElement;

        private bool _serviceCodeReadOnly;
        private bool _instanceIDReadOnly;
        private bool _classIDReadOnly;
        private bool _attributeIDReadOnly;

        private bool _sourceElementReadOnly;
        private bool _sourceLengthReadOnly;
        private bool _destinationElementReadOnly;
        private short _sourceLength;

        public MessageConfigurationViewModel(ITag tag,string title)
        {
            _tag = tag as Tag;
            Title = title;
            MessageDataWrapper dataWrapper = _tag?.DataWrapper as MessageDataWrapper;

            Contract.Assert(dataWrapper != null);

            CreateMessageTypes();

            ServiceTypes = EnumHelper.ToDataSource<CIPServiceTypeEnum>();

            _messageType = (MessageTypeEnum) dataWrapper.Parameters.MessageType;
            if (_messageType == MessageTypeEnum.Unconfigured)
                _messageType = MessageTypeEnum.CIPGeneric;

            if (_messageType == MessageTypeEnum.CIPGeneric)
            {
                var serviceCode = dataWrapper.ServiceCode;
                var classID = dataWrapper.ClassID;
                var instanceID = dataWrapper.InstanceID;
                var attributeID = dataWrapper.AttributeID;
                var sourceLength = dataWrapper.SourceLength;

                _serviceType = ServiceTypeInfo.GetServiceType(serviceCode, classID, instanceID, attributeID);

                UpdateServiceTypeInfo(_serviceType);

                _serviceCode = serviceCode.ToString("x");
                _classID = classID.ToString("x");
                _instanceID = instanceID.ToString();
                _attributeID = attributeID.ToString("x");

                _sourceLength = sourceLength;

            }
            else
            {
                _serviceType = CIPServiceTypeEnum.Custom;

                UpdateServiceTypeInfo(_serviceType);
            }

            _sourceElement = dataWrapper.SourceElement;
            _destinationElement = dataWrapper.DestinationElement;

            _connectionPath = string.IsNullOrEmpty(dataWrapper.ConnectionPath) ? "THIS" : dataWrapper.ConnectionPath;
            _connected = dataWrapper.Connected;
            if (_connected)
            {
                _largeConnection = dataWrapper.Parameters.LargePacketUsage;
            }

            _tagName = _tag.Name;
            _description = _tag.Description;

            _isConfigurationDirty = false;
            _isCommunicationDirty = false;
            _isTagDirty = false;

            _selectedIndex = 0;

            NewTagCommand = new RelayCommand(ExecuteNewTagCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            OKCommand = new RelayCommand(ExecuteOKCommand);
        }

        public MessageConfigurationViewModel()
        {

        }

        public string Title { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }


        public string ConfigurationHeader =>
            LanguageManager.GetInstance().ConvertSpecifier("Configuration") + (IsConfigurationDirty ? "*" : "");

        private bool _isConfigurationDirty;

        public bool IsConfigurationDirty
        {
            get { return _isConfigurationDirty; }
            set
            {
                if (_isConfigurationDirty != value)
                {
                    _isConfigurationDirty = value;

                    RaisePropertyChanged("ConfigurationHeader");
                    ApplyCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public string CommunicationHeader =>
            LanguageManager.GetInstance().ConvertSpecifier("Communication") + (IsCommunicationDirty ? "*" : "");

        private bool _isCommunicationDirty;

        public bool IsCommunicationDirty
        {
            get { return _isCommunicationDirty; }
            set
            {
                if (_isCommunicationDirty != value)
                {
                    _isCommunicationDirty = value;

                    RaisePropertyChanged("CommunicationHeader");
                    ApplyCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public string TagHeader =>
            LanguageManager.GetInstance().ConvertSpecifier("Variable") + (IsTagDirty ? "*" : "");

        private bool _isTagDirty;

        public bool IsTagDirty
        {
            get { return _isTagDirty; }
            set
            {
                if (_isTagDirty != value)
                {
                    _isTagDirty = value;

                    RaisePropertyChanged("TagHeader");
                    ApplyCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public IList MessageTypes { get; private set; }

        public MessageTypeEnum MessageType
        {
            get { return _messageType; }
            set
            {
                if (_messageType != value)
                {
                    Set(ref _messageType, value);

                    IsConfigurationDirty = true;
                }

            }
        }

        public IList ServiceTypes { get; }

        public CIPServiceTypeEnum ServiceType
        {
            get { return _serviceType; }
            set
            {
                if (_serviceType != value)
                {
                    Set(ref _serviceType, value);

                    UpdateServiceTypeInfo(_serviceType);

                    IsConfigurationDirty = true;
                }
            }
        }

        public string ServiceCode
        {
            get { return _serviceCode; }
            set
            {
                if (_serviceCode != value)
                {
                    Set(ref _serviceCode, value);

                    IsConfigurationDirty = true;
                }

            }
        }

        public string InstanceID
        {
            get { return _instanceID; }
            set
            {
                if (_instanceID != value)
                {
                    Set(ref _instanceID, value);

                    IsConfigurationDirty = true;
                }

            }
        }

        public string ClassID
        {
            get { return _classID; }
            set
            {
                if (_classID != value)
                {
                    Set(ref _classID, value);

                    IsConfigurationDirty = true;
                }

            }
        }

        public string AttributeID
        {
            get { return _attributeID; }
            set
            {
                if (_attributeID != value)
                {
                    Set(ref _attributeID, value);

                    IsConfigurationDirty = true;
                }

            }
        }

        public bool ServiceCodeReadOnly
        {
            get { return _serviceCodeReadOnly; }
            set { Set(ref _serviceCodeReadOnly, value); }
        }

        public bool InstanceIDReadOnly
        {
            get { return _instanceIDReadOnly; }
            set { Set(ref _instanceIDReadOnly, value); }
        }

        public bool ClassIDReadOnly
        {
            get { return _classIDReadOnly; }
            set { Set(ref _classIDReadOnly, value); }
        }

        public bool AttributeIDReadOnly
        {
            get { return _attributeIDReadOnly; }
            set { Set(ref _attributeIDReadOnly, value); }
        }

        public bool SourceElementReadOnly
        {
            get { return _sourceElementReadOnly; }
            set { Set(ref _sourceElementReadOnly, value); }
        }

        public bool SourceLengthReadOnly
        {
            get { return _sourceLengthReadOnly; }
            set { Set(ref _sourceLengthReadOnly, value); }
        }

        public bool DestinationElementReadOnly
        {
            get { return _destinationElementReadOnly; }
            set { Set(ref _destinationElementReadOnly, value); }
        }

        public short SourceLength
        {
            get { return _sourceLength; }
            set
            {
                if (_sourceLength != value)
                {
                    Set(ref _sourceLength, value);
                    IsConfigurationDirty = true;
                }

            }
        }

        public string SourceElement
        {
            get { return _sourceElement; }
            set
            {
                if (_sourceElement != value)
                {
                    Set(ref _sourceElement, value);
                    IsConfigurationDirty = true;
                }

            }
        }

        public string DestinationElement
        {
            get { return _destinationElement; }
            set
            {
                if (_destinationElement != value)
                {
                    Set(ref _destinationElement, value);
                    IsConfigurationDirty = true;
                }

            }
        }

        private void CreateMessageTypes()
        {
            MessageTypes = EnumHelper.ToDataSource<MessageTypeEnum>(new List<MessageTypeEnum>()
            {
                MessageTypeEnum.BlockTransferRead,
                MessageTypeEnum.BlockTransferWrite,
                MessageTypeEnum.CIPDataTableRead,
                MessageTypeEnum.CIPDataTableWrite,
                MessageTypeEnum.CIPGeneric,
                MessageTypeEnum.ModuleReconfigure,
                MessageTypeEnum.PLC2UnprotectedRead,
                MessageTypeEnum.PLC2UnprotectedWrite,
                MessageTypeEnum.PLC3TypedRead,
                MessageTypeEnum.PLC3TypedWrite,
                MessageTypeEnum.PLC3WordRangeRead,
                MessageTypeEnum.PLC3WordRangeWrite,
                MessageTypeEnum.PLC5TypedRead,
                MessageTypeEnum.PLC5TypedWrite,
                MessageTypeEnum.PLC5WordRangeRead,
                MessageTypeEnum.PLC5WordRangeWrite,
                MessageTypeEnum.SLCTypedRead,
                MessageTypeEnum.SLCTypedWrite
            });
        }

        private void UpdateServiceTypeInfo(CIPServiceTypeEnum serviceType)
        {
            var serviceTypeInfo = ServiceTypeInfo.GetServiceTypeInfo(serviceType);
            if (serviceTypeInfo != null)
            {
                ServiceCodeReadOnly = serviceTypeInfo.ServiceCodeReadOnly;
                ServiceCode = ServiceCodeReadOnly ? serviceTypeInfo.ServiceCode.ToString("x") : "";

                ClassIDReadOnly = serviceTypeInfo.ClassIDReadOnly;
                ClassID = ClassIDReadOnly ? serviceTypeInfo.ClassID.ToString("x") : "";

                InstanceIDReadOnly = serviceTypeInfo.InstanceIDReadOnly;
                InstanceID = InstanceIDReadOnly ? serviceTypeInfo.InstanceID.ToString() : "";

                AttributeIDReadOnly = serviceTypeInfo.AttributeIDReadOnly;
                AttributeID = AttributeIDReadOnly ? serviceTypeInfo.AttributeID.ToString("x") : "";

                SourceLength = serviceTypeInfo.SourceLength;
                SourceLengthReadOnly = serviceTypeInfo.SourceLengthReadOnly;

                SourceElementReadOnly = serviceTypeInfo.SourceElementReadOnly;
                if (SourceElementReadOnly)
                {
                    SourceElement = string.Empty;
                }

                DestinationElementReadOnly = serviceTypeInfo.DestinationElementReadOnly;
                if (DestinationElementReadOnly)
                {
                    DestinationElement = string.Empty;
                }
            }
        }

        private void ExecuteNewTagCommand()
        {
            //TODO(gjc): open config failed!
            var viewModel = new NewTagViewModel(
                "Dint", _tag.ParentCollection,
                Usage.NullParameterType,
                null, string.Empty, true);
            NewTagDialog dialog = new NewTagDialog(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            dialog.ShowDialog();
        }

        #region Communication

        private string _connectionPath;

        public string ConnectionPath
        {
            get { return _connectionPath; }
            set
            {
                if (_connectionPath != value)
                {
                    Set(ref _connectionPath, value);
                    IsCommunicationDirty = true;
                }

            }
        }

        //TODO(gjc):
        private bool _connected;

        public bool Connected
        {
            get { return _connected; }
            set
            {
                Set(ref _connected, value);

                if (!_connected)
                {
                    _largeConnection = false;
                }

                RaisePropertyChanged("CacheConnectionsEnabled");
                RaisePropertyChanged("LargeConnectionEnabled");

                RaisePropertyChanged("CacheConnections");
                RaisePropertyChanged("LargeConnection");

                IsCommunicationDirty = true;
            }
        }

        public bool CacheConnections
        {
            get
            {
                if (!_connected)
                    return false;

                string value = _tag.GetMemberValue("EN_CC", true);
                return bool.Parse(value);
            }
            set { _tag.SetStringValue("EN_CC", value.ToString()); }
        }

        public bool CacheConnectionsEnabled => _connected;

        private bool _largeConnection;

        public bool LargeConnection
        {
            get { return _connected && _largeConnection; }
            set { Set(ref _largeConnection, value); }
        }

        public bool LargeConnectionEnabled => _connected;

        #endregion

        #region Tag

        private string _tagName;

        public string TagName
        {
            get { return _tagName; }
            set
            {
                if (_tagName != value)
                {
                    Set(ref _tagName, value);
                    IsTagDirty = true;
                }

            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    Set(ref _description, value);
                    IsTagDirty = true;
                }

            }
        }

        public string Scope => _tag.ParentController.Name;

        #endregion

        #region Bottom

        public bool Enable
        {
            get
            {
                string value = _tag.GetMemberValue("EN", true);
                return bool.Parse(value);
            }
        }

        public bool EnableWaiting
        {
            get
            {
                string value = _tag.GetMemberValue("EW", true);
                return bool.Parse(value);
            }
        }

        public bool Start
        {
            get
            {
                string value = _tag.GetMemberValue("ST", true);
                return bool.Parse(value);
            }
        }

        public bool Done
        {
            get
            {
                string value = _tag.GetMemberValue("DN", true);
                return bool.Parse(value);
            }
        }

        public string DoneLength => _tag.GetMemberValue("DN_LEN", true);

        public bool Error
        {
            get
            {
                string value = _tag.GetMemberValue("ER", true);
                return bool.Parse(value);
            }
        }

        public string ErrorCode
        {
            get
            {
                string value = _tag.GetMemberValue("ERR", true);
                ushort errorCode = ushort.Parse(value);
                if (errorCode == 0)
                    return string.Empty;

                return $"16#{errorCode:x4}";
            }
        }

        public string ExtendedErrorCode
        {
            get
            {
                string value = _tag.GetMemberValue("EXERR", true);
                uint extendedErrorCode = uint.Parse(value);

                if (extendedErrorCode == 0)
                    return string.Empty;

                string temp = $"{extendedErrorCode:x8}";
                temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");

                return "16#" + temp;
            }
        }

        //TO
        public bool TimedOut
        {
            get
            {
                string value = _tag.GetMemberValue("TO", true);
                return bool.Parse(value);
            }
            set { _tag.SetStringValue("TO", value.ToString()); }
        }

        public string ErrorPath
        {
            get
            {
                MessageDataWrapper dataWrapper = _tag?.DataWrapper as MessageDataWrapper;
                Contract.Assert(dataWrapper != null);

                return dataWrapper.ConnectionPath;
            }
        }

        public string ErrorText
        {
            get
            {
                string value = _tag.GetMemberValue("ERR", true);
                ushort errorCode = ushort.Parse(value);
                if (errorCode == 0)
                    return string.Empty;

                return SocketErrorCodeDescriptor.GetErrorText(errorCode);
            }
        }

        #endregion

        #region Command

        public RelayCommand NewTagCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand OKCommand { get; }

        private bool CanExecuteApplyCommand()
        {
            return _isConfigurationDirty || _isCommunicationDirty || _isTagDirty;
        }

        private void ExecuteApplyCommand()
        {
            DoApply();
        }

        private void ExecuteOKCommand()
        {
            if (CanExecuteApplyCommand())
            {
                if (DoApply() == 0)
                {
                    DialogResult = true;
                }

            }
            else
            {
                DialogResult = true;
            }
        }

        private int DoApply()
        {
            // check
            int result = CheckInput();
            if (result < 0)
                return result;

            // apply
            ApplyInput();

            // update
            IsConfigurationDirty = false;
            IsCommunicationDirty = false;
            IsTagDirty = false;

            RaisePropertyChanged("Title");
            RaisePropertyChanged("ErrorPath");

            return 0;
        }

        private void ApplyInput()
        {
            MessageDataWrapper dataWrapper = _tag?.DataWrapper as MessageDataWrapper;
            Contract.Assert(dataWrapper != null);

            dataWrapper.Parameters.MessageType = (byte) _messageType;
            dataWrapper.ServiceCode = ushort.Parse(_serviceCode, NumberStyles.HexNumber);
            dataWrapper.ClassID = ushort.Parse(_classID, NumberStyles.HexNumber);
            dataWrapper.InstanceID = uint.Parse(_instanceID);
            dataWrapper.AttributeID = ushort.Parse(_attributeID, NumberStyles.HexNumber);

            dataWrapper.SourceElement = _sourceElement;
            dataWrapper.SourceLength = _sourceLength;
            dataWrapper.DestinationElement = _destinationElement;

            dataWrapper.ConnectionPath = _connectionPath;

            _tag.Name = _tagName;
            _tag.Description = _description;

            //TODO(gjc): for connected, edit later
            Connected = false;
            dataWrapper.Connected = _connected;
            dataWrapper.Parameters.LargePacketUsage = _largeConnection;

            //TODO(gjc): add code here
        }

        #endregion
    }
}
