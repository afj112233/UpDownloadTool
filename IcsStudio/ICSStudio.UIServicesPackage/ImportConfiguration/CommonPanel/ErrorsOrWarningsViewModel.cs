using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.CommonPanel
{
    public class ErrorsOrWarningsViewModel:ViewModelBase,IVerify
    {
        public string Error { get; set; }
    }
}
