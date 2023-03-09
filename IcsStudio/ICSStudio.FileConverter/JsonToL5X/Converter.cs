using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ProgramType = ICSStudio.FileConverter.JsonToL5X.Model.ProgramType;
using TagType = ICSStudio.FileConverter.JsonToL5X.Model.TagType;
using ProgramCollection = ICSStudio.FileConverter.JsonToL5X.Model.ProgramCollection;

namespace ICSStudio.FileConverter.JsonToL5X
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public static partial class Converter
    {
        public static void JsonToL5X(string fileName, Controller con)
        {
            string path = fileName;
            var controller = con;

            RSLogix5000ContentType rsLogix5000ContentType = new RSLogix5000ContentType();
            ControllerType controllerType = new ControllerType();
            TagCollection tagCollection = new TagCollection();
            Collection<TagType> tagTypeCollection = new Collection<TagType>();
            object dataTypeMember = new object();
            object data = new object();

            //导出Controller下Tag的xml
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
                            Collection<DataValue> aoiParameterTypes =
                                SetAoiDataValue(controller, tag.DataTypeInfo.DataType.Name);

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
                                { Name = member.Name, DataType = member.DataTypeInfo.DataType.Name });
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
                }

                //不是数组类型 看是不是原子类型或者组合类型(AOI类型、UDT自定义类型、string类型)
                else
                {
                    //AOI类型
                    if (tagAOIDataType != null)
                    {

                        //创建aoiTag下的第一级Data
                        Data aoiData = new Data();
                        aoiData.Format = "Decorated";

                        //创建aoiTag下的第二级Structure
                        DataStructure dataStructure = new DataStructure();
                        dataStructure.DataType = tag.DataTypeInfo.ToString();

                        Collection<DataValue> dataStructureMember = new Collection<DataValue>();

                        foreach (var aoi in controller.AOIDefinitionCollection)
                        {
                            if (aoi.Name == tag.DataTypeInfo.ToString())
                            {
                                foreach (var aoitag in aoi.Tags)
                                {
                                    if (aoitag.Usage == Usage.Output || aoitag.Usage == Usage.Input)
                                    {
                                        var structureMember = new DataValue();
                                        structureMember.Name = aoitag.Name;
                                        structureMember.DataType = aoitag.DataTypeInfo.DataType.Name;
                                        dataStructureMember.Add(structureMember);
                                    }
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

                    //UDT自定义类型
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
                                //StructureMember下的DataValueMember
                                foreach (var member in type.TypeMembers)
                                {
                                    var structureMember = new DataStructure();
                                    structureMember.Name = member.Name;
                                    structureMember.DataType = member.DataTypeInfo.ToString();
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
                            { Name = member.Name, DataType = member.DataTypeInfo.DataType.Name });

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

                    //其他类型
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

            //TODO(TLM):Module模块


            //导出Program Tag数据
            ProgramCollection programCollection = new ProgramCollection();

            Collection<ProgramType> programTagTypestagType = new Collection<ProgramType>();
            if (controller.Programs.Count > 0)
            {
                foreach (var program in controller.Programs)
                {
                    var programType = new ProgramType();
                    programType.Name = program.Name;

                    programType.TestEdits = program.TestEditsMode == TestEditsModeType.Test ? BoolEnum.@true : BoolEnum.@false;
                    
                    TagCollection programTagCollection = new TagCollection();
                    Collection<TagType> programTypes = new Collection<TagType>();

                    foreach (var tag in program.Tags)
                    {
                        var tagtest = new TagType();
                        tagtest.Name = tag.Name;
                        tagtest.DataType = tag.DataTypeInfo.ToString();
                        programTypes.Add(tagtest);
                    }

                    TagType[] programTagTypes = new TagType[programTypes.Count];

                    for (int i = 0; i < programTypes.Count; i++)
                    {
                        programTagTypes[i] = programTypes[i];
                    }

                    programTagCollection.Tag = programTagTypes;
                    programType.Tags = programTagCollection;


                    programTagTypestagType.Add(programType);
                }

                ProgramType[] programTypesType = new ProgramType[programTagTypestagType.Count];

                for (int i = 0; i < programTagTypestagType.Count; i++)
                {
                    programTypesType[i] = programTagTypestagType[i];
                }

                programCollection.Program = programTypesType;
                controllerType.Programs = programCollection;
            }


            //导出AOI中的数据
            AOIDefinitionCollection aoiDefinitionCollection = new AOIDefinitionCollection();

            Collection<AOIDefinitionType> aoiDefinitions = new Collection<AOIDefinitionType>();

            if (controller.AOIDefinitionCollection.Any())
            {
                foreach (var aoi in controller.AOIDefinitionCollection)
                {
                    var aoiDefinitionType = new AOIDefinitionType();
                    aoiDefinitionType.Name = aoi.Name;

                    Collection<AOIParameterType> aoiParameterTypes = new Collection<AOIParameterType>();
                    foreach (var tag in aoi.Tags)
                    {
                        var tagtest = new AOIParameterType();
                        tagtest.Name = tag.Name;
                        aoiParameterTypes.Add(tagtest);
                    }

                    AOIParameterType[] aoiTagsType = new AOIParameterType[aoiParameterTypes.Count];

                    for (int i = 0; i < aoiParameterTypes.Count; i++)
                    {
                        aoiTagsType[i] = aoiParameterTypes[i];
                    }

                    aoiDefinitionType.Parameters = aoiTagsType;


                    aoiDefinitions.Add(aoiDefinitionType);
                }

                object[] aoiObjects = new object[aoiDefinitions.Count];

                for (int i = 0; i < aoiDefinitions.Count; i++)
                {
                    aoiObjects[i] = aoiDefinitions[i];
                }

                aoiDefinitionCollection.Items = aoiObjects;
                controllerType.AddOnInstructionDefinitions = aoiDefinitionCollection;
            }

            rsLogix5000ContentType.Controller = controllerType;
            JsonToL5XSerializer xmlserializer = new JsonToL5XSerializer();
            xmlserializer.Serialize(path, rsLogix5000ContentType);
        }

        private static Collection<DataValue> SetAoiDataValue(Controller controller, string name)
        {
            Collection<DataValue> aoiParameterTypes = new Collection<DataValue>();
            foreach (var aoi in controller.AOIDefinitionCollection)
            {
                if (aoi.Name == name)
                {
                    foreach (var aoitag in aoi.Tags)
                    {
                        if (aoitag.Usage == Usage.Output || aoitag.Usage == Usage.Input)
                        {
                            aoiParameterTypes.Add(new DataValue()
                            {
                                Name = aoitag.Name,
                                DataType = aoitag.DataTypeInfo.DataType.Name
                            });
                        }
                    }
                }
            }

            return aoiParameterTypes;
        }
    }
}