                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using ICSStudio.SimpleServices.DataType;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ICSStudio.Interfaces.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json;
using NLog;

namespace ICSStudio.SimpleServices.Common
{
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;
    using ParamMemberList = List<Tuple<DataTypeMember, ParameterType>>;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    public sealed partial class AoiDefinition : ProgramModule, IAoiDefinition
    {
        private AoiDefinitionCore _core;
        private string _name;
        private DateTime _editedDate;
        private string _editedBy;
        private bool _isSealed;
        private JObject _config;
        private string _revision;
        private string _revisionNote;
        private string _vendor;
        private DateTime _createdDate;
        private string _createdBy;
        private string _signatureID;

        private bool _isEncrypted;

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public AoiDefinition(JObject config, IController controller, bool isTmp = false) : base(controller)
        {
            IsTmp = isTmp;
            _config = config;
            Name = config["Name"]?.ToString();
            Description = config["Description"]?.ToString();
            EngineeringUnit = (string) config["EngineeringUnit"];

            UpdateRoutines(config["Routines"] as JArray);
            ParseCore(config);
            ParseEncodedData(config);

            //ParserTags();
            _core = new AoiDefinitionCore(this, IsTmp) {Host = this};
            //CollectionChangedEventManager.AddHandler(Tags, Tags_CollectionChanged);
            PropertyChangedEventManager.AddHandler(_core.datatype, OnDataTypePropertyChanged, "");
        }

        public string EngineeringUnit { get; }

        public void UpdateChangeHistory()
        {
            EditedBy = Environment.MachineName + @"\" + Environment.UserName;
            EditedDate = DateTime.Now;
        }

        public bool IsTmp { get; }
        
        public void AddScanRoutine(IRoutine routine)
        {
            //TODO(zyl):add scan routine
        }

        public void DelScanRoutine(IRoutine routine)
        {
            //TODO(zyl):remove scan routine
        }

        public bool CanPostOverwrite { private set; get; }

        public void Overwrite(JObject config, IController controller)
        {
            Name = config["FinalName"]?.ToString();
            Description = config["Description"]?.ToString();

            Revision = config["Revision"]?.ToString();
            RevisionNote = config["RevisionNote"]?.ToString();
            Vendor = config["Vendor"]?.ToString();
            CreatedDate = Convert.ToDateTime(config["CreatedDate"]?.ToString());
            CreatedBy = config["CreatedBy"]?.ToString();
            UpdateChangeHistory();

            _config = config;
            ((TagCollection)Tags).Clear();
            _core.datatype.Reset();
            UpdateRoutines(config["Routines"] as JArray);
            IsChanged = false;
            CanPostOverwrite = true;
        }

        public void PostOverwrite()
        {
            if (CanPostOverwrite)
            {
                //datatype.PostInit(ParentController.DataTypes);

                RaisePropertyChanged("_core");
            }
        }

        public bool IsChanged { set; get; }

        public override string Name
        {
            get { return _name; }
            set
            {
                if (!string.Equals(_name, value))
                {
                    OldName = _name;
                    if (instr != null)
                    {
                        var controller = (Controller)ParentController;
                        controller.STInstructionCollection.RemoveInstruction(_core.instr);
                        controller.RLLInstructionCollection.RemoveInstruction(_core.instr);
                        controller.FBDInstructionCollection.RemoveInstruction(_core.instr);
                        instr.Name = value;
                        ModifyInstrCollectionKeyName(value);
                    }
                    _name = value;
                    if(_core?.datatype!=null)
                        _core.datatype.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void ModifyInstrCollectionKeyName(string newName)
        {
            var coll = RTInstructionCollection.Inst;
            var instrName = Name + ".Logic";
            var oldInfo = coll.FindInstruction(instrName);
            coll.Remove(instrName);
            coll.Add(newName + ".Logic", new RTInstructionInfo(newName + ".Logic", oldInfo.return_type, oldInfo.param_types, oldInfo.is_ref));
            instrName = Name + ".Prescan";
            oldInfo = coll.FindInstruction(instrName);
            coll.Remove(instrName);
            RTInstructionCollection.Inst.Add(newName + ".Prescan",
                new RTInstructionInfo(newName + ".Prescan", oldInfo.return_type, oldInfo.param_types, oldInfo.is_ref));
            
            var controller = (Controller)ParentController;
            controller.STInstructionCollection.AddInstruction(_core.instr);
            controller.RLLInstructionCollection.AddInstruction(_core.instr);
            controller.FBDInstructionCollection.AddInstruction(_core.instr);
        }

        public override string Description
        {
            set
            {
                if (_description1 != value)
                {
                    _description1 = value;
                    if (_core != null)
                    {
                        _core.datatype.Description = value;
                    }

                    RaisePropertyChanged();
                }
            }
            get { return _description1; }
        }

        protected override void DisposeAction()
        {
            //TODO(gjc): need edit later
            if (IsDeleted) return;

            IsDeleted = true;

            PropertyChangedEventManager.RemoveHandler(_core.datatype, OnDataTypePropertyChanged, "");

            for (int i = References.Count - 1; i >= 0; i--)
            {
                var aoiDataReference = References[i];
                var aoi = aoiDataReference.Routine.ParentCollection.ParentProgram as AoiDefinition;
                if (aoi != null)
                {
                    aoi.References.CollectionChanged -= References_CollectionChanged;
                }
                aoiDataReference.Dispose();
                References.Remove(aoiDataReference);
            }

            for (int i = InnerReferences.Count - 1; i >= 0; i--)
            {
                var aoiDataReference = InnerReferences[i];
                var aoi = aoiDataReference.Routine.ParentCollection.ParentProgram as AoiDefinition;
                if (aoi != null)
                {
                    aoi.References.CollectionChanged -= References_CollectionChanged;
                }
                aoiDataReference.Dispose();
                InnerReferences.Remove(aoiDataReference);
            }
            
            _core.Dispose();
            Tags.Dispose();
            
            _name = null;
            _description1 = null;
            _config = null;

            ParentCollection = null;
        }
        
        public string Revision
        {
            get { return _revision; }
            set
            {
                if (_revision != value)
                {
                    _revision = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ExtendedText { get; set; }

        public string RevisionNote
        {
            get { return _revisionNote; }
            set
            {
                if (_revisionNote != value)
                {
                    _revisionNote = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ExecutePrescan { get; set; }
        public bool ExecutePostscan { get; set; }
        public bool ExecuteEnableInFalse { get; set; }
        public IList History { get; private set; }
        public string ExtendedDescription { get; set; }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set
            {
                if (_createdDate != value)
                {
                    _createdDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string CreatedBy
        {
            get { return _createdBy; }
            set
            {
                if (_createdBy != value)
                {
                    _createdBy = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DateTime EditedDate
        {
            get { return _editedDate; }
            set
            {
                _editedDate = value;
                RaisePropertyChanged();
            }
        }

        public string EditedBy
        {
            get { return _editedBy; }
            set
            {
                _editedBy = value;
                RaisePropertyChanged();
            }
        }

        public string SoftwareRevision { get; set; }

        public string Vendor
        {
            get { return _vendor; }
            set
            {
                if (_vendor != value)
                {
                    _vendor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SignatureID
        {
            get { return _signatureID; }
            set
            {
                if (_signatureID != value)
                {
                    _signatureID = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SignatureTimestamp { get; set; }

        public void ParseCore(JObject config)
        {
            History = new List<History>();
            Revision = config["Revision"]?.ToString();
            ExtendedText = config["RevisionExtension"]?.ToString();
            RevisionNote = config["RevisionNote"]?.ToString();
            Vendor = config["Vendor"]?.ToString();

            ExecutePrescan = config["ExecutePrescan"] != null && (bool) config["ExecutePrescan"];
            ExecutePostscan = config["ExecutePostscan"] != null && (bool) config["ExecutePostscan"];
            ExecuteEnableInFalse = config["ExecuteEnableInFalse"] != null && (bool) config["ExecuteEnableInFalse"];
            //CreatedDate = config["CreatedDate"]?.ToString(Formatting.Indented);
            if (config["CreatedDate"] != null)
            {
                string str_time = config["CreatedDate"].ToString(Formatting.Indented).Replace("\"", "");
                DateTime cd = DateTime.Parse(str_time);
                CreatedDate = cd;
            }

            CreatedBy = config["CreatedBy"]?.ToString();
            //EditedDate = config["EditedDate"]?.ToString();
            if (config["EditedDate"] != null)
            {
                string str_time = config["EditedDate"].ToString(Formatting.Indented).Replace("\"", "");
                DateTime cd = DateTime.Parse(str_time);
                EditedDate = cd;
            }

            EditedBy = config["EditedBy"]?.ToString();

            SignatureID = config["SignatureID"]?.ToString();
            SignatureTimestamp = config["SignatureTimestamp"]?.ToString();
            if (SignatureID != null)
            {
                IsSealed = true;
            }

            var histories = config["SignatureHistory"] as JArray;
            if (histories != null)
            {
                foreach (var item in histories)
                {
                    var history = new History()
                    {
                        User = item["User"]?.ToString(),
                        SignatureID = item["Signature"]?.ToString(),
                        Timestamp = item["Timestamp"]?.ToString(),
                        Description = item["Description"]?.ToString()
                    };
                    History.Add(history);
                }
            }

            ExtendedDescription = config["AdditionalHelpText"]?.ToString();
        }

        public IAoiDefinitionCollection ParentCollection { get; set; }

        public bool IsSealed
        {
            get { return _isSealed; }
            set
            {
                _isSealed = value;
                RaisePropertyChanged();
            }
        }

        public override bool IsDeleted
        {
            get
            {
                if (ParentCollection == null)
                    return true;

                return base.IsDeleted;
            }
        }

        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set
            {
                _isEncrypted = value;
                RaisePropertyChanged();
            }
        }

        public uint TrackingGroups { get; set; }
        public bool IsSourceEditable { get; set; }
        public bool IsSourceViewable { get; set; }
        public bool IsSourceCopyable { get; set; }
        public EncodedData EncodedData { get; set; }

        public List<ITag> GetLocalTags()
        {
            return Tags.Where(t => t.Usage == Usage.Local).ToList();
        }

        public List<ITag> GetParameterTags()
        {
            return Tags.Where(t => t.Usage != Usage.Local).ToList();
        }

        public IXInstruction instr
        {
            get { return _core?.instr; }

        }

        public IEnumerable<IAoiInvocationContext> GetInvocationContexts()
        {
            throw new NotImplementedException();
        }

        //private bool _isInitial = false;
        //internal void ParserTags()
        //{
        //    if (_isInitial) return;
        //    _isInitial = true;
        //    try
        //    {
        //        var config = _config;
        //        var parameters = config["Parameters"] as JArray;
        //        var localTags = config["LocalTags"] as JArray;
        //        var coll = Tags as TagCollection;
        //        Debug.Assert(coll != null);
        //        coll.Clear();

        //        #region Parameter

        //        if (parameters != null)
        //        {
        //            foreach (var item in parameters)
        //            {
        //                ParseParameter((JObject)item);
        //            }
        //        }

        //        #endregion

        //        #region Logical

        //        if (localTags != null)
        //        {
        //            foreach (var item in localTags)
        //            {
        //                ParseLocal((JObject)item);
        //            }
        //        }

        //        #endregion
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Trace($"AoiDefinition.ParseTags.Error({e.Message})");
        //    }
        //}

        internal ITag ParseParameter(JObject item, bool resetAoi)
        {
            var coll = Tags as TagCollection;
            Tag parameter = new Tag(coll);
            parameter.Name = item["Name"]?.ToString();
            parameter.Usage = (Usage) ((byte) item["Usage"]);
            parameter.Description = item["Description"]?.ToString();
            int dim1 = 0, dim2 = 0, dim3 = 0;
            if (!string.IsNullOrEmpty(item["Dimensions"]?.ToString()))
            {
                if (item["Dimensions"].ToString().Split(' ').Length == 1)
                    dim1 = int.Parse(item["Dimensions"].ToString());
                else if (item["Dimensions"].ToString().Split(' ').Length == 2)
                {
                    dim1 = int.Parse(item["Dimensions"].ToString().Split(' ')[1]);
                    dim2 = int.Parse(item["Dimensions"].ToString().Split(' ')[0]);
                }
                else if (item["Dimensions"].ToString().Split(' ').Length == 3)
                {
                    dim3 = int.Parse(item["Dimensions"].ToString().Split(' ')[0]);
                    dim2 = int.Parse(item["Dimensions"].ToString().Split(' ')[1]);
                    dim1 = int.Parse(item["Dimensions"].ToString().Split(' ')[2]);
                }
                else
                {
                    Debug.Assert(false, "ParserTag:error dims");
                }
            }

            JToken value = parameter.Usage == Usage.InOut ? null : item["DefaultData"];
            try
            {
                if (value != null)
                {
                    if (value is JObject)
                    {
                        if ("Float".Equals(value["Radix"].ToString()))
                        {
                            parameter.DataWrapper = new DataWrapper.DataWrapper(
                                ParentController.DataTypes[item["DataType"]?.ToString()], dim1, dim2, dim3,
                                float.Parse(AOIExtend.ConvertToInt((JObject) value, ParentController).Value
                                    .ToString()));
                        }
                        else
                        {
                            parameter.DataWrapper = new DataWrapper.DataWrapper(
                                ParentController.DataTypes[item["DataType"]?.ToString()], dim1, dim2, dim3,
                                int.Parse(AOIExtend.ConvertToInt((JObject) value, ParentController).Value.ToString()));
                        }
                    }
                    else
                    {
                        var v = (JValue) value;
                        if (v.Type == JTokenType.Integer)
                        {
                            parameter.DataWrapper = new DataWrapper.DataWrapper(
                                ParentController.DataTypes[item["DataType"]?.ToString()], dim1, dim2, dim3,
                                int.Parse(v.Value.ToString()));
                        }
                        else if (v.Type == JTokenType.Float)
                        {
                            parameter.DataWrapper = new DataWrapper.DataWrapper(
                                ParentController.DataTypes[item["DataType"]?.ToString()], dim1, dim2, dim3,
                                float.Parse(v.Value.ToString()));
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                }
                else
                {
                    //var data = controller.DataTypes[item["DataType"]?.ToString()].Create(null);
                    parameter.DataWrapper =
                        new DataWrapper.DataWrapper(
                            ParentController.DataTypes[item["DataType"]?.ToString()],
                            dim1,
                            dim2, dim3, null);
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
                parameter.DataWrapper =
                    new DataWrapper.DataWrapper(
                        ParentController.DataTypes[item["DataType"]?.ToString()],
                        dim1,
                        dim2, dim3, null);
            }

            parameter.DisplayStyle =
                item["Radix"] == null ? DisplayStyle.Decimal : (DisplayStyle) (byte) item["Radix"];
            parameter.IsRequired = item["Required"] != null && (bool) item["Required"];
            parameter.IsVisible = item["Visible"] != null && (bool) item["Visible"];

            if (parameter.Usage == Usage.InOut)
                parameter.ExternalAccess = ExternalAccess.NullExternalAccess;
            else
                parameter.ExternalAccess = (ExternalAccess) (byte) item["ExternalAccess"];


            parameter.IsConstant = item["Constant"] != null && (bool) item["Constant"];
            parameter.ChildDescription = item["Comments"] as JArray ?? new JArray();
            coll?.AddTag(parameter, resetAoi, false);
            return parameter;
        }

        internal ITag ParseLocal(JObject item, bool resetAoi)
        {
            var coll = Tags as TagCollection;
            Tag localTag = new Tag(coll);
            localTag.Name = item["Name"]?.ToString();
            int dim = item["Dimensions"] == null ? 0 : (int) item["Dimensions"];
            var dataType = ParentController.DataTypes[item["DataType"].ToString()];
            if (IsChanged)
            {
                localTag.DataWrapper = new DataWrapper.DataWrapper(
                    dataType, dim, 0, 0, null);
            }
            else
            {
                try
                {
                    if (item["DefaultData"] != null)
                    {
                        localTag.DataWrapper = new DataWrapper.DataWrapper(
                            dataType, dim, 0, 0,
                            item["DefaultData"]);
                    }
                    else
                    {
                        localTag.DataWrapper = new DataWrapper.DataWrapper(
                            dataType, dim, 0, 0, null);
                    }
                }
                catch (Exception)
                {
                    localTag.DataWrapper = new DataWrapper.DataWrapper(
                        dataType, dim, 0, 0, null);
                }
            }

            localTag.Description = item["Description"]?.ToString();
            localTag.DisplayStyle =
                item["Radix"] == null ? DisplayStyle.Decimal : (DisplayStyle) (byte) item["Radix"];
            localTag.ChildDescription = item["Comments"] as JArray ?? new JArray();
            localTag.Usage = Usage.Local;
            localTag.ExternalAccess = (ExternalAccess) (byte) item["ExternalAccess"];
            coll?.AddTag(localTag, resetAoi, false);
            return localTag;
        }

        public JObject GetConfig()
        {
            if (!Tags.Any()) return _config;
            JObject config = new JObject();
            config["Name"] = Name ?? "";
            config["Description"] = Description ?? "";
            config["Revision"] = Revision ?? "";
            config["RevisionExtension"] = ExtendedText ?? "";
            config["RevisionNote"] = RevisionNote ?? "";
            config["Vendor"] = Vendor ?? "";

            JArray parameters = new JArray();
            AOIExtend.ToAOIParameterConfig(parameters, GetParameterTags());
            config["Parameters"] = parameters;

            JArray localTags = new JArray();
            AOIExtend.ToAOILocalTagConfig(localTags, GetLocalTags());
            config["LocalTags"] = localTags;

            JArray routines = new JArray();
            AOIExtend.ToAOIRoutineConfig(routines, Routines);
            config["Routines"] = routines;

            config["ExecutePrescan"] = ExecutePrescan;
            config["ExecutePostscan"] = ExecutePostscan;
            config["ExecuteEnableInFalse"] = ExecuteEnableInFalse;
            config["CreatedDate"] = CreatedDate;
            config["CreatedBy"] = CreatedBy;
            config["EditedDate"] = EditedDate;
            config["EditedBy"] = EditedBy;
            if (IsSealed)
            {
                config["SignatureID"] = SignatureID;
                config["SignatureTimestamp"] = SignatureTimestamp;
            }

            JArray histories = new JArray();
            foreach (History item in History)
            {
                JObject history = new JObject();
                history["User"] = item.User;
                history["Signature"] = item.SignatureID;
                history["Timestamp"] = item.Timestamp;
                history["Description"] = item.Description;
                histories.Add(history);
            }

            config["SignatureHistory"] = histories;
            config["AdditionalHelpText"] = ExtendedDescription;
            return config;
        }

        public JObject ConvertToJObject(bool needNativeCode = true)
        {
            JObject aoi = _core.ConvertToJObject(needNativeCode);
            //临时解决AOI修改后第一次保存时，EditedDate不更新的问题
            //TODO: 需解决界面属性框更新时，同步更新_core的EditedDate的问题
            if (aoi["EditedDate"] != null)
                aoi["EditedDate"] = EditedDate;
            if (aoi["EditedBy"] != null)
                aoi["EditedBy"] = EditedBy;
            if (aoi.ContainsKey("EncodedData"))
                aoi.Remove("EncodedData");

            if (IsEncrypted)
            {
                Contract.Assert(aoi.ContainsKey("Routines"));
                aoi.Remove("Routines");

                Controller controller = ParentController as Controller;
                SourceProtectionManager manager = controller?.SourceProtectionManager;

                EncodedData encodedData = manager?.CreateEncodedData(this);
                if (encodedData != null)
                {
                    EncodedData = encodedData;
                }

                if (EncodedData != null)
                {
                    aoi.Add("EncodedData", JObject.FromObject(EncodedData));
                }

            }

            return aoi;
        }

        private void ParseEncodedData(JObject config)
        {
            JObject encodedData = (JObject) config["EncodedData"];
            if (encodedData != null)
            {
                EncodedData = encodedData.ToObject<EncodedData>();
                IsEncrypted = true;
            }
        }

        private void OnDataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ByteSize")
            {
                IsChanged = true;
            }

            if (e.PropertyName == "RequestTagUpdateData")
            {
                RaisePropertyChanged("RequestTagUpdateData");
            }
        }

        //private void Tags_CollectionChanged(object sender,
        //    NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (ITag item in e.NewItems)
        //        {
        //            PropertyChangedEventManager.AddHandler(item, Item_PropertyChanged, "");
        //        }
        //    }

        //    if (e.OldItems != null)
        //        foreach (ITag item in e.OldItems)
        //        {
        //            PropertyChangedEventManager.RemoveHandler(item, Item_PropertyChanged, "");
        //        }
        //}

        public bool IsAddListener { private set; get; } = true;

        //private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "ByteSize")
        //        Reset();
        //}

        //private void SetRoutines()
        //{
        //    if (_core != null)
        //        UpdateRoutines(_core.ConvertToJObject()["Routines"] as JArray);
        //}

        internal void UpdateRoutines(JArray routines)
        {
            var collection = Routines as RoutineCollection;
            Contract.Assert(collection != null);

            collection.RemoveAll();

            if (routines != null)
            {
                foreach (var item in routines)
                {
                    switch ((RoutineType)((byte)item["Type"]))
                    {
                        default:
                        {
                            var routine = new RLLRoutine(ParentController)
                            {
                                Name = item["Name"]?.ToString(),
                                Description = item["Description"]?.ToString()
                            };

                            JArray codeText = (JArray)item["CodeText"];
                            JArray rungComments = (JArray)item["RungComments"];
                            JArray rungs = (JArray)item["Rungs"];

                            if (rungs != null)
                            {
                                List<RungType> rungList = new List<RungType>();
                                foreach (var rung in rungs.OfType<JObject>())
                                {
                                    RungType rungType = new RungType();

                                    if (rung.ContainsKey("Type"))
                                    {
                                        rungType.Type = RLLRoutine.ParserRungType(rung["Type"]?.ToString());
                                    }
                                    else
                                    {
                                        rungType.Type = RungTypeEnum.Normal;
                                    }

                                    rungType.Text = rung.ContainsKey("Text") ? rung["Text"]?.ToString() : string.Empty;

                                    if (rung.ContainsKey("Comment"))
                                    {
                                        rungType.Comment = rung["Comment"]?.ToString();
                                    }

                                    rungList.Add(rungType);
                                }

                                routine.UpdateRungs(rungList);
                            }
                            else
                            {
                                routine.UpdateRungs(codeText?.ToObject<List<string>>(),
                                    rungComments?.ToObject<List<KeyValuePair<int, string>>>());
                            }



                            collection.AddRoutine(routine);
                        }
                            break;

                        case RoutineType.FBD:
                        {
                            var routine = new FBDRoutine(ParentController, item as JObject)
                            {
                                Name = item["Name"]?.ToString(),
                                Description = item["Description"]?.ToString()
                            };
                            collection.AddRoutine(routine);
                        }
                            break;

                        case RoutineType.ST:
                        {
                            var routine = new STRoutine(ParentController);
                            routine.Name = item["Name"]?.ToString();
                            var code = item["CodeText"] as JArray;
                            if (code != null)
                            {
                                foreach (var item2 in code)
                                {
                                    routine.CodeText.Add(item2?.ToString());
                                }
                            }

                            routine.Description = item["Description"]?.ToString();
                            collection.AddRoutine(routine);
                        }
                            break;
                    }
                }
            }
        }

        private readonly object _referenceObject = new object();

        public ObservableCollection<AoiDataReference> References { get; private set; } =
            new ObservableCollection<AoiDataReference>();
        
        public void AddReference(AoiDataReference reference)
        {
            lock (_referenceObject)
            {
                References.Add(reference);
            }
        }

        public void RemoveReference(AoiDataReference reference)
        {
            if (References.Contains(reference))
            {
                lock (_referenceObject)
                {
                    References.Remove(reference);
                }
            }
        }

        private readonly object _innerReferencesObject = new object();
        private string _description1;

        public ObservableCollection<AoiDataReference> InnerReferences { get; private set; } =
            new ObservableCollection<AoiDataReference>();

        public void AddInnerReference(AoiDataReference reference)
        {
            var aoi = reference.Routine.ParentCollection.ParentProgram as AoiDefinition;
            Debug.Assert(aoi != null);
            bool isListen = true;
            lock (_innerReferencesObject)
            {
                foreach (var aoiDataReference in InnerReferences.ToList())
                {
                    if (aoiDataReference.InnerAoiDefinition == aoi&&aoiDataReference.OnlineEditType==reference.OnlineEditType)
                    {
                        isListen = false;
                        break;
                    }
                }
            }

            if (isListen)
            {
                aoi.References.CollectionChanged += References_CollectionChanged;
            }

            lock (_innerReferencesObject)
            {
                InnerReferences.Add(reference);
            }

            //ResetInnerReference(aoi);
            foreach (var aoiReference in aoi.References.ToList())
            {
                AddReference(
                    new AoiDataReference(reference.ReferenceAoi, reference.Routine, reference.AoiContext,reference.OnlineEditType)
                    {
                        InnerAoiDefinition = aoi,
                        Line = reference.Line,
                        Column = reference.Column,
                        InnerDataReference = aoiReference
                    });
            }
        }

        //private void ResetInnerReference(AoiDefinition aoi)
        //{
        //    //RemoveInnerReference(aoi);
        //    foreach (var innerReference in InnerReferences.Where(r => r.InnerAoiDefinition == aoi))
        //    {
        //        foreach (var reference in innerReference.InnerAoiDefinition.References)
        //        {
        //            var convertReference =
        //                new AoiDataReference(reference.ReferenceAoi, reference.Routine, reference.AoiContext)
        //                    {InnerDataReference = innerReference};
        //            AddReference(convertReference);
        //        }
        //    }
        //}

        private void RemoveInnerReference(AoiDefinition aoi)
        {
            for (int i = References.Count - 1; i >= 0; i--)
            {
                var reference = References[i];
                if (reference.InnerAoiDefinition == aoi)
                {
                    References.RemoveAt(i);
                }
            }
        }

        private void References_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AoiDataReference newItem in e.NewItems)
                {
                    foreach (var innerReference in InnerReferences)
                    {
                        var aoi = innerReference.Routine.ParentCollection.ParentProgram as AoiDefinition;
                        if (aoi != newItem.ReferenceAoi)
                        {
                            continue;
                        }

                        AddReference(
                            new AoiDataReference(innerReference.ReferenceAoi, innerReference.Routine,
                                innerReference.AoiContext, innerReference.OnlineEditType)
                            {
                                InnerDataReference = newItem,
                                InnerAoiDefinition = aoi,
                                Column = innerReference.Column,
                                Line = innerReference.Line
                            });
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (AoiDataReference oldItem in e.OldItems)
                {
                    for (int i = References.Count - 1; i >= 0; i--)
                    {
                        var reference = References[i];
                        if (reference.InnerDataReference == oldItem)
                        {
                            RemoveReference(reference);
                        }
                    }
                }
            }
        }

        public void CleanReferences(IRoutine routine)
        {
            if (routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                for (int i = InnerReferences.Count - 1; i >= 0; i--)
                {
                    var reference = InnerReferences[i];
                    if (reference == null) continue;

                    for (int j = References.Count - 1; j >= 0; j--)
                    {
                        var referenceI = References[j];
                        if (referenceI == null) continue;
                        if (referenceI.InnerAoiDefinition == routine.ParentCollection.ParentProgram &&
                            referenceI.Routine == routine)
                        {
                            Logger.Trace($"'{Name}' Remove Reference({referenceI.Routine.Name}--{referenceI.Line})");
                            RemoveReference(referenceI);
                        }
                    }

                    if (reference.InnerAoiDefinition == routine.ParentCollection.ParentProgram &&
                        reference.Routine == routine)
                    {
                        var aoi = reference.Routine.ParentCollection.ParentProgram as AoiDefinition;
                        aoi.References.CollectionChanged -= References_CollectionChanged;
                        reference.Dispose();
                        Logger.Trace($"'{Name}' Remove Reference({reference.Routine.Name}--{reference.Line})");
                        lock (_innerReferencesObject)
                        {
                            InnerReferences.Remove(reference);
                        }
                    }
                }

                return;
            }

            for (int i = References.Count - 1; i >= 0; i--)
            {
                var reference = References[i];
                if (reference == null) continue;
                if (reference.Routine == routine)
                {
                    Logger.Trace($"'{Name}' Remove Reference({reference.Routine.Name}--{reference.Line})");
                    RemoveReference(reference);
                }
            }
        }

        public int GetIndexOfAoiParameters(ITag tag)
        {
            var parameters = Tags.Where(t => t.IsRequired).ToList();
            return parameters.IndexOf(tag);
        }

        #region IInstruction

        /*
        public ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
        {
            return _core.Match(param_list, context);
        }

        public ASTNode Prescan(ASTNodeList param_list, Context context = Context.ST)
        {
            return _core.Prescan(param_list, context);
        }

        public bool TypeCheck(ASTNodeList param_list, Context context = Context.ST)
        {
            return _core.TypeCheck(param_list, context);
        }
        */

        internal void PostInit(DataTypeCollection coll)
        {
            _core.Reset();
        }

        public int FindParameterIndex(string name)
        {
            return _core.FindParameterIndex(name);
        }

        public IDataType GetParameterType(int index)
        {
            return _core.GetParameterType(index);
        }

        public string GetParameterName(int index)
        {
            return _core.GetParameterName(index);
        }

        public int GetParameterPos(int index)
        {
            return _core.GetParameterPos(index);
        }

        public void GenCode(Controller controller)
        {
            _core.GenCode(controller);
        }

        public void GenNativeCode(Controller controller, OutputStream writer)
        {
            _core.GenNativeCode(controller, writer);
        }

        public void GenSepNativeCode(Controller controller)
        {
            _core.GenSepNativeCode(controller);
        }

        public bool IsParameter(string name)
        {
            return _core.IsParameter(name);
        }

        public void Clear()
        {
            _core.Clear(Name);
        }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        public AOIDataType datatype => _core?.datatype;

        #endregion

        public class AOIInstruction : FixedInstruction,IDisposable
        {
            public AOIInstruction(string name, ParamInfoList infos, ParamMemberList poses, int argsSize) : base(name,
                infos)
            {
                this.poses = poses;
                ArgsSize = argsSize; // poses.Count(x => x.Item2 == ParameterType.INOUT);

            }

            public ParameterType GetParameterType(int index)
            {
                if (index < Infos.Count)
                {
                    return Infos[index].Item3;
                }
                Debug.Assert(false);
                return default(ParameterType);
            }

            public override List<Tuple<string, IDataType>> GetParameterInfo()
            {
                var controller = Controller.GetInstance();
                var aoi = ((AoiDefinitionCollection) controller.AOIDefinitionCollection).Find(Name);
                var @params = aoi.Tags.Where(t => t.IsRequired && t.Usage != Usage.Local);

                var paramsInfo = new List<Tuple<string, IDataType>>();
                paramsInfo.Add(new Tuple<string, IDataType>(Name, aoi.datatype));
                foreach (var tag in @params)
                {
                    IDataType type;
                    if (tag.DataTypeInfo.Dim1 > 0)
                    {
                        type = new ArrayType(tag.DataTypeInfo.DataType, tag.DataTypeInfo.Dim1, tag.DataTypeInfo.Dim2,
                            tag.DataTypeInfo.Dim3);
                    }
                    else
                    {
                        type = tag.DataTypeInfo.DataType;
                    }

                    paramsInfo.Add(new Tuple<string, IDataType>(tag.Name, type));
                }

                return paramsInfo;
            }

            public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList,
                Context context = Context.ST)
            {
                if (context == Context.FBD)
                {
                    var tmp = paramList.Accept(checker) as ASTNodeList;
                    var arguments = new ASTNodeList();
                    foreach (var node in tmp.nodes)
                    {
                        Debug.Assert(node is ASTName);
                        arguments.AddNode(new ASTNameAddr(node as ASTName));
                    }

                    return arguments;
                }
                else
                {
                    return base.TypeCheck(checker, paramList, context);

                }
            }

            public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
            {
                Call(gen, paramList, ()=> gen.masm().CallName(this.Name + ".Logic", ArgsSize), context);
            }

            public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
            {
                Call(gen, paramList, () => gen.masm().CallName(this.Name + ".Prescan", ArgsSize), Context.ST);
            }


            private void Call(CodeGenVisitor gen, ASTNodeList paramList, Action action, Context context = Context.ST)
            {
                Debug.Assert(paramList.Count() >= 1, paramList.Count().ToString());
                var controller = Controller.GetInstance();
                var aoi = ((AoiDefinitionCollection)controller.AOIDefinitionCollection).Find(Name);
                var type = aoi.datatype;
                
                gen.masm().If(() =>
                {
                    if (context == Context.RLL)
                    {
                        gen.masm().Dup();
                    }
                    else
                    {
                        gen.masm().BiPush(1);
                    }
                }, () =>
                {
                    if (context == Context.ST || context == Context.RLL)
                    {
                        //这个是留给OUTPUT用的
                        paramList.nodes[0].Accept(gen);
                        Instruction.Utils.GenSetBit(gen.masm(), type["EnableOut"]);
                        gen.masm().Dup();

                        Debug.Assert(paramList.Count() == poses.Count, $"{paramList.Count()}:{poses.Count}");

                        for (int i = 0; i < poses.Count; ++i)
                        {
                            var pos = poses[i];
                            if (pos.Item2 != ParameterType.INPUT)
                            {
                                continue;
                            }

                            Instruction.Utils.Store(gen, pos.Item1, paramList.nodes[i] as ASTExpr);
                        }

                        //第一个参数已经在上面压好栈了
                        for (int i = 1; i < paramList.Count(); ++i)
                        {
                            if (poses[i].Item2 != ParameterType.INOUT)
                            {
                                continue;
                            }

                            Debug.Assert(paramList.nodes[i] is ASTNameAddr, paramList.nodes[i].ToString());
                            paramList.nodes[i].Accept(gen);
                            var tp = (paramList.nodes[i] as ASTNameAddr).ref_type.type as BOOL;
                            if (tp != null && (paramList.nodes[i] as ASTNameAddr).name.dim1 == 0)
                            {
                                gen.masm().DupX1();
                                gen.masm().CLoadInteger(8);
                                gen.masm().Div(MacroAssembler.PrimitiveType.DINT);
                                gen.masm().PAdd();
                                gen.masm().Swap();
                                gen.masm().CLoadInteger(8);
                                gen.masm().Mod(MacroAssembler.PrimitiveType.DINT);
                            }
                        }

                    }
                    else
                    {
                        gen.VisitParamList(paramList);
                    }

                    action();

                    //gen.masm().CallName(this.Name + ".Logic", ArgsSize);




                    if (context == Context.ST || context == Context.RLL)
                    {
                        gen.masm().Pop();
                        var enableOut = type["EnableOut"];
                        gen.masm().Dup();
                        gen.masm().CLoadInteger(enableOut.ByteOffset);
                        gen.masm().PAdd();
                        gen.masm().CLoadInteger(enableOut.BitOffset);
                        gen.masm().Load(enableOut.DataTypeInfo.DataType);

                        //把this指针和Call的返回值交换一下
                        gen.masm().Swap();
                        for (int i = 0; i < poses.Count; ++i)
                        {
                            var pos = poses[i];
                            if (pos.Item2 != ParameterType.OUTPUT)
                            {
                                continue;
                            }

                            Instruction.Utils.Store(gen, paramList.nodes[i] as ASTNameAddr, pos.Item1);
                        }

                        //这是弹出最开始放的那个this指针
                        gen.masm().Pop();
                    }

                }, () =>
                {

                    gen.masm().BiPush(0);
                    gen.masm().stack_size -= 1;
                });

            }


            private ParamMemberList poses { get; }

            private int ArgsSize { get; }

            private bool _isDisposed = false;
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if(_isDisposed)return;
                _isDisposed = true;
                if (disposing)
                {
                    Infos.Clear();
                    poses.Clear();
                }
            }
        }
    }
}