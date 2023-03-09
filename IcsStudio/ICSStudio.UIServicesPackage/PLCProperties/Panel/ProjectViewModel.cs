using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class ProjectViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _check1;
        private bool _check2;
        private bool _check3;
        private bool _check4;
        private bool _enabled1;
        private bool _enabled2;
        private bool _enabled3;
        private bool _enabled4;
        private bool _isDirty;
        private IController _controller;
        public ProjectViewModel(Project panel,IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = controller;
            var info =new FileInfo(controller.ProjectLocaleName);
            Name = info.Name;
            Path = info.FullName;
            CreateTime = controller.ProjectCreationDate.ToString("yyyy/MM/dd hh:mm:ss");
            EditedTime = controller.LastModifiedDate.ToString("yyyy/MM/dd hh:mm:ss");
            Enabled1 = !controller.IsOnline;
            Enabled3 = !controller.IsOnline;
        }


        public void Refresh()
        {
            Enabled1 = !_controller.IsOnline;
            Enabled3 = !_controller.IsOnline;
        }

        public string Name { set; get; }
        public string Path { set; get; }
        public string CreateTime { set; get; }
        public string EditedTime { set; get; }

        public bool Check1
        {
            set
            {
                Set(ref _check1 , value);
                if (_check1)
                {
                    Check2 = true;
                    Enabled2 = true;
                }
                else
                {
                    Check2 = false;
                    Enabled2 = false;
                }
            }
            get { return _check1; }
        }

        public bool Check2
        {
            set { Set(ref _check2 , value); }
            get { return _check2; }
        }

        public bool Check3
        {
            set
            {
                Set(ref _check3 , value);
                if (_check3)
                {
                    Check4 = true;
                    Enabled4 = true;
                }
                else
                {
                    Check4 = false;
                    Enabled4 = false;
                }
            }
            get { return _check3; }
        }

        public bool Check4
        {
            set { Set(ref _check4 , value); }
            get { return _check4; }
        }

        public bool Enabled1
        {
            set { Set(ref _enabled1 , value); }
            get { return _enabled1; }
        }

        public bool Enabled2
        {
            set { Set(ref _enabled2 , value); }
            get { return _enabled2; }
        }

        public bool Enabled3
        {
            set { Set(ref _enabled3 , value); }
            get { return _enabled3; }
        }

        public bool Enabled4
        {
            set { Set(ref _enabled4 , value); }
            get { return _enabled4; }
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
    }
}
