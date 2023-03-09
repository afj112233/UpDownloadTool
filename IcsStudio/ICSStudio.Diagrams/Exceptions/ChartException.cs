using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Utils;

namespace ICSStudio.Diagrams.Exceptions
{
    public class ChartException: ICSStudioException
    {
        public ChartException(string message) : base(message)
        {

        }
    }
}
