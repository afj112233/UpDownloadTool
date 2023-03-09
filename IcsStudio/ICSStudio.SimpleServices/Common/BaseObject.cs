using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;

namespace ICSStudio.SimpleServices.Common
{
    public abstract class BaseObject : BaseCommon, IBaseObject
    {
        public virtual bool IsVerified { get; set; }
        public virtual bool IsDeleted { get; protected set; }
        public virtual int ParentProgramUid { get; set; }
        public virtual int ParentRoutineUid { get; set; }

        public virtual void BeginTransactionSet()
        {
        }

        public virtual void EndTransactionSet()
        {
        }

        public virtual void CancelTransactionSet()
        {
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

        public virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, 
                new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
        }
    }
}
