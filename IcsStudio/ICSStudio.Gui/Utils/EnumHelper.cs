using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ICSStudio.Gui.Utils
{
    public class EnumHelper
    {
        public static IList ToDataSource<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(value =>
                {
                    var e = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;

                    if (e != null)
                        return new
                        {
                            DisplayName = e.Value,
                            Value = value
                        };

                    return new
                    {
                        DisplayName = value.ToString(),
                        Value = value
                    };

                })
                .OrderBy(item => item.Value)
                .ToList();
        }

        public static IList ToDataSource<T>(IList enumValues)
        {
            if ((enumValues == null) || (enumValues.Count == 0))
                return null;

            var enumValueList = enumValues as List<T>;

            return enumValueList?.Select(value =>
                {
                    var e = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;

                    if (e != null)
                        return new
                        {
                            DisplayName = e.Value,
                            Value = value
                        };

                    return new
                    {
                        DisplayName = value.ToString(),
                        Value = value
                    };
                }
            ).OrderBy(item => item.Value).ToList();
        }

        public static string GetEnumMember<T>(T enumValue)
        {
            var e = Attribute.GetCustomAttribute(enumValue.GetType().GetField(enumValue.ToString()),
                typeof(EnumMemberAttribute)) as EnumMemberAttribute;

            if (e == null)
                return enumValue.ToString();

            return e.Value;
        }

        public static IList ToDataSourceOrderByEnumMember<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(value =>
                {
                    var e = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;

                    if (e != null)
                        return new
                        {
                            DisplayName = e.Value,
                            Value = value
                        };

                    return new
                    {
                        DisplayName = value.ToString(),
                        Value = value
                    };
                })
                .OrderBy(item => item.DisplayName)
                .ToList();
        }

        public static IList ToSingleDataSource<T>(T enumValue)
        {
            var e = Attribute.GetCustomAttribute(enumValue.GetType().GetField(enumValue.ToString()),
                typeof(EnumMemberAttribute)) as EnumMemberAttribute;

            if (e == null)
                return null;

            var result = new[]
            {
                new {DisplayName = e.Value, Value = enumValue}
            };

            return result.ToList();
        }

        public static ObservableCollection<DisplayItem<T>> ToObservableCollectionSource<T>()
        {
            List<DisplayItem<T>> dataSourceList = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(value =>
                {
                    var e = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;

                    if (e != null)
                        return new DisplayItem<T>
                        {
                            DisplayName = e.Value,
                            Value = value
                        };


                    return new DisplayItem<T>
                    {
                        DisplayName = value.ToString(),
                        Value = value
                    };

                })
                .OrderBy(item => item.Value)
                .ToList();

            return new ObservableCollection<DisplayItem<T>>(dataSourceList);
        }
    }
}
