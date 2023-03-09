using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class TemplateViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private string _selectedTab;
        private bool _isDirty;

        public TemplateViewModel(Template panel)
        {
            panel.DataContext = this;
            Control = panel;
            Tabs.Add("General tab");
            Tabs.Add("Display tab");
            Tabs.Add("Pens tab");
            Tabs.Add("X-Axis tab");
            Tabs.Add("Y-Axis tab");
        }

        public string SelectedTab
        {
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    SetOption();
                }
            }
            get { return _selectedTab; }
        }

        public List<string> Tabs { get; } = new List<string>();
        public ObservableCollection<string> Options { set; get; } = new ObservableCollection<string>();
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
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;

        private void SetOption()
        {
            Options.Clear();
            switch (SelectedTab)
            {
                case "General tab":
                    Options.Add("Display Title");
                    break;
                case "Display tab":
                    Options.Add("Display Line Description");
                    Options.Add("Description Length");
                    Options.Add("Display Min/Max Values");
                    Options.Add("Background Color");
                    Options.Add("Display Pen Bitmaps");
                    Options.Add("Display Pen Values");
                    Options.Add("Display Current Time");
                    Options.Add("Scroll Mode");
                    Options.Add("Display Scroll Mechanism");
                    Options.Add("Display Value Bar");
                    Options.Add("Data Point Connection");
                    Options.Add("Chart Radix");
                    Options.Add("Custom Colors");
                    Options.Add("Display Milliseconds");
                    Options.Add("Display 24 Hour Format");
                    Options.Add("Font Size");
                    Options.Add("Font Information");
                    Options.Add("Legend Position");
                    Options.Add("Legend Max Viewable Pens");
                    Options.Add("Text Color");
                    break;
                case "Pens tab":
                    Options.Add("Visible");
                    Options.Add("Width");
                    Options.Add("Color");
                    Options.Add("Type");
                    Options.Add("Marker");
                    Options.Add("Style");
                    Options.Add("Lower Bound");
                    Options.Add("Upper Bound");
                    Options.Add("Pens");
                    break;
                case "X-Axis tab":
                    Options.Add("Display X-Axis Scale");
                    Options.Add("Display X-Axis Grid Lines");
                    Options.Add("Number of X-Axis Major Grid Lines");
                    Options.Add("X-Axis Gird Color");
                    Options.Add("Start Time");
                    Options.Add("Time Span");
                    Options.Add("Start Date");
                    Options.Add("Number of X-Axis Minor Grid Lines");
                    Options.Add("Display Date On Scale");
                    break;
                case "Y-Axis tab":
                    Options.Add("Display Y-Axis Scale");
                    Options.Add("Display Y-Axis Grid Lines");
                    Options.Add("Number of Y-Axis Major Grid Lines");
                    Options.Add("Number of Decimal Places");
                    Options.Add("Y-Axis Gird Color");
                    Options.Add("Y-Axis Min/Max Options");
                    Options.Add("Y-Axis Scale Options");
                    Options.Add("Isolated Graphing");
                    Options.Add("Percent Isolated");
                    Options.Add("Y-Axis Custom Min Value");
                    Options.Add("Y-Axis custom Max Value");
                    Options.Add("Scale As Percentage");
                    Options.Add("Number of X-Axis Minor Grid Lines");
                    break;
            }
        }

    }
}
