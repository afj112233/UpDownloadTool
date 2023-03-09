using ICSStudio.Cip.Objects;
using ICSStudio.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Dialogs.ChangeCatalogNumber
{
    /// <summary>
    /// ChangeCatalogNumberDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeCatalogNumberDialog
    {
        private readonly ChangeCatalogNumberViewModel _viewModel;

        public ChangeCatalogNumberDialog(int driveTypeId,
            List<FeedbackType> feedbackTypes,
            List<MotorType> motorTypes,
            string catalogNumber)
        {
            InitializeComponent();

            _viewModel = new ChangeCatalogNumberViewModel(
                driveTypeId,
                feedbackTypes,
                motorTypes,
                catalogNumber,
                this);

            DataContext = _viewModel;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public int SearchMotorId
        {
            get
            {
                if (_viewModel.SearchResult != null)
                    return _viewModel.SearchResult.MotorId;

                return 0;
            }
        }

        public string SearchMotorCatalogNumber
        {
            get
            {
                if (_viewModel.SearchResult != null)
                    return _viewModel.SearchResult.CatalogNumber;

                return "<none>";
            }
        }

        private void LbxSearchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbxSearchResult.SelectedItem != null)
                LbxSearchResult.ScrollIntoView(LbxSearchResult.SelectedItem);
        }
    }
}
