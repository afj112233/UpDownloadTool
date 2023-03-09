using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.QuickWatchPackage.View
{
    internal class MonitorContainer: ITagCollectionContainer
    {
        public MonitorContainer()
        {
            Tags=new MonitorTagCollection();
        }
        
        public ITagCollection Tags { get; }
    }
}
