using System.Collections.Generic;

namespace ICSStudio.Interfaces.Common
{
    public class Routine
    {
        public Function Logic { get; set; }
        public Function Prescan { get; set; }    
        public List<byte> Pool { get; set; }
    }

    public interface IRoutineCode
    {

    }
}
