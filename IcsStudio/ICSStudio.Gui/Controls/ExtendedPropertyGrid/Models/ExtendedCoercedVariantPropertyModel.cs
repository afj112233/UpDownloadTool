// ReSharper disable InconsistentNaming
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable ParameterHidesMember

using Imagin.Common;
using Imagin.Common.Extensions;
using Imagin.Common.Input;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models
{
    class ExtendedCoercedVariantPropertyModel<TVariant, TPrimitive> :
        ExtendedCoercedPropertyModel<TPrimitive> where TVariant : IVariant<TPrimitive>
    {

        bool ValueChangeHandled = false;

        TVariant variant = default(TVariant);

        /// <summary>
        /// 
        /// </summary>
        public TVariant Variant
        {
            get { return variant; }
            set
            {
                variant = value;
                OnPropertyChanged("Variant");
            }
        }

        internal ExtendedCoercedVariantPropertyModel()
        {
            Variant = typeof(TVariant).TryCreate<TVariant>();
            Variant.Changed += OnVariantChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>

        protected override void OnValueChanged(object Value)
        {
            base.OnValueChanged(Value);
            if (!ValueChangeHandled)
                Variant.Set(Value.As<TPrimitive>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnVariantChanged(object sender, EventArgs<TPrimitive> e)
        {
            ValueChangeHandled = true;
            Value = e.Value;
            ValueChangeHandled = false;
        }
    }
}
