namespace ICSStudio.UIInterfaces.GlobalClipboard
{
    public interface IGlobalClipboard
    {
        bool CanPasted();
        bool CanCut();
        bool CanCopy();
        void DoPaste();
        void DoCut();
        void DoCopy();
    }
}
