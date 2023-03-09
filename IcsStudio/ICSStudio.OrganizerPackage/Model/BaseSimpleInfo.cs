using System.Collections.ObjectModel;

namespace ICSStudio.OrganizerPackage.Model
{
    internal abstract class BaseSimpleInfo
    {
        protected BaseSimpleInfo(ObservableCollection<SimpleInfo> infoSource)
        {
            InfoSource = infoSource;
        }

        public ObservableCollection<SimpleInfo> InfoSource { get; protected set; }

        protected SimpleInfo GetSimpleInfo(string name)
        {
            if (InfoSource != null)
            {
                foreach (var simpleInfo in InfoSource)
                {
                    if (string.Equals(simpleInfo.Name, name))
                        return simpleInfo;
                }
            }

            return null;
        }

        protected void SetSimpleInfo(string name, string value)
        {
            var simpleInfo = GetSimpleInfo(name);
            if (simpleInfo != null)
            {
                simpleInfo.Value = value;
            }
        }

        public virtual void Clear()
        {
            InfoSource = null;
        }
    }
}
