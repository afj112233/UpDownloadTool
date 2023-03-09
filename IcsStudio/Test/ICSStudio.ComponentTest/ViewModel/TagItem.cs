using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace ICSStudio.ComponentTest.ViewModel
{
    public class TagItem : ViewModelBase, IEditableObject
    {
        public TagItem()
        {
            //IsNewItem = true;
            //IsDirty = true;
        }

        private string _tagName;

        public string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        public bool IsNewItem { get; set; }
        public bool IsDirty { get; set; }

        public void BeginEdit()
        {
        }

        public void EndEdit()
        {
        }

        public void CancelEdit()
        {
        }
    }
}
