using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class ConfigureDetectViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        public ConfigureDetectViewModel(string hexValue)
        {
            hexValue = string.IsNullOrEmpty(hexValue) ? "0" : hexValue;
            var hex = FormatOp.RemoveFormat(hexValue);
            var result = "";
            foreach (char c in hex)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                result += string.Format("{0:d4}",v2).Replace(" ","");
            }

            result = result.PadLeft(38, '0');

            #region SetInfo
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Port configuration has been changed"), IsChecked = GetStatus(result, 0)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Parameter Connection has been modified"), IsChecked = GetStatus(result, 1)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Alarm Log values cleared"), IsChecked = GetStatus(result, 2)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Safety signature deleted allowed in Run mode"), IsChecked = GetStatus(result, 3)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Safety signature delete inhibited in Run mode"), IsChecked = GetStatus(result, 4)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Correlation affected"), IsChecked = GetStatus(result, 5)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Custom Log Entry Added"), IsChecked = GetStatus(result, 6)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Constant Tag attribute set"), IsChecked = GetStatus(result, 7)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Constant Tag attribute clear"), IsChecked = GetStatus(result, 8)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Multiple constant Tag values changed"), IsChecked = GetStatus(result, 9)});
            DetectInfos.Add(
                new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Constant Tag value changed"), IsChecked = GetStatus(result, 10)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Safety unlock"), IsChecked = GetStatus(result, 11)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Safety lock"), IsChecked = GetStatus(result, 12)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Safety signature deleted"), IsChecked = GetStatus(result, 13)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Safety signature created"), IsChecked = GetStatus(result, 14)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Removable media inserted"), IsChecked = GetStatus(result, 15)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Removable media removed"), IsChecked = GetStatus(result, 16)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Controller time slice modified"), IsChecked = GetStatus(result, 17)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Program properties modified"), IsChecked = GetStatus(result, 18)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Task properties modified"), IsChecked = GetStatus(result, 19)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("All major faults cleared though keyswitch"), IsChecked = GetStatus(result, 20)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("All major faults cleared"), IsChecked = GetStatus(result, 21)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("A major fault occurred"), IsChecked = GetStatus(result, 22)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Keyswitch mode changed"), IsChecked = GetStatus(result, 23)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Remote mode changed"), IsChecked = GetStatus(result, 24)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Firmware update from removable media attempted"), IsChecked = GetStatus(result, 25)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Firmware update attempted"), IsChecked = GetStatus(result, 26)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("I/O forces modified"), IsChecked = GetStatus(result, 27)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("I/O forces removed"), IsChecked = GetStatus(result, 28)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("I/O forces disabled"), IsChecked = GetStatus(result, 29)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("I/O forces enabled"), IsChecked = GetStatus(result, 30)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("SFC element force value changed"), IsChecked = GetStatus(result, 31)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("SFC forces removed"), IsChecked = GetStatus(result, 32)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("SFC forces disabled"), IsChecked = GetStatus(result, 33)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("SFC forces enabled"), IsChecked = GetStatus(result, 34)});
            DetectInfos.Add(new DetectInfo() {Content = LanguageManager.GetInstance().ConvertSpecifier("Transaction committed"), IsChecked = GetStatus(result, 35)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Online edits modified controller program"), IsChecked = GetStatus(result, 36)});
            DetectInfos.Add(new DetectInfo()
                {Content = LanguageManager.GetInstance().ConvertSpecifier("Project stored to removable media"), IsChecked = GetStatus(result, 37)});

            DetectInfos.Reverse();
            #endregion

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public RelayCommand OkCommand { set; get; }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public List<DetectInfo> DetectInfos { set; get; } = new List<DetectInfo>();

        public string GetConfigure()
        {
            string config = "";
            for (int i = DetectInfos.Count - 1; i >= 0; i--)
            {
                config += DetectInfos[i].IsChecked ? "1" : "0";
            }

            var hex = Convert.ToInt64(config, 2).ToString("X").PadLeft(16, '0');
            hex = Regex.Replace(hex, @"((?<=\w)(?=(\w{4})+$))", "_");
            return "16#" + hex;
        }

        private bool GetStatus(string configByte, int index)
        {
            if (index > configByte.Length - 1) return false;
            var c = configByte[configByte.Length - 1 - index];
            if (c == '1') return true;
            return false;

        }
    }

    internal class DetectInfo
    {
        public string Content { set; get; }
        public bool IsChecked { set; get; }
    }
}
