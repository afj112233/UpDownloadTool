using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Compiler
{
    class CompilerException:ICSStudioException
    {
        public CompilerException(string str) : base(str)
        {
        }
    }

    class TypeCheckerException:CompilerException
    {
        public TypeCheckerException(string str="") : base(str)
        {
        }
    }
}
