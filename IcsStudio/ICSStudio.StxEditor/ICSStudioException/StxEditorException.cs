using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Utils;

namespace ICSStudio.StxEditor.ICSStudioException
{
    sealed class StxEditorException: Utils.ICSStudioException
    {
        public StxEditorException(string message) : base(message) {  }
    }
}
