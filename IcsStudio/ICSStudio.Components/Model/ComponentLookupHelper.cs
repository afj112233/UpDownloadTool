using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Components.Model
{
    public class ComponentLookupHelper : IDisposable
    {
        public void Dispose()
        {
        }

        public IEnumerable<string> LookupDataTypes()
        {
            throw new NotImplementedException();
        }

        public string CompleteTagSpecifier(string specifier)
        {
            //throw new NotImplementedException();
            return "";
        }

        public string CompleteTagSpecifier(string specifier, string programScope, bool includeControllerScope)
        {
            throw new NotImplementedException();
        }

        public string CompleteFullyScopedSpecifier(string specifier)
        {
            throw new NotImplementedException();
        }

        public string CompleteModuleSpecifier(string specifier)
        {
            throw new NotImplementedException();
        }
    }
}
