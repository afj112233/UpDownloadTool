using System;

namespace ICSStudio.Interfaces.Common
{
    public interface IBaseCommon: IDisposable
    {
        IController ParentController { get; }

        int Uid { get; }
    }
}
