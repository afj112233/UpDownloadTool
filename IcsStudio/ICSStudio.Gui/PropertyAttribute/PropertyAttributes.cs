using System;

namespace ICSStudio.Gui.PropertyAttribute
{
    public class PropertyAttributes : IComparable
    {
        public PropertyAttributes(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool IsBrowsable { get; set; }
        public bool IsReadOnly { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Order { get; set; }

        public int CompareTo(object obj)
        {
            PropertyAttributes other = (PropertyAttributes) obj;
            if (Order == other.Order)
            {
                return string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
            }

            return (Order < other.Order) ? -1 : 1;
        }
    }
}
