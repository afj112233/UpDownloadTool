using System;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
        // ReSharper disable once InconsistentNaming
        private string unit;

        /// <summary>
        /// 
        /// </summary>
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        public UnitAttribute(string unit)
        {
            this.unit = unit;
        }
    }
}
