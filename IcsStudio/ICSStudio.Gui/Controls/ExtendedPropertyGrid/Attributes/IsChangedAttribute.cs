using System;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IsChangedAttribute : Attribute
    {
        // ReSharper disable once InconsistentNaming
        private bool isChanged;

        /// <summary>
        /// 
        /// </summary>
        public bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value; }
        }
        public IsChangedAttribute(bool isChanged)
        {
            this.isChanged = isChanged;
        }
    }
}
