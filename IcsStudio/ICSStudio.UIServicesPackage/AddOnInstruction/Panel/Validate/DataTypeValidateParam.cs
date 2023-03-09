using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel.Validate
{
    public class DataTypeValidateParam:DependencyObject
    {
        public static readonly DependencyProperty RowInfoProperty = DependencyProperty.Register(
            "RowInfo", typeof(ParametersRow), typeof(DataTypeValidateParam), new PropertyMetadata(default(ParametersRow)));

        public static readonly DependencyProperty LocalInfoProperty = DependencyProperty.Register(
            "LocalInfo", typeof(LocalTagRow), typeof(DataTypeValidateParam), new PropertyMetadata(default(LocalTagRow)));

        public static readonly DependencyProperty ParameterViewModelProperty = DependencyProperty.Register(
            "ParameterViewModel", typeof(ParametersViewModel), typeof(DataTypeValidateParam), new PropertyMetadata(default(ParametersViewModel)));

        public static readonly DependencyProperty LocalTagsViewModelProperty = DependencyProperty.Register(
            "LocalTagsViewModel", typeof(LocalTagsViewModel), typeof(DataTypeValidateParam), new PropertyMetadata(default(LocalTagsViewModel)));

        public LocalTagsViewModel LocalTagsViewModel
        {
            get { return (LocalTagsViewModel) GetValue(LocalTagsViewModelProperty); }
            set { SetValue(LocalTagsViewModelProperty, value); }
        }
        public ParametersViewModel ParameterViewModel
        {
            get { return (ParametersViewModel) GetValue(ParameterViewModelProperty); }
            set { SetValue(ParameterViewModelProperty, value); }
        }
        public LocalTagRow LocalInfo
        {
            get { return (LocalTagRow) GetValue(LocalInfoProperty); }
            set { SetValue(LocalInfoProperty, value); }
        }

        public ParametersRow RowInfo
        {
            get { return (ParametersRow) GetValue(RowInfoProperty); }
            set { SetValue(RowInfoProperty, value); }
        }
    }
}
