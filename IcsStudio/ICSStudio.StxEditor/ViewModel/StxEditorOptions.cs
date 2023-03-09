using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.Interfaces;

namespace ICSStudio.StxEditor.ViewModel
{
    public sealed class StxEditorOptions : IStxEditorOptions
    {
        private readonly Dictionary<StxTextItem, StxTextItemColor> _colors;

        private string _fontFamilyName;
        private double _fontSize;
        private bool _showDragPreview;
        private bool _showLineNumbers;
        private bool _showInLineDisplay;
        private bool _canZoom;
        private bool _showOriginal;
        private bool _showTest;
        private bool _showPending;
        private bool _hideAll;
        private bool _cleanup;
        private bool _isConnecting;
        private bool _isRaiseCommandStatus;
        private bool _isDirty;
        private bool _resetLineInitial;
        private bool _canEditorInput;

        public StxEditorOptions()
        {
            _fontFamilyName = "Consolas";
            _fontSize = 12;
            _showDragPreview = true;
            _showLineNumbers = true;
            _colors = new Dictionary<StxTextItem, StxTextItemColor>
            {
                {
                    StxTextItem.LineNumber,
                    new StxTextItemColor(Color.FromRgb(43, 145, 175), Color.FromRgb(255, 255, 255), true)
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string FontFamilyName
        {
            get { return _fontFamilyName; }
            set
            {
                if (_fontFamilyName == value)
                    return;

                _fontFamilyName = value;
                RaisePropertyChanged();
            }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowDragPreview
        {
            get { return _showDragPreview; }
            set
            {
                if (_showDragPreview == value)
                    return;

                _showDragPreview = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowLineNumbers
        {
            get { return _showLineNumbers; }
            set
            {
                if (_showLineNumbers == value)
                    return;

                _showLineNumbers = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowInLineDisplay
        {
            set
            {
                if (_showInLineDisplay == value) return;
                _showInLineDisplay = value;
                RaisePropertyChanged();
            }
            get { return _showInLineDisplay; }
        }

        public bool ShowOriginal
        {
            get { return _showOriginal; }
            set
            {
                if (value)
                {
                    _showTest = false;
                    _showPending = false;
                }

                _showOriginal = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowTest
        {
            get { return _showTest; }
            set
            {
                _showTest = value;
                if (value)
                {
                    _showOriginal = false;
                    _showPending = false;
                }

                RaisePropertyChanged();
            }
        }

        public bool ShowPending
        {
            get { return _showPending; }
            set
            {
                if (value)
                {
                    _showOriginal = false;
                    _showTest = false;
                }

                _showPending = value;
                RaisePropertyChanged();
            }
        }
        
        public bool IsOnlyTextMarker { get; set; }

        public bool CanEditorInput
        {
            get { return _canEditorInput; }
            set
            {
                _canEditorInput = value; 
                RaisePropertyChanged();
            }
        }

        public bool HideAll
        {
            get { return _hideAll; }
            set
            {
                _hideAll = value;
                if (_hideAll)
                {
                    _showOriginal = false;
                    _showPending = false;
                    _showTest = false;
                }

                RaisePropertyChanged();
            }
        }

        public bool CanZoom
        {
            set
            {
                if (_canZoom != value)
                {
                    _canZoom = value;
                    RaisePropertyChanged();
                }
            }
            get { return _canZoom; }
        }

        public bool Cleanup
        {
            get { return _cleanup; }
            set
            {
                _cleanup = value;
                RaisePropertyChanged();
            }
        }

        public StxTextItemColor GetItemColor(StxTextItem item)
        {
            return _colors[item];
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                if (_isConnecting != value)
                {
                    _isConnecting = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ResetLineInitial
        {
            set
            {
                _resetLineInitial = value;
                RaisePropertyChanged();
            }
            get { return _resetLineInitial; }
        }

        public bool IsRaiseCommandStatus
        {
            set
            {
                _isRaiseCommandStatus = value;
                RaisePropertyChanged();
            }
            get { return _isRaiseCommandStatus; }
        }

        public bool IsDirty
        {
            set
            {
                _isDirty = value;
                RaisePropertyChanged();
            }
            get { return _isDirty; }
        }

        public AoiDataReference SelectedDataReference { get; set; }
        public bool IsTopLoaded { get; set; }
        public bool IsBottomLoaded { get; set; }


        public bool CanAcceptPendingRoutineCommand { set; get; } = true;
        public bool CanAssembledAcceptPendingRoutineCommand { set; get; } = false;

        public bool CanCancelAcceptedPendingRoutineCommand { set; get; } = false;
        public bool CanAssembledAcceptedProgramCommand { set; get; } = false;

        public bool CanCancelAcceptedProgramCommand { set; get; } = false;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}