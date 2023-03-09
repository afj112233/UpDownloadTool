using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Instruction
{
    public class InstructionException:ICSStudioException
    {
        public InstructionException() : base()
        {

        }

        public InstructionException(string s) : base(s)
        {

        }
    }
}
