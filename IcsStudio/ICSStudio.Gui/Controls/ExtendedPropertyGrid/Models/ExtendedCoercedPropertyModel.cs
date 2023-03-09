using System;
using Imagin.Common;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models
{
    public class ExtendedCoercedPropertyModel<TPrimitive> :
        ExtendedPropertyModel<TPrimitive>, ICoercable, ICoercable<TPrimitive>
    {
        TPrimitive _maximum;

        /// <summary>
        /// 
        /// </summary>
        public TPrimitive Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                OnPropertyChanged("Maximum");
            }
        }

        TPrimitive _minimum;

        /// <summary>
        /// 
        /// </summary>
        public TPrimitive Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                OnPropertyChanged("Minimum");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public void SetConstraint(object minimum, object maximum)
        {
            Maximum = (TPrimitive)Convert.ChangeType(maximum, typeof(TPrimitive));
            Minimum = (TPrimitive)Convert.ChangeType(minimum, typeof(TPrimitive));
        }

        internal ExtendedCoercedPropertyModel()
        {
        }
    }
}
