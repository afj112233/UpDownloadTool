using System;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Common
{
    public abstract class BaseCommon : IBaseCommon, IEquatable<BaseCommon>
    {
        private bool _disposed;

        protected BaseCommon()
        {
            Uid = Guid.NewGuid().GetHashCode();
        }

        ~BaseCommon()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                DisposeAction();
            }

            _disposed = true;
        }

        protected virtual void DisposeAction()
        {

        }

        public abstract IController ParentController { get; }
        public int Uid { get; }

        public bool Equals(BaseCommon other)
        {
            if (other == null)
                return false;

            if (Uid == other.Uid)
                return true;

            return false;
        }
    }
}
