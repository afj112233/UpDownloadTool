using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Utils;
using MessageBox = System.Windows.MessageBox;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class NewProjectDialogViewModel : ViewModelBase
    {
        private string _name;
        private string _location;
        private bool? _dialogResult;
        private Product _selectedProduct;
        private string _search;

        public NewProjectDialogViewModel()
        {
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            BackCommand = new RelayCommand(ExecuteBackCommand, CanExecuteBackCommand);
            NextCommand = new RelayCommand(ExecuteNextCommand, CanExecuteNextCommand);
            FinishCommand = new RelayCommand(ExecuteFinishCommand, CanExecuteFinishCommand);
            BrowseCommand = new RelayCommand(ExecuteBrowseCommand);
            SelectedItemChangedCommand = new RelayCommand<RoutedEventArgs>(ExecuteSelectedItemChangedCommand);
            Location = $"{System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Documents";
            SetProduct();
        }

        private void SetProduct()
        {
            var icc_b = new Product("ICC-Basic Controller", "", false);
            icc_b.Child.Add(new Product("ICC-B010ERM", icc_b.Name, true));
            icc_b.Child.Add(new Product("ICC-B020ERM", icc_b.Name, true));
            icc_b.Child.Add(new Product("ICC-B030ERM", icc_b.Name, true));
            icc_b.Child.Add(new Product("ICC-B050ERM", icc_b.Name, true));
            icc_b.Child.Add(new Product("ICC-B080ERM", icc_b.Name, true));
            icc_b.Child.Add(new Product("ICC-B0100ERM", icc_b.Name, true));
            Products.Add(icc_b);

            var icc_p=new Product("ICC-Pro Controller","",false);
            icc_p.Child.Add(new Product("ICC-P010ERM", icc_p.Name, true));
            icc_p.Child.Add(new Product("ICC-P020ERM", icc_p.Name, true));
            icc_p.Child.Add(new Product("ICC-P030ERM", icc_p.Name, true));
            icc_p.Child.Add(new Product("ICC-P050ERM", icc_p.Name, true));
            icc_p.Child.Add(new Product("ICC-P080ERM", icc_p.Name, true));
            icc_p.Child.Add(new Product("ICC-P0100ERM", icc_p.Name, true));
            Products.Add(icc_p);

            var icc_t=new Product("ICC-Turbo Controller","",false);
            icc_t.Child.Add(new Product("ICC-T0100ERM",icc_t.Name,true));
            Products.Add(icc_t);
        }

        public Product SelectedProduct
        {
            set
            {
                _selectedProduct = value;
                BackCommand.RaiseCanExecuteChanged();
                NextCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("ProductDescription");
            }
            get { return _selectedProduct; }
        }

        public List<int> Modules { get; } = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7, 8};

        public int SelectedModule { set; get; } = 0;

        public string ProductDescription => $"{SelectedProduct?.Name}  {SelectedProduct?.Description}";

        public List<Product> Products { get; } = new List<Product>();

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public string Name
        {
            set
            {
                Set(ref _name, value);
                BackCommand.RaiseCanExecuteChanged();
                NextCommand.RaiseCanExecuteChanged();
                FinishCommand.RaiseCanExecuteChanged();
            }
            get { return _name; }
        }

        public string Location
        {
            set
            {
                Set(ref _location, value);
                BackCommand.RaiseCanExecuteChanged();
                NextCommand.RaiseCanExecuteChanged();
                FinishCommand.RaiseCanExecuteChanged();
            }
            get { return _location; }
        }

        public string ProjectFile { get; set; }

        public string Search
        {
            set
            {
                _search = value;
                Filter();
            }
            get { return _search; }
        }

        private void Filter()
        {
            //TODO(zyl):
        }

        public string Description { set; get; }

        public Visibility Page1Visibility { set; get; } = Visibility.Visible;

        public Visibility Page2Visibility { set; get; } = Visibility.Collapsed;

        public RelayCommand<RoutedEventArgs> SelectedItemChangedCommand { get; }

        private void ExecuteSelectedItemChangedCommand(RoutedEventArgs e)
        {
            SelectedProduct = (e as RoutedPropertyChangedEventArgs<object>)?.NewValue as Product;
        }

        public RelayCommand BrowseCommand { get; }

        private void ExecuteBrowseCommand()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Location;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Location = dialog.SelectedPath;
            }
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            ProjectFile = string.Empty;
            DialogResult = false;
        }

        public RelayCommand BackCommand { get; }

        private void ExecuteBackCommand()
        {
            Page2Visibility = Visibility.Collapsed;
            Page1Visibility = Visibility.Visible;
            RaisePropertyChanged("Page2Visibility");
            RaisePropertyChanged("Page1Visibility");
            BackCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            FinishCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteBackCommand()
        {
            if (Page2Visibility == Visibility.Visible) return true;
            return false;
        }

        public RelayCommand NextCommand { get; }

        private void ExecuteNextCommand()
        {
            Page2Visibility = Visibility.Visible;
            Page1Visibility = Visibility.Collapsed;
            RaisePropertyChanged("Page2Visibility");
            RaisePropertyChanged("Page1Visibility");
            BackCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            FinishCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteNextCommand()
        {
            if (Page2Visibility == Visibility.Visible) return false;
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Location) ||
                SelectedProduct == null || !SelectedProduct.IsProduct) return false;
            return true;
        }

        public RelayCommand FinishCommand { get; }

        private void ExecuteFinishCommand()
        {
            if (!IsValidName(Name))
            {
                MessageBox.Show(
                    $"Invalid Name '{Name}'",
                    "ICS Studio", MessageBoxButton.OK);
                return;
            }
            
            try
            {
                string dllPath = AssemblyUtils.AssemblyDirectory;
                //string templateFile = dllPath + $@"\Template\{SelectedProduct.Name}_Template.json";
                //TODO(zyl):add more plc type
                string templateFile = "";

                if (SelectedProduct.Name.StartsWith("ICC-P"))
                {
                    switch (SelectedProduct.Name)
                    {
                        case "ICC-P010ERM":
                            templateFile = dllPath + $@"\Template\ICC-P010ERM_Template.json";
                            break;
                        case "ICC-P020ERM":
                            templateFile = dllPath + $@"\Template\ICC-P020ERM_Template.json";
                            break;
                        default:
                            templateFile = dllPath + $@"\Template\ICC-P0100ERM_Template.json";
                            break;
                    }
                }
                else if (SelectedProduct.Name.StartsWith("ICC-T"))
                {
                    templateFile = dllPath + $@"\Template\ICC-T0100ERM_Template.json";
                }
                else
                {
                    templateFile = dllPath + $@"\Template\ICC-B010ERM_Template.json";
                }

                string contents = File.ReadAllText(templateFile);

                string projectName = Name;

                int pointIOBusSize = SelectedModule + 3;

                string projectDescription = string.Empty;
                if (!string.IsNullOrEmpty(Description))
                    projectDescription = Description;

                string projectCreationDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

                contents = contents.Replace("#ProjectName#", projectName);
                contents = contents.Replace("#ProjectDescription#", projectDescription);
                contents = contents.Replace("#ProjectCreationDate#", projectCreationDate);
                contents = contents.Replace("#PointIOBusSize#", pointIOBusSize.ToString());
                contents = contents.Replace("#ProductCode#", Controller.GetProductCode(SelectedProduct.Name).ToString());
                string projectFile = $@"{Location}\{Name}.json";
                if (File.Exists(projectFile))
                {
                    if (MessageBox.Show("The project already exists.Do you want to replace it?", "ICS Studio",
                            MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    if (!System.IO.Directory.Exists(Location))
                    {
                        System.IO.Directory.CreateDirectory(Location);
                    }
                }
                
                using (var sw = File.CreateText(projectFile))
                {
                    sw.Write(contents);
                }

                ProjectFile = projectFile;
                DialogResult = true;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    $"Failed to create '{Name}'",
                    "ICS Studio", MessageBoxButton.OK);

                ProjectFile = string.Empty;
                DialogResult = false;
            }
        }

        private bool CanExecuteFinishCommand()
        {
            if (SelectedProduct == null || !SelectedProduct.IsProduct || string.IsNullOrEmpty(Name) ||
                string.IsNullOrEmpty(Location) || Page1Visibility == Visibility.Visible) return false;

            return true;
        }

        private bool IsValidName(string name)
        {
            bool isValid = !string.IsNullOrEmpty(name);

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") || name.IndexOf("__") > -1)
                {
                    isValid = false;
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                }
            }

            // key word
            if (isValid)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                    }
                }
            }

            return isValid;
        }
    }

    public class Product:INotifyPropertyChanged
    {
        private bool _isExpanded = true;

        public Product(string name, string description, bool isProduct)
        {
            Name = name;
            Description = description;
            IsProduct = isProduct;
        }

        public bool IsProduct { get; }
        public string Name { get; }
        public string Description { get; }
        public List<Product> Child { get; } = new List<Product>();

        public bool IsExpanded
        {
            set
            {
                _isExpanded = value; 
                OnPropertyChanged();
            }
            get { return _isExpanded; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
