using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Antlr4.Runtime;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.CodeCompletion;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.View;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemsGenerator
    {
        private readonly List<StxCompletionItemCodeSnippetData> _codeSnippets;
        private readonly StxEditorViewModel _context;

        private readonly List<StxCompletionItemTagData> _controlTags;
        private readonly List<StxCompletionItemKeywordData> _keywords;
        private readonly List<StxCompletionItemTagData> _programTags;
        private readonly List<StxCompletionItemAoiData> _aoiTags;

        public static readonly string[] Keywords =
        {
            "#endregion", "by", "do", "else", "#region",
            "case", "for", "if", "repeat", "while", "elsif", "else",
            "end_case", "end_for", "end_if", "end_repeat", "end_while",
            "exit", "of", "then", "to", "until", "S:FS", "S:N", "S:Z", "S:V", "S:C", "S:MINOR","and","or","not","xor","mod"
        };

        public StxCompletionItemsGenerator(StxEditorViewModel viewModel, List<StxCompletionItemCodeSnippetData> data)
        {
            _context = viewModel;

            _controlTags = ToStxCompletionItemTagDataList(Controller.Tags);
            _programTags = ToStxCompletionItemTagDataList(Routine.ParentCollection.ParentProgram.Tags);
            _aoiTags = ToStxCompletionItemAoiDataList(Controller.AOIDefinitionCollection);
            _keywords = GenerateKeywordItems();
            //_codeSnippets = GenerateCodeSnippetItems();
            _codeSnippets = data;
            CollectionChangedEventManager.AddHandler(Controller.Tags, ControllerTags_CollectionChanged);
            //Controller.Tags.CollectionChanged += ControllerTags_CollectionChanged;
            CollectionChangedEventManager.AddHandler(Routine.ParentCollection.ParentProgram.Tags,
                RoutineTags_CollectionChanged);
            //Routine.ParentCollection.ParentProgram.Tags.CollectionChanged += RoutineTags_CollectionChanged;
            CollectionChangedEventManager.AddHandler(Controller.AOIDefinitionCollection, AOIDefinitionCollection_CollectionChanged);
        }

        public void Cleanup()
        {
            CollectionChangedEventManager.RemoveHandler(Controller.Tags, ControllerTags_CollectionChanged);
            CollectionChangedEventManager.RemoveHandler(Routine.ParentCollection.ParentProgram.Tags,
                RoutineTags_CollectionChanged);
            CollectionChangedEventManager.RemoveHandler(Controller.AOIDefinitionCollection, AOIDefinitionCollection_CollectionChanged);
        }

        private void AOIDefinitionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var data = new StxCompletionItemAoiData((AoiDefinition)item);
                    _aoiTags.Add(data);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var aoiData = _aoiTags.FirstOrDefault(a => a.Aoi == item);
                    if (aoiData != null)
                        _aoiTags.Remove(aoiData);
                }
            }
        }

        public List<StxCompletionItemAoiData> StxCompletionItemAoiDataCollection => _aoiTags;

        private void RoutineTags_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 0)
                {
                    foreach (var item in e.NewItems)
                    {
                        var data = new StxCompletionItemTagData(Controller, item as Tag);
                        _programTags.Add(data);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count > 0)
                {
                    foreach (var item in e.OldItems)
                    {
                        var data = _programTags.Find(x => x.Tag == item);
                        _programTags.Remove(data);
                    }
                }
            }
        }

        private void ControllerTags_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 0)
                {
                    var data = new StxCompletionItemTagData(Controller, e.NewItems[0] as Tag);
                    _controlTags.Add(data);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count > 0)
                {
                    var data = _controlTags.Find(x => x.Tag == e.OldItems[0]);
                    _controlTags.Remove(data);
                }
            }
        }

        public STRoutine Routine => _context.Routine;
        public Controller Controller => _context.Controller;

        //private IProgram IsEnterAfterProgram(TextEditor editor)
        //{
        //    var text = editor.Text.Substring(0, editor.CaretOffset - 1);
        //    string program = "";
        //    for (int i = text.Length - 1; i >= 0; i--)
        //    {
        //        if (char.IsNumber(text[i]) || char.IsLetter(text[i]) || text[i] == '_')
        //        {
        //            program = text[i] + program;
        //            continue;
        //        }

        //        break;
        //    }

        //    return Controller.Programs[program];
        //}

        public void GenerateItems(CompletionWindow completionWindow, TextEditor editor, string typedText,
            IDataType filterDataType)
        {
            var data = completionWindow.CompletionList.CompletionData;
            if (string.IsNullOrEmpty(typedText))
                return;
            bool isAoi = _context.Routine.ParentCollection.ParentProgram is AoiDefinition;
            //TODO(gjc): add code here
            if (typedText == "\\" && !isAoi)
            {
                foreach (var program in Controller.Programs)
                {
                    var item = new StxCompletionItemProgramData(program.Name);
                    data.Add(new StxCompletionItem(item));
                }
                completionWindow.StartOffset = editor.CaretOffset;
                completionWindow.EndOffset = editor.CaretOffset - 1;
                return;
            }

            if (typedText == ".")
            {
                try
                {
                    int offset = 0;
                    var code  = GetSnippetByPosition.GetParentCode(editor, ref offset);
                    var parentCode = (ObtainValue.GetCodeIgnoreDims(string.Join(".", code)) ?? "").Split('.');

                    completionWindow.StartOffset = editor.CaretOffset;
                    completionWindow.EndOffset = editor.CaretOffset - 1;

                    if (parentCode[0].IndexOf("\\") == 0 && parentCode.Length == 1)
                    {
                        var programName = parentCode[0].Substring(1);
                        var program = Controller.Programs[programName];
                        foreach (Tag tag in program.Tags)
                        {
                            if(program!=_context.Routine.ParentCollection.ParentProgram)
                                if (tag.Usage == Usage.Local)
                                    continue;
                            var item = new StxCompletionItem(new StxCompletionItemTagData(Controller, tag));
                            data.Add(item);
                        }

                        return;
                    }

                    var dataType = ObtainValue.GetDataTypeInfoIgnoreDims($"{string.Join(".", parentCode)}.",
                        _context.Routine.ParentCollection.ParentProgram);
                    var dataList = new List<ICompletionData>();
                    if (dataType is CompositiveType)
                    {
                        foreach (var member in (dataType as CompositiveType).TypeMembers)
                        {
                            if (member.IsHidden) continue;
                            var newTag = new Tag(Routine.ParentCollection.ParentProgram.Tags as TagCollection)
                            {
                                Name = member.Name,
                                Description = "Member",
                                DataWrapper = new DataWrapper(member.DataTypeInfo.DataType,
                                    member.DataTypeInfo.Dim1, member.DataTypeInfo.Dim2, member.DataTypeInfo.Dim3,
                                    null)
                            };

                            var item = new StxCompletionItem(new StxCompletionItemTagData(Controller, newTag));
                            dataList.Add(item);
                        }
                    }
                    else if (dataType is SINT || dataType is DINT || dataType is INT || dataType is LINT)
                    {
                        int size = dataType.BitSize;
                        int i = 0;
                        while (i < size)
                        {
                            var item = new StxCompletionItem(new StxCompletionItemData(i.ToString(), "Index"));
                            dataList.Add(item);
                            i++;
                        }
                        foreach (var completionData in dataList) data.Add(completionData);
                        return;
                    }

                    dataList.Sort((x, y) => string.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase));

                    foreach (var completionData in dataList) data.Add(completionData);

                }
                catch (Exception)
                {
                    data.Clear();
                }
            }
            else
            {
                int start = 0, end = 0;
                var code = CompletionWindow.GetCode(editor.TextArea,true, ref start, ref end);
                var str = code.Item1 + code.Item2;
                completionWindow.StartOffset = str.LastIndexOf(".") + 1 + start;
                completionWindow.EndOffset = end ;
                if (filterDataType is StxEditorView.ProgramDataType)
                {
                    if (_context.Routine.ParentCollection.ParentProgram is Program)
                    {
                        foreach (var program in Controller.GetInstance().Programs)
                        {
                            var item = new StxCompletionItemProgramData(program.Name);
                            data.Add(new StxCompletionItem(item));
                        }
                    }
                    return;
                }

                if (filterDataType is StxEditorView.RoutineDataType)
                {
                    if (_context.Routine.ParentCollection.ParentProgram is Program)
                    {
                        foreach (var routine in _context.Routine.ParentCollection.ParentProgram.Routines)
                        {
                            var item = new StxCompletionItemRoutineData(routine.Name);
                            data.Add(new StxCompletionItem(item));
                        }
                    }
                    return;
                }
                if (filterDataType is StxEditorView.ModuleDataType)
                {
                    if (_context.Routine.ParentCollection.ParentProgram is Program)
                    {
                        foreach (var module in _context.Routine.ParentController.DeviceModules)
                        {
                            if(module is LocalModule||module is DiscreteIO)continue;
                            var item = new StxCompletionItemModuleData(module.Name);
                            data.Add(new StxCompletionItem(item));
                        }
                    }
                    return;
                }
                if (filterDataType is StxEditorView.TaskDataType)
                {
                    if (_context.Routine.ParentCollection.ParentProgram is Program)
                    {
                        foreach (var module in _context.Routine.ParentController.Tasks)
                        {
                            var item = new StxCompletionItemTaskData(module.Name);
                            data.Add(new StxCompletionItem(item));
                        }
                    }
                    return;
                }
                var instrInfo = new InstrEnumInfo();
                if (GetSnippetByPosition.IsEnterInstrEnum(instrInfo, editor, _context.Routine))
                {
                    for (int i = 0; i < instrInfo.Enums.Count; i++)
                    {
                        if (i > 0)
                        {
                            var instrData = new StxCompletionItemKeywordData(instrInfo.Enums[i], "Enum");
                            data.Add(new StxCompletionItem(instrData));
                        }
                    }

                    return;
                }

                if (str.StartsWith("\\") && str.IndexOf(".") < 0)
                {
                    completionWindow.StartOffset = completionWindow.StartOffset + 1;
                    var name = str.Substring(1).ToCharArray();
                    foreach (var program in Controller.Programs)
                    {
                        if (CompletionList.IsFuzzyMatch(program.Name, name))
                        {
                            var item = new StxCompletionItemProgramData(program.Name);
                            data.Add(new StxCompletionItem(item));
                        }
                    }
                    return;
                }

                var parentCode = (ObtainValue.GetCodeIgnoreDims(string.Join(".", str)) ?? "").Split('.');
                if (parentCode[0].IndexOf("\\") == 0 && parentCode.Length == 2)
                {
                    var programName = parentCode[0].Substring(1);
                    var program = Controller.Programs[programName];
                    var codes2 = parentCode[1].ToCharArray();
                    foreach (Tag tag in program.Tags)
                    {
                        IDataType dataType = tag.DataTypeInfo.DataType;
                        if (tag.DataTypeInfo.Dim1 > 0)
                        {
                            dataType = new ArrayType(dataType, tag.DataTypeInfo.Dim1, tag.DataTypeInfo.Dim2,
                                tag.DataTypeInfo.Dim3);
                        }
                        if (((filterDataType==null || dataType.Equal(filterDataType, true)) && CompletionList.IsFuzzyMatch(tag.Name, codes2)))
                        {
                            var item = new StxCompletionItem(new StxCompletionItemTagData(Controller, tag));
                            data.Add(item);
                        }
                    }

                    return;
                }

                var items = CodeMatch.Match(str,_context.Routine.ParentCollection.ParentProgram);
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        var t = new Tag(Routine.ParentCollection.ParentProgram.Tags as TagCollection)
                        {
                            Name = item.Name, DataWrapper = new DataWrapper(item.DataTypeInfo.DataType, 0, 0, 0, null)
                        };
                        var s = new StxCompletionItemTagData(Controller, t);
                        var m = new StxCompletionItem(s);
                        data.Add(m);
                    }
                }
                else
                {

                    if (InstrMatch.Match(editor, Routine, data,_context.StxEditorDocument.SnippetLexer))
                    {
                        return;
                    }


                    var codes = code.Item1.ToCharArray().Concat(code.Item2.ToArray()).ToArray();
                    var dataList = new List<ICompletionData>();

                    if (!isAoi)
                        foreach (var tagData in _controlTags)
                        {
                            if (filterDataType != null)
                            {
                                IDataType dataType = tagData.Tag.DataTypeInfo.DataType;
                                //if (tagData.Tag.DataTypeInfo.Dim1 > 0)
                                //{
                                //    dataType = new ArrayType(dataType, tagData.Tag.DataTypeInfo.Dim1, tagData.Tag.DataTypeInfo.Dim2,
                                //        tagData.Tag.DataTypeInfo.Dim3);
                                //}
                                if (dataType.Equal(filterDataType, true) && CompletionList.IsFuzzyMatch(tagData.Name, codes))
                                    dataList.Add(new StxCompletionItem(tagData));
                            }
                            else
                            {
                                if (CompletionList.IsFuzzyMatch(tagData.Name, codes))
                                    dataList.Add(new StxCompletionItem(tagData));
                            }
                        }

                    foreach (var aoiData in _aoiTags)
                    {
                        if (CompletionList.IsFuzzyMatch(aoiData.Name, codes))
                            dataList.Add(new StxCompletionItem(aoiData));
                    }


                    foreach (var tagData in _programTags)
                    {
                        if (filterDataType != null)
                        {
                            IDataType dataType = tagData.Tag.DataTypeInfo.DataType;
                            //if (tagData.Tag.DataTypeInfo.Dim1 > 0)
                            //{
                            //    dataType = new ArrayType(dataType, tagData.Tag.DataTypeInfo.Dim1, tagData.Tag.DataTypeInfo.Dim2,
                            //        tagData.Tag.DataTypeInfo.Dim3);
                            //}
                            if (dataType.Equal(filterDataType, true) && CompletionList.IsFuzzyMatch(tagData.Name, codes))
                                dataList.Add(new StxCompletionItem(tagData));
                        }
                        else
                        {
                            if (CompletionList.IsFuzzyMatch(tagData.Name, codes))
                                dataList.Add(new StxCompletionItem(tagData));
                        }
                    }

                    if (filterDataType == null)
                    {
                        //Keywords
                        foreach (var keywordData in _keywords)
                        {
                            if (CompletionList.IsFuzzyMatch(keywordData.Name, codes))
                                dataList.Add(new StxCompletionItem(keywordData));
                        }

                        //CodeSnippets
                        foreach (var codeSnippet in _codeSnippets)
                        {
                            if (CompletionList.IsFuzzyMatch(codeSnippet.Name, codes))
                                dataList.Add(new StxCompletionItem(codeSnippet));
                        }
                        
                    }
                    //TODO(gjc): add code here

                    dataList.Sort((x, y) => string.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase));

                    foreach (var completionData in dataList) data.Add(completionData);
                }
            }
        }

        

        //public void GenerateCodeSnippets(IList<StxCompletionItemCodeSnippetData> data)
        //{
        //    var dataList = new List<StxCompletionItemCodeSnippetData>();
        //    foreach (var codeSnippet in _codeSnippets)
        //    {
        //        dataList.Add(codeSnippet);
        //        //TODO(zyl):remove if,end_if,cae and so on;
        //    };
        //    dataList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
        //    foreach (var completionData in dataList) data.Add(completionData);
        //}

        //private List<StxCompletionItemCodeSnippetData> GenerateCodeSnippetItems()
        //{
        //    var dataList = new List<StxCompletionItemCodeSnippetData>
        //    {
        //        new StxCompletionItemCodeSnippetData("#region", ""),
        //        new StxCompletionItemCodeSnippetData("case",
        //            "Use CASE...OF to select what to do based on a numerical value"),
        //        new StxCompletionItemCodeSnippetData("elsif",
        //            "Use ELSIF...THEN to do something if or when specific conditions occur"),
        //        new StxCompletionItemCodeSnippetData("for",
        //            "Use the FOR...DO loop to do something a specific number of times before doing anything else"),
        //        new StxCompletionItemCodeSnippetData("if",
        //            "Use IF...THEN to do something if or when specific conditions occur"),
        //        new StxCompletionItemCodeSnippetData("repeat",
        //            "Use the REPEAT...UNTIL loop to keep doing something until conditions are true"),
        //        new StxCompletionItemCodeSnippetData("while",
        //            "Use the WHILE...DO loop to keep doing something as long as certain conditions are true"),
        //        new StxCompletionItemCodeSnippetData("then", ""),
        //        new StxCompletionItemCodeSnippetData("until","")
        //    };

        //    #region Add Funciton keywords

        //    dataList.Add(new StxCompletionItemCodeSnippetData("ABS","Absolute Value","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ACOS", "Arccosine","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ALM", "Alarm","ALMTag","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ALMA", "Analog Alarm","ALMA,In,ProgAckAll,ProDisable,ProgEnable","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ALMD", "Digital","ALMD,In,ProgAck,ProgReset,ProgDisable,ProgEnable","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ASIN", "Arcsine","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ATAN", "Arctangent","Source","Function"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("BTDT", "Bit Field Distribute with Target","BTDTTag","Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("CC", "Coordinated Control","CCTag","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("CONCAT", "String Concatenate","Source A,Source B,Dest","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("COP", "Copy File","Source,Dest,Length","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("COS", "Cosine","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("CPS", "Synchronous Copy File","Source,Dest,Length","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("CTUD", "Count Up/Down","CTUDTag","Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("D2SD", "Discrete 2-State Device","D2SDTag","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("D3SD", "Discrete 3-State Device", "D3SDTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("DEDT", "Deadtime","DEDTTag,StorageArray","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("DEG", "Radians To Degrees","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("DELETE", "String Delete","Source,Qty,Start,Dest","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("DERV", "Derivate","DERVTag","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("DFF", "D Flip Flop","DFFTag","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("DTOS", "DINT to String","Source,Dest","Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("EOT", "End Of Transition","State Bit","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("ESEL", "Enhanced Select","ESELTag","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("EVENT", "Triggrt Event Task","Task","Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("FGEN", "Function Generator","FGENTag,X1,Y1,X2,Y2","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("FIND", "Find String","Source,Search,Start,Result","Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("HLL", "High/Low Limit","HLLTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("HMIBC", "HMI Button Control","HMIBC", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("HPF", "High-Pass-Filter","HPFTag", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("IMC", "Internal Model Control","IMCTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("INSERT", "Insert String","Source A,Source B,Start,Dest", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("INTG", "Integrator","INTGTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("IOT", "Immediate","Update Tag", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("JKFF", "JK Flip Flop","JKFFTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("JSR", "Jump To Subroutine","Routine Name, Number of Inputs[,Input Par][,Return Par]", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("LDL2", "Second-Order Lead-Lag","LDL2Tag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("LDLG", "Lead-Lag","LDLGTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("LN", "Nature Log","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("LOG", "Log Base  10","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("LOWER", "Lower Case","Source,Dest", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("LPF", "Low-Pass Filter","LPFTag", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAAT", "Motion Apply Axis Tuning","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAFR", "Motion Axis Fault Reset","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAG", "Motion Axis Gear","Slave Axis,Master Axis,Motion Control,Direction,Ratio,Slave Counts,Master Counts,Master Reference,Ratio Format,Clutch,Accel Rate,Accel Units","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAH", "Motion Axis Home","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAHD", "Motion Apply Hookup Diagnostics","Axis,Motion Control,Diagnostic Test,Observed Direction", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAJ", "Motion Axis Jog","Axis,Motion Control,Direction,Speed,Speed Units,Accel Rate,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Merge,Merge Speed,Lock Position,Lock Direction", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAM", "Motion Axis Move","Axis,Motion Control,Move Type,Position,Speed,Speed Units,Accel Rate,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Merge,Merge Speed,Lock Position,Lock Position,Lock Direction,Event Distance,Calculated Data", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAOC", "Motion Arm Output Cam","Axis,Execution Target,Motion Control,Output,Input,Output Cam,Cam Start Position,Cam End Position,Output Compensation,Execution Mode,Execution Schedule,Axis Arm position,Cam Arm Position,Cam Arm Position,Cam Arm Position,Position Reference", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAPC", "Motion Axis Position Cam","Slave Axis,Master Axis,Motion Control,Direction,Cam Profile,Slave Scaling,Master Scaling,Execution Mode,Execution Schedule,Master Lock Position,Cam Lock Position ,Master Reference,Master Direction", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAR", "Motion Arm Registration","Axis Control,Trigger Condition,Windowed Registration,Min.Position,Max.Position,Input Number", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAS", "Motion Axis Stop","Axis,Motion Control,Stop Type,Change Decel,Decel Rate, Decel Units,Change Decel Jerk,Decel Jerk,Jerk Units", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MASD", "Motion Axis Shutdown","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MASR", "Motion Axis Shutdown Reset","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MATC", "Motion Axis Time Cam","Axis,Motion Control,Direction,Cam Profile,Distance Scaling,Time Scaling,Execution Mode,Execution Schedule,Lock Position,Lock Direction,Instruction Mode", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAVE", "Move Average","MAVETag,StorageArray,WeightArray", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAW", "Motion Arm Watch","Axis,Motion Control,Trigger Condition,Position", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MAXC", "Maximum Capture","MAXCTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCCD", "Motion Coordinated Change Dynamics","Coordinate System,Motion Control,Motion Type,Change Speed,Speed,Speed Units,Change Accel,Accel Rate,Accel Units,Change Decel,Decel Rate,Decel Units,Change Accel Jerk,Accel Jerk,Change Decel Jerk,Decel Jerk,Jerk Units,Scope", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCCM", "Motion Coordinated Circular Move","Coordinate System,Motion Control,Move Type,Position,Circle Type,Via/Center/Radius,Direction,Speed,Speed Untis,Accel Rate,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Termination Type,Merge,Merge Speed,Command Tolerance,Lock Position,Lock Direction,Event Distance,Calculated Data", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCCP", "Motion Calculate Cam Profile","Motion Control,Cam,Length,Start Slope,End Slope,CamProfile", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCD", "Motion Change Dynamics","Axis,Motion Control,Motion Type,Change Speed,Speed,Change Accel,Accel Rate,Change Decel Rate,Change Decel,Decel Rate,Change Accel Jerk,Accel Jerk,Change Decel Jerk,Decel Jerk,Speed Units,Accel Units,Decel Units,Jerk Units", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCLM", "Motion Coordinated Linear Move","Coordinate System,Motion Control,Move Type,Position,Speed,Speed Units,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Termination Type,Merge,Merge Speed,Command Tolerance,Lock Position,Lock Direction,Event Distance,Calculated Data", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCPM", "Motion Coordinated Path Move","Coordinate System,Motion Control,Path,Length,Dynamics,Lock Position,Lock Direction", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCS", "Motion Coordinated Stop","Coordinate System,Motion Control,Stop Type,Change Decel,Decel Rate,Decel Units,Change Decel Jerk,Jerk Units", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCSD", "Motion Coordinated Shutdown","Coordinate System,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCSR", "Motion Coordinated Shutdown Reset","Coordinate System,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCSV", "Motion Calculate Slave Values","Motion Control,Cam Profile,Master Value,Slave Value,Slop Value,Slope Derivative", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCT", "Motion Coordinated Transform","Source System,Target System,Motion Control,Orientation,Translation", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCTO", "Motion Coordinated Transform with Orientation","Cartesian System,Robot System,Motion Control,Work Frame,Tool Frame", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCTP", "Motion Calculate Transform Position","Source System,Target System,Motion Control,Orientation,Translation,Transform,Transform Direction,Reference Position,Transform Position", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MCTPO", "Motion Calculate Transform Position with Orientation","Cartesian System,Robot System,Motion Control,Work Frame,Tool Frame,Transform Direction,Reference Position,Transform Position,Transform Position,Robot Configuration,Turns Counter", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDAC", "Motion Master Driven Axis Control","Slave Axis,Master Axis,Motion Control,Motion Type,Master Reference", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDCC", "Motion Master Driven Coordinated Control","Slave System,Master Axis,Motion Control,Master Reference,Nominal Master Velocity", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDF", "Motion Direct Drive Off","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDO", "Motion Direct Drive On","Axis,Motion Control,Driver Output,Drive Units", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDOC", "Motion Disarm Output Cam","Axis,Execution Target,Motion Control,Disarm Type", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDR", "Motion Disarm Registration","Axis,Motion Control,Input Number", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDS", "Motion Drive Start","Axis,Motion Control,Speed,Speed Units", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MDW", "Motion Disarm Watch","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MGS", "Motion Group Stop","Group,Motion Control,Stop Model", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MGSD", "Motion Group Shutdown","Group,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MGSP", "Motion Group Strobe Position","Group,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MGSR", "Motion Group Shutdown Reset","Group,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MID", "Middle String","Source,Qty,Start,Dest", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MINC", "Minimum Capture","MINCTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MMC", "Modular Multivariable Control","MMCTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MRAT", "Motion Run Axis Tuning","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MRHD", "Motion Run Hookup Diagnostics","Axis,Motion Control,Diagnostic Test", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MRP", "Motion Redefine Position","Axis,Motion Control,Type,Position Select,Position", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MSF", "Motion Servo Off","Axis,Motion Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MSG", "Message","Message Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MSO", "Motion Servo On","Axis,Motion Control","Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MSTD", "Moving Standard Deviation","MSTDTag,StorageArray", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("MVMT", "Masked Move with Target","MVMTTag", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("NTCH", "Notch Filter","NTCHTag", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("OSFI", "One Shot Falling with Input","OSFITag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("OSRI", "One Shot Rising with Input","OSRITag", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("PATT", "Attach to Equipment Phase","Phase Name,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PCLF", "Equipment Phase Clear Failure","Phase Name", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PCMD", "Equipment Phase Command","Phase Name,Command,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PDET", "Detach from Equipment Phase","Phase Name", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PFL", "Equipment Phase Failure","Source", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PI", "Proportional+Integral","PITag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PID", "Proportional Integral Derivative","PID,Process Variable,Tieback,Control Variable,PID Master Loop,Inhold Bit,Inhold Value", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PIDE", "Enhanced PID","PIDETag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PMUL", "Pulse Multiplier","PMULTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("POSP", "Position Proportional","POSPTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("POVR", "Equipment Phase Override Command","Phase Name,Command ,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PPD", "Equipment Phase Pause","", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PRNP", "Equipment Phase New Parameters","", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PXRQ", "Equipment Phase External Request", "Phase Instruction,External Request,Data Value", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("PSC", "Equipment Phase State Complete","", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("RAD", "Degrees To Radians","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("RESD", "Reset Dominant","RESDTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("RET", "Return from Subroutine","[Return Par]", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("RLIM", "Rate Limiter","RLIMTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("RMPS", "Ramp/Soak","RMPSTAg,RampValue,SoakValue,SoakTime", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("RTOR", "Retentive Timer On with Reset","RTORTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("RTOS", "Real to String","Source,Dest", "Instruction"));

        //    dataList.Add(new StxCompletionItemCodeSnippetData("SASI", "Sequence Assign Sequence Id","Sequence Name,Sequence Id,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SATT", "Attach to Sequence","Sequence Name,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SBR", "Subroutine","[Input Par]", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SCL", "Scale","SCLTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SCLF", "Sequence Clear Failure","Sequence Name,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SCMD", "Sequence Command","Sequence Name,Command,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SCRV", "S-Curve","SCRVTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SDET", "Detach from Sequence","Sequence Name", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SETD", "Set Dominant","SETDTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SFP", "SFC Pause","SFC Routine Name,Target State", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SFR", "SFC Reset","SFC Routine Name,Step Name", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SIN", "Sine","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SIZE", "Size in Elements","Source,Dim,To Vary,Size", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SNEG", "Selectable Negate","SNEGTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SOC", "Second-Order-Controller","SOCTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SOVR", "Sequence Override Command","Sequence Name,Command,Result", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SQRT", "Square Root","Source","Function"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SRTP", "Split Range Time Proportional","SRTPTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SRT", "Sort File","Array,Dim,To Vary,Control", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SSUM", "Selected Summer","SSUMTag", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SSV", "Set System Value","Class Name,Instance Name,Attribute Name,Source", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("STOD", "String To DINT","Source,Dest", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("STOR", "String to Real","Source,Dest", "Instruction"));
        //    dataList.Add(new StxCompletionItemCodeSnippetData("SWPB", "Swap Byte","Source,Order Mode,Dest", "Instruction"));
        //    #endregion
        //    return dataList;
        //}

        private List<StxCompletionItemKeywordData> GenerateKeywordItems()
        {
            var dataList = new List<StxCompletionItemKeywordData>();

            foreach (var keyword in Keywords)
            {
                var data = new StxCompletionItemKeywordData(keyword.ToUpper());
                dataList.Add(data);
            }

            return dataList;
        }

        private List<StxCompletionItemAoiData> ToStxCompletionItemAoiDataList(
            IAoiDefinitionCollection aoiDefinitionCollection)
        {
            var dataList = new List<StxCompletionItemAoiData>();

            if (aoiDefinitionCollection == null)
                return dataList;

            foreach (var aoi in aoiDefinitionCollection)
            {
                var data = new StxCompletionItemAoiData((AoiDefinition) aoi);
                dataList.Add(data);
            }

            return dataList;
        }

        private List<StxCompletionItemTagData> ToStxCompletionItemTagDataList(ITagCollection tags)
        {
            var dataList = new List<StxCompletionItemTagData>();

            if (tags == null)
                return dataList;

            foreach (var tag in tags)
            {
                var data = new StxCompletionItemTagData(Controller, tag as Tag);
                dataList.Add(data);
            }

            return dataList;
        }
    }
}