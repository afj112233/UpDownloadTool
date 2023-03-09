using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using ICSStudio.Gui.Annotations;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    internal class ParameterItem : INotifyPropertyChanged, IDisposable
    {
        private readonly AxisCIPParameters _host;
        private readonly PropertyInfo _info;

        private bool _disposed;


        private bool _isVisible;
        private bool _isReadOnly;
        private bool _isChanged;
        private string _unit;
        private object _value;

        public ParameterItem(
            AxisCIPParameters host,
            PropertyInfo info)
        {
            _host = host;
            _info = info;

            Name = _info.Name;

            PropertyChangedEventManager.AddHandler(_host, OnHostPropertyChanged, Name);

            Refresh();
        }

        ~ParameterItem()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            PropertyChangedEventManager.RemoveHandler(_host, OnHostPropertyChanged, Name);

            _disposed = true;
        }

        public Visibility Visibility
        {
            get
            {
                if (IsVisible)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public bool IsEnabled => !IsReadOnly;

        public bool IsVisible
        {
            get { return _isVisible; }
            private set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Visibility");
                }
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            private set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            private set
            {
                if (_isChanged != value)
                {
                    _isChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name { get; }

        public string Unit
        {
            get { return _unit; }
            private set
            {
                if (!string.Equals(_unit, value))
                {
                    _unit = value;
                    OnPropertyChanged();
                }
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _info.SetValue(_host, _value);
                OnPropertyChanged();
            }
        }

        public object EnumSource { get; set; }

        public string Category { get; private set; }

        public Type ParameterType => _info.PropertyType;

        public void Refresh()
        {
            var descriptors = TypeDescriptor.GetProperties(_host);
            var descriptor = descriptors[Name];

            foreach (Attribute attribute in descriptor.Attributes)
            {
                BrowsableAttribute browsableAttribute = attribute as BrowsableAttribute;
                if (browsableAttribute != null)
                {
                    IsVisible = browsableAttribute.Browsable;
                }

                ReadOnlyAttribute readOnlyAttribute = attribute as ReadOnlyAttribute;
                if (readOnlyAttribute != null)
                {
                    IsReadOnly = readOnlyAttribute.IsReadOnly;
                }

                IsChangedAttribute isChangedAttribute = attribute as IsChangedAttribute;
                if (isChangedAttribute != null)
                {
                    IsChanged = isChangedAttribute.IsChanged;
                }

                UnitAttribute unitAttribute = attribute as UnitAttribute;
                if (unitAttribute != null)
                {
                    Unit = unitAttribute.Unit;
                }

                CategoryAttribute categoryAttribute = attribute as CategoryAttribute;
                if (categoryAttribute != null)
                {
                    Category = categoryAttribute.Category;
                }

            }

            if (_info.PropertyType.IsEnum)
            {
                var sourceProperty = _host.GetType().GetProperty(Name + "Source",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Contract.Assert(sourceProperty != null);

                EnumSource = sourceProperty.GetValue(_host);
                OnPropertyChanged("EnumSource");
            }


            _value = default(object);
            try
            {
                _value = _info.GetValue(_host);
                OnPropertyChanged("Value");
            }
            catch (Exception)
            {
                // nothing
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnHostPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                _value = _info.GetValue(_host);
                OnPropertyChanged("Value");
            }
            catch (Exception)
            {
                // nothing
            }
        }
    }
}
