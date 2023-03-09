using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.TagToL5XTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Xmlserializer xmlserializer = new Xmlserializer();
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string path = directory + @"TestFile\TestL5X.L5X";
            string savepath = directory + @"TestFile\Test.json";

            var controller = Controller.Open(savepath);

            RsLogix5000ContentType rsLogix5000ContentType = new RsLogix5000ContentType();
            ControllerType controllerType = new ControllerType();


            TagCollection tagCollection = new TagCollection();
            Collection<TagType> tagTypeCollection = new Collection<TagType>();
            object dataTypeMember = new object();
            object data = new object();

            //导出tag的xml
            foreach (var tag in controller.Tags)
            {
                var tagType = new TagType();
                tagType.Name = tag.Name;
                tagType.DataType = tag.DataTypeInfo.ToString();
                //data
                Data tagData = new Data();
                tagData.Format = "Decorated";

                var tagAOIDataType = tag.DataTypeInfo.DataType as AOIDataType;
                var tagDataType = tag.DataTypeInfo.DataType as UserDefinedDataType;

                //首先判断是不是数组类型的
                if (tag.DataTypeInfo.Dim1 != 0)
                {
                    //array
                    DataArray array = new DataArray();
                    array.DataType = tag.DataTypeInfo.DataType.Name;
                    array.Dimensions = tag.DataTypeInfo.Dim1.ToString();
                    //elements
                    var elements = new DataArrayElement[tag.DataTypeInfo.Dim1];

                    for (int i = 0; i < elements.Length; i++)
                    {
                        //element
                        elements[i] = new DataArrayElement();
                        elements[i].Index = "[" + i + "]";

                        var dataStructures = new DataStructure[1];
                        dataStructures[0] = new DataStructure();
                        dataStructures[0].DataType = tag.DataTypeInfo.DataType.Name;

                        //AOI
                        if (tagAOIDataType != null)
                        {
                            Collection<DataValue> aoiParameterTypes = new Collection<DataValue>();
                            foreach (var aoiDefinition in controller.AOIDefinitionCollection)
                            {
                                if (aoiDefinition.Name == tag.DataTypeInfo.DataType.Name)
                                {
                                    foreach (var aoitag in aoiDefinition.Tags)
                                    {
                                        if (aoitag.Usage == Usage.Output || aoitag.Usage == Usage.Input)
                                        {
                                            aoiParameterTypes.Add(new DataValue()
                                                {Name = aoitag.Name, DataType = aoitag.DataTypeInfo.DataType.Name});
                                        }
                                    }
                                }
                            }

                            object[] dataValueMemberObjects = new object[aoiParameterTypes.Count];

                            for (int j = 0; j < aoiParameterTypes.Count; j++)
                            {
                                dataValueMemberObjects[j] = aoiParameterTypes[j];
                            }

                            dataStructures[0].Items = dataValueMemberObjects;
                        }

                        //自定义
                        if (tagDataType != null)
                        {
                            var dataTypeMembers = (tag.DataTypeInfo.DataType as UserDefinedDataType).TypeMembers;
                            Collection<DataValue> dataValueMember = new Collection<DataValue>();

                            foreach (var member in dataTypeMembers)
                            {
                                dataValueMember.Add(new DataValue()
                                    {Name = member.Name, DataType = member.DataTypeInfo.DataType.Name});

                            }


                            object[] dataValueMembers = new object[dataValueMember.Count];

                            for (int j = 0; j < dataValueMember.Count; j++)
                            {
                                dataValueMembers[j] = dataValueMember[j];
                            }

                            dataStructures[0].Items = dataValueMembers;
                        }

                        elements[i].Structure = dataStructures;
                    }

                    array.Element = elements;

                    tagData.Items = new object[1];
                    tagData.Items[0] = array;

                    tagType.Items = new object[1];
                    tagType.Items[0] = tagData;

                    //ArrayTagType.Add(tagtest);
                }

                //不是数组类型 看是不是原子类型或者组合类型(AOI类型、自定义类型、string类型)
                else
                {
                    if (tagAOIDataType != null)
                    {

                        //创建aoiTag下的第一级Data
                        Data aoiData = new Data();
                        aoiData.Format = "Decorated";

                        //创建aoiTag下的第二级Structure
                        DataStructure dataStructure = new DataStructure();
                        dataStructure.DataType = tag.DataTypeInfo.ToString();


                        Collection<DataStructure> dataStructureMember = new Collection<DataStructure>();
                        foreach (var dataType in controller.DataTypes)
                        {
                            var type = dataType as UserDefinedDataType;
                            if (type != null && type.Name == tag.DataTypeInfo.ToString())
                            {
                                foreach (var member in type.TypeMembers)
                                {
                                    var structureMember = new DataStructure();
                                    structureMember.Name = member.Name;
                                    structureMember.DataType = member.DataTypeInfo.ToString();

                                    //StructureMember下的DataValueMember
                                    Collection<DataValue> aoiParameterTypes = new Collection<DataValue>();
                                    foreach (var aoi in controller.AOIDefinitionCollection)
                                    {
                                        if (aoi.Name == structureMember.DataType)
                                        {
                                            foreach (var aoitag in aoi.Tags)
                                            {
                                                if (aoitag.Usage == Usage.Output || aoitag.Usage == Usage.Input)
                                                {
                                                    aoiParameterTypes.Add(new DataValue()
                                                    {
                                                        Name = aoitag.Name, DataType = aoitag.DataTypeInfo.DataType.Name
                                                    });
                                                }
                                            }
                                        }
                                    }

                                    object[] dataValueMemberObjects = new object[aoiParameterTypes.Count];

                                    for (int j = 0; j < aoiParameterTypes.Count; j++)
                                    {
                                        if (aoiParameterTypes[j].DataType == "BOOL")
                                        {
                                            aoiParameterTypes[j].Value =
                                                aoiParameterTypes[j].Name == "EnableIn" ? "1" : "0";
                                        }

                                        dataValueMemberObjects[j] = aoiParameterTypes[j];
                                    }

                                    dataStructureMember.Add(structureMember);
                                }
                            }
                        }

                        object[] memberObjects = new object[dataStructureMember.Count];

                        for (int j = 0; j < dataStructureMember.Count; j++)
                        {
                            memberObjects[j] = dataStructureMember[j];
                        }

                        dataStructure.Items = memberObjects;

                        aoiData.Items = new object[1];
                        aoiData.Items[0] = dataStructure;

                        tagType.Items = new object[1];
                        tagType.Items[0] = aoiData;

                    }

                    if (tagDataType != null)
                    {
                        dataTypeMember = (tag.DataTypeInfo.DataType as UserDefinedDataType).TypeMembers;
                        //导出UserDefinedTagType的xml

                        //创建userDefinedTagType下的第一级Data
                        Data userDefinedData = new Data();
                        userDefinedData.Format = "Decorated";

                        //创建userDefinedTagType下的第二级Structure
                        DataStructure dataStructure = new DataStructure();
                        dataStructure.DataType = tag.DataTypeInfo.ToString();


                        //创建Structure下的DataValueMember
                        Collection<DataStructure> dataStructureMember = new Collection<DataStructure>();
                        foreach (var dataType in controller.DataTypes)
                        {
                            var type = dataType as UserDefinedDataType;
                            if (type != null && type.Name == tag.DataTypeInfo.ToString())
                            {
                                foreach (var member in type.TypeMembers)
                                {
                                    var structureMember = new DataStructure();
                                    structureMember.Name = member.Name;
                                    structureMember.DataType = member.DataTypeInfo.ToString();

                                    //StructureMember下的DataValueMember
                                    Collection<DataValue> aoiParameterTypes = new Collection<DataValue>();
                                    foreach (var aoi in controller.AOIDefinitionCollection)
                                    {
                                        if (aoi.Name == structureMember.DataType)
                                        {
                                            foreach (var aoitag in aoi.Tags)
                                            {
                                                if (aoitag.Usage == Usage.Output || aoitag.Usage == Usage.Input)
                                                {
                                                    aoiParameterTypes.Add(new DataValue()
                                                    {
                                                        Name = aoitag.Name, DataType = aoitag.DataTypeInfo.DataType.Name
                                                    });
                                                }
                                            }
                                        }
                                    }

                                    object[] dataValueMemberObjects = new object[aoiParameterTypes.Count];

                                    for (int j = 0; j < aoiParameterTypes.Count; j++)
                                    {
                                        if (aoiParameterTypes[j].DataType == "BOOL")
                                        {
                                            aoiParameterTypes[j].Value =
                                                aoiParameterTypes[j].Name == "EnableIn" ? "1" : "0";
                                        }

                                        dataValueMemberObjects[j] = aoiParameterTypes[j];
                                    }

                                    dataStructureMember.Add(structureMember);
                                }
                            }
                        }

                        object[] memberObjects = new object[dataStructureMember.Count];

                        for (int j = 0; j < dataStructureMember.Count; j++)
                        {
                            memberObjects[j] = dataStructureMember[j];
                        }

                        dataStructure.Items = memberObjects;

                        //当是其他普通类型的StructureMember时候
                        Collection<DataValue> dataValueMember = new Collection<DataValue>();

                        foreach (var member in dataTypeMember as ITypeMemberComponentCollection)
                        {
                            dataValueMember.Add(new DataValue()
                                {Name = member.Name, DataType = member.DataTypeInfo.DataType.Name});

                        }

                        object[] dataValueMembers = new object[dataValueMember.Count];

                        for (int j = 0; j < dataValueMember.Count; j++)
                        {
                            if (dataValueMember[j].DataType == "BOOL")
                            {
                                dataValueMember[j].Value = dataValueMember[j].Name == "EnableIn" ? "1" : "0";
                            }

                            dataValueMembers[j] = dataValueMember[j];
                        }

                        dataStructure.Items = dataValueMembers;

                        object[] aoiDefinitionTypes = new object[dataStructure.Items.Length];

                        for (int j = 0; j < dataStructure.Items.Length; j++)
                        {
                            aoiDefinitionTypes[j] = dataStructure.Items[j];
                        }

                        dataStructure.Items = aoiDefinitionTypes;

                        userDefinedData.Items = new object[1];
                        userDefinedData.Items[0] = dataStructure;

                        tagType.Items = new object[1];
                        tagType.Items[0] = userDefinedData;
                    }
                    else
                    {
                        if (tagType.DataType == "BOOL")
                        {
                            //创建bool类型的Tag下第一级Data
                            Data aoiData = new Data();
                            aoiData.Format = "Decorated";
                            //创建第二级data数据
                            DataValue dataStructure = new DataValue();
                            dataStructure.DataType = tagType.DataType;
                            dataStructure.Radix = tagType.Radix;
                            if (dataStructure.DataType == "BOOL")
                            {
                                dataStructure.Value = dataStructure.Name == "EnableIn" ? "1" : "0";
                            }

                            aoiData.Items = new object[1];
                            aoiData.Items[0] = dataStructure;

                            tagType.Items = new object[1];
                            tagType.Items[0] = aoiData;
                        }
                    }
                }

                tagTypeCollection.Add(tagType);
            }

            TagType[] allTagsType = new TagType[tagTypeCollection.Count];

            for (int i = 0; i < tagTypeCollection.Count; i++)
            {
                allTagsType[i] = tagTypeCollection[i];

            }

            tagCollection.Tag = allTagsType;
            controllerType.Tags = tagCollection;



            //Modules模块
            //    var moduleCollection = new ModuleCollection();
            //    Collection<ModuleType> moduleTypes = new Collection<ModuleType>();

            //    foreach (var tag in controller.Tags)
            //    {
            //        if (tag.DataTypeInfo.DataType.IsIOType)
            //        {
            //            var moduleTag = new ModuleType();
            //            moduleTag.Name = tag.DataTypeInfo.DataType.Name;
            //            moduleTypes.Add(moduleTag);
            //        }
            //    }

            //    var moduleType = new ModuleType[moduleTypes.Count];
            //    for (int i = 0; i < moduleTypes.Count; i++)
            //    {
            //        moduleType[i] = new ModuleType();
            //        moduleType[i].Name = moduleTypes[i].Name;

            //        //Communications模块
            //        var communicationsType = new CommunicationsType();

            //        var communicationsItems = new object[2];
            //        var configTag = new ConfigTagType();
            //        var configData = new Data();
            //        configData.Format = "Decorated";
            //        configData.Items = new object[1];

            //        //Structure
            //        DataStructure dataStructure = new DataStructure();
            //        dataStructure.DataType = moduleTypes[i].Name;

            //        dataStructure.Items = new object[1];

            //        //Structure下的DataValueMember

            //        foreach (var tag in controller.Tags)
            //        {
            //            if (tag.DataTypeInfo.DataType.IsIOType && tag.DataTypeInfo.DataType.Name == moduleType[i].Name)
            //            {
            //                dataStructure.Items[0] = (tag.DataTypeInfo.DataType as ModuleDefinedDataType).TypeMembers;
            //                break;
            //            }
            //        }

            //        configData.Items[0] = dataStructure;
            //        configTag.Items = new object[1];
            //        configTag.Items[0] = configData;
            //        communicationsItems[0] = configTag;

            //        var connection = new ConnectionCollection();
            //        ConnectionType[] connectionTypes = new ConnectionType[1];
            //        connectionTypes[0] = new ConnectionType();
            //        //connectionTypes[0].Name = "Data";
            //        var inputTag = new InputTagType();
            //        inputTag.Items = new object[1];
            //        var inputTagData = new Data();
            //        inputTagData.Items = new object[1];
            //        var inputTagDataStructure = new DataStructure() { DataType = moduleTypes[i].Name };

            //        object inputTagDataStructureIteCollection = new object();
            //        foreach (var tag in controller.Tags)
            //        {
            //            if (tag.Name == "AB:Embedded_DiscreteIO:I:0")
            //            {
            //                inputTagDataStructureIteCollection = (tag.DataTypeInfo.DataType as ModuleDefinedDataType).TypeMembers;
            //            }
            //        }

            //        inputTagDataStructure.Items = new object[(inputTagDataStructureIteCollection as ITypeMemberComponentCollection).Count];
            //        for (int j = 0; j < inputTagDataStructure.Items.Length; j++)
            //        {
            //            inputTagDataStructure.Items[j] =
            //                (inputTagDataStructureIteCollection as ITypeMemberComponentCollection)[j];
            //        }





            //        inputTagData.Items[0] = inputTagDataStructure;


            //        inputTag.Items[0] = inputTagData;


            //        connectionTypes[0].InputTag = inputTag;

            //        connection.Connection = connectionTypes;
            //        communicationsItems[1] = connection;
            //        communicationsType.Items = communicationsItems;
            //        moduleType[i].Communications = communicationsType;
            //    }

            //    moduleCollection.Module = moduleType;
            //    controllerType.Modules = moduleCollection;

            rsLogix5000ContentType.Controller = controllerType;
            xmlserializer.Serialize(path, rsLogix5000ContentType);
        }

        public class Xmlserializer
        {
            public void Serialize(string path, object obj)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                string content = string.Empty;
                //serialize
                using (StringWriter writer = new StringWriter())
                {
                    content = ToXml(obj);
                }

                //save to file
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    streamWriter.Write(content);
                }
            }
        }

        public static string ToXml(
            this object serializableObject,
            bool includeXmlDeclaration = true,
            bool pretty = true)
        {
            string str = string.Empty;
            if (includeXmlDeclaration)
                str = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + (pretty ? Environment.NewLine : string.Empty);
            var serialize = serializableObject.ToXml(new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                Indent = pretty,
                IndentChars = "  ",
                NewLineChars = pretty ? Environment.NewLine : string.Empty
            });

            return str + serialize;
        }

        internal static string ToXml(
            this object serializableObject,
            XmlWriterSettings xmlWriterSettings)
        {
            if (serializableObject == null)
                throw new ArgumentNullException(nameof(serializableObject));
            if (xmlWriterSettings == null)
                throw new ArgumentNullException(nameof(xmlWriterSettings));
            StringBuilder output = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(serializableObject.GetType());
            using (XmlWriter xmlWriter = XmlWriter.Create(output, xmlWriterSettings))
            {
                // 强制指定命名空间，覆盖默认的命名空间
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                xmlSerializer.Serialize(xmlWriter, serializableObject, namespaces);
                xmlWriter.Close();
            }

            ;

            return output.ToString();
        }
    }
}
