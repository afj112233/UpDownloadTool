using Imagin.Common.Extensions;
using System;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models
{
    public class ExtendedPropertyModel<T> : ExtendedPropertyModel
    {
        /// <summary>
        /// Gets the default value of the property.
        /// </summary>
        protected virtual T Default => default(T);

        /// <summary>
        /// Gets the underlying property type.
        /// </summary>
        public override Type Primitive => typeof(T);

        internal ExtendedPropertyModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        protected override object OnPreviewValueChanged(object oldValue, object newValue)
        {
            if (newValue == null)
            {
                return Default;
            }
            else
            {
                object result;
                switch (typeof(T).Name.ToLower())
                {
                    case "byte":
                        result = newValue.ToString().ToByte();
                        break;
                    case "decimal":
                        result = newValue.ToString().ToDecimal();
                        break;
                    case "double":
                        result = newValue.ToString().ToDouble();
                        break;
                    case "guid":
                        Guid g;
                        result = Guid.TryParse(newValue.ToString(), out g) ? g : new Guid(oldValue.ToString());
                        break;
                    case "int16":
                        result = newValue.ToString().ToInt16();
                        break;
                    case "int32":
                        result = newValue.ToString().ToInt32();
                        break;
                    case "int64":
                        result = newValue.ToString().ToInt64();
                        break;
                    case "uri":
                        Uri u;
                        result = Uri.TryCreate(newValue.ToString(), UriKind.Absolute, out u)
                            ? u
                            : new Uri(oldValue.ToString());
                        break;
                    case "version":
                        Version v;
                        result = Version.TryParse(newValue.ToString(), out v) ? v : new Version(oldValue.ToString());
                        break;
                    default:
                        result = (T) newValue;
                        break;
                }
                return result.To<T>();
            }
        }
    }
}
