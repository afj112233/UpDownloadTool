using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Common
{
    internal class BaseRoutine
    {

    }

    class RoutineComplication
    {
        public Routine Code { get; }
        public string CodePath => _path;
        private volatile string _path = null;

        public RoutineComplication(Routine code)
        {
            Code = code;
        }
    }

    class RoutineCode : IRoutineCode
    {

        public void UpdateCode(Routine code)
        {
            _code = new RoutineComplication(code);
        }

        private volatile RoutineComplication _code = null;
        public Routine Code => _code?.Code;
    }
}
