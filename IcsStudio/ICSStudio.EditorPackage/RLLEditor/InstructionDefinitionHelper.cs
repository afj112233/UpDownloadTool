using System.Collections.Generic;
using ICSStudio.Interfaces.Tags;
using ICSStudio.Ladder.Graph;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.EditorPackage.RLLEditor
{
    public static class InstructionDefinitionHelper
    {
        public static InstructionDefinition GetRLLInstructionDefinition(this AoiDefinition aoiDefinition)
        {
            if (aoiDefinition == null)
                return null;

            InstructionDefinition definition = new InstructionDefinition();

            definition.Mnemonic = aoiDefinition.Name;
            definition.Description = aoiDefinition.Description;
            definition.Type = "Box";

            // Parameters
            var parameters = new List<InstructionParameter>();
            var bitlegs = new List<string>();

            // aoi type
            InstructionParameter parameter = new InstructionParameter
            {
                Label = aoiDefinition.Name,
                Type = "Read_none",
                DataType = aoiDefinition.Name,
                Visible = true,
                HasConfig = true,
                Formats = new List<FormatType> { FormatType.Tag }
            };
            parameters.Add(parameter);

            // parameter and bitLegs
            foreach (var tag in aoiDefinition.Tags)
            {
                if (tag.Name == "EnableIn" || tag.Name == "EnableOut")
                    continue;

                if (tag.Usage == Usage.Local)
                    continue;

                var dataType = tag.DataTypeInfo.DataType;

                if (tag.Usage == Usage.Output
                    && tag.IsVisible
                    && !tag.IsRequired
                    && dataType.IsBool)
                {
                    bitlegs.Add(tag.Name);
                    continue;
                }

                if (tag.Usage == Usage.Input || tag.Usage == Usage.Output)
                {
                    if (tag.IsRequired && dataType.IsAtomic)
                    {
                        // two lines
                        parameter = new InstructionParameter
                        {
                            Label = tag.Name,
                            Type = "Read_DataSource",
                            DataType = tag.DataTypeInfo.ToString(),
                            Visible = true,
                            HasConfig = false,
                            Formats = new List<FormatType> { FormatType.Tag }
                        };
                        parameters.Add(parameter);

                        parameter = new InstructionParameter
                        {
                            Label = string.Empty,
                            Type = "Read_DataTarget",
                            DataType = string.Empty,
                            Visible = true,
                            HasConfig = false,
                            Formats = new List<FormatType> { FormatType.Tag }
                        };
                        parameters.Add(parameter);

                        continue;
                    }

                    if (tag.IsVisible && !tag.IsRequired)
                    {
                        parameter = new InstructionParameter
                        {
                            Label = tag.Name,
                            Type = "Read_DataTarget",
                            DataType = tag.DataTypeInfo.ToString(),
                            Visible = true,
                            HasConfig = false,
                            Formats = new List<FormatType> { FormatType.Tag }
                        };
                        parameters.Add(parameter);

                        continue;
                    }

                    continue;
                }

                // default , InOut
                parameter = new InstructionParameter
                {
                    Label = tag.Name,
                    Type = "Read_none",
                    DataType = tag.DataTypeInfo.ToString(),
                    Visible = tag.IsVisible,
                    HasConfig = false,
                    Formats = new List<FormatType> { FormatType.Tag }
                };
                parameters.Add(parameter);

            }

            definition.Parameters = parameters;
            definition.BitLegs = bitlegs;

            return definition;
        }
    }
}
