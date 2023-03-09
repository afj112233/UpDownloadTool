using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Interfaces.Common
{
    public interface ISourceProtected
    {
        bool IsSourceEditable { get; }

        bool IsSourceViewable { get; }

        bool IsSourceCopyable { get; }
    }
}
