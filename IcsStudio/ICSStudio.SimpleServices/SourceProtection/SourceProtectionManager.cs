using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SourceProtection;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.SourceProtection
{
    [Flags]
    public enum SourcePermission
    {
        None = 0,

        Use = 1 << 0,
        View = 1 << 1,
        Export = 1 << 2,
        Copy = 1 << 3,
        Edit = 1 << 4,
        Protect = 1 << 5,

        All = Use | View | Export | Copy | Edit | Protect,
    }

    public class SourceProtectionManager
    {
        // ISourceProtected->IRoutine,IAoiDefinition

        private readonly Dictionary<IRoutine, string> _protectedRoutines;
        private readonly Dictionary<IAoiDefinition, string> _protectedAoiDefinitions;

        public SourceProtectionManager(Controller controller)
        {
            Controller = controller;

            _protectedRoutines = new Dictionary<IRoutine, string>();
            _protectedAoiDefinitions = new Dictionary<IAoiDefinition, string>();

            Provider = new SourceKeyProvider();

            Update();
        }

        public Controller Controller { get; }

        public SourceKeyProvider Provider { get; }

        public void Update()
        {
            Provider.LoadConfig();

            Provider.LoadSourceKeys();

            UpdateProtectedRoutines();

            UpdateProtectedAoiDefinitions();
        }

        public int Protect(ISourceProtected source, string key)
        {
            var routine = source as IRoutine;
            if (routine != null)
                return Protect(routine, key);

            var aoiDefinition = source as IAoiDefinition;
            if (aoiDefinition != null)
                return Protect(aoiDefinition, key);

            throw new NotImplementedException("add code for Protect");
        }

        public int Unprotect(ISourceProtected source)
        {
            var routine = source as IRoutine;
            if (routine != null)
                return Unprotect(routine);

            var aoiDefinition = source as IAoiDefinition;
            if (aoiDefinition != null)
                return Unprotect(aoiDefinition);

            throw new NotImplementedException("add code for Unprotect");
        }

        public SourcePermission GetPermission(ISourceProtected source)
        {
            var routine = source as IRoutine;
            if (routine != null)
                return GetPermission(routine);

            var aoiDefinition = source as IAoiDefinition;
            if (aoiDefinition != null)
                return GetPermission(aoiDefinition);

            throw new NotImplementedException("add code for GetPermission");
        }

        public EncodedData CreateEncodedData(IRoutine routine)
        {
            var permission = GetPermission(routine);
            if (permission == SourcePermission.All)
            {
                string key = _protectedRoutines[routine];
                var protection = ProtectionFactory.Create();

                STRoutine stRoutine = routine as STRoutine;
                if (stRoutine != null)
                {
                    string plainText = JArray.FromObject(stRoutine.CodeText).ToString();

                    EncodedData encodedData = new EncodedData
                    {
                        EncryptionConfig = protection.EncryptionConfig,
                        Text = protection.Encrypt(plainText, key)
                    };

                    return encodedData;
                }

                RLLRoutine rllRoutine = routine as RLLRoutine;
                if (rllRoutine != null)
                {
                    string plainText = JArray.FromObject(rllRoutine.CodeText).ToString();

                    EncodedData encodedData = new EncodedData
                    {
                        EncryptionConfig = protection.EncryptionConfig,
                        Text = protection.Encrypt(plainText, key)
                    };

                    return encodedData;
                }

                //TODO(gjc): add code here
            }


            return null;
        }

        public EncodedData CreateEncodedData(IAoiDefinition aoiDefinition)
        {
            var permission = GetPermission(aoiDefinition);

            if (permission == SourcePermission.All)
            {
                string key = _protectedAoiDefinitions[aoiDefinition];
                var protection = ProtectionFactory.Create();

                AoiDefinition definition = aoiDefinition as AoiDefinition;
                if (definition != null)
                {
                    JArray routines = new JArray();
                    AOIExtend.ToAOIRoutineConfig(routines, definition.Routines);

                    string plainText = routines.ToString();

                    EncodedData encodedData = new EncodedData
                    {
                        EncryptionConfig = protection.EncryptionConfig,
                        Text = protection.Encrypt(plainText, key)
                    };

                    return encodedData;
                }
            }

            //TODO(gjc): add code here

            return null;
        }

        public string GetKeyBySource(ISourceProtected source)
        {
            var routine = source as IRoutine;
            if (routine != null)
            {
                if (_protectedRoutines.ContainsKey(routine))
                    return _protectedRoutines[routine];
            }

            var aoiDefinition = source as IAoiDefinition;
            if (aoiDefinition != null)
            {
                if (_protectedAoiDefinitions.ContainsKey(aoiDefinition))
                    return _protectedAoiDefinitions[aoiDefinition];
            }

            return null;
        }

        public string GetDisplayNameByKey(string key)
        {
            Contract.Assert(!string.IsNullOrEmpty(key));

            string name = Provider.KeyDictionary[key];

            if (!string.IsNullOrEmpty(name))
                return name;

            int length = key.Length;
            int displayLength = length / 3;
            displayLength = Math.Max(displayLength, 1);

            int replayLength = length - displayLength;
            replayLength = Math.Max(replayLength, 3);

            return key.Substring(0, displayLength) + new string('*', replayLength);
        }

        public void Decode()
        {
            DecodeRoutines();

            DecodeAoiDefinitions();
        }

        private void UpdateProtectedRoutines()
        {
            _protectedRoutines.Clear();

            foreach (var program in Controller.Programs)
            {
                foreach (var routine in program.Routines)
                {
                    if (routine.IsEncrypted)
                    {
                        STRoutine stRoutine = routine as STRoutine;
                        if (stRoutine != null)
                        {
                            int config = stRoutine.EncodedData.EncryptionConfig;
                            var sourceProtection = ProtectionFactory.Create(config);

                            foreach (var key in Provider.KeyDictionary.Keys)
                            {
                                if (sourceProtection.CheckSourceKey(stRoutine.EncodedData.Text, key))
                                {
                                    _protectedRoutines.Add(routine, key);
                                }
                            }
                        }

                        RLLRoutine rllRoutine = routine as RLLRoutine;
                        if (rllRoutine != null)
                        {
                            int config = rllRoutine.EncodedData.EncryptionConfig;
                            var sourceProtection = ProtectionFactory.Create(config);

                            foreach (var key in Provider.KeyDictionary.Keys)
                            {
                                if (sourceProtection.CheckSourceKey(rllRoutine.EncodedData.Text, key))
                                {
                                    _protectedRoutines.Add(routine, key);
                                }
                            }
                        }

                        //TODO(gjc): add code here
                    }
                }
            }
        }

        private void UpdateProtectedAoiDefinitions()
        {
            _protectedAoiDefinitions.Clear();

            foreach (var aoiDefinition in Controller.AOIDefinitionCollection.OfType<AoiDefinition>())
            {
                if (aoiDefinition.IsEncrypted)
                {
                    int config = aoiDefinition.EncodedData.EncryptionConfig;
                    var sourceProtection = ProtectionFactory.Create(config);

                    foreach (var key in Provider.KeyDictionary.Keys)
                    {
                        if (sourceProtection.CheckSourceKey(aoiDefinition.EncodedData.Text, key))
                        {
                            _protectedAoiDefinitions.Add(aoiDefinition, key);
                        }
                    }
                }
            }
        }

        private SourcePermission GetPermission(IRoutine routine)
        {
            if (!routine.IsEncrypted)
                return SourcePermission.None;

            if (!_protectedRoutines.ContainsKey(routine))
                return SourcePermission.Use;

            return SourcePermission.All;
        }

        private SourcePermission GetPermission(IAoiDefinition aoiDefinition)
        {
            if (!aoiDefinition.IsEncrypted)
                return SourcePermission.None;

            if (!_protectedAoiDefinitions.ContainsKey(aoiDefinition))
                return SourcePermission.Use;

            return SourcePermission.All;
        }

        private int Unprotect(IRoutine routine)
        {
            if (!routine.IsEncrypted)
                return 0;

            var permission = GetPermission(routine);
            if (permission == SourcePermission.Use)
                return -1;

            STRoutine stRoutine = routine as STRoutine;
            if (stRoutine != null)
            {
                stRoutine.IsEncrypted = false;

                stRoutine.EncodedData = null;

                _protectedRoutines.Remove(routine);

                return 0;
            }

            RLLRoutine rllRoutine = routine as RLLRoutine;
            if (rllRoutine != null)
            {
                rllRoutine.IsEncrypted = false;

                rllRoutine.EncodedData = null;

                _protectedRoutines.Remove(routine);

                return 0;
            }

            //TODO(gjc): add other routine here

            return -1;
        }

        private int Unprotect(IAoiDefinition aoiDefinition)
        {
            AoiDefinition definition = aoiDefinition as AoiDefinition;
            if (definition == null)
                return -1;

            if (!definition.IsEncrypted)
                return 0;

            var permission = GetPermission(definition);
            if (permission == SourcePermission.Use)
                return -1;

            definition.IsEncrypted = false;
            definition.EncodedData = null;

            _protectedAoiDefinitions.Remove(definition);

            return 0;

        }

        private int Protect(IRoutine routine, string key)
        {
            if (routine.IsEncrypted && !_protectedRoutines.ContainsKey(routine))
            {
                return -1;
            }

            if (_protectedRoutines.ContainsKey(routine))
                _protectedRoutines[routine] = key;
            else
                _protectedRoutines.Add(routine, key);

            STRoutine stRoutine = routine as STRoutine;
            if (stRoutine != null)
            {
                stRoutine.IsEncrypted = true;
                stRoutine.EncodedData = CreateEncodedData(routine);

                return 0;
            }

            RLLRoutine rllRoutine = routine as RLLRoutine;
            if (rllRoutine != null)
            {
                rllRoutine.IsEncrypted = true;
                rllRoutine.EncodedData = CreateEncodedData(routine);

                return 0;
            }

            //TODO(gjc): add other routine here

            return -1;

        }

        private int Protect(IAoiDefinition aoiDefinition, string key)
        {
            AoiDefinition definition = aoiDefinition as AoiDefinition;
            if (definition == null)
                return -1;

            if (definition.IsEncrypted && !_protectedAoiDefinitions.ContainsKey(definition))
            {
                return -1;
            }

            if (_protectedAoiDefinitions.ContainsKey(definition))
                _protectedAoiDefinitions[definition] = key;
            else
                _protectedAoiDefinitions.Add(definition, key);


            definition.IsEncrypted = true;

            definition.EncodedData = CreateEncodedData(definition);

            return 0;

        }

        private void DecodeRoutines()
        {
            // EncodedData-> Text
            foreach (var program in Controller.Programs)
            {
                foreach (var routine in program.Routines)
                {
                    if (routine.IsEncrypted)
                    {
                        string sourceKey = GetKeyBySource(routine);

                        STRoutine stRoutine = routine as STRoutine;
                        if (stRoutine != null)
                        {
                            int config = stRoutine.EncodedData.EncryptionConfig;
                            var protection = ProtectionFactory.Create(config);

                            string plainText;
                            protection.Decrypt(stRoutine.EncodedData.Text, sourceKey, out plainText);

                            if (!string.IsNullOrEmpty(plainText))
                            {
                                try
                                {
                                    stRoutine.CodeText = JArray.Parse(plainText).ToObject<List<string>>();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                            }

                        }

                        RLLRoutine rllRoutine = routine as RLLRoutine;
                        if (rllRoutine != null)
                        {
                            int config = rllRoutine.EncodedData.EncryptionConfig;
                            var protection = ProtectionFactory.Create(config);

                            string plainText;
                            protection.Decrypt(rllRoutine.EncodedData.Text, sourceKey, out plainText);

                            if (!string.IsNullOrEmpty(plainText))
                            {
                                try
                                {
                                    var codeText = JArray.Parse(plainText).ToObject<List<string>>();
                                    rllRoutine.UpdateRungs(codeText, rllRoutine.RungComments);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                            }
                        }

                        //TODO(gjc): add other routine here
                    }
                }
            }
        }

        private void DecodeAoiDefinitions()
        {
            foreach (var aoiDefinition in Controller.AOIDefinitionCollection.OfType<AoiDefinition>())
            {
                if (aoiDefinition.IsEncrypted)
                {
                    string sourceKey = GetKeyBySource(aoiDefinition);
                    int config = aoiDefinition.EncodedData.EncryptionConfig;
                    var protection = ProtectionFactory.Create(config);

                    string plainText;
                    protection.Decrypt(aoiDefinition.EncodedData.Text, sourceKey, out plainText);

                    if (!string.IsNullOrEmpty(plainText))
                    {
                        try
                        {
                            var routines = JArray.Parse(plainText);
                            aoiDefinition.UpdateRoutines(routines);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }

                    }
                }
            }
        }
    }
}
