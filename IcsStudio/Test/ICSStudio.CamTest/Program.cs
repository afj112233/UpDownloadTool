using System;
using System.Windows;
using ICSStudio.Dialogs.ConfigDialogs;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.CamTest
{
    public class Program : Application
    {
        [STAThread]
        static void Main()
        {
            Application app = new Application();

            string fileName = @"C:\Users\tlm\Desktop\l18.json";
            var controller = Controller.Open(fileName);

            Tag tag = TagsFactory.CreateTag((TagCollection)controller.Tags, "cam", "CAM", 3, 0, 0);
            //Tag tag = TagsFactory.CreateTag((TagCollection)controller.Tags, "cam", "CAM_PROFILE", 3, 0, 0);

            CamEditorDialog dialog = new CamEditorDialog(new CamEditorViewModel(tag));
            dialog.ShowDialog();

            app.Run();
            //app.Run(dialog);
        }

    }
}
