using System.Collections.Generic;
using System.Xml;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.SimpleServices.Common;
using RungType = ICSStudio.FileConverter.JsonToL5X.Model.RungType;
using RungTypeEnum = ICSStudio.FileConverter.JsonToL5X.Model.RungTypeEnum;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public static partial class Converter
    {
        private static STContentType[] ToSTContent(STRoutine routine)
        {
            STContentType stContentType = new STContentType();

            List<STLineType> lines = new List<STLineType>();
            if (routine.CodeText != null)
            {
                ulong number = 0;
                XmlDocument xmlDocument = new XmlDocument();
                foreach (var lineCode in routine.CodeText)
                {
                    STLineType stLineType = new STLineType
                    {
                        Number = number,
                        Text = new XmlNode[] { xmlDocument.CreateCDataSection(lineCode.Replace("\r","").Replace("\n", "")) }
                    };

                    lines.Add(stLineType);

                    number++;
                }
            }

            stContentType.Line = lines.ToArray();

            return new[] {stContentType};
        }

        private static RLLContentType[] ToRLLContent(RLLRoutine routine)
        {
            RLLContentType rllContentType = new RLLContentType();

            List<RungType> rungs = new List<RungType>();

            ulong number = 0;
            XmlDocument xmlDocument = new XmlDocument();
            foreach (string rungCode in routine.CodeText)
            {
                RungType rungType = new RungType
                {
                    Number = number,
                    Type = RungTypeEnum.N,
                    Text = xmlDocument.CreateCDataSection(rungCode)
                };

                rungs.Add(rungType);

                number++;
            }

            if (rungs.Count > 0)
            {
                rllContentType.Rung = rungs.ToArray();

                return new[] {rllContentType};
            }

            return null;
        }
    }
}
