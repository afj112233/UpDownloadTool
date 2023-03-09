using System.ComponentModel;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.StxEditor.Interfaces
{
    public interface IStxEditorOptions : INotifyPropertyChanged
    {
        string FontFamilyName { get; set; }

        double FontSize { get; set; }

        bool ShowDragPreview { get; set; }
        bool ShowLineNumbers { get; set; }
        bool ShowInLineDisplay { get; set; }
        bool ShowOriginal { get; set; }
        bool ShowTest { get; set; }
        bool ShowPending { get; set; }
        bool HideAll { set; get; }
        bool CanZoom { get; set; }
        bool Cleanup { set; get; }
        StxTextItemColor GetItemColor(StxTextItem item);
        bool IsConnecting { set; get; }
        AoiDataReference SelectedDataReference { set; get; }
        bool IsTopLoaded { set; get; }
        bool IsBottomLoaded { set; get; }
        //OnlineEditType GetMode();
        bool IsOnlyTextMarker { set; get; }
        bool CanEditorInput { set; get; }
    }
}