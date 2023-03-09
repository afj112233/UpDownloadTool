//------------------------------------------------------------------------------
// <copyright file="QuickWatchPackageControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.QuickWatchPackage.View;
using ICSStudio.QuickWatchPackage.View.Models;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.QuickWatchPackage
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for QuickWatchPackageControl.
    /// </summary>
    public partial class QuickWatchPackageControl : UserControl
    {
        private readonly QuickWatchViewModel _vm;
        private readonly DispatcherTimer _uiUpdateTimer;
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickWatchPackageControl"/> class.
        /// </summary>
        public QuickWatchPackageControl()
        {
            this.InitializeComponent();
            _vm = new QuickWatchViewModel(this);
            DataContext = _vm;
            _uiUpdateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle)
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };

            _uiUpdateTimer.Tick += OnTick;
            Controller controller = Controller.GetInstance();
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsVisible)
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        if(!_uiUpdateTimer.IsEnabled)
                            _uiUpdateTimer.Start();
                    }
                    else
                    {
                        if (_uiUpdateTimer.IsEnabled)
                            _uiUpdateTimer.Stop();
                    }
                }
            });
        }
        private void OnTick(object sender, EventArgs e)
        {
            try
            {

                var controller = Controller.GetInstance();
                if (controller == null)
                    return;

                if (!controller.IsOnline)
                    return;
                
                TagSyncController tagSyncController
                    = controller.Lookup(typeof(TagSyncController)) as TagSyncController;
                if (tagSyncController == null)
                    return;

                foreach (var item in MonitorControl.MainDataGrid.ItemContainerGenerator.Items.OfType<MonitorTagItem>())
                {
                    if (!(item.ItemType == TagItemType.Array || item.ItemType == TagItemType.Struct))
                    {
                        var row = MonitorControl.MainDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                        if (row != null && row.IsVisible)
                        {
                            tagSyncController.Update(item.Tag, item.Name);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        public void AddMonitorRoutine(IRoutine routine)
        {
           _vm.AddMonitorRoutine(routine);
            _vm.ExplicitProgramModule = routine.ParentCollection.ParentProgram;
        }

        public void RemoveMonitorRoutine(IRoutine routine)
        {
            _vm.RemoveMonitorRoutine(routine);
        }

        public void AddScope(ITagCollectionContainer container)
        {
            _vm.AddScope(container);
        }

        public void RemoveScope(ITagCollectionContainer container)
        {
            _vm.RemoveScope(container);
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var textBox = ((TextBox)sender);
                var caretIndex = textBox.CaretIndex;
                textBox.Text = ((TextBox)sender).Text.Insert(caretIndex, "_");
                textBox.CaretIndex = caretIndex + 1;
                e.Handled = true;
            }
        }

        public void AddExplicitScope(ITagCollectionContainer container)
        {
            _vm.ExplicitProgramModule = container as Program;
        }

        public void RemoveExplicitScope(ITagCollectionContainer container)
        {
            if (_vm.ExplicitProgramModule == container)
                _vm.ExplicitProgramModule = null;
        }

        public void SetAoiMonitor(IRoutine routine, AoiDataReference reference)
        {
            _vm.SetAoiMonitor(routine,reference);
        }

        public void Clean()
        {
            _vm.Clean();
        }
        
        public void UnlockQuickWatch()
        {
            _vm.UnlockQuickWatch();
        }

        public void LockQuickWatch()
        {
            _uiUpdateTimer.Stop();
            _vm.LockQuickWatch();
        }

        private void QuickWatchPackageControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Controller.GetInstance().IsOnline)
            {
                _uiUpdateTimer.Start();
            }
        }

        private void QuickWatchPackageControl_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_uiUpdateTimer.IsEnabled && IsVisible&&Controller.GetInstance().IsOnline)
                _uiUpdateTimer.Start();
        }
    }
}