using System;
using System.Data;
using System.IO;

namespace ICSStudio.MultiLanguage
{
    public class LanguageInfo
    {
        /// <summary>
        /// 当前语言
        /// </summary>
        public static string CurrentLanguage {
            get
            {
                return Properties.Settings.Default.Language;
            }
            set
            {
                Properties.Settings.Default.Language = value;
                Properties.Settings.Default.Save();
            } }
    }
}
