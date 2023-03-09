using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel
{
    public class DataViewModel : ViewModelBase, IOptionPanel
    {
        public DataViewModel(Data panel, JObject tag)
        {
            Control = panel;
            panel.DataContext = this;
            var controller = Controller.GetInstance();
            if ("Alias".Equals(tag["TagType"]?.ToString(), StringComparison.OrdinalIgnoreCase)) return;
            string typeName;
            int dim1, dim2, dim3;
            int errorCode;
            var dataType = tag["DataType"]?.ToString();
            Debug.Assert(dataType != null);
            var isValid = controller.DataTypes.ParseDataType(
                dataType, out typeName,
                out dim1, out dim2, out dim3, out errorCode);
            Debug.Assert(isValid);
            var dataWrapper = new DataWrapper(controller.DataTypes[typeName], dim1, dim2, dim3,tag);
            
            Items.Add(NodeOp.CreateDataView(dataWrapper,(DisplayStyle)(byte)tag["Radix"],tag["FinalName"]?.ToString(), Node_PropertyChanged));
        }

        public override void Cleanup()
        {
            NodeOp.RemoveViewListen(Items[0], Node_PropertyChanged);
        }

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

        public ObservableCollection<Node> Items { get; } = new ObservableCollection<Node>();
    }
}
