using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.SimpleServices.Common
{
    public class AoiDefinitionCollection : IEnumerable<AoiDefinition>, IAoiDefinitionCollection
    {
        private readonly List<AoiDefinition> _aois = new List<AoiDefinition>();

        IEnumerator<IAoiDefinition> IEnumerable<IAoiDefinition>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<AoiDefinition> GetEnumerator() => _aois.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {

        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public IEnumerable<IAoiDefinition> TrackedAoiDefinitions { get; set; }

        public void Remove(int aoiUid)
        {
            throw new System.NotImplementedException();
        }

        public int Count => _aois.Count;

        public IAoiDefinition this[int index] => _aois[index];

        public void Remove(string aoiName, bool isTmp = false)
        {
            if (string.IsNullOrEmpty(aoiName)) return;
            foreach (var aoi in _aois)
            {
                if (isTmp == aoi.IsTmp && aoi.Name == aoiName)
                {
                    DataTypeCollection dataTypeCollection = aoi.ParentController.DataTypes as DataTypeCollection;
                    dataTypeCollection?.DeleteDataType(aoi.datatype);

                    _aois.Remove(aoi);

                    if (!aoi.IsTmp)
                    {
                        aoi.Dispose();
                        PropertyChangedEventManager.RemoveHandler(aoi, OnAoiPropertyChanged, "Name");
                    }

                    CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, aoi));

                    return;
                }
            }
        }

        public void Remove(IAoiDefinition aoiDefinition, bool isTmp = false)
        {
            if (aoiDefinition == null) return;
            var aoi = (AoiDefinition) aoiDefinition;
            DataTypeCollection dataTypeCollection = aoi.ParentController.DataTypes as DataTypeCollection;
            dataTypeCollection?.DeleteDataType(aoi.datatype);

            aoi.Dispose();

            _aois.Remove(aoi);

            if (!aoi.IsTmp)
            {
                PropertyChangedEventManager.RemoveHandler(aoi, OnAoiPropertyChanged, "Name");
            }

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, aoi));
        }

        public void Add(AoiDefinition aoi)
        {
            aoi.ParentCollection = this;

            _aois.Add(aoi);

            if (!aoi.IsTmp)
            {
                PropertyChangedEventManager.AddHandler(aoi, OnAoiPropertyChanged, "Name");
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, aoi));
            }
        }

        public AoiDefinition Find(string name, bool isTmp = false)
        {
            return _aois.Find(aoi => aoi.IsTmp == isTmp && aoi.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Clear()
        {
            var removeAois = _aois.ToList();

            _aois.Clear();

            foreach (var aoi in removeAois)
            {
                aoi.Dispose();
            }
            
        }

        public int GetIndex(AoiDefinition aoi)
        {
            return _aois.IndexOf(aoi);
        }

        private void OnAoiPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var aoi = (AoiDefinition) sender;
            var regex = new Regex($@"(?<!\w){aoi.OldName}(?=[ \t\r\n]*\()");
            var controller = Controller.GetInstance();
            var pendingCompileRoutine = PendingCompileRoutine.GetInstance();
            
            foreach (var program in controller.Programs)
            {
                foreach (STRoutine routine in program.Routines.Where(r => r is STRoutine))
                {
                    var isChanged = false;
                    routine.CodeText = UpdateCode(routine.CodeText, regex, aoi.Name, ref isChanged);
                    routine.PendingCodeText = UpdateCode(routine.PendingCodeText, regex, aoi.Name, ref isChanged);
                    routine.TestCodeText = UpdateCode(routine.TestCodeText, regex, aoi.Name, ref isChanged);
                    if (isChanged)
                    {
                        pendingCompileRoutine.Add(routine, false);
                        routine.IsUpdateChanged = true;
                    }
                }
            }

            foreach (var aoiDefinition in controller.AOIDefinitionCollection)
            {
                if (aoiDefinition == aoi) continue;
                foreach (STRoutine aoiDefinitionRoutine in aoiDefinition.Routines.Where(r => r is STRoutine))
                {
                    var isChanged = false;
                    aoiDefinitionRoutine.CodeText =
                        UpdateCode(aoiDefinitionRoutine.CodeText, regex, aoi.Name, ref isChanged);
                    aoiDefinitionRoutine.PendingCodeText = UpdateCode(aoiDefinitionRoutine.PendingCodeText, regex,
                        aoi.Name, ref isChanged);
                    aoiDefinitionRoutine.TestCodeText = UpdateCode(aoiDefinitionRoutine.TestCodeText, regex, aoi.Name,
                        ref isChanged);
                    if (isChanged)
                    {
                        aoiDefinitionRoutine.IsUpdateChanged = true;
                        pendingCompileRoutine.Add(aoiDefinitionRoutine, false);
                    }
                        
                }
            }
        }

        private List<string> UpdateCode(List<string> codeList, Regex regex, string name, ref bool isChanged)
        {
            if (codeList == null) return null;
            var code = string.Join("\n", codeList);
            var cleanCode = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(code, null);
            if (!string.IsNullOrEmpty(cleanCode))
            {
                var matches = regex.Matches(cleanCode);
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    var match = matches[i];
                    code = code.Remove(match.Index, match.Value.Length);
                    code = code.Insert(match.Index, name);
                }

                isChanged = true;
                return code.Split('\n').ToList();
            }

            return codeList;
        }
    }
}