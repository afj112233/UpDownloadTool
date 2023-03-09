using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Cip.DataTypes;

namespace ICSStudio.Cip.Objects
{
    public class CipAttributeHelper
    {
        private static readonly Dictionary<Type, Dictionary<ushort, string>> Cache;

        static CipAttributeHelper()
        {
            Cache = new Dictionary<Type, Dictionary<ushort, string>>();
        }

        public static Dictionary<ushort, string> GetAttributeMap(Type T)
        {
            if (Cache.ContainsKey(T))
                return Cache[T];

            // Create Attribute Map
            var attributeMap = CreateAttributeMap(T);

            Cache.Add(T, attributeMap);

            return attributeMap;
        }

        private static Dictionary<ushort, string> CreateAttributeMap(Type T)
        {
            Debug.WriteLine("CreateAttributeMap " + T.Name);

            var attributeMap = new Dictionary<ushort, string>();

            var properties = T.GetProperties();
            foreach (var p in properties)
            {
                var cipDetailInfo =
                    (CipDetailInfoAttribute)
                    p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                if (cipDetailInfo != null)
                    attributeMap.Add(cipDetailInfo.Id, p.Name);
            }

            return attributeMap;
        }

        public static bool EqualByAttributeNames<T>(T cipObject1, T cipObject2, string[] attributeNames)
        {
            if (attributeNames == null)
                return false;

            foreach (var attributeName in attributeNames)
            {
                var p = typeof(T).GetProperty(attributeName);
                if (p == null)
                    throw new ArgumentException($"Do not have {attributeName}");

                if (CompareTo(p.GetValue(cipObject1), p.GetValue(cipObject2)) != 0)
                    return false;
            }

            return true;
        }

        public static bool EqualByAttributeName<T>(T cipObject1, T cipObject2, string attributeName)
        {
            var p = typeof(T).GetProperty(attributeName);
            if (p == null)
                throw new ArgumentException($"Do not have {attributeName}");

            if (CompareTo(p.GetValue(cipObject1), p.GetValue(cipObject2)) != 0)
                return false;

            return true;
        }


        public static List<ushort> GetDifferentAttributeList<T>(T cipObject1, T cipObject2)
        {
            var differentList = new List<ushort>();

            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                var cipDetailInfo =
                    (CipDetailInfoAttribute)
                    p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                if (cipDetailInfo != null)
                    if (CompareTo(p.GetValue(cipObject1), p.GetValue(cipObject2)) != 0)
                        differentList.Add(cipDetailInfo.Id);
            }

            return differentList;
        }

        public static List<string> GetDifferentAttributeNameList<T>(T cipObject1, T cipObject2)
        {
            var differentList = new List<string>();

            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                var cipDetailInfo =
                    (CipDetailInfoAttribute)
                    p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                if (cipDetailInfo != null)
                    if (CompareTo(p.GetValue(cipObject1), p.GetValue(cipObject2)) != 0)
                        differentList.Add(p.Name);
            }

