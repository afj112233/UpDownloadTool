using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Gui.Annotations;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    public class ParameterRow : INotifyPropertyChanged
    {
        private string _dataType;
        private string _externalAccessDisplay;
        private ExternalAccess _externalAccess;
        private string _displayUsage;
        private Usage _usage;
        private Visibility _expanderVisibility = Visibility.Collapsed;
        private string _name;
        private bool _isConstant;
        private string _description = "";
        private bool _sequencing;
        private Visibility _errorVisibility = Visibility.Collapsed;
        private Visibility _penVisibility = Visibility.Visible;
        private bool _isCreate;
        private string _connectionInfo;
        private bool _updateConnectionRows;
        public List<ConnectionInfo> ConnectionRows { set; get; } = new List<ConnectionInfo>();

        public ParameterRow(TempScope tempScope, ITag tag, ParameterRow parent,
            List<ConnectionInfo> parameterConnectionList,
            JArray descriptionField, bool isMember = true, bool isNew = false)
        {
            TempScope = tempScope;
            Tag = tag;
            Parent = parent;
            if (isNew)
            {
                ParameterConnectionList = new List<ConnectionInfo>();
            }
            else
            {
                ParameterConnectionList = parameterConnectionList ?? new List<ConnectionInfo>();
            }

            IsMember = isMember;
            IsNew = isNew;
            IsNeedSetDefault = isNew;
            DescriptionField = descriptionField;
            var desc = SimpleServices.Tags.Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, DescriptionField,
                SimpleServices.Tags.Tag.GetOperand(GetFullName()));
            _description = desc;
            SourceDescription = desc;
            AddListen();
        }

        public ParameterRow GetParameterRowBySpecial(string special)
        {
            var name = GetFullName();
            if (special.StartsWith(name, StringComparison.OrdinalIgnoreCase))
            {
                if (special.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return this;
                }
                else
                {
                    if (!Children.Any())
                        GetChildren();
                    foreach (var parameterRow in Children)
                    {
                        var row = parameterRow.GetParameterRowBySpecial(special);
                        if (row == null) continue;
                        else return row;
                    }
                }
            }

            return null;
        }

        public ParameterRow GetBaseParameterRow()
        {
            if (Parent != null)
            {
                return Parent.GetBaseParameterRow();
            }

            return this;
        }

        public void Clean()
        {
            RemoveListen();
            foreach (var connectionInfo in ConnectionRows)
            {
                connectionInfo.Clean();
            }
        }

        public TempScope TempScope { get; }

        public void AddListen()
        {
            ((Tag) Tag).UpdateConnection += ParameterRow_UpdateConnection;
        }

        public void RemoveListen()
        {
            ((Tag) Tag).UpdateConnection -= ParameterRow_UpdateConnection;
        }

        private void ParameterRow_UpdateConnection(object sender, EventArgs e)
        {
            SetParameterConnectionList();
        }

        protected void GetConnectionInfo()
        {
            int count = 0;
            int subCount = 0;
            var special = GetFullName();
            foreach (var connectionInfo in ParameterConnectionList)
            {
                if (!connectionInfo.IsCreate) continue;
                var name = connectionInfo.Parent.GetFullName();
                if (name.StartsWith(special, StringComparison.OrdinalIgnoreCase))
                {
                    if (name.Length > special.Length)
                    {
                        subCount++;
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            if (count == 1 && subCount == 0)
            {
                ConnectionInfo = ParameterConnectionList[0].Name;
                Parent?.GetConnectionInfo();
            }
            else if (subCount + count == 0)
            {
                ConnectionInfo = "";
            }
            else
            {
                ConnectionInfo = "{" + count + ":" + subCount + "} Connections";
                Parent?.GetConnectionInfo();
            }
        }

        public string ConnectionInfo
        {
            set
            {
                if ("0".Equals(Name) && "dint".Equals(Tag?.Name))
                {

                }

                _connectionInfo = value;
                var f = IsNeedSetDefault;
                IsNeedSetDefault = false;
                OnPropertyChanged();
                IsNeedSetDefault = f;
            }
            get { return _connectionInfo; }
        }

        public ParameterRow Parent { get; }

        public string GetFullName()
        {
            if (Parent == null)
            {
                return $"\\{Tag.ParentCollection.ParentProgram.Name}.{Name}";
            }
            else
            {
                return $"{Parent.GetFullName()}.{Name}";
            }
        }

        public JArray DescriptionField { get; }

        public string SourceDescription { set; get; } = "";

        protected bool SetDescriptionField { set; get; } = true;

        public void UpdateDescription()
        {
            var desc = SimpleServices.Tags.Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo,
                IsMember ? DescriptionField : ((Tag) Tag).ChildDescription,
                SimpleServices.Tags.Tag.GetOperand(GetFullName()));
            if (SourceDescription.Equals(Description))
            {
                Description = desc;
                SourceDescription = desc;
                foreach (var parameterRow in Children)
                {
                    parameterRow.SetDescriptionField = false;
                    parameterRow.UpdateDescription();
                    parameterRow.SetDescriptionField = true;
                }
            }
        }

        public List<ParameterRow> Children { get; } = new List<ParameterRow>();

        public void SetParameterConnectionList()
        {
            var parameterConnections =
                Tag.ParentController.ParameterConnections.GetTagParameterConnections(Tag)?.ToList();
            if (parameterConnections?.Any() ?? false)
            {
                foreach (ParameterConnection parameterConnection in parameterConnections)
                {
                    if (ConnectionRows.Any(c => c.ParameterConnection == parameterConnection)) continue;
                    var name = parameterConnection.SourcePath.StartsWith(GetFullName())
                        ? parameterConnection.SourcePath
                        : parameterConnection.DestinationPath;
                    if (name.Equals(GetFullName(), StringComparison.OrdinalIgnoreCase))
                        AddConnection(parameterConnection);
                }

                if ((Usage == Usage.Output || Usage == Usage.SharedData) && ConnectionRows.Any() &&
                    !string.IsNullOrEmpty(ConnectionRows.Last()?.UsageDisplay))
                {
                    ConnectionRows.Add(new ConnectionInfo(TempScope, this));
                }
            }
            else
            {
                if (!ConnectionRows.Any())
                {
                    ConnectionRows.Add(new ConnectionInfo(TempScope, this));
                }
            }

            GetConnectionInfo();
            UpdateConnectionRows = true;
        }

        public ParameterRow GetChild(string name)
        {
            Debug.Assert(Children.Any());
            if (!Children.Any()) GetChildren();
            if (GetFullName().Equals(name, StringComparison.OrdinalIgnoreCase)) return this;
            foreach (var parameterRow in Children)
            {
                var fullName = parameterRow.GetFullName();
                if (name.StartsWith(fullName, StringComparison.OrdinalIgnoreCase))
                {
                    if (name.Equals(fullName, StringComparison.OrdinalIgnoreCase))
                    {
                        return parameterRow;
                    }

                    return parameterRow.GetChild(name);
                }
            }

            return null;
        }

        public void OnMonitor(ConnectionInfo connectionInfo)
        {
            connectionInfo.PropertyChanged += ConnectionInfo_PropertyChanged;
        }

        private void ConnectionInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var info = (ConnectionInfo) sender;
            if (e.PropertyName == "IsCreate")
            {
                if (ParameterConnectionList.Any(c => c.ParameterConnection == info.ParameterConnection)) return;
                ParameterConnectionList.Add(info);
                if (Usage == Usage.Output || Usage == Usage.SharedData)
                {
                    var connInfo = new ConnectionInfo(TempScope, this);
                    ConnectionRows.Insert(ConnectionRows.Count, connInfo);
                }

                GetConnectionInfo();
                UpdateConnectionRows = true;
                return;
            }

            if (e.PropertyName == "IsDelete")
            {
                ParameterConnectionList.Remove(info);
                OffMonitor(info);
                info.RemoveReferenceParameterConnection(info, true);
                if (info.ParameterConnection != null)
                    DeleteParameterConnections.Add(info.ParameterConnection);
                GetConnectionInfo();
                return;
            }

        }

        public bool UpdateConnectionRows
        {
            set
            {
                var f = IsNeedSetDefault;
                IsNeedSetDefault = false;
                _updateConnectionRows = value;
                OnPropertyChanged();
                IsNeedSetDefault = f;
            }
            get { return _updateConnectionRows; }
        }

        public List<ParameterConnection> DeleteParameterConnections { get; } = new List<ParameterConnection>();

        public void OffMonitor(ConnectionInfo connectionInfo)
        {
            connectionInfo.PropertyChanged -= ConnectionInfo_PropertyChanged;
            connectionInfo.Clean();
        }

        public List<ConnectionInfo> ParameterConnectionList { get; }

        public void GetChildren()
        {
            DataTypeInfo dataTypeInfo = Controller.GetInstance().DataTypes.ParseDataTypeInfo(DataType);
            if (dataTypeInfo.DataType == null || (dataTypeInfo.DataType.IsBool && dataTypeInfo.Dim1 == 0)) return;
            if (Enumerable.Any(Children)) return;
            int dim = dataTypeInfo.Dim1;
            IDataType dataType = dataTypeInfo.DataType;
            if (dim > 0)
            {
                ExpandDataTypeArray(this);
            }
            else
            {
                if (dataType is CompositiveType)
                {
                    foreach (var m in (dataType as CompositiveType).TypeMembers)
                    {
                        ParameterRow row = new ParameterRow(TempScope, Tag, this, ParameterConnectionList,
                            DescriptionField);

                        row.Name = m.Name;
                        row.Usage = Usage;
                        row.DataType = m.DataTypeInfo.ToString().Replace(":SINT", "");
                        row.Sequencing = Sequencing;
                        row.BaseTag = BaseTag;
                        row.ExternalAccess = m.ExternalAccess;
                        row.IsConstant = IsConstant;
                        row.SetParameterConnectionList();
                        Children.Add(row);
                    }
                }
                else
                {
                    int size = dataType.BitSize;
                    for (int i = 0; i < size; i++)
                    {
                        ParameterRow row = new ParameterRow(TempScope, Tag, this, ParameterConnectionList,
                            DescriptionField);
                        row.ExpanderVisibility = Visibility.Collapsed;
                        row.Name = $"{i}";
                        row.Usage = Usage;
                        row.DataType = "BOOL";
                        row.Sequencing = Sequencing;
                        row.BaseTag = BaseTag;
                        row.ExternalAccess = ExternalAccess;
                        row.IsConstant = IsConstant;
                        row.SetParameterConnectionList();
                        Children.Add(row);
                    }
                }
            }
        }

        private void ExpandDataTypeArray(ParameterRow parameterRow)
        {
            DataTypeInfo dataTypeInfo = Controller.GetInstance().DataTypes.ParseDataTypeInfo(parameterRow.DataType);
            string dataTypeString = parameterRow.DataType.IndexOf("[") > 0
                ? parameterRow.DataType.Substring(0, parameterRow.DataType.IndexOf("["))
                : parameterRow.DataType;
            var tag = (Tag) parameterRow.Tag;
            if (dataTypeInfo.Dim3 > 0)
            {
                for (int i = 0; i < dataTypeInfo.Dim3; i++)
                {
                    for (int j = 0; j < dataTypeInfo.Dim2; j++)
                    {
                        for (int k = 0; k < dataTypeInfo.Dim1; k++)
                        {
                            ParameterRow row = new ParameterRow(TempScope, parameterRow.Tag, this,
                                ParameterConnectionList,
                                DescriptionField);
                            row.Name = $"[{i},{j},{k}]";
                            row.Usage = parameterRow.Usage;
                            row.DataType = dataTypeString;
                            row.Sequencing = parameterRow.Sequencing;
                            row.BaseTag = parameterRow.BaseTag;
                            row.ExternalAccess = parameterRow.ExternalAccess;
                            row.IsConstant = parameterRow.IsConstant;
                            row.SetParameterConnectionList();
                            Children.Add(row);
                        }
                    }
                }
            }
            else if (dataTypeInfo.Dim2 > 0)
            {
                for (int j = 0; j < dataTypeInfo.Dim2; j++)
                {
                    for (int k = 0; k < dataTypeInfo.Dim1; k++)
                    {
                        ParameterRow row = new ParameterRow(TempScope, parameterRow.Tag, this, ParameterConnectionList,
                            DescriptionField);
                        row.Name = $"[{j},{k}]";
                        row.Usage = parameterRow.Usage;
                        row.DataType = dataTypeString;
                        row.Sequencing = parameterRow.Sequencing;
                        row.BaseTag = parameterRow.BaseTag;
                        row.ExternalAccess = parameterRow.ExternalAccess;
                        row.IsConstant = parameterRow.IsConstant;
                        row.SetParameterConnectionList();
                        Children.Add(row);
                    }
                }
            }
            else if (dataTypeInfo.Dim1 > 0)
            {
                for (int k = 0; k < dataTypeInfo.Dim1; k++)
                {
                    ParameterRow row = new ParameterRow(TempScope, parameterRow.Tag, this, ParameterConnectionList,
                        DescriptionField);
                    row.Name = $"[{k}]";
                    row.Usage = parameterRow.Usage;
                    row.DataType = dataTypeString;
                    row.Sequencing = parameterRow.Sequencing;
                    row.BaseTag = parameterRow.BaseTag;
                    row.ExternalAccess = parameterRow.ExternalAccess;
                    row.IsConstant = parameterRow.IsConstant;
                    row.SetParameterConnectionList();
                    Children.Add(row);
                }
            }
        }

        public void DeleteConnection(ConnectionInfo connectionInfo, bool needDel)
        {
            if (connectionInfo == null || connectionInfo.ErrorVisibility == Visibility.Visible) return;
            ConnectionRows.Remove(connectionInfo);
            connectionInfo.IsDelete = needDel;
            if (connectionInfo.ReferenceConnectionInfo != null)
            {
                connectionInfo.Parent.CheckAllConnection();
                connectionInfo.ReferenceConnectionInfo.Parent.CheckAllConnection();
                connectionInfo.ReferenceConnectionInfo.ReferenceConnectionInfo = null;
                connectionInfo.ReferenceConnectionInfo = null;
            }

            if (ConnectionRows.Count == 0)
            {
                ConnectionRows.Add(new ConnectionInfo(TempScope, this));
                if (Usage == Usage.InOut)
                {
                    ErrorVisibility = Visibility.Visible;
                    Parent?.CheckChildren();
                }
            }

            UpdateConnectionRows = true;
            if (connectionInfo.ErrorVisibility == Visibility.Visible || ErrorVisibility == Visibility.Visible)
                if (Parent == null)
                {
                    CheckAllConnection();
                }
                else
                {
                    Parent?.CheckAllConnection();
                }
        }

        public void DeleteConnection(ParameterConnection conn)
        {
            var connectionInfo = ConnectionRows.FirstOrDefault(c => c.ParameterConnection == conn);
            Debug.Assert(connectionInfo != null);
            ConnectionRows.Remove(connectionInfo);
            connectionInfo.IsDelete = true;
            connectionInfo.Clean();
            if (ConnectionRows.Count == 0)
            {
                ConnectionRows.Add(new ConnectionInfo(TempScope, this));
            }

            UpdateConnectionRows = true;
            if (connectionInfo.ErrorVisibility == Visibility.Visible)
                Parent.CheckConnection();
        }

        public void AddConnection(ParameterConnection conn)
        {
            if (ConnectionRows.Any(c => c.ParameterConnection == conn)) return;
            if (Usage == Usage.InOut || Usage == Usage.Input)
            {
                if (ConnectionRows.Any() && !ConnectionRows.Last().IsCreate)
                {
                    ConnectionRows[0].Name = conn.SourcePath.Equals(GetFullName(), StringComparison.OrdinalIgnoreCase)
                        ? conn.DestinationPath
                        : conn.SourcePath;
                    ConnectionRows[0].ParameterConnection = conn;
                }
                else
                {
                    var connInfo = new ConnectionInfo(TempScope, this, conn);
                }

                CheckConnection();
            }
            else
            {
                var connInfo = new ConnectionInfo(TempScope, this, conn);

                //ParameterConnectionList.Add(connInfo);
                CheckConnection();
            }
        }

        public void AddConnection(ConnectionInfo referenceConnectionInfo, bool needAddReferenceParameterConnection)
        {
            var parameterRow = referenceConnectionInfo.Parent;
            if (ConnectionRows.Any(c => c.ReferenceConnectionInfo == referenceConnectionInfo)) return;
            if (Usage == Usage.InOut || Usage == Usage.Input)
            {
                if (ConnectionRows.Any() && !ConnectionRows.Last().IsCreate)
                {
                    var connInfo = ConnectionRows.Last();
                    connInfo.NeedAddReferenceParameterConnection = needAddReferenceParameterConnection;
                    connInfo.ReferenceConnectionInfo = referenceConnectionInfo;
                    referenceConnectionInfo.ReferenceConnectionInfo = connInfo;
                    connInfo.ParameterConnection = referenceConnectionInfo.ParameterConnection;
                    connInfo.Name = parameterRow.GetFullName();
                    connInfo.NeedAddReferenceParameterConnection = true;
                }
                else
                {
                    var connInfo = new ConnectionInfo(TempScope, this);
                    connInfo.NeedAddReferenceParameterConnection = needAddReferenceParameterConnection;
                    connInfo.ReferenceConnectionInfo = referenceConnectionInfo;
                    referenceConnectionInfo.ReferenceConnectionInfo = connInfo;
                    connInfo.ParameterConnection = referenceConnectionInfo.ParameterConnection;
                    if (ConnectionRows.Any())
                    {
                        var index = ConnectionRows.Count -
                                    (string.IsNullOrEmpty(ConnectionRows.Last()?.UsageDisplay) ? 1 : 0);
                        ConnectionRows.Insert(index, connInfo);

                        //ParameterConnectionList.Add(ConnectionRows[index]);
                    }
                    else
                    {
                        ConnectionRows.Add(connInfo);
                        //ParameterConnectionList.Add(ConnectionRows.Last());
                    }

                    connInfo.Name = parameterRow.GetFullName();
                    connInfo.NeedAddReferenceParameterConnection = true;
                }

                CheckConnection();
            }
            else
            {
                var connInfo = new ConnectionInfo(TempScope, this);
                connInfo.NeedAddReferenceParameterConnection = needAddReferenceParameterConnection;
                connInfo.ParameterConnection = referenceConnectionInfo.ParameterConnection;
                connInfo.ReferenceConnectionInfo = referenceConnectionInfo;
                referenceConnectionInfo.ReferenceConnectionInfo = connInfo;
                if (ConnectionRows.Any())
                {
                    ConnectionRows.Insert(
                        ConnectionRows.Count - (string.IsNullOrEmpty(ConnectionRows.Last()?.UsageDisplay) ? 1 : 0),
                        connInfo);
                }
                else
                {
                    ConnectionRows.Add(connInfo);
                }

                connInfo.Name = parameterRow.GetFullName();
                connInfo.NeedAddReferenceParameterConnection = true;
                //ParameterConnectionList.Add(connInfo);
                CheckConnection();
            }
        }

        public bool IsMember { get; }

        public string OldName { private set; get; }

        public string Name
        {
            set
            {
                if (_name != null && _name.Equals(value)) return;
                if (!(_name?.Equals(value, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OldName = _name;
                    _name = value;
                    if (!IsMember)
                    {
                        Regex regex = new Regex(@"^([A-Za-z]([0-9]+)?)+((_[a-zA-Z0-9]+)*|[a-zA-Z0-9]*)$");
                        if (_name == null || !regex.IsMatch(_name))
                        {
                            ErrorVisibility = Visibility.Visible;
                        }
                    }

                    PenVisibility = Visibility.Collapsed;
                    OnPropertyChanged();
                    TempScope.CheckErrorParameter();
                }
                else
                {
                    PenVisibility = Visibility.Visible;
                }
            }
            get { return _name; }
        }

        public ITag Tag { get; }

        private bool _needVerityConnections = false;

        public Usage Usage
        {
            set
            {
                _usage = value;
                var attribute =
                    Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                DisplayUsage = attribute?.Value == "ShareData" ? "Public" : attribute?.Value;
                if (_needVerityConnections)
                {
                    if (value == Usage.Input || value == Usage.InOut)
                    {
                        if (ConnectionRows.Count > 1)
                        {
                            var connection = ConnectionRows[ConnectionRows.Count - 1];
                            if (!connection.IsCreate)
                            {
                                ConnectionRows.Remove(connection);
                                UpdateConnectionRows = true;
                            }
                        }
                    }
                    else
                    {
                        var connection = ConnectionRows[ConnectionRows.Count - 1];
                        if (connection.IsCreate)
                        {
                            ConnectionRows.Add(new ConnectionInfo(TempScope, this));
                            UpdateConnectionRows = true;
                        }
                    }

                    VerityConnections();
                }
                else
                {
                    _needVerityConnections = true;
                }

                OnPropertyChanged();
            }
            get
            {
                if (Parent != null)
                    return GetBaseParameterRow().Usage;
                return _usage;
            }
        }

        internal void VerityConnections()
        {
            if (ParameterConnectionList.Any())
            {
                foreach (var connectionInfo in ParameterConnectionList)
                {
                    connectionInfo.Verify();
                }
            }
            else
            {
                if (Usage == Usage.InOut)
                {
                    if (!ParameterConnectionList.Any())
                    {
                        ErrorVisibility = Visibility.Visible;
                    }
                }
                else
                {
                    ErrorVisibility = Visibility.Collapsed;
                }
            }
        }

        public bool Sequencing
        {
            set
            {
                _sequencing = value;
                OnPropertyChanged();
            }
            get { return _sequencing; }
        }

        public string DataType
        {
            set
            {
                _dataType = value;
                Children.Clear();
                int dim = 0;
                if (string.IsNullOrEmpty(_dataType))
                {
                    ErrorVisibility = Visibility.Visible;
                }

                DataTypeInfo dataTypeInfo = Controller.GetInstance().DataTypes.ParseDataTypeInfo(value);
                dim = dataTypeInfo.Dim1;
                if (dim > 0 || (!(dataTypeInfo.DataType is BOOL || dataTypeInfo.DataType is REAL) &&
                                dataTypeInfo.DataType != null))
                {
                    ExpanderVisibility = Visibility.Visible;
                }
                else
                {
                    ExpanderVisibility = Visibility.Collapsed;
                }

                if (_needGetChildren)
                    GetChildren();
                OnPropertyChanged();
            }
            get { return _dataType; }
        }

        public string DisplayUsage
        {
            set
            {
                _displayUsage = value;
                OnPropertyChanged();
            }
            get
            {
                if (Parent != null)
                    return GetBaseParameterRow().DisplayUsage;
                return _displayUsage;
            }
        }

        public Visibility ErrorVisibility
        {
            set
            {
                if (_errorVisibility != value)
                {
                    _errorVisibility = value;
                    OnPropertyChanged();
                    if (_errorVisibility == Visibility.Visible && (Usage == Usage.InOut || Usage == Usage.Input))
                    {
                        foreach (var connectionInfo in ConnectionRows)
                        {
                            connectionInfo.ErrorVisibility = Visibility.Visible;
                        }
                    }

                    CheckConnectionReferenceParameter(this);
                }
            }
            get { return _errorVisibility; }
        }

        private bool _isNeedCheckParameterConnection = true;

        private void CheckConnectionReferenceParameter(ParameterRow parameterRow)
        {
            if (parameterRow == this) return;
            foreach (var connectionInfo in ConnectionRows)
            {

                if (connectionInfo.ReferenceConnectionInfo?.Parent != null)
                {
                    connectionInfo.ReferenceConnectionInfo?.Parent?.CheckConnectionReferenceParameter(parameterRow);
                    foreach (var conn in connectionInfo.ReferenceConnectionInfo?.Parent.ConnectionRows)
                    {
                        if (conn.IsCreate)
                        {
                            conn.Verify();
                        }
                    }
                }

            }
        }

        public Visibility PenVisibility
        {
            set
            {
                if (_penVisibility != value)
                {
                    _penVisibility = value;
                    OnPropertyChanged();
                }
            }
            get { return _penVisibility; }
        }

        public string ExternalAccessDisplay
        {
            set
            {
                _externalAccessDisplay = value;
                OnPropertyChanged();
            }
            get { return _externalAccessDisplay; }
        }

        public string AliasFor { set; get; } = "";
        public string BaseTag { set; get; } = "";

        public string Description
        {
            set
            {
                _description = value;
                if (SetDescriptionField)
                {
                    var operand = SimpleServices.Tags.Tag.GetOperand(GetFullName());
                    var exist = DescriptionField.FirstOrDefault(c => operand.Equals((string) c["Operand"]));
                    if (exist != null)
                    {
                        exist["Value"] = value;
                    }
                    else
                    {
                        var obj = new JObject();
                        obj["Operand"] = operand;
                        obj["Value"] = value;
                        DescriptionField.Add(obj);
                    }
                }

                OnPropertyChanged();
            }
            get { return _description; }
        }

        public ExternalAccess ExternalAccess
        {
            set
            {
                _externalAccess = value;
                ExternalAccessDisplay = value.ToString();
                OnPropertyChanged();
            }
            get { return _externalAccess; }
        }

        public bool IsConstant
        {
            set
            {
                _isConstant = value;
                OnPropertyChanged();
            }
            get { return _isConstant; }
        }

        public bool IsNew { set; get; } = true;

        public bool IsNeedSetDefault { set; get; }

        public string Connection { set; get; }

        public Visibility ExpanderVisibility
        {
            set
            {
                _expanderVisibility = value;
                OnPropertyChanged();
            }
            get { return _expanderVisibility; }
        }

        public void ApplyChanges()
        {
            Tag tag = (Tag) Tag;
            tag.Name = Name;
            tag.Usage = Usage;
            tag.IsSequencing = Sequencing;
            DataTypeInfo dataTypeInfo = Controller.GetInstance().DataTypes.ParseDataTypeInfo(DataType);
            tag.DataWrapper = new DataWrapper(dataTypeInfo.DataType, dataTypeInfo.Dim1, dataTypeInfo.Dim2,
                dataTypeInfo.Dim3, null);
            tag.DisplayStyle = dataTypeInfo.DataType.DefaultDisplayStyle;
            //TODO(ZYL):alias for
            tag.ChildDescription = DescriptionField;
            tag.Description = Description;
            tag.ExternalAccess = ExternalAccess;
            tag.IsConstant = IsConstant;
        }

        public bool IsVerifying = false;

        public void CheckConnection()
        {
            if (IsVerifying) return;
            IsVerifying = true;
            CheckChildren();
            GetConnectionInfo();
            Parent?.CheckChildren();
            IsVerifying = false;
        }

        protected void CheckAllConnection()
        {
            if (!_isNeedCheckParameterConnection) return;
            _isNeedCheckParameterConnection = false;
            ErrorVisibility = Visibility.Collapsed;
            foreach (var connectionInfo in ConnectionRows)
            {
                if (connectionInfo.IsCreate)
                {
                    connectionInfo.Verify();
                    connectionInfo.ReferenceConnectionInfo?.Parent.CheckAllConnection();
                }
            }

            CheckConnection();
            _isNeedCheckParameterConnection = true;

        }

        protected void CheckChildren()
        {
            if (Children.Any(c => c.ErrorVisibility == Visibility.Visible) || ConnectionRows.Any(c =>
                    c.ErrorVisibility == Visibility.Visible ||
                    c.ReferenceConnectionInfo?.ErrorVisibility == Visibility.Visible))
            {
                ErrorVisibility = Visibility.Visible;
            }
            else
            {
                if (Usage == Usage.InOut)
                {
                    if (!ParameterConnectionList.Any())
                    {
                        ErrorVisibility = Visibility.Visible;
                    }
                }
                else
                {
                    ErrorVisibility = Visibility.Collapsed;
                }
            }
        }

        public bool IsCreate
        {
            set
            {
                _isCreate = value;
                OnPropertyChanged();
            }
            get { return _isCreate; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (IsNeedSetDefault)
            {
                IsNeedSetDefault = false;
                SetDefault();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _needGetChildren = true;

        private void SetDefault()
        {
            if (DisplayUsage == null)
            {
                Usage = Usage.Input;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Usage"));
            }

            if (DataType == null)
            {
                _needGetChildren = false;
                DataType = "DINT";
                _needGetChildren = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DataType"));
            }

            if (Name == null)
            {
                _name = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }

            if (ExternalAccessDisplay == null)
            {
                ExternalAccess = ExternalAccess.ReadWrite;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExternalAccessDisplay"));
            }

            PenVisibility = Visibility.Collapsed;
            IsCreate = true;
        }
    }

    public class ConnectionInfo : INotifyPropertyChanged, IConsumer
    {
        private string _name = "";
        private Usage _usage;
        private string _dataType = "";
        private ExternalAccess _externalAccess;
        private string _description = "";
        private bool _isConstant;
        private Visibility _errorVisibility = Visibility.Collapsed;
        private Visibility _penVisibility = Visibility.Visible;
        private bool _isCreate;
        private bool _isDelete;
        private string _externalAccessDisplay = "";
        private string _usageDisplay;
        private bool _isDirty;
        private Tag _tag;
        private TempScope _tempScope;
        private ParameterRow _referenceParameterRow;

        public ConnectionInfo(TempScope tempScope, ParameterRow parent)
        {
            _tempScope = tempScope;
            Parent = parent;
            parent?.OnMonitor(this);
            Notifications.ConnectConsumer(this);
        }

        public ConnectionInfo(TempScope tempScope, ParameterRow parent, ParameterConnection connection)
        {
            _tempScope = tempScope;
            Parent = parent;
            ParameterConnection = connection;
            parent.OnMonitor(this);
            Notifications.ConnectConsumer(this);
            var special = parent.GetFullName();

            if (parent.ConnectionRows.Any())
            {
                var index = parent.ConnectionRows.Count -
                            (string.IsNullOrEmpty(parent.ConnectionRows.Last()?.UsageDisplay) ? 1 : 0);
                parent.ConnectionRows.Insert(index, this);

                //ParameterConnectionList.Add(ConnectionRows[index]);
            }
            else
            {
                parent.ConnectionRows.Add(this);
                //ParameterConnectionList.Add(ConnectionRows.Last());
            }

            Name = ParameterConnection?.SourcePath == special
                ? ParameterConnection?.DestinationPath
                : ParameterConnection?.SourcePath;
        }

        public ConnectionInfo ReferenceConnectionInfo { set; get; }

        public void Clean()
        {
            Notifications.DisconnectConsumer(this);
        }

        public void AdviseConnectionParameter()
        {
            Tag?.UpdateConnectionEvent();
        }

        public bool IsNeedVerify { set; get; } = true;

        public Tag Tag
        {
            private set
            {
                if (_tag != null)
                    _tag.PropertyChanged -= _tag_PropertyChanged;
                _tag = value;
                if (_tag != null)
                    _tag.PropertyChanged += _tag_PropertyChanged;
            }
            get { return _tag; }
        }

        public ParameterRow ReferenceParameterRow
        {
            set
            {
                if (_referenceParameterRow != null)
                    _referenceParameterRow.PropertyChanged -= _referenceParameterRow_PropertyChanged;
                ;
                _referenceParameterRow = value;
                if (_referenceParameterRow != null)
                    _referenceParameterRow.PropertyChanged += _referenceParameterRow_PropertyChanged;
                ;
            }
            get { return _referenceParameterRow; }
        }

        private void _referenceParameterRow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                var old =
                    $"\\{_referenceParameterRow.Tag.ParentCollection.ParentProgram.Name}.{_referenceParameterRow.OldName}";
                var n =
                    $"\\{_referenceParameterRow.Tag.ParentCollection.ParentProgram.Name}.{_referenceParameterRow.Name}";
                _name = _name.Replace(old, n);
                OnPropertyChanged("Name");
            }

            if (e.PropertyName == "Usage")
            {
                Usage = _referenceParameterRow.Usage;
                Verify();
                return;
            }

            if (e.PropertyName == "DataType")
            {
                DataType = _referenceParameterRow.DataType;
                Verify();
                return;
            }

            if (e.PropertyName == "Description")
            {
                Description = Tag.GetChildDescription(_referenceParameterRow.Description,
                    Controller.GetInstance().DataTypes.ParseDataTypeInfo(_referenceParameterRow.DataType),
                    _referenceParameterRow.DescriptionField,
                    Tag.GetOperand(_referenceParameterRow.GetFullName()));
            }
        }

        private void _tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                var extendedEventArgs = e as PropertyChangedExtendedEventArgs<string>;
                if (extendedEventArgs != null)
                {
                    var old = _tag.ParentCollection.ParentProgram == null
                        ? extendedEventArgs.OldValue
                        : $"\\{_tag.ParentCollection.ParentProgram.Name}.{extendedEventArgs.OldValue}";
                    var n = _tag.ParentCollection.ParentProgram == null
                        ? extendedEventArgs.NewValue
                        : $"\\{_tag.ParentCollection.ParentProgram.Name}.{extendedEventArgs.NewValue}";
                    _name = _name.Replace(old, n);
                    OnPropertyChanged("Name");
                    return;
                }
            }

            if (e.PropertyName == "Usage")
            {
                Usage = _tag.Usage;
                Verify();
                return;
            }

            if (e.PropertyName == "DataWrapper")
            {
                DataType = _tag.DataTypeInfo.ToString();
                Verify();
                return;
            }

            if (e.PropertyName == "Description")
            {
                Description = Tag.GetChildDescription(_tag.Description, _tag.DataTypeInfo, _tag.ChildDescription,
                    Tag.GetOperand(Name));
            }
        }

        public ParameterConnection ParameterConnection { get; internal set; }
        public ParameterRow Parent { get; }

        public Visibility PenVisibility
        {
            set
            {
                _penVisibility = value;
                OnPropertyChanged();
            }
            get { return _penVisibility; }
        }

        internal void RemoveReferenceParameterConnection(ConnectionInfo info, bool needDel)
        {
            Debug.Assert(Tag == null || ReferenceParameterRow == null);
            if (Tag != null)
            {

            }

            if (ReferenceParameterRow != null)
            {
                var connectionInfo =
                    ReferenceParameterRow.ConnectionRows.FirstOrDefault(c => c.ReferenceConnectionInfo == info);
                ReferenceParameterRow.DeleteConnection(connectionInfo, needDel);
            }
        }

        internal void AddReferenceParameterConnection()
        {
            if (!NeedAddReferenceParameterConnection) return;
            if (Tag != null)
            {

            }

            if (ReferenceParameterRow != null)
            {
                if (Parent == ReferenceParameterRow) return;
                ReferenceParameterRow.AddConnection(this, false);
                ReferenceParameterRow?.CheckConnection();
            }
        }

        public bool NeedAddReferenceParameterConnection { set; get; } = true;

        internal void GetInfo()
        {
            if (string.IsNullOrEmpty(Name)) return;
            try
            {
                RemoveReferenceParameterConnection(this, false);
                _isCreate = false;
                Tag = null;
                ReferenceParameterRow = null;
                if (Name.StartsWith($"\\{Parent.Tag.ParentCollection.ParentProgram.Name}",
                    StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var row in _tempScope.GetParameterRows())
                    {
                        var referenceRow = row.GetParameterRowBySpecial(Name);
                        if (referenceRow != null)
                        {
                            ReferenceParameterRow = referenceRow;
                            Usage = referenceRow.Usage;
                            DataType = referenceRow.DataType;
                            Description = Tag.GetChildDescription(row.Description,
                                Controller.GetInstance().DataTypes.ParseDataTypeInfo(row.DataType),
                                row.DescriptionField,
                                Tag.GetOperand(_referenceParameterRow.GetFullName()));
                            ExternalAccess = row.ExternalAccess;
                            IsConstant = row.IsConstant;
                            break;
                        }
                    }

                    if (ReferenceParameterRow == null)
                    {
                        UsageDisplay = "";
                        DataType = "";
                        Description = "";
                        ExternalAccessDisplay = "";
                        IsConstant = false;
                    }
                }
                else
                {
                    var info = ObtainValue.NameToTag(Name, null);
                    var astName = ObtainValue.GetLoadTag(Name, null, null);
                    Usage = info.Item1.Usage;
                    DataType = astName.Expr.type is ArrayType ? astName.Expr.type.ToString() : astName.Expr.type.Name;
                    if (info.Item1 != null)
                    {
                        Description = Tag.GetChildDescription(info.Item1.Description, info.Item1.DataTypeInfo,
                            ((Tag) info.Item1).ChildDescription, Tag.GetOperand(Name));

                        if (info.Item1.ParentCollection.ParentProgram == null)
                            UsageDisplay = "<controller>";
                        Tag = (Tag) info.Item1;
                        ExternalAccess = info.Item1.ExternalAccess;
                        IsConstant = info.Item1.IsConstant;
                    }
                    else
                    {
                        UsageDisplay = "";
                        DataType = "";
                        Description = "";
                        ExternalAccessDisplay = "";
                        IsConstant = false;
                    }
                }

                Verify();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorVisibility = Visibility.Visible;
            }
        }

        internal void Verify()
        {
            if (!IsNeedVerify) return;
            if (Parent.Usage == Usage.Input || Parent.Usage == Usage.InOut)
            {
                if (Parent.ConnectionRows.Count(c => c.IsCreate && c != this) > 0)
                {
                    ErrorVisibility = Visibility.Visible;
                    return;
                }
            }

            if (ReferenceParameterRow != null)
            {
                //if (ReferenceParameterRow?.ErrorVisibility == Visibility.Visible&&)
                //{
                //    ErrorVisibility = Visibility.Visible;
                //    return;
                //}
                var existConnections = Parent.ConnectionRows.Where(c =>
                    c != this && c.IsCreate && c.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                if (existConnections.Any())
                {
                    foreach (var existConnection in existConnections)
                    {
                        existConnection.ErrorVisibility = Visibility.Visible;
                    }

                    ErrorVisibility = Visibility.Visible;
                    return;
                }
            }

            var arg = Controller.GetInstance().ParameterConnections.VerifyConnection(Parent.GetFullName(), Parent.Usage,
                Parent.ExternalAccess, Parent.DataType, Name, Usage, ExternalAccess, DataType);
            if (!string.IsNullOrEmpty(arg?.Message))
            {
                if (arg.Message.Contains(" exists."))
                {
                    if (arg.Connection == ParameterConnection)
                    {
                        ErrorVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ErrorVisibility = Visibility.Visible;
                    }
                }
                else
                {
                    ErrorVisibility = Visibility.Visible;
                }
            }
            else
            {
                ErrorVisibility = Visibility.Collapsed;
            }
        }

        public string Name
        {
            set
            {
                if (string.Compare(_name, value, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
                {
                    _name = value;
                    if (ErrorVisibility != Visibility.Visible)
                    {
                        PenVisibility = string.IsNullOrEmpty(_name) ? Visibility.Visible : Visibility.Collapsed;
                    }

                    GetInfo();
                    IsDirty = true;
                    IsCreate = true;
                    OnPropertyChanged();
                }
            }
            get { return _name; }
        }

        public bool IsCreate
        {
            set
            {
                if (_isCreate != value)
                {
                    _isCreate = value;
                    OnPropertyChanged();
                    AddReferenceParameterConnection();
                }
            }
            get { return _isCreate; }
        }

        public bool IsDelete
        {
            set
            {
                if (_isDelete != value)
                {
                    _isDelete = value;
                    OnPropertyChanged();
                }
            }
            get { return _isDelete; }
        }

        public bool IsDirty
        {
            set
            {
                _isDirty = value;
                if (value)
                {
                    var parent = Parent;
                    while (true)
                    {
                        if (parent.Parent != null)
                        {
                            parent = parent.Parent;
                            continue;
                        }

                        break;
                    }

                    parent.Description = parent.Description;
                }
            }
            get { return _isDirty; }
        }

        public Usage Usage
        {
            set
            {
                _usage = value;
                UsageDisplay = _usage == Usage.SharedData ? "Public" : _usage.ToString();
            }
            get { return _usage; }
        }

        public string UsageDisplay
        {
            set
            {
                _usageDisplay = value;
                OnPropertyChanged();
            }
            get { return _usageDisplay; }
        }

        public string DataType
        {
            set
            {
                _dataType = value;
                OnPropertyChanged();
            }
            get { return _dataType; }
        }

        public string AliasFor { set; get; }

        public string BaseTag { set; get; }

        public ExternalAccess ExternalAccess
        {
            set
            {
                _externalAccess = value;
                ExternalAccessDisplay = _externalAccess.ToString();
            }
            get { return _externalAccess; }
        }

        public string ExternalAccessDisplay
        {
            set
            {
                _externalAccessDisplay = value;
                OnPropertyChanged();
            }
            get { return _externalAccessDisplay; }
        }

        public string Description
        {
            set
            {
                _description = value;
                OnPropertyChanged();
            }
            get { return _description; }
        }

        public bool IsConstant
        {
            set
            {
                _isConstant = value;
                OnPropertyChanged();
            }
            get { return _isConstant; }
        }

        public Visibility ErrorVisibility
        {
            set
            {
                if (_errorVisibility != value)
                {
                    if (PenVisibility == Visibility.Visible) return;
                    _errorVisibility = value;
                    if (ReferenceConnectionInfo != null)
                        ReferenceConnectionInfo.ErrorVisibility = value;
                    Parent?.CheckConnection();
                    //ReferenceParameterRow?.CheckConnection();
                    OnPropertyChanged();
                }
            }
            get { return _errorVisibility; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Gui.Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.DelTag)
            {
                var tags = message.Object as List<ITag>;
                if (tags != null)
                {
                    if (tags.Contains(Tag))
                    {
                        Tag = null;
                        DataType = "";
                        Description = "";
                        ExternalAccessDisplay = "";
                        IsConstant = false;
                        ErrorVisibility = Visibility.Visible;
                    }
                }

                return;
            }

            if (message.Type == MessageData.MessageType.AddTag)
            {
                var tag = message.Object as ITag;
                if (tag != null)
                {
                    var name = tag.ParentCollection.ParentProgram == null
                        ? tag.Name
                        : $"\\{tag.ParentCollection.ParentProgram.Name}.{tag.Name}";
                    if (ErrorVisibility == Visibility.Visible &&
                        Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                    {
                        GetInfo();
                    }
                }

                return;
            }
        }
    }

    public class TempScope
    {
        internal ParameterCollection ParameterRows { set; get; } =
            new ParameterCollection();

        internal List<ParameterRow> TempParameterRows { set; get; } = new List<ParameterRow>();

        public List<ParameterRow> GetParameterRows()
        {
            if (TempParameterRows.Any())
            {
                return TempParameterRows;
            }
            else
            {
                return ParameterRows?.ToList();
            }
        }

        public void Add(ParameterRow row)
        {
            if (TempParameterRows.Any())
            {
                TempParameterRows.Insert(TempParameterRows.Count - 1, row);
            }
            else
            {
                ParameterRows.Insert(ParameterRows.Count - 1, row);
            }
        }

        public void CheckParameterConnection()
        {
            foreach (var parameterRow in GetParameterRows())
            {
                parameterRow.CheckConnection();
            }
        }

        public void CheckErrorParameter()
        {
            if (TempParameterRows.Any())
            {
                foreach (var parameterRow in TempParameterRows.Where(p => p.ErrorVisibility == Visibility.Visible))
                {
                    parameterRow.CheckConnection();
                }
            }
            else
            {
                foreach (var parameterRow in ParameterRows.Where(p => p.ErrorVisibility == Visibility.Visible))
                {
                    parameterRow.CheckConnection();
                }
            }
        }
    }

    internal class ParameterCollection : LargeCollection<ParameterRow>
    {
        public override void AddRange(int index, List<ParameterRow> insertItems)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (insertItems == null || insertItems.Count == 0)
                return;

            CheckReentrancy();

            if (insertItems.Count == 1)
            {
                Insert(index, insertItems[0]);
                return;
            }

            List<ParameterRow> items = (List<ParameterRow>) Items;
            int startIndex = index;
            items.InsertRange(index, insertItems);
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    insertItems,
                    startIndex));
        }

        public override void RemoveTagItems(List<ParameterRow> listItem)
        {
            if (listItem.Count == 0)
                return;

            CheckReentrancy();

            if (listItem.Count == 1)
            {
                RemoveTagItem(listItem[0]);
                return;
            }

            List<TagItem> items = (List<TagItem>) Items;
            //foreach (var item in listItem)
            //{
            //    items.Remove(item);
            //    item.IsExtend = false;
            //}
            items.RemoveRange(Items.IndexOf(listItem[0]), listItem.Count);
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
        }
    }
}
