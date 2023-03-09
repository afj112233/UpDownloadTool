using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.FBD.Model;
using ICSStudio.FBD.View;
using ICSStudio.FBD.ViewModel;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.EditorPackage.FBDEditor
{
    public class FBDEditorViewModel : ViewModelBase, IEditorPane
    {
        public FBDEditorViewModel(IRoutine routine, UserControl userControl)
        {
            Routine = routine as FBDRoutine;
            Contract.Assert(Routine != null);

            Control = userControl;
            userControl.DataContext = this;

            InitializingFBDControl();
        }

        private void InitializingFBDControl()
        {
            //TODO(gjc): only for sheet 1
            var sheetModels = ConfigToSheetModel(Routine.config);
            if (sheetModels == null)
                return;

            SheetViewModel viewModel = new SheetViewModel(sheetModels[0]);
            var stylesManager = viewModel.GraphStylesManager;

            DiagramEditor bottomEditor = new DiagramEditor() { DataContext = viewModel };
            BottomControl = bottomEditor;

            DiagramEditor topEditor = new DiagramEditor() { DataContext = viewModel };
            TopControl = topEditor;

            stylesManager.DiagramControl = bottomEditor.DiagramControl;
            viewModel.LoadChartData();
        }

        private List<SheetModel> ConfigToSheetModel(JObject config)
        {
            if (config == null)
                return null;

            List<SheetModel> result = new List<SheetModel>();

            JArray sheets = config["Sheets"] as JArray;
            if (sheets != null)
            {
                foreach (var sheet in sheets)
                {
                    SheetModel sheetModel = new SheetModel();

                    sheetModel.Description = (string)sheet["Description"];

                    //int number = (int)sheet["Number"];
                    JArray blocks = sheet["Blocks"] as JArray;

                    if (blocks != null)
                    {
                        foreach (var block in blocks)
                        {
                            string blockType = (string)block["Class"];

                            switch (blockType)
                            {
                                case "IRef":
                                    IRefData irefData = new IRefData();
                                    irefData.ID = (ulong)block["ID"];
                                    irefData.Operand = (string)block["Operand"];
                                    irefData.X = (ulong)block["X"];
                                    irefData.Y = (ulong)block["Y"];
                                    sheetModel.IRefCollection.Add(irefData);
                                    break;

                                case "ORef":
                                    ORefData orefData = new ORefData();
                                    orefData.ID = (ulong)block["ID"];
                                    orefData.Operand = (string)block["Operand"];
                                    orefData.X = (ulong)block["X"];
                                    orefData.Y = (ulong)block["Y"];
                                    sheetModel.ORefCollection.Add(orefData);
                                    break;

                                case "ICon":
                                    IConData iconData = new IConData();
                                    iconData.ID = (ulong)block["ID"];
                                    iconData.Name = (string)block["Name"];
                                    iconData.X = (ulong)block["X"];
                                    iconData.Y = (ulong)block["Y"];
                                    sheetModel.IConCollection.Add(iconData);
                                    break;

                                case "OCon":
                                    OConData oconData = new OConData();
                                    oconData.ID = (ulong)block["ID"];
                                    oconData.Name = (string)block["Name"];
                                    oconData.X = (ulong)block["X"];
                                    oconData.Y = (ulong)block["Y"];
                                    sheetModel.OConCollection.Add(oconData);
                                    break;

                                case "Block":
                                    BlockData blockData = new BlockData();
                                    blockData.ID = (ulong)block["ID"];
                                    blockData.Operand = (string)block["Operand"];
                                    blockData.Type = (string)block["Type"];
                                    blockData.VisiblePins = (string)block["VisiblePins"];
                                    blockData.X = (ulong)block["X"];
                                    blockData.Y = (ulong)block["Y"];
                                    sheetModel.BlockCollection.Add(blockData);
                                    break;

                                case "Wire":
                                    WireData wireData = new WireData();
                                    wireData.FromID = (ulong)block["FromID"];
                                    wireData.ToID = (ulong)block["ToID"];
                                    wireData.ToParam = (string)block["ToParam"];
                                    wireData.FromParam = (string)block["FromParam"];
                                    sheetModel.WireCollection.Add(wireData);
                                    break;

                                case "FeedbackWire":
                                    FeedbackWireData feedbackWireData = new FeedbackWireData();
                                    feedbackWireData.FromID = (ulong)block["FromID"];
                                    feedbackWireData.ToID = (ulong)block["ToID"];
                                    feedbackWireData.ToParam = (string)block["ToParam"];
                                    feedbackWireData.FromParam = (string)block["FromParam"];
                                    sheetModel.FeedbackWireCollection.Add(feedbackWireData);
                                    break;

                                case "AddOnInstruction":
                                    throw new NotImplementedException();

                                case "JSR":
                                    throw new NotImplementedException();

                                case "SBR":
                                    throw new NotImplementedException();

                                case "RET":
                                    throw new NotImplementedException();

                                default:
                                    throw new NotSupportedException();
                            }

                        }
                    }

                    result.Add(sheetModel);
                }
            }

            if (result.Count == 0)
                result.Add(new SheetModel());

            return result;
        }

        public FBDRoutine Routine { get; }

        public string Caption => $"{Routine.ParentCollection.ParentProgram?.Name} - {Routine.Name}";
        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public object TopControl { get; private set; }
        public object BottomControl { get; private set; }
    }
}
