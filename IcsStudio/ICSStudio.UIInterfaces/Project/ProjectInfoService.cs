using System.Collections.Generic;
using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.UIInterfaces.Project
{
    [Guid("DD66E577-67A1-4160-AF29-434332B6DB43")]
    [ComVisible(true)]
    public interface IProjectInfoService
    {
        IProject CurrentProject { get; }
        IController Controller { get; }

        void OpenJsonFile(string fileName, bool needAddDataType = true);
        void ImportFile(string importFile, string saveFile);
        int Save(bool needNativeCode);
        int SaveAs(string fileName);
        int ExportFile(string exportFile);
        bool ImportData(ProjectItemType kind, IBaseObject associatedObject, int? startIndex = null, int? endIndex = null);

        string NewFile();

        bool VerifyInDialog();
        void Verify(IController controller);
        void Verify(ITag tag);
        void Verify(IRoutine routine);
        void VerifyReferenceProgram(IProgram deleteProgram);
        void Verify(IDeviceModule module);
        void VerifyAxisTag(ITag tag);

        void VerifyReference(List<ITag> delTags);

        void VerifyParameterConnection();

        void VerifyToolchain();

        //TODO(gjc): edit later
        void SetProjectDirty();
    }

    [Guid("B5F627C5-B4E5-4FE4-B03F-6CA055E9FF25")]
    // ReSharper disable once InconsistentNaming
    public interface SProjectInfoService
    {

    }
}
