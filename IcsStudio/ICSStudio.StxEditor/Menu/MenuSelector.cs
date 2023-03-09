using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.Interfaces;
using ICSStudio.StxEditor.Menu.Dialog.ViewModel;
using ICSStudio.StxEditor.ViewModel;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using Type = ICSStudio.StxEditor.ViewModel.CodeSnippets.Type;

namespace ICSStudio.StxEditor.Menu
{
    internal class MenuSelector
    {
        private ContextMenu _selectedContextMenu,
            _tagContextMenu,
            _stContextMenu,
            _emptyContextMenu,
            _programContextMenu;

        private readonly StxEditorDocument _editorDocument;
        private readonly TextEditor _textEditor;

        public MenuSelector(StxEditorDocument stxEditorDocument, TextEditor textEditor)
        {
            _editorDocument = stxEditorDocument;
            _textEditor = textEditor;
        }

        public ContextMenu SetSelectedContextMenu(IController controller, SnippetInfo info,
            Point cursorPoint, BrowseAdorner browseAdorner, IStxEditorOptions options, IProgramModule program)
        {
            _selectedContextMenu = new ContextMenu();
            var browseMenu = new BrowseTagsMenuItem(_textEditor, cursorPoint, browseAdorner, options, info);
            var addStElementMenu = new AddSTElementMenuItem(controller, _textEditor.Document, info);
            var separator = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, program);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, program);
            if (controller.IsOnline && !options.ShowPending)
            {
                cutMenu.IsEnabled = false;
                pasteMenu.IsEnabled = false;
                browseMenu.IsEnabled = false;
            }
            else
            {
                if (options.ShowOriginal || options.ShowTest || options.IsConnecting)
                {
                    cutMenu.IsEnabled = false;
                    pasteMenu.IsEnabled = false;
                    browseMenu.IsEnabled = false;
                }
            }

            _selectedContextMenu.Items.Add(browseMenu);
            _selectedContextMenu.Items.Add(addStElementMenu);
            _selectedContextMenu.Items.Add(separator);
            _selectedContextMenu.Items.Add(cutMenu);
            _selectedContextMenu.Items.Add(copyMenu);
            _selectedContextMenu.Items.Add(pasteMenu);

