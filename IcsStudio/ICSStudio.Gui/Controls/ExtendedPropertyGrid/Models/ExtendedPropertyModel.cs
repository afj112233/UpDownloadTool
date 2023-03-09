// ReSharper disable InconsistentNaming
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable ParameterHidesMember

using System;
using System.Collections;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using Imagin.Common;
using Imagin.Common.Attributes;
using Imagin.Common.Extensions;
using Imagin.Common.Primitives;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models
{
    public abstract class ExtendedPropertyModel : NamedObject, IDisposable
    {
        #region Properties

        bool disposed = false;

        bool hostPropertyChangeHandled = false;

        bool valueChangeHandled = false;

        /// <summary>
        /// A list of all supported types.
        /// </summary>
        public static object[] SupportedTypes => new object[]
        {
            typeof(bool),
            typeof(byte),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(ushort),
            typeof(uint),
            typeof(string),
        };

        string category = string.Empty;

        /// <summary>
        /// Gets or sets the property category.
        /// </summary>
        public string Category
        {
            get { return category; }
            private set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        string description = string.Empty;

        /// <summary>
        /// Gets or sets the property description.
        /// </summary>
        public string Description
        {
            get { return description; }
            private set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        object host = null;

        /// <summary>
        /// Gets or sets the object this property belongs to.
        /// </summary>
        public object Host
        {
            get { return host; }
            set
            {
                host = value;
                OnHostChanged(host);
                OnPropertyChanged("Host");
            }
        }

        PropertyInfo info = null;

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the property.
        /// </summary>
        public PropertyInfo Info
        {
            get { return info; }
            private set
            {
                info = value;
                OnPropertyChanged("Info");

                //If the property has a public setter, it is settable.
                IsSettable = value?.GetSetMethod(true) != null;
            }
        }

        bool isFeatured = false;

        /// <summary>
        /// Gets whether or not the property is featured.
        /// </summary>
        public bool IsFeatured
        {
            get { return isFeatured; }
            private set
            {
                isFeatured = value;
                OnPropertyChanged("IsFeatured");
            }
        }

        bool isReadOnly = false;

        /// <summary>
        /// Gets whether or not the property is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            private set
            {
                isReadOnly = value;
                OnPropertyChanged("IsReadOnly");
            }
        }

        bool isSettable = false;

        /// <summary>
        /// Gets whether or not the property has a public setter; if it doesn't, the property is automatically readonly.
        /// </summary>
        public bool IsSettable
        {
            get { return isSettable; }
            private set
            {
                isSettable = value;

                //If NOT settable, it is readonly; else, it is whatever was already specified.
                IsReadOnly = value ? isReadOnly : true;
            }
        }

        /// <summary>
        /// Gets the actual property type (note, if the property is nullable, this will be the underlying type).
        /// </summary>
        public abstract Type Primitive { get; }

        string stringFormat = string.Empty;

        /// <summary>
        /// Gets or sets the string format for the property.
        /// </summary>
        public string StringFormat
        {
            get { return stringFormat; }
            set
            {
                stringFormat = value;
                OnPropertyChanged("StringFormat");
            }
        }

        object tag = null;

        /// <summary>
        /// An object for general use.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                OnPropertyChanged("Tag");
            }
        }

        /// <summary>
        /// Gets the representation of the property's type.
        /// </summary>
        /// <remarks>
        /// Used for sorting only.
        /// </remarks>
        public string Type => Primitive.Name;

        object _value = null;

        /// <summary>
        /// Gets or sets the current value for the property.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set
            {
                //If the property is settable OR we're making an internal change...
                if (IsSettable || valueChangeHandled)
                {
                    _value = OnPreviewValueChanged(_value, value);
                    OnPropertyChanged("Value");
                    OnValueChanged(_value);
                }
            }
        }

        // add by gjc
        private object _oldValue = null;

        public object OldValue
        {
            get { return _oldValue; }
            set
            {
                _oldValue = value;
                OnPropertyChanged("OldValue");
            }
        }



        string unit = string.Empty;

        /// <summary>
        /// Gets or sets the string format for the property.
        /// </summary>
        public string Unit
        {
            get { return unit; }
            set
            {
                unit = value;
                OnPropertyChanged("Unit");
            }
        }


        private bool isChanged = false;

        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                isChanged = value;
                OnPropertyChanged("IsChanged");
            }
        }

        private object enumSource = null;

        public object EnumSource
        {
            get { return enumSource; }
            set
            {
                enumSource = value;
                OnPropertyChanged("EnumSource");
            }
        }

        // end add

        #endregion

        #region PropertyModel

        protected ExtendedPropertyModel()
        {
            OnPropertyChanged("Type");
        }

        /// <summary>
        /// 
        /// </summary>
        ~ExtendedPropertyModel()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        public void Refresh()
        {
            if (Host == null || Info == null)
                return;

            var tdProperties = TypeDescriptor.GetProperties(Host);
            var tdAttributes = tdProperties[info.Name].Attributes;

            foreach (Attribute tdAttribute in tdAttributes)
            {
                if (tdAttribute is ReadOnlyAttribute)
                {
                    IsReadOnly = (bool) tdAttribute.GetValue("IsReadOnly");
                }

                if (tdAttribute is UnitAttribute)
                {
                    Unit = (string) tdAttribute.GetValue("Unit");
                }

                if (tdAttribute is IsChangedAttribute)
                {
                    IsChanged = (bool) tdAttribute.GetValue("IsChanged");
                }
            }

            // refresh enum
            if (this is ExtendedPropertyModel<Enum>)
            {
                var sourceProperty = Host.GetType().GetProperty(info.Name + "Source");
                if (sourceProperty != null)
                {
                    EnumSource = sourceProperty.GetValue(Host);
                }

            }

            _value = OnPreviewValueChanged(_value, Info.GetValue(Host));
            OnPropertyChanged("Value");
        }

        /// <summary>
        /// Initializes a new instance of the class based on given type. 
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        static ExtendedPropertyModel New(Type Type)
        {
            //If type is null able, get underlying type!
            if (Type.IsNullable())
                Type = Type.GetGenericArguments().WhereFirst(i => true);

            if (Type == typeof(bool))
                return new ExtendedPropertyModel<bool>();

            if (Type == typeof(byte))
                return new ExtendedCoercedPropertyModel<byte>();

            if (typeof(IList).IsAssignableFrom(Type))
                return new ExtendedPropertyModel<IList>();

            if (Type == typeof(DateTime))
                return new ExtendedCoercedPropertyModel<DateTime>();

            if (Type == typeof(decimal))
                return new ExtendedCoercedPropertyModel<decimal>();

            if (Type == typeof(double))
                return new ExtendedCoercedPropertyModel<double>();

            if (Type == typeof(float))
                return new ExtendedCoercedPropertyModel<float>();

            if (Type.IsEnum)
                return new ExtendedPropertyModel<Enum>();

            if (Type == typeof(Guid))
                return new ExtendedPropertyModel<Guid>();

            if (Type == typeof(int))
                return new ExtendedCoercedPropertyModel<int>();

            if (Type == typeof(long))
                return new ExtendedCoercedPropertyModel<long>();

            if (Type == typeof(ushort))
                return new ExtendedCoercedPropertyModel<ushort>();

            if (Type == typeof(uint))
                return new ExtendedCoercedPropertyModel<uint>();

            if (Type == typeof(LinearGradientBrush))
                return new ExtendedPropertyModel<Brush>();

            if (Type == typeof(NetworkCredential))
                return new ExtendedPropertyModel<NetworkCredential>();

            if (Type == typeof(Point))
                return new ExtendedCoercedVariantPropertyModel<Position, Point>();

            if (Type == typeof(RadialGradientBrush))
                return new ExtendedPropertyModel<Brush>();

            if (Type == typeof(short))
                return new ExtendedCoercedPropertyModel<short>();

            if (Type == typeof(Size))
                return new ExtendedCoercedVariantPropertyModel<Proportions, Size>();

            if (Type == typeof(SolidColorBrush))
                return new ExtendedPropertyModel<SolidColorBrush>();

            if (Type == typeof(string))
                return new ExtendedPropertyModel<string>();

            if (Type == typeof(Uri))
                return new ExtendedPropertyModel<Uri>();

            if (Type == typeof(Version))
                return new ExtendedCoercedVariantPropertyModel<Release, Version>();

            if (Type == typeof(object))
                return new ExtendedNestedPropertyModel();

            return null;
        }

        /// <summary>
        /// Initializes a new instance of the class based on given data; some restrictions are observed.
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Property"></param>
        /// <param name="Attributes"></param>
        /// <param name="IsNested"></param>
        /// <returns></returns>
        internal static ExtendedPropertyModel New(object Host, PropertyInfo Property,
            Imagin.Controls.Extended.PropertyAttributes Attributes, bool IsNested)
        {
            var Result = New(IsNested ? typeof(object) : Property.PropertyType);

            if (Result != null)
            {
                Result.Info = Property;

                var Name = Attributes.Get<DisplayNameAttribute, string>();
                Name = Name.IsNullOrEmpty() ? Property.Name : Name;

                var Value = default(object);

                try
                {
                    //Will fail if locked
                    Value = Property.GetValue(Host);
                }
                catch (Exception)
                {
                    //Do nothing!
                }

                //Set the important stuff first
                Result.Host = Host;
                Result.Name = Name;
                Result.Value = Value;
                Result.OldValue = Value;

                //Set the minor stuff
                Result.Category = Attributes.Get<CategoryAttribute, string>();
                Result.Description = Attributes.Get<DescriptionAttribute, string>();
                Result.StringFormat = Attributes.Get<StringFormatAttribute, string>();
                Result.IsFeatured = Attributes.Get<FeaturedAttribute, bool>();

                // add by gjc
                Result.Unit = Attributes.Get<UnitAttribute, string>();
                Result.IsChanged = Attributes.Get<IsChangedAttribute, bool>();

                if (Result is ExtendedPropertyModel<Enum>)
                {
                    var sourceProperty = Host.GetType().GetProperty(Property.Name + "Source");
                    if (sourceProperty != null)
                    {
                        Result.EnumSource = sourceProperty.GetValue(Host);
                    }

                }
                // end add

                //Honor the attribute if the property is settable; if it isn't, it is automatically readonly and should always be!
                if (Result.IsSettable)
                    Result.IsReadOnly = Attributes.Get<ReadOnlyAttribute, bool>();

                if (Result is ExtendedPropertyModel<string>)
                    Result.As<ExtendedPropertyModel<string>>().Tag = Attributes.Get<StringKindAttribute, StringKind>();

                if (Result is ExtendedPropertyModel<long>)
                    Result.As<ExtendedPropertyModel<long>>().Tag = Attributes.Get<Int64KindAttribute, Int64Kind>();

                //Honor constraints
                if (Result is ICoercable)
                {
                    var Constraint = Attributes.Get<ConstraintAttribute, ConstraintAttribute>();

                    if (Constraint != null)
                        Result.As<ICoercable>().SetConstraint(Constraint.Minimum, Constraint.Maximum);
                }
            }

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the class based on given type and values; no restrictions.
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Host"></param>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="Category"></param>
        /// <param name="Description"></param>
        /// <param name="StringFormat"></param>
        /// <param name="IsReadOnly"></param>
        /// <param name="IsFeatured"></param>
        /// <returns></returns>
        internal static ExtendedPropertyModel New(Type Type, object Host, string Name, object Value, string Category,
            string Description, string StringFormat, bool IsReadOnly, bool IsFeatured)
        {
            var Result = New(Type);

            if (Result != null)
            {
                Result.Host = Host;
                Result.Name = Name;
                Result.Value = Value;
                Result.Category = Category;
                Result.Description = Description;
                Result.StringFormat = StringFormat;
                Result.IsReadOnly = IsReadOnly;
                Result.IsFeatured = IsFeatured;
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
            }

            if (Host is INotifyPropertyChanged)
                Host.As<INotifyPropertyChanged>().PropertyChanged -= OnHostPropertyChanged;

            disposed = true;
        }

        /// <summary>
        /// Occurs when the host object changes.
        /// </summary>
        /// <param name="Value"></param>

        protected virtual void OnHostChanged(object Value)
        {
            if (Value is INotifyPropertyChanged)
                Value.As<INotifyPropertyChanged>().PropertyChanged += OnHostPropertyChanged;
        }

        /// <summary>
        /// Occurs if the host object implements <see cref="INotifyPropertyChanged"/> and one of it's properties has changed; this is necessary to capture external changes while the object is a host.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHostPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //If the property that changed is modelled by the current instance...
            if (!hostPropertyChangeHandled && e.PropertyName == Name && Info != null)
            {
                //Update the property of the current instance
                valueChangeHandled = true;
                //If the property is NOT settable, it is still okay to set here.
                Value = Info.GetValue(Host);
                valueChangeHandled = false;
            }
        }

        /// <summary>
        /// Occurs just before setting the value.
        /// </summary>
        /// <param name="OldValue">The original value.</param>
        /// <param name="NewValue">The new value.</param>
        /// <returns>The actual value to set for the property.</returns>
        protected virtual object OnPreviewValueChanged(object OldValue, object NewValue)
        {
            return NewValue;
        }

        /// <summary>
        /// Occurs when the value changes.
        /// </summary>
        /// <param name="Value">The new value.</param>
        protected virtual void OnValueChanged(object Value)
        {
            if (Host is ResourceDictionary)
            {
                if (Host.As<ResourceDictionary>().Contains(Name))
                    Host.As<ResourceDictionary>()[Name] = Value;
            }
            //If the property is NOT settable, this condition will (or should) always be false.
            else if (!valueChangeHandled && Info != null)
            {
                hostPropertyChangeHandled = true;
                Info.SetValue(Host, Value, null);
                hostPropertyChangeHandled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
