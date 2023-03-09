using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIServicesPackage.RoutineProperties.Panel
{
    class SheetLayoutViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;

        public SheetLayoutViewModel(SheetLayout panel, IRoutine routine)
        {
            Control = panel;
            panel.DataContext = this;
            SheetSizeList = new List<string>()
            {
                "Letter - 8.5 x 11 in", "Legal - 8.5 x 14 in", "Tabloid - 11 x 17 in", "A - 8.5 x 11 in",
                "B - 11 x 17 in", "C - 17 x 22 in", "D - 22 x 34 in", "E - 34 x 44 in", "A4 - 210 x 297 mm",
                "A3 - 297 x 420 mm", "A2 - 420 x 594 mm", "A1 - 594 x 841 mm", "A0 - 841 x 1189 mm"
            };
        }

        public List<string> SheetSizeList { set; get; }
        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {
           
        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