            return differentList;
        }

        public static List<ushort> GetDifferentAttributeList<T>(T cipObject1, T cipObject2,
            List<string> comparePropertiesList)
        {
            var differentList = new List<ushort>();

            foreach (var attributeName in comparePropertiesList)
            {
                var p = typeof(T).GetProperty(attributeName);
                if (p != null)
                {
                    var cipDetailInfo =
                        (CipDetailInfoAttribute)
                        p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                    if (cipDetailInfo != null)
                        if (CompareTo(p.GetValue(cipObject1), p.GetValue(cipObject2)) != 0)
                        {
                            if (!differentList.Contains(cipDetailInfo.Id))
                                differentList.Add(cipDetailInfo.Id);
                        }

                }
                else
                {
                    Console.WriteLine($"not found attribute: {attributeName}");
                }
            }

            return differentList;
        }

        public static List<ushort> AttributeNamesToIdList<T>(string[] attributeNames)
        {
            if (attributeNames == null)
                return null;

            var attributeIdList = new List<ushort>();

            // get attribute id list
            foreach (var attributeName in attributeNames)
            {
                var p = typeof(T).GetProperty(attributeName);
                if (p == null)
                    return null;

                var cipDetailInfo =
                    (CipDetailInfoAttribute)
                    p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                if (cipDetailInfo != null)
                    attributeIdList.Add(cipDetailInfo.Id);
                else

                    return null;
            }

            return attributeIdList;
        }

        public static List<ushort> AttributeNamesToIdList(Type T, string[] attributeNames)
        {
            if (attributeNames == null)
                return null;

            var attributeIdList = new List<ushort>();

            // get attribute id list
            foreach (var attributeName in attributeNames)
            {
                var p = T.GetProperty(attributeName);
                if (p == null)
                    return null;

                var cipDetailInfo =
                    (CipDetailInfoAttribute)
                    p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                if (cipDetailInfo != null)
                    attributeIdList.Add(cipDetailInfo.Id);
                else

                    return null;
            }

            return attributeIdList;
        }

        public static List<string> AttributeIdsToNames(Type T, List<ushort> idList)
        {
            if (idList == null)
                return null;

            var resultList = new List<string>();
            var attributeMap = GetAttributeMap(T);

            foreach (var id in idList)
                if (attributeMap.ContainsKey(id))
                {
                    resultList.Add(attributeMap[id]);
                }

            return resultList;
        }

        public static List<AttributePair> AttributeNamesToAttributePairList(object cipObject, string[] attributeNames)
        {
            var T = cipObject.GetType();

            if (attributeNames == null)
                return null;

            var resultList = new List<AttributePair>();

            // get attribute id list
            foreach (var attributeName in attributeNames)
            {
                var p = T.GetProperty(attributeName);
                if (p == null)
                    return null;

                var cipDetailInfo =
                    (CipDetailInfoAttribute)
                    p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                if (cipDetailInfo != null)
                {
                    var propertyValue = p.GetValue(cipObject);
                    var attributePair = new AttributePair
                    {
                        AttributeId = cipDetailInfo.Id,
                        AttributeValue = GetBytes(propertyValue)
                    };

                    resultList.Add(attributePair);
                }
                else

                    return null;
            }

            return resultList;
        }

        public static List<AttributePair> AttributeIdListToAttributePairList(object cipObject, List<ushort> idList)
        {
            var T = cipObject.GetType();

            if (idList == null)
                return null;

            var resultList = new List<AttributePair>();
            var attributeMap = GetAttributeMap(cipObject.GetType());

            foreach (var id in idList)
                if (attributeMap.ContainsKey(id))
                {
                    var attributeName = attributeMap[id];
                    var p = T.GetProperty(attributeName);
                    if (p == null)
                        return null;

                    var propertyValue = p.GetValue(cipObject);
                    var attributePair = new AttributePair
                    {
                        AttributeId = id,
                        AttributeValue = GetBytes(propertyValue)
                    };

                    resultList.Add(attributePair);
                }
                else
                {
                    return null;
                }

            return resultList;
        }

        public static ushort? AttributeNameToId<T>(string attributeName)
        {
            // TODO(gjc): need edit, use cache
            if (attributeName == null)
                return null;

            var p = typeof(T).GetProperty(attributeName);
            if (p == null)
                return null;

            var cipDetailInfo =
                (CipDetailInfoAttribute)
                p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

            return cipDetailInfo?.Id;
        }

        public static ushort? AttributeNameToId(Type T, string attributeName)
        {
            // TODO(gjc): need edit, use cache
            if (attributeName == null)
                return null;

            var p = T.GetProperty(attributeName);
            if (p == null)
                return null;

            var cipDetailInfo =
                (CipDetailInfoAttribute)
                p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

            return cipDetailInfo?.Id;
        }

        public static string AttributeNameToUnits<T>(string attributeName)
        {
            if (attributeName == null)
                return null;

            var p = typeof(T).GetProperty(attributeName);
            if (p == null)
                return null;

            var cipDetailInfo =
                (CipDetailInfoAttribute)
                p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

            return cipDetailInfo?.Unit;
        }


        public static byte[] GetBytes(object cipValue)
        {
            if (cipValue is CipBool)
                return ((CipBool) cipValue).GetBytes();

            var array = cipValue as CipByteArray;
            if (array != null)
                return array.GetBytes();

            if (cipValue is CipInt)
                return ((CipInt) cipValue).GetBytes();

            if (cipValue is CipDint)
                return ((CipDint) cipValue).GetBytes();

            if (cipValue is CipReal)
                return ((CipReal) cipValue).GetBytes();

            var realArray = cipValue as CipRealArray;
            if (realArray != null)
                return realArray.GetBytes();

            if (cipValue is CipRevision)
                return ((CipRevision) cipValue).GetBytes();

            var cipShortString = cipValue as CipShortString;
            if (cipShortString != null)
                return cipShortString.GetBytes();

            if (cipValue is CipSint)
                return ((CipSint) cipValue).GetBytes();

            var cipString = cipValue as CipString;
            if (cipString != null)
                return cipString.GetBytes();

            if (cipValue is CipStringi)
                return ((CipStringi) cipValue).GetBytes();

            if (cipValue is CipUdint)
                return ((CipUdint) cipValue).GetBytes();

            if (cipValue is CipUint)
                return ((CipUint) cipValue).GetBytes();

            if (cipValue is CipUlint)
                return ((CipUlint) cipValue).GetBytes();

            if (cipValue is CipUsint)
                return ((CipUsint) cipValue).GetBytes();

            // TODO:add code

            return null;
        }

        public static object Parse(Type T, byte[] data, int arraySize, ref int startIndex)
        {
            if (T == typeof(CipBool))
                return CipBool.Parse(data, ref startIndex);

            if (T == typeof(CipByteArray))
                return CipByteArray.Parse(arraySize, data, ref startIndex);

            if (T == typeof(CipInt))
                return CipInt.Parse(data, ref startIndex);

            if (T == typeof(CipDint))
                return CipDint.Parse(data, ref startIndex);

            if (T == typeof(CipReal))
                return CipReal.Parse(data, ref startIndex);

            if (T == typeof(CipRealArray))
                return CipRealArray.Parse(data, ref startIndex);

            if (T == typeof(CipRevision))
                return CipRevision.Parse(data, ref startIndex);

            if (T == typeof(CipShortString))
                return CipShortString.Parse(data, ref startIndex);

            if (T == typeof(CipSint))
                return CipSint.Parse(data, ref startIndex);

            if (T == typeof(CipString))
                return CipString.Parse(data, ref startIndex);

            if (T == typeof(CipStringi))
                return CipStringi.Parse(data, ref startIndex);

            if (T == typeof(CipUdint))
                return CipUdint.Parse(data, ref startIndex);

            if (T == typeof(CipUint))
                return CipUint.Parse(data, ref startIndex);

            if (T == typeof(CipUlint))
                return CipUlint.Parse(data, ref startIndex);

            if (T == typeof(CipUsint))
                return CipUsint.Parse(data, ref startIndex);

            Debug.Assert(false, $"{T}");
            // TODO:add code

            return null;
        }

        public static int CompareTo(object cipValue1, object cipValue2)
        {
            if (cipValue1 == null && cipValue2 == null)
                return 0;

            if (cipValue1 != null && cipValue2 == null)
                return 1;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (cipValue1 == null && cipValue2 != null)
                return -1;


            if (cipValue1 is CipBool)
                return ((CipBool) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipByteArray)
                return ((CipByteArray) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipInt)
                return ((CipInt) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipDint)
                return ((CipDint) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipReal)
                return ((CipReal) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipRealArray)
                return ((CipRealArray) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipRevision)
                return ((CipRevision) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipShortString)
                return ((CipShortString) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipSint)
                return ((CipSint) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipString)
                return ((CipString) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipStringi)
                return ((CipStringi) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipUdint)
                return ((CipUdint) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipUint)
                return ((CipUint) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipUlint)
                return ((CipUlint) cipValue1).CompareTo(cipValue2);

            if (cipValue1 is CipUsint)
                return ((CipUsint) cipValue1).CompareTo(cipValue2);

            Debug.Assert(false, $"{cipValue1.GetType()}");
            // TODO:add code

            return 1;
        }

        public static int BytesCompareTo(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null && bytes2 == null)
                return 0;

            if (bytes1 == null)
                return -1;

            if (bytes2 == null)
                return 1;

            if (bytes1.Length > bytes2.Length)
                return 1;
            if (bytes1.Length < bytes2.Length)
                return -1;

            for (var i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] > bytes2[i])
                    return 1;
                if (bytes1[i] < bytes2[i])
                    return -1;
            }

            return 0;
        }
    }
}