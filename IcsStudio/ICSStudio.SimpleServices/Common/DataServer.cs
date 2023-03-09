using System;
using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Notification;

namespace ICSStudio.SimpleServices.Common
{
    // operand = tag.Name + . +  subMember
    internal class DataServer : IDataServer, INotifiable<TagNotificationData>
    {
        private bool _monitorValue;
        private bool _monitorAttribute;

        protected Dictionary<int, WeakReference> Operands;
        private readonly NotificationListener<TagNotificationData> _notificationListener;

        internal DataServer(Controller controller)
        {
            ParentController = controller;

            Operands = new Dictionary<int, WeakReference>();

            _notificationListener = new NotificationListener<TagNotificationData>(this);

            Notifications.RegisterNotification(_notificationListener);
        }

        ~DataServer()
        {
            lock (Operands)
            {
                Operands.Clear();
            }

            Notifications.UnRegisterNotification(_notificationListener);
        }

        public IController ParentController { get; }

        public bool IsMonitoring => _monitorValue || _monitorAttribute;

        public virtual void StartMonitoring(bool monitorValue, bool monitorAttribute)
        {
            _monitorValue = monitorValue;
            _monitorAttribute = monitorAttribute;
        }

        public virtual void StopMonitoring(bool stopMonitorValue, bool stopMonitorAttribute)
        {
            if (_monitorValue && stopMonitorValue)
                _monitorValue = false;

            if (_monitorAttribute && stopMonitorAttribute)
                _monitorAttribute = false;
        }

        public virtual IDataOperand CreateDataOperand()
        {
            return new DataOperand(this);
        }

        public virtual IDataOperand CreateDataOperand(ITag tag, string subMember)
        {
            return CreateDataOperand(tag, subMember, true);
        }

        public virtual IDataOperand CreateDataOperand(ITag tag, string subMember, bool pending)
        {
            return CreateDataOperand(tag, subMember, pending, false);
        }

        public virtual IDataOperand CreateDataOperand(ITag tag, string subMember, bool pending,
            bool allowPrivateMemberReferences)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            string operand = tag.Name;
            if (!string.IsNullOrEmpty(subMember))
                operand = operand + "." + subMember;

            return CreateDataOperand(tag.ParentCollection, operand, pending, allowPrivateMemberReferences);
        }

        public virtual IDataOperand CreateDataOperand(IBaseComponent parent, string operand)
        {
            return CreateDataOperand(parent, operand, true);
        }

        public virtual IDataOperand CreateDataOperand(IBaseComponent parent, string operand, bool pending)
        {
            return CreateDataOperand(parent, operand, pending, false);
        }

        public virtual IDataOperand CreateDataOperand(IBaseComponent parent, string operand, bool pending,
            bool allowPrivateMemberReferences)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            ITagCollection tagCollection = null;

            IController controller = parent as IController;
            IProgram program = parent as IProgram;
            if (controller != null)
            {
                tagCollection = controller.Tags;
            }
            else if (program != null)
            {
                tagCollection = program.Tags;
            }

            if (tagCollection == null)
                throw new ArgumentException("Invalid parent component", nameof(parent));

            return CreateDataOperand(tagCollection, operand, pending, allowPrivateMemberReferences);
        }

        public virtual IDataOperand CreateDataOperand(
            ITagCollection tagCollection, string operand)
        {
            return CreateDataOperand(tagCollection, operand, true);
        }

        public virtual IDataOperand CreateDataOperand(
            ITagCollection tagCollection, string operand, bool pending)
        {
            return CreateDataOperand(tagCollection, operand, pending, false);
        }

        public virtual IDataOperand CreateDataOperand(
            ITagCollection tagCollection,
            string operand,
            bool pending, bool allowPrivateMemberReferences)
        {
            return CreateDataOperand(tagCollection, operand, pending, allowPrivateMemberReferences, null);
        }

        public virtual IDataOperand CreateDataOperand(
            ITagCollection tagCollection, string operand,
            bool pending, bool allowPrivateMemberReferences,
            ITagDataContext dataContext)
        {
            if (tagCollection == null)
                throw new ArgumentNullException(nameof(tagCollection), "Invalid Tag Collection");

            var dataOperand = new DataOperand(this);

            dataOperand.SetOperandString(operand, tagCollection, allowPrivateMemberReferences, dataContext);

            OnDataOperandCreated(dataOperand);

            return dataOperand;
        }

        public void OnNotification(TagNotificationData notificationData)
        {
            bool beNotify = false;

            if (notificationData == null)
                return;

            if (_monitorValue && notificationData.Type == TagNotificationData.NotificationType.Value)
            {
                beNotify = true;
            }

            if (_monitorAttribute && notificationData.Type == TagNotificationData.NotificationType.Attribute)
            {
                beNotify = true;
            }

            if (!beNotify)
                return;

            lock (Operands)
            {
                foreach (var item in Operands)
                {
                    WeakReference operand = item.Value;
                    if (operand.IsAlive)
                        ((DataOperand) operand.Target)?.NotifyDataOperandChanged(notificationData);
                }
            }

        }

        internal void OnDataOperandCreated(DataOperand dataOperand)
        {
            if (dataOperand == null)
                return;

            lock (Operands)
            {
                int hashCode = dataOperand.GetHashCode();
                if (!Operands.ContainsKey(hashCode))
                {
                    Operands[hashCode] = new WeakReference(dataOperand, false);
                }
            }
        }

        internal void OnDataOperandDeleted(DataOperand dataOperand)
        {
            if (dataOperand == null)
                return;

            lock (Operands)
            {
                int hashCode = dataOperand.GetHashCode();
                if (Operands.ContainsKey(hashCode))
                    Operands.Remove(hashCode);

            }
        }
    }
}
