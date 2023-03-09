using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Diagrams.Annotations;

namespace ICSStudio.Diagrams.Flowchart
{
    public sealed class FlowChartEditorOption: INotifyPropertyChanged
    {
        public FlowChartEditorOption()
        {

        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
