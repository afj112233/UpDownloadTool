using System;
using System.Collections.Generic;
using System.Text;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.DataTypes
{
    public class CipStringi : ICipDataType, IComparable
    {
        private readonly List<SubString> _allStrings;

        public CipStringi()
        {
            _allStrings = new List<SubString>();
        }

        public byte[] GetBytes()
        {
            if ((_allStrings == null) || (_allStrings.Count == 0))
                return null;

            var byteList = new List<byte>();
            var number = (byte) _allStrings.Count;

            // Number
            byteList.Add(number);

            for (var i = 0; i < number; i++)
            {
                // languageChar1-3
                var langCode = _allStrings[i].LangCode;
                byteList.AddRange(Encoding.ASCII.GetBytes(langCode));

                // CharStringStruct, SHORT_STRING
                byteList.Add(0xda);

                // CharSet
                var charSet = _allStrings[i].CharSet;
                byteList.AddRange(BitConverter.GetBytes(charSet));

                // InternationalString
                CipShortString shortString = _allStrings[i].Name;
                byteList.AddRange(shortString.GetBytes());
            }

            return byteList.ToArray();
        }

        public string GetFirstString()
        {
            if (_allStrings.Count == 0)
                return null;

            return _allStrings[0].Name;
        }

        public void AddString(string name, string langCode = "eng", ushort charSet = 4)
        {
            var subString = new SubString(name, langCode, charSet);

            _allStrings.Add(subString);
        }

        public static CipStringi Parse(byte[] data, ref int startIndex)
        {
            var cipStringi = new CipStringi();

            var number = data[startIndex];
            startIndex++;

            for (var i = 0; i < number; i++)
            {
                var langCode = Encoding.ASCII.GetString(data, startIndex, 3);
                startIndex += 3;

                var charStringStruct = data[startIndex];
                startIndex++;

                var charSet = BitConverter.ToUInt16(data, startIndex);
                startIndex += 2;

                if (charStringStruct == 0xDA)
                {
                    var shortString = CipShortString.Parse(data, ref startIndex);

                    cipStringi.AddString(shortString.GetString(), langCode, charSet);
                }
                else
                {
                    // TODO: add other string parse
                    throw new NotImplementedException();
                }
            }

            return cipStringi;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipStringi result)
        {
            // TODO: need check data length!!!

            result = new CipStringi();

            var number = data[startIndex];
            startIndex++;

            for (var i = 0; i < number; i++)
            {
                var langCode = Encoding.ASCII.GetString(data, startIndex, 3);
                startIndex += 3;

                var charStringStruct = data[startIndex];
                startIndex++;

                var charSet = BitConverter.ToUInt16(data, startIndex);
                startIndex += 2;

                if (charStringStruct == 0xDA)
                {
                    var shortString = CipShortString.Parse(data, ref startIndex);

                    result.AddString(shortString.GetString(), langCode, charSet);
                }
                else
                {
                    // TODO: add other string parse
                    return false;
                }
            }

            return true;
        }

        private struct SubString
        {
            public readonly ushort CharSet;
            public readonly string Name;
            public readonly string LangCode;

            public SubString(string name, string code = "eng", ushort charset = 4)
            {
                CharSet = charset;
                Name = name;
                LangCode = code;
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CipStringi))
                throw new ArgumentException("Arg must be CipStringi");

            CipStringi cipStringi = (CipStringi) obj;

            byte[] bytes1 = GetBytes();
            byte[] bytes2 = cipStringi.GetBytes();

            return CipAttributeHelper.BytesCompareTo(bytes1, bytes2);
        }

        
    }
}