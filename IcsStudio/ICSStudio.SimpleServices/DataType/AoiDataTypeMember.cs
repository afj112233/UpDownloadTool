using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.SimpleServices.DataType
{
    public sealed class AoiDataTypeMember : DataTypeMember
    {
        public AoiDataTypeMember(ITag aoiTag)
        {
            AoiTag = (Tag)aoiTag;
            PropertyChangedEventManager.AddHandler(AoiTag, AoiTag_PropertyChanged, "");
        }

        public Tag AoiTag { get; }

        public override string Name => AoiTag.Name;

        public override string Description => Tag.GetChildDescription(AoiTag.Description, AoiTag.DataTypeInfo, AoiTag.ChildDescription, Tag.GetOperand(AoiTag.Name), false);

        public void OffMonitor()
        {
            if (AoiTag != null)
            {
                PropertyChangedEventManager.RemoveHandler(AoiTag, AoiTag_PropertyChanged, "");
            }
        }

        public void OnMonitor()
        {
            if (AoiTag != null)
                PropertyChangedEventManager.AddHandler(AoiTag, AoiTag_PropertyChanged, "");
        }

        private void AoiTag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Description")
            {
                var aoi = (AoiDefinition) AoiTag.ParentCollection.ParentProgram;
                aoi.datatype.RaisePropertyChanged("Description");
            }
        }
    }
}
