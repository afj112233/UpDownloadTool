using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Common
{
    public abstract class BaseComponent : BaseObject, IBaseComponent
    {
        private string _name;
        private string _description;

        public virtual string Name
        {
            get { return _name; }
            set
            {
                OldName = _name;
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string OldName { get; protected set; }

        public virtual string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
        }


        public virtual int InstanceNumber { get; set; }
        public virtual bool IsSafety { get; set; }
        public virtual bool IsTypeLess { get; set; }

        public virtual bool IsDescriptionDefaultLocale()
        {
            return true;
        }

        public virtual Language[] GetDescriptionTranslations()
        {
            return null;
        }
    }
}
