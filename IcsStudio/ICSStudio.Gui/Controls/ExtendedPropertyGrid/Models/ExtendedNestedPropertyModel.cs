using System;
using System.ComponentModel;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models
{
    public class ExtendedNestedPropertyModel : ExtendedPropertyModel
    {
        /// <summary>
        /// 
        /// </summary>
        public override Type Primitive => typeof(object);

        internal ExtendedNestedPropertyModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnHostPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Do nothing! :-)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected override void OnValueChanged(object value)
        {
            //Do nothing! :-)
        }
    }
}
