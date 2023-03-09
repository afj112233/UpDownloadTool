using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public class AXIS_COMMON : CompositiveType
    {
        protected AXIS_COMMON()
        {
            Name = "$AXIS_COMMON$";
        }

        public static readonly AXIS_COMMON Inst = new AXIS_COMMON();
        public override bool IsStruct => true;
    }

}
