using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    public class ParameterConnection : IParameterConnection
    {
        private string _sourcePath;
        private bool _sourceIsCorrect;
        private bool _destIsCorrect;
        private string _destinationPath;

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                var astName = ObtainValue.GetLoadTag(_sourcePath, null, null);
                _sourceIsCorrect = astName != null;
            }
        }

        public bool IsCorrect => _sourceIsCorrect && _destIsCorrect;

        public bool SourceIsCorrect => _sourceIsCorrect;

        public bool DestIsCorrect => _destIsCorrect;

        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                _destinationPath = value;
                var astName = ObtainValue.GetLoadTag(_destinationPath, null, null);
                _destIsCorrect = astName != null;
            }
        }

        public JObject ConvertToJObject()
        {
            JObject connection = new JObject()
            {
                {"EndPoint1", SourcePath},
                {"EndPoint2", DestinationPath}
            };

            return connection;
        }

        public void ApplyChanged(string specificA, Usage usageA, string specificB, Usage usageB)
        {
            if (usageA == Usage.Input)
            {
                SourcePath = specificB;
                DestinationPath = specificA;
            }
            else if (usageA == Usage.Output)
            {
                SourcePath = specificA;
                DestinationPath = specificB;
            }
            else if (usageA == Usage.InOut)
            {
                SourcePath = specificA;
                DestinationPath = specificB;
            }
            else if (usageA == Usage.SharedData)
            {
                if (usageB == Usage.Input)
                {
                    SourcePath = specificA;
                    DestinationPath = specificB;
                }
                else
                {
                    SourcePath = specificB;
                    DestinationPath = specificA;
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }

    public class ParameterConnectionCollection : IParameterConnectionCollection
    {
        private readonly List<IParameterConnection> _parameterConnections;

        public ParameterConnectionCollection(IController controller)
        {
            ParentController = controller;
            Uid = Guid.NewGuid().GetHashCode();

            _parameterConnections = new List<IParameterConnection>();
        }

        public IEnumerator<IParameterConnection> GetEnumerator()
        {
            return _parameterConnections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
        }

        public IController ParentController { get; }
        public int Uid { get; }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Count => _parameterConnections.Count;

        public IParameterConnection this[int uid]
        {
            get { throw new NotImplementedException(); }
        }

        public IParameterConnection this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public IParameterConnection TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public IParameterConnection TryGetChildByName(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ComponentCoreInfo> GetComponentCoreInfoList()
        {
            throw new NotImplementedException();
        }

        public ComponentCoreInfo GetComponentCoreInfo(int uid)
        {
            throw new NotImplementedException();
        }

        public void Add(IParameterConnection parameterConnection)
        {
            _parameterConnections.Add(parameterConnection);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, parameterConnection));
        }

        public void Remove(IParameterConnection parameterConnection)
        {
            _parameterConnections.Remove(parameterConnection);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, parameterConnection));
        }

        internal void Clean()
        {
            _parameterConnections.Clear();
        }

        public IParameterConnection FindConnection(string operand)
        {
            return _parameterConnections.Find(p =>
                p.SourcePath.Equals(operand, StringComparison.OrdinalIgnoreCase) ||
                p.DestinationPath.Equals(operand, StringComparison.OrdinalIgnoreCase));
        }

        public IParameterConnection FindConnection(string source, string dest)
        {
            return _parameterConnections.Find(p =>
                p.SourcePath.Equals(source, StringComparison.OrdinalIgnoreCase) &&
                p.DestinationPath.Equals(dest, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveConnection(ITag tag)
        {
            var delList = GetTagParameterConnections(tag).ToList();
            foreach (var parameterConnection in delList)
            {
                Remove(parameterConnection);
            }
        }

        public IEnumerable<IParameterConnection> GetTagParameterConnections(ITag tag)
        {
            var special = tag.Name;
            if (string.IsNullOrEmpty(special)) return null;
            if (tag.ParentCollection.ParentProgram != null)
            {
                special = $"\\{tag.ParentCollection.ParentProgram.Name}.{tag.Name}";
            }

            return _parameterConnections.Where(p =>
                (p.SourcePath.StartsWith(special, StringComparison.OrdinalIgnoreCase) &&
                 ((ParameterConnection) p).SourceIsCorrect) ||
                (p.DestinationPath.StartsWith(special, StringComparison.OrdinalIgnoreCase) &&
                 ((ParameterConnection) p).DestIsCorrect));
        }

        public IParameterConnectionVerifyExceptionArg VerifyInOutTag(ITag tag)
        {
            var list = GetTagParameterConnections(tag).ToList();
            if (!list.Any())
                return new ParameterConnectionVerifyExceptionArg(null,
                    $"Error:Parameter '\\{tag.ParentCollection.ParentProgram.Name}.{tag.Name}':InOut parameter must have exactly one connection.");
            return null;
        }
        
        public IParameterConnectionVerifyExceptionArg VerifyConnection(IParameterConnection parameterConnection)
        {
            var conn = (ParameterConnection) parameterConnection;
            if (!conn.SourceIsCorrect)
            {
                return new ParameterConnectionVerifyExceptionArg(parameterConnection,
                    $"Error: Parameter '{parameterConnection.SourcePath}':Tag doesn't reference valid object or target.");
            }

            if (!conn.DestIsCorrect)
            {
                return new ParameterConnectionVerifyExceptionArg(parameterConnection,
                    $"Error: Parameter '{parameterConnection.DestinationPath}':Tag doesn't reference valid object or target.");
            }

            var sourceTag = ObtainValue.NameToTag(parameterConnection.SourcePath, null);
            var destTag = ObtainValue.NameToTag(parameterConnection.DestinationPath, null);

            return VerifyConnection(parameterConnection.SourcePath, sourceTag.Item1.Usage,
                sourceTag.Item1.ExternalAccess, sourceTag.Item1.DataTypeInfo.ToString(),
                parameterConnection.DestinationPath, destTag.Item1.Usage,
                destTag.Item1.ExternalAccess, destTag.Item1.DataTypeInfo.ToString());
        }

        public IParameterConnectionVerifyExceptionArg VerifyConnection(string specificA, Usage usageA,
            ExternalAccess externalAccessA, string dataTypeA, string specificB, Usage usageB,
            ExternalAccess externalAccessB, string dataTypeB)
        {
            if (!dataTypeA.Equals(dataTypeB, StringComparison.OrdinalIgnoreCase))
            {
                return new ParameterConnectionVerifyExceptionArg(null,
                    "Parameter can only be connected to tag or parameter with the same data type.");
            }

            Debug.Assert(!string.IsNullOrEmpty(specificA) && !string.IsNullOrEmpty(specificB));
            if (specificA.Equals(specificB, StringComparison.OrdinalIgnoreCase))
            {
                return new ParameterConnectionVerifyExceptionArg(null, "Parameter cannot connect to itself.");
            }

            switch (usageA)
            {
                case Usage.Input:
                    if (specificA.StartsWith("\\"))
                    {
                        specificA = string.Join(".", specificA.Split('.'), 0, 2);
                    }
                    else
                    {
                        specificA = specificA.Split('.')[0];
                    }

                    if (usageB == Usage.Input || usageB == Usage.Local)
                    {
                        if (specificB.StartsWith("\\"))
                            return new ParameterConnectionVerifyExceptionArg(null,
                                "Input parameters cannot be connected to Local tags or Input parameters");
                    }
                    else if (usageB == Usage.InOut)
                    {
                        return VerifyInoutAndInput(specificB, externalAccessB, specificA, externalAccessA);
                    }
                    else if (usageB == Usage.Output)
                    {
                        var conn = _parameterConnections.FirstOrDefault(p => p.DestinationPath.StartsWith(specificA));
                        if (conn == null)
                        {
                            return null;
                        }
                        else
                        {
                            return new ParameterConnectionVerifyExceptionArg(conn,
                                "The parameter connection already exists.");
                        }
                    }
                    else if (usageB == Usage.SharedData)
                    {
                        var conn = _parameterConnections.FirstOrDefault(p => p.DestinationPath.StartsWith(specificA));
                        if (conn == null)
                        {
                            return null;
                        }
                        else
                        {
                            return new ParameterConnectionVerifyExceptionArg(conn,
                                "The parameter connection already exists.");
                        }
                    }

                    return null;
                case Usage.InOut:
                    if (usageB == Usage.Input)
                    {
                        var conn = _parameterConnections.FirstOrDefault(p => p.DestinationPath.StartsWith(specificB));
                        if (conn == null)
                        {
                            return null;
                        }
                        else
                        {
                            return new ParameterConnectionVerifyExceptionArg(conn,
                                "The parameter connection already exists.");
                        }
                    }
                    else if (usageB == Usage.InOut || usageB == Usage.Local)
                    {
                        if (specificB.StartsWith("\\"))
                            return new ParameterConnectionVerifyExceptionArg(null,
                                "InOut parameters cannot be connected to Local tags or InOut parameters");
                    }
                    else if (usageB == Usage.Output)
                    {
                        return VerifyInoutAndOutput(specificA, externalAccessA, specificB, externalAccessB);
                    }
                    else if (usageB == Usage.SharedData)
                    {
                        return VerifyInoutAndPublic(specificA, externalAccessA, specificB, externalAccessB);
                    }

                    return null;
                case Usage.Output:
                    if (usageB == Usage.Input)
                    {
                        var conn = _parameterConnections.FirstOrDefault(p => p.DestinationPath.StartsWith(specificB));
                        if (conn == null)
                        {
                            return null;
                        }
                        else
                        {
                            return new ParameterConnectionVerifyExceptionArg(conn,
                                "The parameter connection already exists.");
                        }
                    }
                    else if (usageB == Usage.InOut)
                    {
                        return VerifyInoutAndOutput(specificB, externalAccessB, specificA, externalAccessA);
                    }
                    else if (usageB == Usage.Output)
                    {
                        if (specificB.StartsWith("\\"))
                            return new ParameterConnectionVerifyExceptionArg(null,
                                "InOut parameters cannot be connected to Local tags or Output parameters");
                    }
                    else if (usageB == Usage.SharedData)
                    {
                        return VerifyPublicAndAny(specificB, externalAccessB, specificA, usageA, externalAccessA);
                    }

                    return null;
                case Usage.SharedData:
                    return VerifyPublicAndAny(specificA, externalAccessA, specificB, usageB, externalAccessB);
                default:
                    return null;
            }
        }

        private ParameterConnectionVerifyExceptionArg VerifyInoutAndInput(string inout, ExternalAccess inoutAccess,
            string input, ExternalAccess inputAccess)
        {
            if (ObtainValue.IsBaseTag(inout))
            {
                //input access 权限大于inout
                if ((byte) inputAccess <= (byte) inoutAccess)
                {
                    if (_parameterConnections.Any(p =>
                        p.DestinationPath.StartsWith(input) && !p.SourcePath.Equals(inout)))
                    {
                        return new ParameterConnectionVerifyExceptionArg(null,
                            "InOut parameter must have exactly one connection.");
                    }
                }
                else
                {
                    return new ParameterConnectionVerifyExceptionArg(null,
                        "External Access of InOut parameter is not compatible with connection(s)");
                }
            }
            else
            {
                return new ParameterConnectionVerifyExceptionArg(null,
                    "Members of InOut parameter cannot be connected.");
            }

            return null;
        }

        private ParameterConnectionVerifyExceptionArg VerifyInoutAndOutput(string inout, ExternalAccess inoutAccess,
            string output, ExternalAccess outputAccess)
        {
            if (ObtainValue.IsBaseTag(inout))
            {
                if (!ObtainValue.IsConstant(inout))
                {
                    return new ParameterConnectionVerifyExceptionArg(null,
                        "InOut parameter must be a constant in order to connect to an output parameter.");
                }

                //output access 权限大于inout
                if ((byte) outputAccess > (byte) inoutAccess)
                    return new ParameterConnectionVerifyExceptionArg(null,
                        "External Access of InOut parameter is not compatible with connection(s)");
            }
            else
            {
                return new ParameterConnectionVerifyExceptionArg(null,
                    "Members of InOut parameter cannot be connected.");
            }

            return null;
        }

        private ParameterConnectionVerifyExceptionArg VerifyInoutAndPublic(string inout, ExternalAccess inoutAccess,
            string share, ExternalAccess shareAccess)
        {
            if (ObtainValue.IsBaseTag(inout))
            {
                //public access 权限大于inout
                if ((byte) shareAccess > (byte) inoutAccess)
                    return new ParameterConnectionVerifyExceptionArg(null,
                        "External Access of InOut parameter is not compatible with connection(s)");
            }
            else
            {
                return new ParameterConnectionVerifyExceptionArg(null,
                    "Members of InOut parameter cannot be connected.");
            }

            return null;
        }

        private ParameterConnectionVerifyExceptionArg VerifyPublicAndAny(string share,
            ExternalAccess shareExternalAccess, string any, Usage anyUsage,
            ExternalAccess anyExternalAccess)
        {
            if (anyUsage == Usage.SharedData || anyUsage == Usage.Local)
            {
                return new ParameterConnectionVerifyExceptionArg(null, "Tag or parameter usage types are incompatible");
            }

            //if (ObtainValue.NameToTag(any, null).Item1?.ParentCollection.ParentProgram == null)
            //    return new ParameterConnectionVerifyExceptionArg(null, "Tag or parameter usage types are incompatible");
            if (anyUsage == Usage.Input)
            {
                var conn = _parameterConnections.FirstOrDefault(p => p.DestinationPath.StartsWith(any));
                if (conn == null)
                {
                    return null;
                }
                else
                {
                    return new ParameterConnectionVerifyExceptionArg(conn, "The parameter connection already exists.");
                }
            }

            if (anyUsage == Usage.Output)
            {
                //var shareTag = ObtainValue.NameToTag(share,null)?.Item1;
                //Debug.Assert(shareTag!=null);
                //var shareTagName = shareTag.ParentCollection.ParentProgram == null
                //    ? shareTag.Name
                //    : $"\\{shareTag.ParentCollection.ParentProgram.Name}.{shareTag.Name}";
                var connColl = _parameterConnections.Where(p => p.DestinationPath.StartsWith(share));
                if (connColl.Any())
                {
                    foreach (var parameterConnection in connColl)
                    {
                        var sourceName = parameterConnection.SourcePath;
                        Debug.Assert(sourceName.StartsWith("\\"),sourceName);
                        var program = sourceName.Substring(0, sourceName.IndexOf("."));
                        var otherOutput = connColl.FirstOrDefault(c =>
                            c.SourcePath.StartsWith(program) && c != parameterConnection);
                        if (otherOutput != null)
                        {
                            return new ParameterConnectionVerifyExceptionArg(parameterConnection, "Multiple output parameters in the same program cannot connect to the same tag or parameter.");
                        }
                    }
                }
            }

            if (anyUsage == Usage.InOut)
            {
                return VerifyInoutAndPublic(any, anyExternalAccess, share, shareExternalAccess);
            }
            return null;
        }

        public IParameterConnection CreateConnection(ITag tag, string specificB)
        {
            var connection = new ParameterConnection();
            var specificA = tag.ParentCollection.ParentProgram == null
                ? tag.Name
                : $"\\{tag.ParentCollection.ParentProgram.Name}.{tag.Name}";
            if (tag.Usage == Usage.Input)
            {
                connection.SourcePath = specificB;
                connection.DestinationPath = specificA;
            }
            else if (tag.Usage == Usage.Output)
            {
                connection.SourcePath = specificA;
                connection.DestinationPath = specificB;
            }
            else if (tag.Usage == Usage.InOut)
            {
                connection.SourcePath = specificA;
                connection.DestinationPath = specificB;
            }
            else if (tag.Usage == Usage.SharedData)
            {
                var tagB = ObtainValue.NameToTag(specificB, null);
                if (tagB?.Item1 == null)
                {
                    connection.SourcePath = specificB;
                    connection.DestinationPath = specificA;
                }
                else
                {
                    if (tagB.Item1.Usage == Usage.Input)
                    {
                        connection.SourcePath = specificA;
                        connection.DestinationPath = specificB;
                    }
                    else
                    {
                        connection.SourcePath = specificB;
                        connection.DestinationPath = specificA;
                    }
                }
            }
            else
            {
                Debug.Assert(false);
            }

            Add(connection);
            return connection;
        }


        public IParameterConnection CreateConnection(string specificA, Usage usageA, string specificB, Usage usageB)
        {
            var connection = new ParameterConnection();

            if (usageA == Usage.Input)
            {
                connection.SourcePath = specificB;
                connection.DestinationPath = specificA;
            }
            else if (usageA == Usage.Output)
            {
                connection.SourcePath = specificA;
                connection.DestinationPath = specificB;
            }
            else if (usageA == Usage.InOut)
            {
                connection.SourcePath = specificA;
                connection.DestinationPath = specificB;
            }
            else if (usageA == Usage.SharedData)
            {
                if (usageB == Usage.Input)
                {
                    connection.SourcePath = specificA;
                    connection.DestinationPath = specificB;
                }
                else
                {
                    connection.SourcePath = specificB;
                    connection.DestinationPath = specificA;
                }
            }
            else
            {
                Debug.Assert(false);
            }
            
            Add(connection);
            return connection;
        }

        private bool CheckDataType(string a, string b)
        {
            var astNameA = ObtainValue.GetLoadTag(a, null, null);
            var astNameB = ObtainValue.GetLoadTag(b, null, null);
            if (astNameA != null && astNameB != null)
            {
                if (astNameA.Expr.type.Equal(astNameB.Expr.type, true)) return true;
            }

            return false;
        }
    }

    public class ParameterConnectionVerifyExceptionArg : IParameterConnectionVerifyExceptionArg
    {
        public ParameterConnectionVerifyExceptionArg(IParameterConnection connection, string mes)
        {
            Connection = connection;
            Message = mes;
        }

        public IParameterConnection Connection { get; }

        public string Message { get; }
    }
}
