using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Dialogs.SelectDataType
{
    class DisableDataTypes
    {
        public List<string> DisableData = new List<string>
        {
            "USINT","UINT","UDINT","LREAL","LINT",
            "SAFELY_LIMITED_POSITION","SAFELY_LIMITED_SPEED","SAFETY_FEEDBACK_INTERFACE","SAFETY_MAT","SAFE_BRAKE_CONTROL","SAFE_DIRECTION","SAFE_OPERATING_STOP","SAFE_STOP_1","SAFE_STOP_2",
            "SEQUENCE","SEQ_BOOL","SEQ_DINT","SEQ_INT","SEQ_REAL","SEQ_SINT","SEQ_STEP","SEQ_STRING","SEQ_TRANSITION"
        };
    }
}
