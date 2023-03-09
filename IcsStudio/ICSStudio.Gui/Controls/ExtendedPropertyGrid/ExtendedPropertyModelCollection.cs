using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models;
using Imagin.Common.Attributes;
using Imagin.Common.Collections.Concurrent;
using Imagin.Common.Extensions;
using Imagin.Common.Primitives;
using PropertyAttributes = Imagin.Controls.Extended.PropertyAttributes;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid
{
    public class ExtendedPropertyModelCollection :
        ConcurrentCollection<ExtendedPropertyModel>
    {
        private readonly object _root = new object();

        #region Properties

        ExtendedPropertyModel _activeProperty;

        /// <summary>
        /// The active, or selected, property.
        /// </summary>
        public ExtendedPropertyModel ActiveProperty
        {
            get { return _activeProperty; }
            set
            {
                _activeProperty = value;
                OnPropertyChanged("ActiveProperty");
            }
        }

        ExtendedPropertyModel _featured;

        /// <summary>
        /// Gets the featured property, which is placed above all others.
        /// </summary>
        public ExtendedPropertyModel Featured
        {
            get { return _featured; }
            private set
            {
                _featured = value;
                OnPropertyChanged("Featured");
            }
        }

        /// <summary>
        /// Gets or sets the object that is currently hosted.
        /// </summary>
        public object Object;

        #endregion

        #region PropertyModelCollection

        #endregion

        #region Methods

        /// <summary>
        /// If the property is public (i.e., have public getter AND setter) AND:
        /// 
        /// a) Has a type that is supported (null able or not), 
        /// b) Is <see cref="Enum"/>, or 
        /// c) Implements <see cref="IList"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        bool IsSupported(PropertyInfo property)
        {
            //If property has a public getter, with or without a setter...
            var a = property.GetGetMethod(false) != null;

            var t = property.PropertyType;

            if (t.IsNullable())
                t = t.GetGenericArguments().WhereFirst(i => true);

            var b = t.EqualsAny(ExtendedPropertyModel.SupportedTypes);

            b = b || t.IsEnum;
            b = b || t.Implements<IList>();

            return a && b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected override void OnItemAdded(ExtendedPropertyModel item)
        {
            base.OnItemAdded(item);

            if (item.IsFeatured)
                Featured = item;
        }

        /// <summary>
        /// Set and add custom properties.
        /// </summary>
        /// <param name="source">A function that enumerates an object and returns a list of property models.</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task BeginFrom(Func<object,
                IEnumerable<ExtendedPropertyModel>> source,
            Action callback = null)
        {
            var i = Object;
            await Task.Run(() =>
            {
                var properties = source(i);
                if (properties != null)
                {
                    foreach (var j in properties)
                        Add(j);
                }
            });

            callback.InvokeIf(x => !x.IsNull());
        }

        /// <summary>
        /// Set properties by enumerating the properties of an object.
        /// </summary>
        /// <param name="objectInput">The object to examine.</param>
        /// <param name="callback">What to do afterwards.</param>
        /// <remarks>
        /// TO-DO: Evaluate dynamic properties if the object implements a certain interface? 
        /// 
        /// Dynamic properties would be properties that don't need to be owned by the object
        /// and cannot be modified, but should be displayed to the user anyway. 
        /// 
        /// The object would have to specify how to get the value for each dynamic property 
        /// internally using an action; the action simply returns the object we want.
        /// 
        /// The interface would expose a method that accepts the latter-described action,
        /// invokes it, and returns the resulting object (the current value of the 
        /// dynamic property). Note, this enables you to calculate the value for the 
        /// dynamic property however you like.
        /// 
        /// If the object implements this interface, we can safely check for dynamic 
        /// properties. While enumerating, we'd get the initial value; subsequently,
        /// we'd need a way of updating it when it should be (another TO-DO).
        /// 
        /// Assuming each dynamic property specifies a general type, we'll know what type
        /// to cast to when retrieving the it's value.
        /// 
        /// Ultimately, this would enable you to display properties in addition to the ones
        /// the object already owns without the additional overhead.
        /// </remarks>
        public async Task BeginFromObject(object objectInput, Action callback = null)
        {
            await Task.Run(() =>
            {
                var properties = objectInput.GetType().GetProperties();

                for (int i = 0, length = properties.Length; i < length; i++)
                {
                    var property = properties[i];

                    var isNested = false;
                    //If the type isn't explicitly supported...
                    if (!IsSupported(property))
                    {
                        //...and it's not a reference type, skip it
                        if (property.PropertyType.IsValueType)
                        {
                            continue;
                        }
                        //...and it is a reference type, consider it "nested"
                        else isNested = true;
                    }

                    var a = new PropertyAttributes()
                    {
                        {typeof(BrowsableAttribute), "Browsable", true},
                        {typeof(CategoryAttribute), "Category", string.Empty},
                        {typeof(ConstraintAttribute), null, null},
                        {typeof(DescriptionAttribute), "Description", string.Empty},
                        {typeof(DisplayNameAttribute), "DisplayName", string.Empty},
                        {typeof(FeaturedAttribute), "IsFeatured", false},
                        {typeof(Int64KindAttribute), "Kind", Int64Kind.Default},
                        {typeof(ReadOnlyAttribute), "IsReadOnly", false},
                        {typeof(StringKindAttribute), "Kind", StringKind.Default},
                        {typeof(StringFormatAttribute), "Format", string.Empty},
                        {typeof(UnitAttribute), "Unit", string.Empty},
                        {typeof(IsChangedAttribute), "IsChanged", false}
                    };

                    a.ExtractFrom(property);

                    if (a.Get<BrowsableAttribute, bool>())
                    {
                        var model = ExtendedPropertyModel.New(objectInput, property, a, isNested);

                        if (model != null)
                            Add(model);
                    }
                }
            });

            callback.InvokeIf(x => !x.IsNull());
        }

        public async Task BeginFromObjectExtended(object objectInput, Action callback = null)
        {
            await Task.Run(() =>
                {
                    lock (_root)
                    {
                        BeginFromObjectExtendedMethod(objectInput);
                    }
                   
                }
            );
            
            callback.InvokeIf(x => !x.IsNull());
        }

        public void Refresh()
        {
            lock (_root)
            {
                foreach (ExtendedPropertyModel extendedPropertyModel in this)
                {
                    extendedPropertyModel.Refresh();
                }
            }
        }

        public void BeginFromObjectExtendedMethod(object objectInput)
        {
           
            var properties = objectInput.GetType().GetProperties();
            var tdProperties = TypeDescriptor.GetProperties(objectInput);

            for (int i = 0, length = properties.Length; i < length; i++)
            {
                var property = properties[i];
                var tdProperty = tdProperties[property.Name];
                var tdAttributes = tdProperty.Attributes;

                if (!tdProperty.IsBrowsable)
                    continue;

                //If the type isn't explicitly supported...
                if (!IsSupported(property))
                {
                    BeginFromObjectExtendedMethod(property.GetValue(objectInput));
                    continue;
                }

                var a = new PropertyAttributes()
                {
                    {typeof(BrowsableAttribute), "Browsable", true},
                    {typeof(CategoryAttribute), "Category", string.Empty},
                    {typeof(ConstraintAttribute), null, null},
                    {typeof(DescriptionAttribute), "Description", string.Empty},
                    {typeof(DisplayNameAttribute), "DisplayName", string.Empty},
                    {typeof(FeaturedAttribute), "IsFeatured", false},
                    {typeof(Int64KindAttribute), "Kind", Int64Kind.Default},
                    {typeof(ReadOnlyAttribute), "IsReadOnly", false},
                    {typeof(StringKindAttribute), "Kind", StringKind.Default},
                    {typeof(StringFormatAttribute), "Format", string.Empty},
                    {typeof(UnitAttribute), "Unit", string.Empty},
                    {typeof(IsChangedAttribute), "IsChanged", false}
                };

                //a.ExtractFrom(property);
                ExtractFrom(a, tdAttributes);

                if (a.Get<BrowsableAttribute, bool>())
                {
                    var model = ExtendedPropertyModel.New(objectInput, property, a, false);
                    
                    if (model != null)
                        Add(model);
                }
            }
        }

        private void ExtractFrom(
            PropertyAttributes propertyAttributes, 
            AttributeCollection attributeCollection)
        {
            foreach (Attribute attribute in attributeCollection)
            {
                var j = propertyAttributes.WhereFirst(k => k.First == attribute.GetType());

                if (j != null)
                    j.Third = j.Second == null ? attribute : attribute.GetValue(j.Second);
            }
        }

        /// <summary>
        /// Set properties by enumerating a resource dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to enumerate.</param>
        /// <param name="callback">What to do afterwards.</param>
        public async Task BeginFromResourceDictionary(
            ResourceDictionary dictionary, Action callback = null)
        {
            if (dictionary == null) return;

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                foreach (DictionaryEntry i in dictionary)
                {
                    if (i.Value != null)
                    {
                        var type = i.Value.GetType();
                        if (type.EqualsAny(typeof(LinearGradientBrush), typeof(SolidColorBrush)))
                        {
                            var model = ExtendedPropertyModel.New(type, dictionary, i.Key.ToString(), i.Value,
                                type.Name.SplitCamelCase(), string.Empty, string.Empty, false, false);

                            if (model != null)
                                Add(model);
                        }
                    }
                }
            }));

            callback.InvokeIf(x => !x.IsNull());
        }

        /// <summary>
        /// Clear all properties and assign a reference to the given object, if any.
        /// </summary>
        /// <param name="value"></param>
        public void Reset(object value = null)
        {
            lock (_root)
            {
                Featured = null;
                Object = value;
                Clear();
            }

        }

        #endregion
    }
}
