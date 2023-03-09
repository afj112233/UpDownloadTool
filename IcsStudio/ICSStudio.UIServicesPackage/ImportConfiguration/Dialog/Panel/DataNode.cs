using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel
{
    public class Node : ViewModelBase
    {
        public Node Parent { set; get; }

        public Node()
        {
            Command = new RelayCommand(ExecuteCommand);
            Control = new StackPanel();

            Control.Orientation = Orientation.Horizontal;
        }

        public Node GetParent(int gen)
        {
            if (Parent == null) return null;
            if (gen == 0) return this;
            return Parent.GetParent(--gen);
        }

        public bool IsLast => Parent == null || Parent.Child.Count - 1 == Parent.Child.IndexOf(this);

        public void AddEmpty()
        {
            var empty = new Rectangle();
            empty.Stroke = Brushes.Transparent;
            empty.Width = 14;
            Control.Children.Add(empty);
        }

        public void AddConnectionLine()
        {
            var connectionLine = new Rectangle();
            connectionLine.Width = 14;
            connectionLine.Width = 1;
            connectionLine.Stroke = Brushes.Gray;
            connectionLine.Margin = new Thickness(8.5, 0, 4.5, 0);
            connectionLine.VerticalAlignment = VerticalAlignment.Stretch;
            Control.Children.Add(connectionLine);
        }

        public void AddBranch()
        {
            var path = new Path { Stroke = Brushes.Gray };
            path.Width = 15;
            var pathGeometry = new PathGeometry();
            path.Data = pathGeometry;
            var pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);
            pathFigure.StartPoint = new Point(9, -2);
            var line = new PolyLineSegment(new[] { new Point(9, 9), new Point(15, 9), new Point(9, 9), new Point(9, 15) },
                true);
            pathFigure.Segments.Add(line);
            Control.Children.Add(path);
        }

        public void AddEndBranch()
        {
            var path = new Path { Stroke = Brushes.Gray };
            path.Width = 15;
            var pathGeometry = new PathGeometry();
            path.Data = pathGeometry;
            var pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);
            pathFigure.StartPoint = new Point(9, -2);
            var line = new PolyLineSegment(new[] { new Point(9, 8), new Point(15, 8) }, true);
            pathFigure.Segments.Add(line);
            Control.Children.Add(path);
        }

        public Visibility CommandVisibility { set; get; } = Visibility.Collapsed;
        private bool _isExpanded = false;
        private int _level;
        public string Name { set; get; }
        public string Value { set; get; } = "{...}";

        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                RaisePropertyChanged("Visibility");
                RaisePropertyChanged();
            }
            get { return _isExpanded; }
        }

        public StackPanel Control { set; get; }

        public int Level
        {
            set { _level = value; }
            get { return _level; }
        }

        public Visibility Visibility { private set; get; }

        public RelayCommand Command { get; }

        private void ExecuteCommand()
        {
            IsExpanded = !IsExpanded;
        }

        public Thickness Margin
        {
            get { return new Thickness((Level - 1) * 14, 0, 0, 0); }
        }

        public List<Node> Child { get; } = new List<Node>();

        public void AddChildNode(Node child)
        {
            Child.Add(child);
            child.Parent = this;
            CommandVisibility = Visibility.Visible;
        }
    }

    public class NodeOp
    {
        public static Node CreateDataView(DataWrapper dataWrapper, DisplayStyle style, string name,
            EventHandler<PropertyChangedEventArgs> handler)
        {
            var view = NodeOp.CreateNodes(dataWrapper, style,name);
            NodeOp.SetLines(view, handler);
            PropertyChangedEventManager.AddHandler(view, handler, "");
            return view;
        }

        public static void RemoveViewListen(Node node, EventHandler<PropertyChangedEventArgs> handler)
        {
            PropertyChangedEventManager.RemoveHandler(node, handler, "");
            RemoveNodeListen(node, handler);
        }

        private static void RemoveNodeListen(Node node, EventHandler<PropertyChangedEventArgs> handler)
        {
            if (node.Child.Count == 0) return;
            foreach (var child in node.Child)
            {
                PropertyChangedEventManager.RemoveHandler(child, handler, "");
                RemoveNodeListen(child, handler);
            }
        }

        private static void SetLines(Node node, EventHandler<PropertyChangedEventArgs> handler)
        {
            foreach (var child in node.Child)
            {
                if (child.Child.Count > 0) PropertyChangedEventManager.AddHandler(child, handler, "");
                SetLines(child, handler);
                child.AddEmpty();
                for (int i = child.Level - 1; i >= 1; i--)
                {
                    var parent = child.GetParent(i);
                    if (parent != null && parent != child)
                    {
                        if (!parent.IsLast)
                            child.AddConnectionLine();
                        else
                        {
                            child.AddEmpty();
                        }
                    }
                }

                if (child.CommandVisibility == Visibility.Collapsed)
                {
                    if (child.IsLast)
                    {
                        child.AddEndBranch();
                    }
                    else
                    {
                        child.AddBranch();
                    }
                }
            }
        }

        private static Node CreateNodes(DataWrapper dataWrapper, DisplayStyle style,string name)
        {
            var value = "{...}";
            if (dataWrapper.DataTypeInfo.Dim1 == 0)
            {
                if (dataWrapper.DataTypeInfo.DataType.IsAtomic)
                {
                    value = FormatOp.ConvertField(dataWrapper.Data, style);
                    var node = new Node() {Name = $"{name}", Level = 1, Value = value};
                    //PropertyChangedEventManager.AddHandler(node,handler,"");
                    ConvertToBitNode(dataWrapper.DataTypeInfo.DataType, node, dataWrapper.Data);
                    return node;
                }
                else
                {
                    var parent = new Node() {Name = $"{name}", Level = 1};
                    //PropertyChangedEventManager.AddHandler(parent, handler, "");
                    var assetDefinedDataType = dataWrapper.DataTypeInfo.DataType as AssetDefinedDataType;
                    foreach (DataTypeMember childMember in assetDefinedDataType.TypeMembers)
                    {
                        Debug.Assert(dataWrapper.Data is ICompositeField);
                        var childData = (dataWrapper.Data as ICompositeField).fields[childMember.FieldIndex].Item1;
                        if (childMember.IsBit)
                            childData = new BoolField(childData.GetBitValue(childMember.BitOffset) ? 1 : 0);
                        MemberConvertToNode(childMember, childData, parent);
                    }

                    return parent;
                }
            }
            else
            {
                var node = new Node() {Name = $"{name}", Level = 1};
                //PropertyChangedEventManager.AddHandler(node, handler, "");
                ConvertDim(dataWrapper.DataTypeInfo, dataWrapper.Data, style, node);
                return node;
            }
        }

        private static void ConvertDim(DataTypeInfo dataTypeInfo, IField field, DisplayStyle style, Node parent)
        {
            Debug.Assert(dataTypeInfo.Dim1 > 0 && parent != null);
            if (dataTypeInfo.Dim3 > 0)
            {
                for (int i = 0; i < dataTypeInfo.Dim3; i++)
                {
                    for (int j = 0; j < dataTypeInfo.Dim2; j++)
                    {
                        for (int k = 0; k < dataTypeInfo.Dim1; k++)
                        {
                            IField data;
                            var boolArray = field as BoolArrayField;
                            var index = i * dataTypeInfo.Dim2 * dataTypeInfo.Dim1 + j * dataTypeInfo.Dim1 + k;
                            if (boolArray != null)
                            {
                                data = new BoolField(boolArray.Get(index) ? 1 : 0);
                            }
                            else
                            {
                                var arrayField = field as ArrayField;
                                Debug.Assert(arrayField != null);
                                data = arrayField.fields[index].Item1;
                            }

                            var value = "{...}";
                            if (dataTypeInfo.DataType.IsAtomic)
                            {
                                value = FormatOp.ConvertField(data, style);
                                var node = new Node()
                                    {Name = $"{parent.Name}[{i},{j},{k}]", Level = parent.Level + 1, Value = value};
                                //node.PropertyChanged += Node_PropertyChanged;
                                parent.AddChildNode(node);
                                ConvertToBitNode(dataTypeInfo.DataType, node, data);
                            }
                            else
                            {
                                var node = new Node()
                                    {Name = $"{parent.Name}[{i},{j},{k}]", Level = parent.Level + 1, Value = value};
                                //node.PropertyChanged += Node_PropertyChanged;
                                parent.AddChildNode(node);
                                var assetDefinedDataType = dataTypeInfo.DataType as AssetDefinedDataType;
                                foreach (DataTypeMember member in assetDefinedDataType.TypeMembers)
                                {
                                    Debug.Assert(data is ICompositeField);
                                    var memberData = (data as ICompositeField).fields[member.FieldIndex].Item1;
                                    MemberConvertToNode(member, memberData, node);
                                }
                            }
                        }
                    }
                }

                return;
            }

            if (dataTypeInfo.Dim2 > 0)
            {
                for (int j = 0; j < dataTypeInfo.Dim2; j++)
                {
                    for (int k = 0; k < dataTypeInfo.Dim1; k++)
                    {
                        IField data;
                        var boolArray = field as BoolArrayField;
                        var index = j * dataTypeInfo.Dim1 + k;
                        if (boolArray != null)
                        {
                            data = new BoolField(boolArray.Get(index) ? 1 : 0);
                        }
                        else
                        {
                            var arrayField = field as ArrayField;
                            Debug.Assert(arrayField != null);
                            data = arrayField.fields[index].Item1;
                        }

                        var value = "{...}";
                        if (dataTypeInfo.DataType.IsAtomic)
                        {
                            value = FormatOp.ConvertField(data, style);
                            var node = new Node()
                                {Name = $"{parent.Name}[{j},{k}]", Level = parent.Level + 1, Value = value};
                            //node.PropertyChanged += Node_PropertyChanged;
                            parent.AddChildNode(node);
                            ConvertToBitNode(dataTypeInfo.DataType, node, data);
                        }
                        else
                        {
                            var node = new Node()
                                {Name = $"{parent.Name}[{j},{k}]", Level = parent.Level + 1, Value = value};
                            //node.PropertyChanged += Node_PropertyChanged;
                            parent.AddChildNode(node);
                            var assetDefinedDataType = dataTypeInfo.DataType as AssetDefinedDataType;
                            foreach (DataTypeMember member in assetDefinedDataType.TypeMembers)
                            {
                                Debug.Assert(data is ICompositeField);
                                var memberData = (data as ICompositeField).fields[member.FieldIndex].Item1;
                                MemberConvertToNode(member, memberData, node);
                            }
                        }
                    }
                }

                return;
            }

            if (dataTypeInfo.Dim1 > 0)
            {
                for (int k = 0; k < dataTypeInfo.Dim1; k++)
                {
                    IField data;
                    var boolArray = field as BoolArrayField;
                    var index = k;
                    if (boolArray != null)
                    {
                        data = new BoolField(boolArray.Get(index) ? 1 : 0);
                    }
                    else
                    {
                        var arrayField = field as ArrayField;
                        Debug.Assert(arrayField != null);
                        data = arrayField.fields[index].Item1;
                    }

                    var value = "{...}";
                    if (dataTypeInfo.DataType.IsAtomic)
                    {
                        value = FormatOp.ConvertField(data, style);
                        var node = new Node() {Name = $"{parent.Name}[{k}]", Level = parent.Level + 1, Value = value};
                        //node.PropertyChanged += Node_PropertyChanged;
                        parent.AddChildNode(node);
                        ConvertToBitNode(dataTypeInfo.DataType, node, data);
                    }
                    else
                    {
                        var node = new Node() {Name = $"{parent.Name}[{k}]", Level = parent.Level + 1, Value = value};
                        //node.PropertyChanged += Node_PropertyChanged;
                        parent.AddChildNode(node);
                        var assetDefinedDataType = dataTypeInfo.DataType as AssetDefinedDataType;
                        foreach (DataTypeMember member in assetDefinedDataType.TypeMembers)
                        {
                            Debug.Assert(data is ICompositeField);
                            var memberData = (data as ICompositeField).fields[member.FieldIndex].Item1;
                            MemberConvertToNode(member, memberData, node);
                        }
                    }
                }
            }
        }

        private static void ConvertToBitNode(IDataType dataType, Node parent, IField field)
        {
            if (dataType.IsBool || dataType.IsReal) return;
            for (int i = 0; i < dataType.BitSize; i++)
            {
                var bitNode = new Node()
                    {Name = $"{parent.Name}.{i}", Level = parent.Level + 1, Value = field.GetBitValue(i) ? "1" : "0"};
                parent.AddChildNode(bitNode);
            }
        }

        private static void MemberConvertToNode(IDataTypeMember member, IField data, Node parent)
        {
            var value = "{...}";
            if (member.DataTypeInfo.Dim1 == 0)
            {
                if (member.DataTypeInfo.DataType.IsAtomic)
                {
                    value = FormatOp.ConvertField(data, member.DisplayStyle);
                    var node = new Node()
                        {Name = $"{parent.Name}.{member.Name}", Level = parent.Level + 1, Value = value};
                    //node.PropertyChanged += Node_PropertyChanged;
                    parent.AddChildNode(node);
                    ConvertToBitNode(member.DataTypeInfo.DataType, node, data);
                }
                else
                {
                    var parentNode = new Node() {Name = $"{parent.Name}.{member.Name}", Level = parent.Level + 1};
                    //parentNode.PropertyChanged += Node_PropertyChanged;
                    parent.AddChildNode(parentNode);
                    var assetDefinedDataType = member.DataTypeInfo.DataType as AssetDefinedDataType;
                    foreach (DataTypeMember childMember in assetDefinedDataType.TypeMembers)
                    {
                        Debug.Assert(data is ICompositeField);
                        var childData = (data as ICompositeField).fields[childMember.FieldIndex].Item1;
                        if (childMember.IsBit)
                            childData = new BoolField(childData.GetBitValue(member.BitOffset) ? 1 : 0);

                        MemberConvertToNode(childMember, childData, parentNode);
                    }
                }
            }
            else
            {
                var node = new Node() {Name = $"{parent.Name}.{member.Name}", Level = parent.Level + 1};
                //node.PropertyChanged += Node_PropertyChanged;
                parent.AddChildNode(node);
                ConvertDim(member.DataTypeInfo, data, member.DisplayStyle, node);
            }
        }
    }
}
