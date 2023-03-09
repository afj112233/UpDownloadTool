using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.AddOnInstruction
{
    /// <summary>
    /// Help.xaml 的交互逻辑
    /// </summary>
    public partial class Help
    {
        private AoiDefinition _aoiDefinition;
        //private readonly string _strHeaders;
        private readonly byte[] _bytePost;

        public Help(AoiDefinition aoiDefinition)
        {
            InitializeComponent();
            _aoiDefinition = aoiDefinition;
            //_strHeaders = "Content-Type: application/x-www-form-urlencoded\r\n";
            ASCIIEncoding AE = new ASCIIEncoding();
            _bytePost = AE.GetBytes(GetData());
            if (LanguageInfo.CurrentLanguage.Equals("English"))
            {
                //Browser.Navigate(new Uri("http://localhost:50743/InstructionHelp/InstructionEnglishHelp.aspx"), null, _bytePost,
                //    _strHeaders);
            }
            else
            {
                //Browser.Navigate(new Uri("http://localhost:50743/InstructionHelp/InstructionChineseHelp.aspx"), null, _bytePost,
                //    _strHeaders);
            }
           
            LanguageManager.GetInstance().SetLanguage(this);

            PropertyChangedEventManager.AddHandler(_aoiDefinition, OnPropertyChanged,"");
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            if (LanguageInfo.CurrentLanguage.Equals("English"))
            {
                //Browser.Navigate(new Uri("http://localhost:50743/InstructionHelp/InstructionEnglishHelp.aspx"), null, _bytePost,
                //    _strHeaders);
            }
            else
            {
                //Browser.Navigate(new Uri("http://localhost:50743/InstructionHelp/InstructionChineseHelp.aspx"), null, _bytePost,
                //    _strHeaders);
            }
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == _aoiDefinition)
            {
                string strHeaders = "Content-Type: application/x-www-form-urlencoded\r\n";
                ASCIIEncoding AE = new ASCIIEncoding();
                byte[] bytePost = AE.GetBytes(GetData());
                Browser.Navigate(new Uri("http://localhost:50743/InstructionHelp/InstructionHelp.aspx"), null, bytePost,
                    strHeaders);
            }
        }

        public string GetData()
        {
            string strData = "";
            strData += $"name={_aoiDefinition.Name}&";
            strData += $"description={_aoiDefinition.Description}&";
            strData += $"revision={_aoiDefinition.Revision}&";
            strData += $"extendedText={_aoiDefinition.ExtendedText}&";
            strData += $"vendor={_aoiDefinition.Vendor}&";
            var parameters = new JArray();
            foreach (var item in _aoiDefinition.GetParameterTags())
            {
                var jObject = new JObject();
                jObject["Name"] = item.Name;
                jObject["DataType"] = item.DataTypeInfo.DataType.Name;
                jObject["Usage"] = item.Usage.ToString();
                jObject["Description"] = item.Description;
                parameters.Add(jObject);
            }
            var history=new JArray();
            foreach (History item in _aoiDefinition.History)
            {
                var jObject = new JObject();
                jObject["User"] = item.User;
                jObject["ID"] = item.SignatureID;
                jObject["Time"] = item.Timestamp;
                jObject["Description"] = item.Description;
                history.Add(jObject);
            }
            strData += $"" +
                       $"parameters" +
                       $"" +
                       $"={parameters.ToString()}&";
            strData += $"extended={_aoiDefinition.ExtendedDescription}&";
            strData += $"signature={history.ToString()}&";
            if (_aoiDefinition.IsSealed)
            {
                strData += $"ID={MD5Encrypt(_aoiDefinition.Name)}&";
                strData += $"EditedDate={_aoiDefinition.EditedDate}&";
            }
            strData += $"revisionNote={_aoiDefinition.RevisionNote}";
            return strData;
        }

        private string MD5Encrypt(string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("X2"));
            }

            return tmp.ToString().Substring(11, 8);
        }

        private void Help_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(ExtendedBox);
            if (ExtendedBox.IsFocused)
            {
                ExtendedBox.SelectAll();
            }
        }
    }
}
