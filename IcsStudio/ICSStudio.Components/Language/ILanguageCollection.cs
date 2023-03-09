using System.Collections.ObjectModel;

namespace ICSStudio.Components.Language
{
    interface ILanguageCollection
    {
        ObservableCollection<LanguageVM> Languages { get; set; }
    }
}
