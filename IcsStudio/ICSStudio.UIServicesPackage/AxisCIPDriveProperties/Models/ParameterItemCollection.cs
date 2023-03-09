using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal class ParameterItemCollection : ObservableCollection<ParameterItem>
    {
        private readonly AxisCIPParameters _parameters;

        public ParameterItemCollection(AxisCIPParameters parameters)
        {
            _parameters = parameters;

            InitializeItems();

        }

        private void InitializeItems()
        {
            if (_parameters == null)
                return;

            var propertyInfos =
                _parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<ParameterItem> items = new List<ParameterItem>();
            foreach (var propertyInfo in propertyInfos)
            {
                ParameterItem item =
                    new ParameterItem(_parameters, propertyInfo);

                items.Add(item);
            }

            items.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Refresh()
        {
            foreach (var item in Items)
            {
                item.Refresh();
            }
        }
    }
}
