using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.MultiLanguage
{
    public class LanguageManager
    {
        private static ResourceDictionary _resourceDictionary;

        public static LanguageManager Instance => InnerClass.instance;

        public static LanguageManager GetInstance()
        {
            return InnerClass.instance;
        }

        private class InnerClass
        {
            // 利用「C# 静态构造函数只执行一次」的特性构造单例
            internal static readonly LanguageManager instance = new LanguageManager();
            static InnerClass() { }
        }

        private LanguageManager()
        {
            _resourceDictionary = new ResourceDictionary();
            GetLanguageResource();
        }

        private void GetLanguageResource()
        {
            if (LanguageInfo.CurrentLanguage == "English")
            {
                _resourceDictionary.Source =
                    new Uri("pack://application:,,,/ICSStudio.MultiLanguage;component/LanguageResource/English.xaml");
            }
            else
            {
                _resourceDictionary.Source =
                    new Uri("pack://application:,,,/ICSStudio.MultiLanguage;component/LanguageResource/Chinese.xaml");
            }
        }

        public void SetLanguage(Control control)
        {
            if (control == null) return;
            var dic = control.Resources.MergedDictionaries.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.Contains("English.xaml") || d.Source.OriginalString.Contains("Chinese.xaml")));
            control.Resources.MergedDictionaries.Remove(dic);
            control.Resources.MergedDictionaries.Add(_resourceDictionary);
        }

        public void ChangeLanguage()
        {
            GetLanguageResource();
            LanguageChanged?.Invoke(Instance, new EventArgs());
        }

        public string ConvertSpecifier(string specifier)
        {
            if (specifier == null) return null;
            if (_resourceDictionary.Contains(specifier))
            {
                return (string)_resourceDictionary[specifier];
            }

            return specifier;
        }

        public event EventHandler LanguageChanged;

        public static string GetMenuText(string currentText)
        {
            return LanguageManager.GetInstance().ConvertSpecifier(currentText);
        }

    }
}
