using System;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    public abstract class ProgramModule : BaseComponent, IProgramModule
    {
        private readonly RoutineCollection _routineCollection;
        private readonly TagCollection _tagCollection;
        private bool _isInMonitor;

        protected ProgramModule(IController controller)
        {
            ParentController = controller;
            _routineCollection = new RoutineCollection(this);
            _tagCollection = new TagCollection(this);
        }

        public bool IsInMonitor
        {
            set
            {
                _isInMonitor = value;
                if(value)
                    Notifications.Publish(new MessageData() { Type = MessageData.MessageType.MonitorTag, Object = this });
            }
            get { return _isInMonitor; }
        }

        public override IController ParentController { get; }
        public ITagCollection Tags => _tagCollection;
        public IRoutineCollection Routines => _routineCollection;
        public ProgramType Type { get; set; }

        public string FaultRoutineName
        {
            get
            {
                foreach (var routine in _routineCollection)
                {
                    if (routine.IsFaultRoutine)
                        return routine.Name;
                }

                return string.Empty;
            }
            //set
            //{
            //    var isChange = _faultRoutineName != value;
            //    _faultRoutineName = value;
            //    if (isChange) RaisePropertyChanged();
            //}
        }

        public string MainRoutineName
        {
            get
            {
                foreach (var routine in _routineCollection)
                {
                    if (routine.IsMainRoutine)
                        return routine.Name;
                }

                return string.Empty;
            }
            //set
            //{
            //    var isChange = _mainRoutineName != value;
            //    _mainRoutineName = value;
            //    if (isChange) RaisePropertyChanged();
            //}
        }

        public ITagDataContext GetDefaultDataContext()
        {
            throw new NotImplementedException();
        }

        public void AddTag(JToken jsonTag)
        {
            var tag = TagsFactory.CreateTag(_tagCollection, jsonTag);

            _tagCollection.AddTag(tag,false,false);
        }

        public void AddRoutine(IRoutine routine)
        {
            _routineCollection.AddRoutine(routine);
        }
    }
}
