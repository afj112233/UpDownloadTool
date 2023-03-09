using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DataGridTest.Models;

namespace ICSStudio.DataGridTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            MonitorTagCollection = new MonitorTagCollection()
            {
                new MonitorTagItem() {Name = "Name0"}
            };

            TestCommand = new RelayCommand(ExecuteTestCommand);
            RemoveCommand = new RelayCommand(ExecuteRemoveCommand);
        }

        public MonitorTagCollection MonitorTagCollection { get; set; }
        public RelayCommand TestCommand { get; }
        public RelayCommand RemoveCommand { get; }

        private void ExecuteTestCommand()
        {
            MonitorTagItem root = new MonitorTagItem() { Name = "Test" };
            List<MonitorTagItem> listItems = new List<MonitorTagItem>();

            for (int i = 0; i < 1000; i++)
            {
                listItems.Add(new MonitorTagItem() { Name = $"Test.{i}", ParentItem = root });
            }

            root.Children = listItems;

            MonitorTagCollection.AddMonitorTagItem(root);
        }

        private void ExecuteRemoveCommand()
        {
            MonitorTagCollection.RemoveMonitorTagItems("Test");
        }
    }
}