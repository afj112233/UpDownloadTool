namespace ICSStudio.Gui.Dialogs
{
    public interface IOptionPanel
    {
        object Owner { get; set; }

        object Control
        {
            get;
        }

        void LoadOptions();
        bool SaveOptions();
    }
}
