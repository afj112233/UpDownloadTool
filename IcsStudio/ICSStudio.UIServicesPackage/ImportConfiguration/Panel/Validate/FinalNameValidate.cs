using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate
{
    class FinalNameValidate: ValidationRule
    {
        public FinalNameValidateParam Param { set; get; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Param.Type == ProjectItemType.UserDefined || Param.Type == ProjectItemType.Strings)
            {
                if (!IsValidName((string)value)) return new ValidationResult(false, "");
                var parent = GetParent(Param.Target);
                var dataTypes = parent["DataTypes"];
                var aois = parent["AddOnInstructionDefinitions"];
                foreach (var dataType in dataTypes)
                {
                    if(dataType==Param.Target)continue;
                    if(((string)value).Equals((string)dataType["FinalName"],StringComparison.OrdinalIgnoreCase))
                    {
                        Param.Source.Error = $"Failed to set the name '{value}'.\nThis name is already in use as a Final Name in this collection.";
                        return new ValidationResult(false, "");
                    }
                }

                foreach (var aoi in aois)
                {
                    if (((string)value).Equals((string)aoi["FinalName"],StringComparison.OrdinalIgnoreCase))
                    {
                        Param.Source.Error = $"Failed to set the name '{value}'.\nA Add-On-Defined Data Type by the same name already exists.";
                        return new ValidationResult(false, "");
                    }
                }

                if(!VerifyDataType((string)value)) return new ValidationResult(false, "");
            }
            if (Param.Type == ProjectItemType.AddOnDefined || Param.Type == ProjectItemType.AddOnInstructions)
            {
                if (!IsValidName((string)value)) return new ValidationResult(false, "");
                var parent = GetParent(Param.Target);
                var dataTypes = parent["DataTypes"];
                var aois = parent["AddOnInstructionDefinitions"];
                foreach (var dataType in dataTypes)
                {
                    if (((string)value).Equals((string)dataType["FinalName"], StringComparison.OrdinalIgnoreCase))
                    {
                        Param.Source.Error =
                            $"Failed to set the name '{value}'.\nA User-Defined Data Type by the same name already exists.which would collide with the Instruction's Add-On-Defined Data Type.";
                        return new ValidationResult(false, "");
                    }
                }

                foreach (var aoi in aois)
                {
                    if (aoi == Param.Target) continue;
                    if (((string)value).Equals((string)aoi["FinalName"], StringComparison.OrdinalIgnoreCase))
                    {
                        Param.Source.Error = $"Failed to set the name '{value}'.\nThis name is already in use a Final Name in this collection.";
                        return new ValidationResult(false, "");
                    }
                }

                if (!VerifyAoi((string) value)) return new ValidationResult(false, "");
            }else if (Param.Type == ProjectItemType.ProgramTags||Param.Type==ProjectItemType.AddOnDefinedTags||Param.Type==ProjectItemType.ControllerTags)
            {
                if (value == null || string.IsNullOrEmpty((string) value))
                {
                    Param.Source.Error = $"Failed to set the name .\nName is Invalid."; ;
                    return new ValidationResult(false,"");
                }

                var name = (string) value;
                if (name.StartsWith("\\"))
                {
                    var index = name.IndexOf(".");
                    var program = name.Substring(0, index == -1 ? name.Length : index);
                    if (!name.Equals(program)) name = name.Substring(program.Length + 1);
                }

                if (!IsValidName(name)) return new ValidationResult(false, "");
                var tags = (JArray)Param.Target.Parent;
               
                foreach (var tag in tags)
                {
                    if(tag==Param.Target)continue;
                    var tagName = (string) tag["FinalName"];
                    if (tagName.StartsWith("\\"))
                    {
                        tagName = tagName.Substring(tagName.IndexOf(".") + 1);
                    }
                    if (tagName.Equals((string)value, StringComparison.OrdinalIgnoreCase))
                    {
                        Param.Source.Error = $"Failed to set the name '{value}'.\nThis name is already in use a Final Name in this collection.";
                        return new ValidationResult(false, "");
                    }
                }
            }
            else
            {
                if (!IsValidName((string)value)) return new ValidationResult(false, "");
            }

            Param.Source.Error = "";
            return new ValidationResult(true,"");
        }
        
        private JToken GetParent(JToken jObject)
        {
            while (jObject.Parent != null)
            {
                return GetParent(jObject.Parent);
            }

            return jObject;
        }

        private bool VerifyDataType(string name)
        {
            var preDataTypes = Controller.GetInstance().DataTypes.FirstOrDefault(d => d.IsPredefinedType&&d.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
            var flag = preDataTypes == null;
            if(!flag) Param.Source.Error = $"Failed to set the name '{name}'.\nThe Specified name is reserved for one of predefined Data Types.";
            return flag;
        }

        private bool VerifyAoi(string name)
        {
            if (VerifyDataType(name))
            {
                var instruction = Controller.GetInstance().STInstructionCollection.FindInstruction(name);
                if (instruction is AoiDefinition.AOIInstruction) return true;
                Param.Source.Error = $"Failed to set the name '{name}'.\nThe Specified name is reserved for one of instructions.";
            }

            return false;
        }

        private bool IsValidName(string name)
        {
            bool isValid = !string.IsNullOrEmpty(name);

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") || name.IndexOf("__") > -1)
                {
                    isValid = false;
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                }
            }

            // key word
            if (isValid)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                    }
                }
            }

            if (!isValid) Param.Source.Error = $"Failed to set the name .\nName is Invalid."; ;
            return isValid;
        }
    }

    class FinalNameValidateParam: DependencyObject
    {
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target", typeof(JObject), typeof(FinalNameValidateParam), new PropertyMetadata(default(JObject)));

        public JObject Target
        {
            get { return (JObject) GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(ProjectItemType), typeof(FinalNameValidateParam), new PropertyMetadata(default(ProjectItemType)));

        public ProjectItemType Type
        {
            get { return (ProjectItemType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(IVerify), typeof(FinalNameValidateParam), new PropertyMetadata(default(IVerify)));

        public IVerify Source
        {
            get { return (IVerify) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}
