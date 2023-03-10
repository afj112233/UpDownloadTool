using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ICSStudio.ComponentTest.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ItemCollection = new ObservableCollection<TagItem>
            {
                new TagItem {TagName = "tag0"},
                new TagItem {TagName = "tag1"}
            };
        }

        public ObservableCollection<TagItem> ItemCollection { get; set; }

        public TagItem SelectedItem { get; set; }

    }
}