using System.Collections.Generic;
using System.Linq;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    public class StringListEqual
    {
        public static bool Equal(List<string> list1, List<string> list2)
        {
            int list1Count = 0;
            if (list1 != null)
                list1Count = list1.Count;

            int list2Count = 0;
            if (list2 != null)
                list2Count = list2.Count;

            if (list1Count == list2Count && list1Count == 0)
                return true;

            if (list1Count != list2Count)
                return false;
            
            // ReSharper disable PossibleNullReferenceException
            list1.Sort();
            list2.Sort();
            return list1.SequenceEqual(list2);
            // ReSharper restore PossibleNullReferenceException
        }
    }
}
