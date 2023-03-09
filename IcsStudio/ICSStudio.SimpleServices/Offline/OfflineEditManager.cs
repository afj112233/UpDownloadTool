using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.EditLogs;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices
{
    internal class OfflineEditManager
    {
        private readonly List<IEditLog> _editLogs;

        internal OfflineEditManager(Controller controller)
        {
            Controller = controller;

            _editLogs = new List<IEditLog>();
        }

        internal Controller Controller { get; }

        internal void AddLog(IEditLog log)
        {
            if (log != null)
            {
                ReplaceCodeLog replaceCodeLog = log as ReplaceCodeLog;
                if (replaceCodeLog != null && replaceCodeLog.Context == "Routine")
                {
                    ReplaceCodeLog foundLog = FindReplaceCodeLog("Routine", replaceCodeLog.Program, replaceCodeLog.AOI,
                        replaceCodeLog.Routine);

                    if (foundLog != null)
                    {
                        foundLog.UpdateEditTime(replaceCodeLog.EditTime);
                        return;
                    }
                }

                SetInhibitLog setInhibitLog = log as SetInhibitLog;
                if (setInhibitLog != null && (setInhibitLog.Context == "Task" || setInhibitLog.Context == "Program"))
                {
                    SetInhibitLog foundLog = FindSetInhibitLog(setInhibitLog.Context, setInhibitLog.Name);

                    if (foundLog != null)
                    {
                        foundLog.UpdateValue(setInhibitLog.Value, setInhibitLog.EditTime);
                        return;
                    }
                }

                SetPeriodLog setPeriodLog = log as SetPeriodLog;
                if (setPeriodLog != null && setPeriodLog.Context == "Task")
                {
                    SetPeriodLog foundLog = FindSetPeriodLog(setPeriodLog.Context, setPeriodLog.Name);

                    if (foundLog != null)
                    {
                        foundLog.UpdateValue(setPeriodLog.Value, setPeriodLog.EditTime);
                        return;
                    }
                }

                //TODO(gjc): add other log here


                _editLogs.Add(log);

            }
        }

        private SetPeriodLog FindSetPeriodLog(string context, string name)
        {
            foreach (var log in _editLogs.OfType<SetPeriodLog>())
            {
                if (log.Context == context && log.Name == name)
                    return log;
            }

            return null;
        }

        private SetInhibitLog FindSetInhibitLog(string context, string name)
        {
            foreach (var log in _editLogs.OfType<SetInhibitLog>())
            {
                if (log.Context == context && log.Name == name)
                    return log;
            }

            return null;
        }

        private ReplaceCodeLog FindReplaceCodeLog(string context, string program, string aoi, string routine)
        {
            foreach (var log in _editLogs.OfType<ReplaceCodeLog>())
            {
                if (log.Context == context && log.Routine == routine)
                {
                    if (!string.IsNullOrEmpty(log.Program) && log.Program == program)
                        return log;

                    if (!string.IsNullOrEmpty(log.AOI) && log.AOI == aoi)
                        return log;
                }
            }

            return null;
        }

        internal void AddLogs(JArray jArray)
        {
            foreach (var jObject in jArray.OfType<JObject>())
            {
                AddLog(EditLog.CreateLog(jObject));
            }
        }

        internal JArray ConvertToJArray()
        {
            if (_editLogs == null)
                return null;

            if (_editLogs.Count == 0)
                return null;

            JArray array = new JArray();

            foreach (var log in _editLogs)
            {
                array.Add(log.ConvertToJObject());
            }

            return array;
        }

        internal IEditLog CreateAddTagLog(ITagCollection tagCollection, Tag tag)
        {
            AddTagLog addTagLog = AddTagLog.Create(tagCollection, tag);

            return addTagLog;
        }

        internal IEditLog CreateReplaceCodeLog(IRoutine routine)
        {
            ReplaceCodeLog replaceCodeLog = ReplaceCodeLog.Create(routine);

            return replaceCodeLog;
        }

        internal IEditLog CreateSetInhibitLog(ITask task)
        {
            SetInhibitLog setInhibitLog = SetInhibitLog.Create(task);

            return setInhibitLog;
        }

        internal IEditLog CreateSetInhibitLog(IProgram program)
        {
            SetInhibitLog setInhibitLog = SetInhibitLog.Create(program);

            return setInhibitLog;
        }

        internal IEditLog CreateSetPeriodLog(ITask task)
        {
            SetPeriodLog setInhibitLog = SetPeriodLog.Create(task);

            return setInhibitLog;
        }

        internal void Reset()
        {
            _editLogs.Clear();
        }

        internal bool HasLog => _editLogs.Count > 0;

        internal ReadOnlyCollection<IEditLog> EditLogs => _editLogs.AsReadOnly();

        internal List<ReplaceCodeLog> GetReplaceCodeLogByProgram(string programName)
        {
            List<ReplaceCodeLog> logs = new List<ReplaceCodeLog>();

            foreach (var replaceCodeLog in _editLogs.OfType<ReplaceCodeLog>())
            {
                if (string.Equals(replaceCodeLog.Program, programName))
                    logs.Add(replaceCodeLog);
            }

            return logs;
        }

        internal void RemoveLogs<T>(List<T> logs) where T : IEditLog
        {
            foreach (var log in logs)
            {
                _editLogs.Remove(log);
            }
        }

        public AddTagLog GetAddTagLog(string context, string programName, string tagName)
        {
            foreach (var addTagLog in _editLogs.OfType<AddTagLog>())
            {
                if (addTagLog.Context == context && addTagLog.Program == programName && addTagLog.TagName == tagName)
                    return addTagLog;

            }

            return null;
        }

        public void AddOrUpdateChangeAxisPropertyLog(string tagName, string propertyName)
        {
            ChangeAxisPropertyLog log = GetChangeAxisPropertyLog(tagName);
            if (log == null)
            {
                log = ChangeAxisPropertyLog.Create(tagName, propertyName);
                AddLog(log);
            }
            else
            {
                log.AddPropertyName(propertyName);
            }
        }

        private ChangeAxisPropertyLog GetChangeAxisPropertyLog(string tagName)
        {
            foreach (var log in _editLogs.OfType<ChangeAxisPropertyLog>())
            {
                if (log.TagName == tagName)
                {
                    return log;
                }
            }

            return null;
        }
    }
}