            return _selectedContextMenu;
        }

        public ContextMenu SetTagContextMenu(StxEditorDocument stxEditorDocument,ITag tag, Type type,
            SnippetInfo info, IProgramModule parentProgram, Point cursorPoint, BrowseAdorner browseAdorner, bool isOtherProgram, BrowseEnumAdorner browseEnumAdorner,
            Hashtable transformTable, IRoutine routine)
        {
            _tagContextMenu = new ContextMenu();
            IXInstruction instr = null;
            if (type == Type.Enum)
            {
                var browseEnum = new BrowseEnumMenuItem(_textEditor, cursorPoint, stxEditorDocument, info.Enums,
                    browseEnumAdorner, info);
                if (parentProgram.ParentController.IsOnline && !stxEditorDocument.Options.ShowPending)
                {
                    browseEnum.IsEnabled = false;
                }
                else
                {
                    if (stxEditorDocument.Options.ShowOriginal || stxEditorDocument.Options.ShowTest || stxEditorDocument.Options.IsConnecting)
                    {
                        browseEnum.IsEnabled = false;
                    }
                }

                _tagContextMenu.Items.Add(browseEnum);
            }
            else if (type == ICSStudio.StxEditor.ViewModel.CodeSnippets.Type.Instr)
            {
                instr = Controller.GetInstance().STInstructionCollection.FindInstruction(info.CodeText);
                Debug.Assert(instr != null);
                if (instr is AoiDefinition.AOIInstruction)
                {
                    info.IsAOI = true;
                }
                var createInstr = new CreateInstrMenuItem(info.CodeText, _editorDocument);
                var browseMenu = new BrowseTagsMenuItem(_textEditor, cursorPoint, browseAdorner, stxEditorDocument.Options, info);
                int count = 0;
                var parameters = GetInstrParameters(info, ref count,stxEditorDocument.SnippetLexer.GetEndOfCode(info.Offset,info.Parent),(STRoutine)routine);
                var argumentList = new ArgumentListMenuItem(info.CodeText, _editorDocument, info.Offset, parameters,
                    transformTable);
                if (parentProgram.ParentController.IsOnline && !stxEditorDocument.Options.ShowPending)
                {
                    browseMenu.IsEnabled = false;
                    createInstr.IsEnabled = false;
                }
                else
                {
                    if (stxEditorDocument.Options.ShowOriginal || stxEditorDocument.Options.ShowTest || stxEditorDocument.Options.IsConnecting)
                    {
                        browseMenu.IsEnabled = false;
                        createInstr.IsEnabled = false;
                    }
                }

                _tagContextMenu.Items.Add(createInstr);
                _tagContextMenu.Items.Add(browseMenu);
                _tagContextMenu.Items.Add(argumentList);
            }
            else
            {
                ITag tmpTag = null;
                if (parentProgram is IAoiDefinition)
                {
                    var editProperties = new EditAoiTagMenuItem(tag);
                    _tagContextMenu.Items.Add(editProperties);
                    if (stxEditorDocument.SnippetLexer.TransformTable != null)
                        tmpTag = ObtainValue.NameToTag(info.CodeText, stxEditorDocument.SnippetLexer.TransformTable,
                            stxEditorDocument.SnippetLexer.ConnectionReference.GetReferenceProgram())?.Item1;
                }
                else
                {
                    var editProperties = new EditPropertiesMenuItem(tag, parentProgram, isOtherProgram);
                    _tagContextMenu.Items.Add(editProperties);
                }

                if (tmpTag != null)
                {
                    var tmp = tag;
                    tag = tmpTag;
                    tmpTag = tmp;
                }
                
                if (tag.DataTypeInfo.DataType.IsMessageType)
                {
                    if ($"{tag.Name}".Equals(info.CodeText, StringComparison.OrdinalIgnoreCase) ||
                        $"\\{tag.ParentCollection.ParentProgram.Name}.{tag.Name}".Equals(info.CodeText,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        var item = ObtainValue.NameToTag(tag.Name, transformTable);
                        var mes = new MessageConfigurationMenuItem(item?.Item1??tag,tag.Name);
                        _tagContextMenu.Items.Add(mes);
                    }
                }
                else
                {
                    int subscriptLength = 0;
                    if (!string.IsNullOrEmpty(info.CodeText) && info.CodeText[info.CodeText.Length - 1] == ']')
                    {
                        var stack = new Stack<char>();
                        for (var i = info.CodeText.Length - 1; i >= 0; i--)
                        {
                            if (info.CodeText[i] == ']')
                                stack.Push(']');
                            else if (info.CodeText[i] == '[')
                                if (stack.Count > 0)
                                    stack.Pop();

                            if (stack.Count == 0)
                            {
                                subscriptLength = info.CodeText.Length - i;
                                break;
                            }
                        }
                    }
                    var targetSpecifer = info.CodeText.Remove(info.CodeText.Length - subscriptLength, subscriptLength);

                    var tuple = ObtainValue.GetSpecifierDataInfo(targetSpecifer, (Tag)tag,
                        stxEditorDocument.SnippetLexer.ConnectionReference?.GetReferenceProgram() ?? parentProgram, transformTable);
                    var variableType = tag.DataTypeInfo.DataType is UserDefinedDataType
                        ? (tuple?.Item2.DataType as ArrayType)?.Type : tuple?.Item2.DataType;
                    if ((variableType is CAM_PROFILE || variableType is CAM) && tuple.Item2.Dim1 > 0)
                    {
                        var title = targetSpecifer;
                        if (transformTable != null)
                        {
                            title = $"{title} <{transformTable[targetSpecifer.ToUpper()]}>";
                        }
                        var configure = new ConfigureMenuItem(tuple.Item3 as ArrayField, info.CodeText, title,tuple.Item4);
                        _tagContextMenu.Items.Add(configure);
                    }
                }

                if (tmpTag != null)
                {
                    tag = tmpTag;
                }

                var cross = new CrossTagMenuItem(tag, info.CodeText,info.OriginalCodeText);
                var monitorTagMenuItem = new MonitorTagMenuItem(tag, info, stxEditorDocument.Options);

                var isTrendTagMenuEnable = false;
                if (!(parentProgram is AoiDefinition) && Controller.GetInstance().IsOnline)
                {
                    Tag refTag = null;
                    var value = ObtainValue.GetTagValue(info.CodeText, tag.ParentCollection.ParentProgram, null, ref refTag);
                    if (refTag == tag)
                    {
                        isTrendTagMenuEnable = !string.IsNullOrEmpty(value);
                    }
                }
                var trendTag = new TrendTag(Controller.GetInstance(),tag,info.CodeText,isTrendTagMenuEnable);
                var separator = new Separator();
                var browseMenu = new BrowseTagsMenuItem(_textEditor, cursorPoint, browseAdorner, stxEditorDocument.Options, info);
                var addStElement = new AddSTElementMenuItem(tag.ParentController, _textEditor.Document, info);
                if (parentProgram.ParentController.IsOnline && !stxEditorDocument.Options.ShowPending)
                {
                    browseMenu.IsEnabled = false;
                }
                else
                {
                    if (stxEditorDocument.Options.ShowOriginal || stxEditorDocument.Options.ShowTest || stxEditorDocument.Options.IsConnecting)
                    {
                        browseMenu.IsEnabled = false;
                    }
                }

                _tagContextMenu.Items.Add(cross);
                _tagContextMenu.Items.Add(monitorTagMenuItem);
                _tagContextMenu.Items.Add(trendTag);
                _tagContextMenu.Items.Add(separator);
                _tagContextMenu.Items.Add(browseMenu);
                _tagContextMenu.Items.Add(addStElement);
            }

            var separator2 = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, parentProgram);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, parentProgram);
            if (parentProgram.ParentController.IsOnline && !stxEditorDocument.Options.ShowPending)
            {
                cutMenu.IsEnabled = false;
                pasteMenu.IsEnabled = false;
            }
            else
            {
                if (stxEditorDocument.Options.ShowOriginal || stxEditorDocument.Options.ShowTest || stxEditorDocument.Options.IsConnecting)
                {
                    cutMenu.IsEnabled = false;
                    pasteMenu.IsEnabled = false;
                }
            }

            if (type == Type.Instr)
            {
                var separator = new Separator();
                var insertDefault = new InsertDefault(info, instr, stxEditorDocument.Document);
                var saveDefault = new SaveDefault(info, instr);
                var clearDefault = new ClearDefault(instr);
                _tagContextMenu.Items.Add(separator);
                _tagContextMenu.Items.Add(insertDefault);
                _tagContextMenu.Items.Add(saveDefault);
                _tagContextMenu.Items.Add(clearDefault);
            }

            var separator3 = new Separator();
            var goToMenu = new GoToMenuItem(_textEditor, null, info.CodeText, tag, parentProgram, null);
            var watchTagsMenu = new WatchTagsMenuItem();

            _tagContextMenu.Items.Add(separator2);
            _tagContextMenu.Items.Add(cutMenu);
            _tagContextMenu.Items.Add(copyMenu);
            _tagContextMenu.Items.Add(pasteMenu);
            _tagContextMenu.Items.Add(separator3);
            _tagContextMenu.Items.Add(goToMenu);
            _tagContextMenu.Items.Add(watchTagsMenu);

            if (type == Type.Instr && info.IsAOI)
            {
                var aoi = ((AoiDefinitionCollection) Controller.GetInstance().AOIDefinitionCollection).Find(instr?.Name);
                var separator = new Separator();
                var openLogic = new OpenLogic(info, aoi, routine);
                var openDefinition = new OpenDefinition(aoi);
                _tagContextMenu.Items.Add(separator);
                _tagContextMenu.Items.Add(openLogic);
                _tagContextMenu.Items.Add(openDefinition);
            }

            return _tagContextMenu;
        }

        public ContextMenu SetCreateNewTagContextMenu(SnippetInfo info, IRoutine routine,
            Point cursorPoint, BrowseAdorner browseAdorner, IStxEditorOptions options)
        {
            _tagContextMenu = new ContextMenu();
            if (routine.ParentCollection.ParentProgram is IAoiDefinition)
            {
                var parameter =
                    new NewAoiTagMenuItem(info, routine.ParentCollection.ParentProgram, true, _editorDocument);
                _tagContextMenu.Items.Add(parameter);
                var local = new NewAoiTagMenuItem(info, routine.ParentCollection.ParentProgram, false, _editorDocument);
                _tagContextMenu.Items.Add(local);
            }
            else
            {
                var setting = GlobalSetting.GetInstance().TagSetting.Scope;
                if ((setting is Program|| setting is AoiDefinition )&& setting!=routine.ParentCollection.ParentProgram)
                {
                    setting = routine.ParentCollection.ParentProgram;
                }
                ITagCollection tagCollection = setting == null ? Controller.GetInstance().Tags : setting.Tags;
                var createTag =
                    new CreateNewTagMenuItem(info, tagCollection, _editorDocument);
                _tagContextMenu.Items.Add(createTag);
            }

            var browseMenu = new BrowseTagsMenuItem(_textEditor, cursorPoint, browseAdorner, options, info);
            _tagContextMenu.Items.Add(browseMenu);
            var separator2 = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            if (routine.ParentController.IsOnline && !options.ShowPending)
            {
                cutMenu.IsEnabled = false;
                pasteMenu.IsEnabled = false;
                browseMenu.IsEnabled = false;
            }
            else
            {
                if (options.ShowOriginal || options.ShowTest || options.IsConnecting)
                {
                    cutMenu.IsEnabled = false;
                    pasteMenu.IsEnabled = false;
                    browseMenu.IsEnabled = false;
                }
            }

            var separator3 = new Separator();
            var goToMenu = new GoToMenuItem(_textEditor, null, info.CodeText, null,
                routine.ParentCollection.ParentProgram as IProgram, null);
            var watchTagsMenu = new WatchTagsMenuItem();

            _tagContextMenu.Items.Add(separator2);
            _tagContextMenu.Items.Add(cutMenu);
            _tagContextMenu.Items.Add(copyMenu);
            _tagContextMenu.Items.Add(pasteMenu);
            _tagContextMenu.Items.Add(separator3);
            _tagContextMenu.Items.Add(goToMenu);
            _tagContextMenu.Items.Add(watchTagsMenu);
            return _tagContextMenu;
        }

        public ContextMenu SetSTContextMenu(IRoutine routine, SnippetInfo info, Point cursorPoint,
            BrowseAdorner browseAdorner, StxEditorOptions options, StxEditorDocument document)
        {
            _stContextMenu = new ContextMenu();
            if (routine.ParentCollection.ParentProgram is IAoiDefinition)
            {
                var parameter = new NewAoiTagMenuItem(null, routine.ParentCollection.ParentProgram, true, null);
                _stContextMenu.Items.Add(parameter);
                var local = new NewAoiTagMenuItem(null, routine.ParentCollection.ParentProgram, false, null);
                _stContextMenu.Items.Add(local);
            }
            else
            {
                var setting = GlobalSetting.GetInstance().TagSetting.Scope;
                if ((setting is Program || setting is AoiDefinition) && setting != routine.ParentCollection.ParentProgram)
                {
                    setting = routine.ParentCollection.ParentProgram;
                }
                ITagCollection tagCollection = setting == null ? Controller.GetInstance().Tags : setting.Tags;

                var newTagMenu = new NewTagMenuItem(tagCollection, null);
                _stContextMenu.Items.Add(newTagMenu);
            }

            var browseMenu = new BrowseTagsMenuItem(_textEditor, cursorPoint, browseAdorner, options, info);
            var addStElementMenu = new AddSTElementMenuItem(routine.ParentController, _textEditor.Document, info);
            var separator = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            if (routine.ParentController.IsOnline && !options.ShowPending)
            {
                cutMenu.IsEnabled = false;
                pasteMenu.IsEnabled = false;
                browseMenu.IsEnabled = false;
            }
            else
            {
                if (options.ShowOriginal || options.ShowTest || options.IsConnecting)
                {
                    cutMenu.IsEnabled = false;
                    pasteMenu.IsEnabled = false;
                    browseMenu.IsEnabled = false;
                }
            }

            var separator2 = new Separator();
            var goToMenu = new GoToMenuItem(_textEditor, routine, null, null,
                routine.ParentCollection.ParentProgram as IProgram, null);
            var separator3 = new Separator();
            var closeRoutineMenu = new CloseRoutineMenuItem(routine);
            var separator4 = new Separator();
            var optionsMenu = new OptionsMenuItem();
            var propertiesMenu = new PropertiesMenuItem(routine);


            _stContextMenu.Items.Add(browseMenu);
            _stContextMenu.Items.Add(addStElementMenu);
            _stContextMenu.Items.Add(separator);
            _stContextMenu.Items.Add(cutMenu);
            _stContextMenu.Items.Add(copyMenu);
            _stContextMenu.Items.Add(pasteMenu);
            _stContextMenu.Items.Add(separator2);
            _stContextMenu.Items.Add(goToMenu);
            _stContextMenu.Items.Add(new Separator());
            _stContextMenu.Items.Add(new OnlineEditsMenuitem((STRoutine) routine, options, document));
            _stContextMenu.Items.Add(separator3);
            _stContextMenu.Items.Add(closeRoutineMenu);
            _stContextMenu.Items.Add(separator4);
            _stContextMenu.Items.Add(optionsMenu);
            _stContextMenu.Items.Add(propertiesMenu);
            return _stContextMenu;
        }

        public ContextMenu SetEmptyContextMenu(IRoutine routine, Point cursorPoint,
            BrowseAdorner browseAdorner, IStxEditorOptions options)
        {
            _emptyContextMenu = new ContextMenu();
            var info = new SnippetInfo(_textEditor.Text) {Offset = _textEditor.CaretOffset};
            var browseMenu = new BrowseTagsMenuItem(_textEditor, cursorPoint, browseAdorner, options, info);
            var separator = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            if (routine.ParentController.IsOnline && !options.ShowPending)
            {
                cutMenu.IsEnabled = false;
                pasteMenu.IsEnabled = false;
                browseMenu.IsEnabled = false;
            }
            else
            {
                if (options.ShowOriginal || options.ShowTest || options.IsConnecting)
                {
                    cutMenu.IsEnabled = false;
                    pasteMenu.IsEnabled = false;
                    browseMenu.IsEnabled = false;
                }
            }

            var separator2 = new Separator();
            var goToMenu = new GoToMenuItem(_textEditor, routine, null, null,
                routine.ParentCollection.ParentProgram as IProgram, null);
            var watchMenu = new WatchTagsMenuItem();

            _emptyContextMenu.Items.Add(browseMenu);
            _emptyContextMenu.Items.Add(separator);
            _emptyContextMenu.Items.Add(cutMenu);
            _emptyContextMenu.Items.Add(copyMenu);
            _emptyContextMenu.Items.Add(pasteMenu);
            _emptyContextMenu.Items.Add(separator2);
            _emptyContextMenu.Items.Add(goToMenu);
            _emptyContextMenu.Items.Add(watchMenu);
            return _emptyContextMenu;
        }

        public ContextMenu SetProgramContextMenu(IProgramModule program, Point point, SnippetInfo info,
            TextEditor editor,
            IProgram parentProgram, IStxEditorOptions options)
        {
            _programContextMenu = new ContextMenu();

            var edit = new EditProgramPropertiesMenuItem(program);
            var find = new FindAllProgramMenuItem(program);
            var cross = new CrossProgramMenuItem(program);
            var separator = new Separator();
            var browse = new BrowseProgramMenuItem(program, editor, point, info);
            var separator2 = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, program);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, program);
            if (parentProgram.ParentController.IsOnline && !options.ShowPending)
            {
                cutMenu.IsEnabled = false;
                pasteMenu.IsEnabled = false;
            }
            else
            {
                if (options.ShowOriginal || options.ShowTest || options.IsConnecting)
                {
                    cutMenu.IsEnabled = false;
                    pasteMenu.IsEnabled = false;
                }
            }

            var separator3 = new Separator();
            var goToMenu = new GoToMenuItem(_textEditor, null, program.Name, null, parentProgram, program);
            var watchMenu = new WatchTagsMenuItem();

            _programContextMenu.Items.Add(edit);
            _programContextMenu.Items.Add(find);
            _programContextMenu.Items.Add(cross);
            _programContextMenu.Items.Add(separator);
            _programContextMenu.Items.Add(browse);
            _programContextMenu.Items.Add(separator2);
            _programContextMenu.Items.Add(cutMenu);
            _programContextMenu.Items.Add(copyMenu);
            _programContextMenu.Items.Add(pasteMenu);
            _programContextMenu.Items.Add(separator3);
            _programContextMenu.Items.Add(goToMenu);
            _programContextMenu.Items.Add(watchMenu);
            return _programContextMenu;
        }

        public ContextMenu SetRoutineContextMenu(string name,STRoutine routine, Point cursorPoint, BrowseRoutinesAdorner browseAdorner, SnippetInfo info)
        {
            var routineContextMenu=new ContextMenu();
            var target = routine.ParentCollection[name];
            if (target == null)
            {
                var newRoutineMenu = new NewRoutineMenuItem(name, routine.ParentCollection.ParentProgram);

                routineContextMenu.Items.Add(newRoutineMenu);
            }
            else
            {
                var editRoutineMenu = new EditRoutineMenuItem(target);
                var findAllMenu = new FindAllMenu(name);
                var crossReferenceMenu = new CrossReferenceRoutineMenuItem(name, routine);
                var openMenu = new OpenRoutineMenuItem(name, routine);

                routineContextMenu.Items.Add(editRoutineMenu);
                routineContextMenu.Items.Add(findAllMenu);
                routineContextMenu.Items.Add(crossReferenceMenu);
                routineContextMenu.Items.Add(openMenu);
            }

            var separator = new Separator();
            var browseMenu = new BrowseRoutineMenuItem(_textEditor, cursorPoint, browseAdorner, info);

            var separator2 = new Separator();
            var cutMenu = new CutMenuItem(_textEditor, routine.ParentCollection.ParentProgram);
            var copyMenu = new CopyMenuItem(_textEditor);
            var pasteMenu = new PasteMenuItem(_textEditor, routine.ParentCollection.ParentProgram);

            var separator3 = new Separator();
            var goToMenu = new GoToMenuItem(_textEditor, routine, null, null,
                routine.ParentCollection.ParentProgram as IProgram, null);
            var watchMenu = new WatchTagsMenuItem();

            routineContextMenu.Items.Add(separator);
            routineContextMenu.Items.Add(browseMenu);
            routineContextMenu.Items.Add(separator2);
            routineContextMenu.Items.Add(cutMenu);
            routineContextMenu.Items.Add(copyMenu);
            routineContextMenu.Items.Add(pasteMenu);
            routineContextMenu.Items.Add(separator3);
            routineContextMenu.Items.Add(goToMenu);
            routineContextMenu.Items.Add(watchMenu);

            return routineContextMenu;
        }
        
        public static List<ParamInfo> GetInstrParameters(SnippetInfo info, ref int count,int end,STRoutine routine)
        {
            if (!routine.IsCompiling)
            {
                var variables = routine.GetCurrentVariableInfos(routine.CurrentOnlineEditType);
                if (variables?.Count > 0)
                {
                    var instrNode = variables.FirstOrDefault(v => v.IsHit(info.Offset));
                    if (instrNode != null && instrNode.IsInstr)
                    {
                        var parameterNodes = ((VariableInfo)instrNode).Parameters;
                        var parameters = new List<ParamInfo>();

                        foreach (var parameter in parameterNodes.nodes)
                        {
                            var name = info.Parent.Substring(parameter.ContextStart,
                                parameter.ContextEnd - parameter.ContextStart + 1);
                            parameters.Add(new ParamInfo(parameter.ContextStart, name));
                        }

                        return parameters;
                    }
                }
            }

            return GetInstrParametersWithoutVariables(info, ref count, end, routine);
        }

        public static List<ParamInfo> GetInstrParametersWithoutVariables(SnippetInfo info, ref int count, int end, STRoutine routine,bool isContainPlaceholder=false)
        {
            {
                var instr = Controller.GetInstance().STInstructionCollection.FindInstruction(info.CodeText);
                if(instr==null)return new List<ParamInfo>();
                Debug.Assert(instr != null, info.CodeText);
                count = GetInstrParamCount(instr);
                var parentCode = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(info.Parent, null);
                var parameters = new List<ParamInfo>();
                var param = "";
                bool startGetParam = false;
                //bool lookForComma = false;
                int i = info.EndOffset + 1;
                int offsetOfParam = i;
                for (; i < info.Parent.Length; i++)
                {
                    if (parameters.Count == count || i == end) break;
                    var c = parentCode[i];
                    if (char.IsWhiteSpace(c))
                    {
                        if (string.IsNullOrEmpty(param))
                            continue;
                        else
                        {
                            //parameters.Add(new ParamInfo(offsetOfParam - param.Length + 1, param));
                            //param = "";
                            //lookForComma = true;
                            param += c;
                            continue;
                        }
                    }

                    if (c == ',')
                    {
                        parameters.Add(new ParamInfo(offsetOfParam - param.Length + 1, param));
                        offsetOfParam = offsetOfParam + 1;
                        //lookForComma = false;
                        param = "";
                        continue;
                    }

                    if (char.IsLetter(c) || char.IsNumber(c) || c == '%' || c == '.' || c == '_' || c == '[' ||
                        c == ']')
                    {
                        //if (lookForComma) break;
                        if (!startGetParam) break;
                        offsetOfParam = i;
                        param += c;
                        continue;
                    }

                    if (c == '(')
                    {
                        if (startGetParam)
                        {
                            parameters.Add(new ParamInfo(offsetOfParam - param.Length + 1, param));
                            param = "";
                            break;
                        }

                        startGetParam = true;
                    }

                    if (c == ')')
                    {
                        parameters.Add(new ParamInfo(offsetOfParam - param.Length + 1, param));
                        param = "";
                        startGetParam = false;
                        break;
                    }
                }
                
                if (!string.IsNullOrEmpty(param))
                    parameters.Add(new ParamInfo(offsetOfParam - param.Length + 1, param));
                else
                {
                    if (isContainPlaceholder)
                    {
                        if (startGetParam)
                        {
                            parameters.Add(new ParamInfo(i, param));
                        }
                    }
                }
                return parameters;
            }
        }
        
        public static int GetInstrParamCount(IXInstruction instr)
        {
            if (instr is FixedInstruction)
            {
                return (instr as FixedInstruction).ParametersCount;
            }
            else
            {
                if (instr.Name.Equals("GSV", StringComparison.OrdinalIgnoreCase) ||
                    instr.Name.Equals("SSV", StringComparison.OrdinalIgnoreCase))
                {
                    return 4;
                }

                if (instr.Name.Equals("RET", StringComparison.OrdinalIgnoreCase) ||
                    instr.Name.Equals("SBR", StringComparison.OrdinalIgnoreCase))
                {
                    return int.MaxValue;
                }

                if (instr.Name.Equals("JSR", StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }

                //TODO(ZTL):Add other instr
            }

            //Debug.Assert(false);
            return instr.GetParameterInfo().Count;
        }
        
    }
}
