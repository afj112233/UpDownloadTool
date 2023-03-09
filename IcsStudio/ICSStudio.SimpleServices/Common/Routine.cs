using System.Collections.Generic;

namespace ICSStudio.SimpleServices.Common
{
    public class Routine
    {
        public Function Logic { get; set; }
        public Function Prescan { get; set; }    
        public List<byte> Pool { get; set; }
    }
}
