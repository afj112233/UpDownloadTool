using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.Dialogs.Filter
{
    public class NameFilterPopup : Popup
    {
        private readonly FilterView _filterView;
        private readonly FilterViewModel _filterViewModel;

        public NameFilterPopup()
        {
            StaysOpen = false;
            AllowsTransparency = true;
            _filterView = new FilterView(this);
            _filterViewModel = new FilterViewModel(Controller.GetInstance(), null, false, false, String.Empty);
            _filterView.DataContext = _filterViewModel;
            _filterView.HorizontalAlignment = HorizontalAlignment.Left;
            _filterView.VerticalAlignment = VerticalAlignment.Top;
            Child = _filterView;
            LostFocus += NameFilterPopup_LostFocus;
            GotFocus += NameFilterPopup_GotFocus;
        }

        private void NameFilterPopup_GotFocus(object sender, RoutedEventArgs e)
        {
            IsGotFocused = true;
        }

        public bool IsGotFocused { get; set; } = true;
        private void NameFilterPopup_LostFocus(object sender, RoutedEventArgs e)
        {
            IsGotFocused = false;
        }
        
        public NameFilterPopup(IProgramModule program, string preFilterDataTypes, bool isReference)
        {
            StaysOpen = false;
            AllowsTransparency = true;
            _filterView = new FilterView(this);
            _filterViewModel =
                new FilterViewModel(Controller.GetInstance(), program, false, false, preFilterDataTypes, isReference);
            _filterView.DataContext = _filterViewModel;
            _filterView.HorizontalAlignment = HorizontalAlignment.Left;
            _filterView.VerticalAlignment = VerticalAlignment.Top;
            _filterViewModel.IsCrossReference = isReference;
            Child = _filterView;
            LostFocus += NameFilterPopup_LostFocus;
            GotFocus += NameFilterPopup_GotFocus;
        }

        public void SetPreFilterDataTypes(string preFilterDataTypes)
        {
            _filterViewModel.SetDataTypeFilter(preFilterDataTypes);
        }

        public void ResetAll(string scopeName, FrameworkElement followControl, bool showController = true)
        {
            IsGotFocused = true;
            ResetScope(scopeName, showController);
            ResetPosition(followControl);
        }

        public void ResetPosition(FrameworkElement followControl, PlacementMode placementMode = PlacementMode.Bottom)
        {
            Placement = placementMode;
            PlacementTarget = followControl;
        }

        public void ResetScope(string scopeName, bool showController)
        {
            _filterViewModel.SetScopeName(scopeName, showController);
        }

        public void Reset()
        {
            _filterViewModel.Name = _filterViewModel.Name;
        }

        public FilterViewModel FilterViewModel => _filterViewModel;
    }
}
