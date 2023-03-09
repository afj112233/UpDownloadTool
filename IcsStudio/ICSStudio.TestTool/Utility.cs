using System;
using System.Collections.Generic;
using System.IO;

namespace ICSStudio.TestTool
{
    public class Utility
    {
        public static List<string> GetFileList(string path)
        {
            List<string> fileList = new List<string>();

            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    var ext = Path.GetExtension(file).ToLower();

                    if (ext.Equals(".json") || ext.Equals(".l5x"))
                        fileList.Add(file);
                }

                foreach (string directory in Directory.GetDirectories(path))
                {
                    fileList.AddRange(GetFileList(directory));
                }
            }

            return fileList;
        }

        /// <summary>
        /// 检测IP地址是否合法
        /// </summary>
        /// <param name="strJudgeString"></param>
        /// <returns></returns>
        public static bool JudgeIPFormat(string strJudgeString)
        {
            bool result = true;

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}$");
            var blnTest = regex.IsMatch(strJudgeString);
            if (blnTest)
            {
                string[] strTemp = strJudgeString.Split(new char[] { '.' }); 
                int nDotCount = strTemp.Length - 1; //字符串中.的数量，若.的数量小于3，则是非法的ip地址
                if (3 == nDotCount)//判断字符串中.的数量
                {
                    for (int i = 0; i < strTemp.Length; i++)
                    {
                        if (Convert.ToInt32(strTemp[i]) > 255)
                        { //大于255则提示，不符合IP格式
                            result = false;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
