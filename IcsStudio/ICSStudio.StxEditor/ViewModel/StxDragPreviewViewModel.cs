using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using ICSStudio.StxEditor.Interfaces;

namespace ICSStudio.StxEditor.ViewModel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class StxDragPreviewViewModel : ViewModelBase, IWeakEventListener
    {
        private readonly IStxEditorOptions _options;
        private Point _position;
        private string _text;
        private Visibility _visibility;

        public StxDragPreviewViewModel(IStxEditorOptions options)
        {
            _options = options;
            PropertyChangedEventManager.AddListener(options, this, string.Empty);
        }

        public FontFamily FontFamily => new FontFamily(_options.FontFamilyName);

        public double FontSize => _options.FontSize;

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility == value)
                    return;

                _visibility = _options.ShowDragPreview ? value : Visibility.Hidden;
                RaisePropertyChanged();
            }
        }

        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }

        public Thickness Margin
        {
            get
            {
                var position = Position;
                var x = position.X;
                position = Position;
                var y = position.Y;
                return new Thickness(x, y, 0.0, 0.0);
            }
        }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                    return;

                _position = value;

                RaisePropertyChanged();
                RaisePropertyChanged("Margin");
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(PropertyChangedEventManager)))
                return false;

            var e1 = e as PropertyChangedEventArgs;
            if (sender == _options)
                EditorOptionsPropertyChanged(sender, e1);

            return true;
        }

        private void EditorOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != _options)
                return;

            if (e.PropertyName == "FontFamilyName")
                RaisePropertyChanged("FontFamily");

            if (e.PropertyName == "FontSize")
                RaisePropertyChanged("FontSize");

            if (e.PropertyName == "ShowInLineDisplay")
                RaisePropertyChanged("ShowInLineDisplay");

            if (e.PropertyName == "CanZoom")
                RaisePropertyChanged("CanZoom");

            if (e.PropertyName == "ShowTest")
                RaisePropertyChanged("ShowTest");

            if (e.PropertyName == "ShowOriginal")
                RaisePropertyChanged("ShowOriginal");

            if (e.PropertyName == "ShowPending")
                RaisePropertyChanged("ShowPending");

            if (e.PropertyName == "HideAll")
                RaisePropertyChanged("HideAll");

            if (e.PropertyName == "Cleanup")
                RaisePropertyChanged("Cleanup");

            if (e.PropertyName == "IsConnecting")
                RaisePropertyChanged("IsConnecting");

            if(e.PropertyName== "CanEditorInput")
                RaisePropertyChanged("CanEditorInput");
        }
    }
}