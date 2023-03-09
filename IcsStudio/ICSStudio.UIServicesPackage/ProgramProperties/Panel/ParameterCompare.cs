using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{

    public enum ColumnType
    {
        Name,
        Usage,
        DataType,
        AliasFor,
        BaseTag,
        Description,
        ExternalAccess,
        Constant,
        Connections
    }
    class ParameterCompare:IComparer<ParameterRow>,IComparer
    {
        private readonly bool _descending;
        private readonly ColumnType _columnType;
        public ParameterCompare(bool descending,ColumnType type)
        {
            _descending = descending;
            _columnType = type;
        }

        public ColumnType ColumnType => _columnType;
        public bool Descending => _descending;
        public int Compare(ParameterRow x, ParameterRow y)
        {
            if (string.IsNullOrEmpty(x.Name))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(y.Name))
            {
                return -1;
            }
            switch (_columnType)
            {
                case ColumnType.Name:
                    return FixResult(string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
                case ColumnType.Usage:
                    return FixResult(string.Compare(x.DisplayUsage, y.DisplayUsage, StringComparison.OrdinalIgnoreCase));
                case ColumnType.DataType:
                    return FixResult(string.Compare(x.DataType, y.DataType, StringComparison.OrdinalIgnoreCase));
                case ColumnType.AliasFor:
                    return FixResult(string.Compare(x.AliasFor, y.AliasFor, StringComparison.OrdinalIgnoreCase));
                case ColumnType.BaseTag:
                    return FixResult(string.Compare(x.BaseTag, y.BaseTag, StringComparison.OrdinalIgnoreCase));
                case ColumnType.Description:
                    return FixResult(string.Compare(x.Description, y.Description, StringComparison.OrdinalIgnoreCase));
                case ColumnType.ExternalAccess:
                    return FixResult(string.Compare(x.ExternalAccessDisplay, y.ExternalAccessDisplay, StringComparison.OrdinalIgnoreCase));
                case ColumnType.Constant:
                    return FixResult(BoolCompare(x.IsConstant, y.IsConstant));
                case ColumnType.Connections:
                    return FixResult(string.Compare(x.Connection, y.Connection, StringComparison.OrdinalIgnoreCase));
            }
            return FixResult(string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
        }

        private int BoolCompare(bool a, bool b)
        {
            if (a ^ b)
            {
                if (a) return 1;
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private int FixResult(int res)
        {
            if (_descending)
                return -res;
            return res;
        }

        public int Compare(object x, object y)
        {
            if(!(x is ParameterRow)||!(y is ParameterRow))
                throw new ArgumentException("TagItemComparer can only sort TagItem objects.");
            return Compare((ParameterRow)x, (ParameterRow)y);
        }
    }

    class ConnectionInfoCompare : IComparer<ConnectionInfo>, IComparer
    {
        private readonly bool _descending;
        private readonly ColumnType _columnType;
        public ConnectionInfoCompare(bool descending, ColumnType type)
        {
            _descending = descending;
            _columnType = type;
        }
        public ColumnType ColumnType => _columnType;
        public bool Descending => _descending;
        public int Compare(ConnectionInfo x, ConnectionInfo y)
        {
            if (string.IsNullOrEmpty(x.Name))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(y.Name))
            {
                return -1;
            }
            switch (_columnType)
            {
                case ColumnType.Name:
                    return FixResult(string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
                case ColumnType.Usage:
                    return FixResult(string.Compare(x.UsageDisplay, y.UsageDisplay, StringComparison.OrdinalIgnoreCase));
                case ColumnType.DataType:
                    return FixResult(string.Compare(x.DataType, y.DataType, StringComparison.OrdinalIgnoreCase));
                case ColumnType.AliasFor:
                    return FixResult(string.Compare(x.AliasFor, y.AliasFor, StringComparison.OrdinalIgnoreCase));
                case ColumnType.BaseTag:
                    return FixResult(string.Compare(x.BaseTag, y.BaseTag, StringComparison.OrdinalIgnoreCase));
                case ColumnType.Description:
                    return FixResult(string.Compare(x.Description, y.Description, StringComparison.OrdinalIgnoreCase));
                case ColumnType.ExternalAccess:
                    return FixResult(string.Compare(x.ExternalAccessDisplay, y.ExternalAccessDisplay, StringComparison.OrdinalIgnoreCase));
                case ColumnType.Constant:
                    return FixResult(BoolCompare(x.IsConstant, y.IsConstant));
            }
            return FixResult(string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
        }

        private int BoolCompare(bool a, bool b)
        {
            if (a ^ b)
            {
                if (a) return 1;
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private int FixResult(int res)
        {
            if (_descending)
                return -res;
            return res;
        }

        public int Compare(object x, object y)
        {
            if (!(x is ParameterRow) || !(y is ParameterRow))
                throw new ArgumentException("TagItemComparer can only sort TagItem objects.");
            return Compare((ParameterRow)x, (ParameterRow)y);
        }
    }
}
