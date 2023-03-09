using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel
{
    public class DataCompareViewModel : ViewModelBase, IOptionPanel
    {
        private bool _radioCheck;

        public DataCompareViewModel(DataCompare panel, JObject tag,Tag existTag)
        {
            Control = panel;
            panel.DataContext = this;
            var controller = Controller.GetInstance();
            if (!"Alias".Equals(tag["TagType"]?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                string typeName;
                int dim1, dim2, dim3;
                int errorCode;
                var dataType = tag["DataType"]?.ToString();
                Debug.Assert(dataType != null);
                var isValid = controller.DataTypes.ParseDataType(
                    dataType, out typeName,
                    out dim1, out dim2, out dim3, out errorCode);
                Debug.Assert(isValid);
                var dataWrapper = new DataWrapper(controller.DataTypes[typeName], dim1, dim2, dim3, tag);
                var style = (DisplayStyle) (byte) tag["Radix"];
                Items.Add(NodeOp.CreateDataView(dataWrapper, style,tag["Name"]?.ToString(), Node_PropertyChanged));
            }

            {
                ExistItems.Add(NodeOp.CreateDataView(existTag.DataWrapper,existTag.DisplayStyle,existTag.Name, ExistNode_PropertyChanged));
            }
            RadioCheck = true;
        }

        public bool RadioCheck
        {
            set
            {
                _radioCheck = value;
                if (value)
                {
                    ImportVisibility = Visibility.Visible;
                    ExistVisibility = Visibility.Collapsed;
                    RaisePropertyChanged("ImportVisibility");
                    RaisePropertyChanged("ExistVisibility");
                }
                else
                {
                    ImportVisibility = Visibility.Collapsed;
                    ExistVisibility = Visibility.Visible;
                    RaisePropertyChanged("ImportVisibility");
                    RaisePropertyChanged("ExistVisibility");
                }
            }
            get { return _radioCheck; }
        }

        public Visibility ImportVisibility { set; get; }

        public Visibility ExistVisibility { set; get; }

        public ObservableCollection<Node> Items { get; } = new ObservableCollection<Node>();

        public ObservableCollection<Node> ExistItems { get; } = new ObservableCollection<Node>();

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }
        
        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
            {
                var node = (Node) sender;
                if (node.IsExpanded)
                {
                    var index = Items.IndexOf(node) + 1;
                    for (int i = node.Child.Count - 1; i >= 0; i--)
                    {
                        var child = node.Child[i];
                        Items.Insert(index, child);
                    }
                }
                else
                {
                    foreach (var child in node.Child)
                    {
                        Items.Remove(child);
                        child.IsExpanded = false;
                    }
                }
            }
        }

        private void ExistNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
            {
                var node = (Node)sender;
                if (node.IsExpanded)
                {
                    var index = ExistItems.IndexOf(node) + 1;
                    for (int i = node.Child.Count - 1; i >= 0; i--)
                    {
                        var child = node.Child[i];
                        ExistItems.Insert(index, child);
                    }
                }
                else
                {
                    foreach (var child in node.Child)
                    {
                        ExistItems.Remove(child);
                        child.IsExpanded = false;
                    }
                }
            }
        }
    }

}
