using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    internal class TagRowCompare : IComparer<BaseTagRow>, IComparer
    {
        private readonly bool _descending;
        private readonly bool _includeMember;

        public TagRowCompare(bool descending, bool includeMember)
        {
            _descending = descending;
            _includeMember = includeMember;
        }
        public int Compare(BaseTagRow x, BaseTagRow y)
        {
            Contract.Assert(x != null);
            Contract.Assert(y != null);

            int result;

            if (x == y)
                return 0;

            if (x.Tag != y.Tag)
            {
                if (x.TmpTag.TmpName == null) return 1;
                if (y.TmpTag.TmpName == null) return -1;
                result = string.Compare(x.TmpTag.TmpName, y.TmpTag.TmpName, StringComparison.OrdinalIgnoreCase);

                if (_descending)
                    return -result;

                return result;
            }

            var xPath = GetPath(x);
            var yPath = GetPath(y);

            int xPathCount = xPath.Count;
            int yPathCount = yPath.Count;

            Contract.Assert(xPathCount >= 1);
            Contract.Assert(yPathCount >= 1);

            if (xPathCount < yPathCount && xPath[xPathCount - 1] == yPath[xPathCount - 1])
                return -1;

            if (yPathCount < xPathCount && yPath[yPathCount - 1] == xPath[yPathCount - 1])
                return 1;

            //
            int minCount = Math.Min(xPathCount, yPathCount);

            var xItem = xPath[minCount - 1];
            var yItem = yPath[minCount - 1];
            while (xItem.Parent != yItem.Parent)
            {
                xItem = xItem.Parent;
                yItem = yItem.Parent;
            }

            var parentItem = xItem.Parent;
            if (parentItem.DataTypeL is AssetDefinedDataType||parentItem.DataTypeL.FamilyType==FamilyType.StringFamily)
            {
                if (_includeMember)
                {
                    result = string.Compare(xItem.Name, yItem.Name, StringComparison.OrdinalIgnoreCase);
                    if (_descending)
                        return -result;

                    return result;
                }

                return xItem.SortMemberIndex - yItem.SortMemberIndex;
            }

            if (parentItem.DataTypeL.IsInteger)
            {
                if ((xItem.DataTypeL.IsBool
                     && yItem.DataTypeL.IsBool))
                {
                    return xItem.SortBitOffset - yItem.SortBitOffset;
                }
            }

            if (parentItem.DataType.IndexOf("[")>0)
            {
                return xItem.SortDimIndex - yItem.SortDimIndex;
            }

            throw new NotImplementedException();
        }

        private List<BaseTagRow> GetPath(BaseTagRow item)
        {
            List<BaseTagRow> path = new List<BaseTagRow>();

            do
            {
                path.Add(item);
                item = item.Parent;

            } while (item?.Name != null);

            path.Reverse();

            return path;
        }
        
        public int Compare(object x, object y)
        {
            if (!(x is BaseTagRow) && !(y is BaseTagRow))
            {
                throw new ArgumentException("TagRowComparer can only sort BaseTagRow objects.");
            }

            var r = Compare(x as BaseTagRow, y as BaseTagRow);
            return r;
        }
        
    }
}
