using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.DownloadChange
{
    public partial class ProjectDiffBuilder
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly JsonDiffPatch.JsonDiffPatch _diffPath;

        public ProjectDiffBuilder()
        {
            _diffPath = new JsonDiffPatch.JsonDiffPatch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue">from plc</param>
        /// <param name="newValue">from ics studio</param>
        /// <returns></returns>
        public ProjectDiffModel BuildDiffModel(JObject oldValue, JObject newValue)
        {
            ProjectInfoExtractor oldInfoExtractor = new ProjectInfoExtractor(oldValue);
            ProjectInfoExtractor newInfoExtractor = new ProjectInfoExtractor(newValue);

            ProjectDiffModel diffModel = new ProjectDiffModel();

            diffModel.ControllerProperties =
                DiffControllerProperties(oldInfoExtractor.ControllerProperties, newInfoExtractor.ControllerProperties);

            diffModel.ControllerTags =
                DiffControllerTags(oldInfoExtractor.ControllerTags, newInfoExtractor.ControllerTags);

            diffModel.DataTypes =
                DiffDataTypes(oldInfoExtractor.DataTypes, newInfoExtractor.DataTypes);

            diffModel.AOIDefinitions =
                DiffAOIDefinitions(oldInfoExtractor.AOIDefinitions, newInfoExtractor.AOIDefinitions);

            diffModel.Tasks =
                DiffTasks(oldInfoExtractor.Tasks, newInfoExtractor.Tasks);

            diffModel.Programs =
                DiffPrograms(oldInfoExtractor.Programs, newInfoExtractor.Programs);

            diffModel.Modules =
                DiffModules(oldInfoExtractor.Modules, newInfoExtractor.Modules);

            return diffModel;
        }
    }
}
