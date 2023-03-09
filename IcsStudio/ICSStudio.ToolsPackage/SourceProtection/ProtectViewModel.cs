using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Warning;
using ICSStudio.SimpleServices.SourceProtection;
using System.Collections.Generic;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ToolsPackage.SourceProtection
{
    public class ProtectViewModel : ViewModelBase
    {
        private readonly SourceProtectionManager _manager;
        private readonly SourceKeyProvider _provider;


        public ProtectViewModel(SourceProtectionManager manager)
        {
            _manager = manager;
            _provider = manager.Provider;


            //SourceKeys = new ListCollectionView(new List<Item>());

            OkCommand = new RelayCommand(ExecuteOk);
            CancelCommand = new RelayCommand(ExecuteCancel);
            HelpCommand = new RelayCommand(ExecuteHelp);
            LostFocusCommand = new RelayCommand(ExecuteLostFocus);
            TxtLostFocusCommand = new RelayCommand(ExecuteTxtLostFocus);
            PreInputCommand = new RelayCommand(ExecutePreInput);
            SelectChangeCommand = new RelayCommand(ExecuteSelectChange);

            UpdateSourceKeys();
        }

        private Visibility _isShowPass = Visibility.Visible;

        public Visibility IsShowPass
        {
            get { return _isShowPass; }
            set
            {
                Set(ref _isShowPass, value);
                IsShowTxt = value == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private Visibility _isShowTxt = Visibility.Hidden;

        public Visibility IsShowTxt
        {
            get { return _isShowTxt; }
            set { Set(ref _isShowTxt, value); }
        }

        private bool? _isShowPassChecked = false;

        public bool? IsShowPassChecked
        {
            get { return _isShowPassChecked; }
            set
            {
                Set(ref _isShowPassChecked, value);
                IsShowPass = value == true ? Visibility.Hidden : Visibility.Visible;
                if (value == true)
                {
                    //IsConfirmBoxEnable = false;
                    SourceKey = Pass;
                }
            }
        }

        //private bool _isConfirmBoxEnable = true;

        //public bool IsConfirmBoxEnable
        //{
        //    get { return _isConfirmBoxEnable; }
        //    set
        //    {
        //        Set(ref _isConfirmBoxEnable, value);
        //        if (value == false)
        //            ConfirmPass = "";
        //    }
        //}

        private string _confirmPass = "";

        public string ConfirmPass
        {
            get { return _confirmPass; }
            set { Set(ref _confirmPass, value); }
        }

        private bool _isKeyNameEnable;

        public bool IsKeyNameEnable
        {
            get { return _isKeyNameEnable; }
            set { Set(ref _isKeyNameEnable, value); }
        }

        private string _keyName = "";

        public string KeyName
        {
            get { return _keyName; }
            set { Set(ref _keyName, value); }
        }

        private string _pass = string.Empty;

        public string Pass
        {
            get { return _pass; }
            set { Set(ref _pass, value); }
        }

        private string _sourceKey = "";
        public string SourceKey
        {
            get { return _sourceKey;}
            set { Set(ref _sourceKey, value); }
        }

        //public ObservableCollection<DisplayItem<string>> SourceKeys { get; }
        private ListCollectionView _sourceKeys = new ListCollectionView(new List<Item>());
        public ListCollectionView SourceKeys
        {
            get { return _sourceKeys;}
            set { Set(ref _sourceKeys, value); }
        }

        public string SourceKeyFileName => _provider.SourceKeyFile;

        private bool? _dialogResult;

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public RelayCommand OkCommand { get; }

        public RelayCommand CancelCommand { get; }

        public RelayCommand HelpCommand { get; }

        public RelayCommand LostFocusCommand { get; }

        public RelayCommand PreInputCommand { get; }

        public RelayCommand TxtLostFocusCommand { get; }

        public RelayCommand SelectChangeCommand { get; }

        private void ExecuteOk()
        {
            //如果是已有的key，则可以继续
            if (_provider.KeyDictionary.ContainsKey(SourceKey))
            {
                DialogResult = true;
            }
            else if (IsShowPassChecked != true && !Pass.Equals(ConfirmPass))// && !string.IsNullOrEmpty(ConfirmPass)
            {
                //不显示密码时，confirm与key不匹配
                string message = "Entered source keys do not match.";
                string reason = "";
                string errorCode = "Error 409-0";

                WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                {
                    Owner = Application.Current.MainWindow
                };

                dialog.ShowDialog();
            }
            else if (string.IsNullOrEmpty(Pass) || Pass.Length > 40)
            {
                //Key非法的情况
                string message = "Source key is invalid.";
                string reason = "Source key must be 1-40 characters in length.";
                string errorCode = "Error 413-800420B9";

                WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                {
                    Owner = Application.Current.MainWindow
                };

                dialog.ShowDialog();
            }
            else if (!string.IsNullOrEmpty(KeyName) && _provider.KeyDictionary.ContainsValue(KeyName))
            {
                //有Key Name的情况
                var pair = _provider.KeyDictionary.FirstOrDefault(p => p.Value == KeyName);
                if (pair.Key != Pass)
                {
                    string message = "Failed to create source key.";
                    string reason = "Source key name is already associated with a different source key.";
                    string errorCode = "Error 25580-800420CD";

                    WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    dialog.ShowDialog();
                }
                else
                {
                    UpdateKeys();
                    DialogResult = true;
                }
            }
            else
            {
                //无Key Name的情况
                UpdateKeys();
                if (!string.IsNullOrEmpty(Pass))
                    SourceKey = Pass;
                DialogResult = true;
            }

        }

        private void UpdateKeys()
        {
            _provider.KeyDictionary[Pass] = KeyName;
            //if (_provider.KeyDictionary.ContainsKey(Pass))
            //    return;

            //_provider.KeyDictionary.Add(Pass, KeyName);
            //更新文件
            _provider.SaveSourceKeys();
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }

        private void ExecuteHelp()
        {
            //Debug.WriteLine($"=============key is {SourceKey}; pass is{Pass}");
        }

        private void ExecuteSelectChange()
        {
            //Debug.WriteLine($"=============key is {SourceKey}; pass is{Pass}");
            Pass = SourceKey;
        }

        private void ExecuteLostFocus()
        {
            //密码框失去焦点或者combo选中
            //Pass绑密码框，SourceKey绑明码框及Combo值

            //如果密码框为空，不处理
            if (string.IsNullOrEmpty(Pass))
            {
                //Debug.WriteLine($"1=============key is {SourceKey}; pass is{Pass}");
                return;
            }

            //如果密码框输入的是新密码，需确认密码，并启用命名框
            if (!_provider.KeyDictionary.ContainsKey(Pass))
            {
                //Debug.WriteLine($"2=============key is {SourceKey}; pass is{Pass}");
                IsKeyNameEnable = false;
                //IsConfirmBoxEnable = true;
                return;
            }

            //Debug.WriteLine($"3=============key is {SourceKey}; pass is{Pass}");
            //如果是已有密码
            string keyname = _provider.KeyDictionary[Pass];
            KeyName = keyname;
            Pass = SourceKey;
            IsKeyNameEnable = true;
            //IsConfirmBoxEnable = false;
        }

        private void ExecuteTxtLostFocus()
        {
            //明码框失去焦点
            Pass = SourceKey;
        }

        private void ExecutePreInput()
        {
            if (IsShowPassChecked == true)
                ConfirmPass = "";
            if (_provider.KeyDictionary.ContainsKey(Pass))
                ConfirmPass = "";
        }

        private void UpdateSourceKeys()
        {
            List<Item> items = new List<Item>();
            foreach (var pair in _provider.KeyDictionary)
            {
                string val = pair.Key;
                string category = string.IsNullOrEmpty(pair.Value) ? "Unnamed Source Keys" : "Named Source Keys";
                items.Add(new Item() { DisplayName = _manager.GetDisplayNameByKey(val), Value = val, Category = category });
                //SourceKeys.AddNewItem(new Item(){Name = _manager.GetDisplayNameByKey(val), Value = val, Category = category});
            }

            SourceKeys = new ListCollectionView(items);
            SourceKeys.GroupDescriptions?.Add(new PropertyGroupDescription("Category"));
        }
        //private void UpdateSourceKeys()
        //{
        //    SourceKeys.Clear();

        //    foreach (var pair in _provider.KeyDictionary)
        //    {
        //        DisplayItem<string> item = new DisplayItem<string>
        //        {
        //            Value = pair.Key,
        //            DisplayName = _manager.GetDisplayNameByKey(pair.Key)
        //        };

        //        SourceKeys.Add(item);
        //    }
        //}

    }


    public class Item
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }
    }
    /// <summary>
    /// 增加Password扩展属性
    /// </summary>
    public static class PasswordBoxHelper
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata("", OnPasswordPropertyChanged));

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            string password = (string)e.NewValue;
            if (box != null && box.Password != password)
            {
                box.Password = password;
            }
        }
    }

    /// <summary>
    /// 接收PasswordBox的密码修改事件
    /// </summary>
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PasswordChanged -= OnPasswordChanged;
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            string password = PasswordBoxHelper.GetPassword(box);
            if (box != null && box.Password != password)
            {
                PasswordBoxHelper.SetPassword(box, box.Password);
            }
        }

        //public static readonly DependencyProperty IsLostFocusProperty=
    }
}
