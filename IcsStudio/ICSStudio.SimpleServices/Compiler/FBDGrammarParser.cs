using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSStudio.SimpleServices.Tags;
using Xunit;

namespace ICSStudio.SimpleServices.Compiler
{
    public abstract class IFBDBlock
    {
        protected IFBDBlock(int id)
        {
            this.id = id;
        }
        public int id { get; }
    }

    internal class FBDIRefBlock : IFBDBlock
    {
        public string operand { get; }
        public ASTFBDIRef expr { get; set; }
        public FBDIRefBlock(int id, string operand) : base(id)
        {
            this.operand = operand;
        }
    }

    public class FBDORefBlock : IFBDBlock
    {
        public string operand { get; }
        public IFBDBlock from { get; set; }
        public string src_member { get; set; }
        public FBDORefBlock(int id, string operand) : base(id)
        {
            this.operand = operand;
        }
    }

    public class FBDIcon : IFBDBlock
    {
        public string name { get; }
        public List<int> to_ids { get; } = new List<int>();

        public FBDIcon(int id, string name) : base(id)
        {
            this.name = name;
        }
    }

    public class FBDOcon : IFBDBlock
    {
        public string name { get; }
        public int from_id { get; set; } = -1;
        public string from_member { get; set; }
        public FBDOcon(int id, string name) : base(id)
        {
            this.name = name;
        }
    }

    public abstract class FBDInstr : IFBDBlock
    {
        protected FBDInstr(int id) : base(id)
        {

        }
        public virtual string name()
        {
            throw new NotImplementedException();
        }
        public List<Tuple<IFBDBlock, string, string>> inputs = new List<Tuple<IFBDBlock, string, string>>();
        public string operand { get; protected set; }

        //for DFS VISIT
        public int order { get; set; } = -1;
        public bool is_visited { get; set; } = false;
    }

    public class FBDBlock : FBDInstr
    {
        public string type { get; }
        //from block, from member, to member
        public FBDBlock(int id, string type, string operand) : base(id)
        {
            this.type = type;
            this.operand = operand;
        }

        public override string name() 
        {
            throw new NotImplementedException();
        }
    }

    public class FBDAOI : FBDInstr
    {
        public List<Tuple<string, string>> parameters;
        private string _name;

        public FBDAOI(string name, int id, string operand, List<Tuple<string, string>> parameters) : base(id)
        {
            this._name = name;
            this.operand = operand;
            this.parameters = parameters;
        }

        public override string name()
        {
            return _name;
        }
    }

    class FBDConnector
    {
        public FBDOcon input { get; }
        public List<IFBDBlock> outputs { get; } = new List<IFBDBlock>();

        public FBDConnector(FBDOcon input)
        {
            Debug.Assert(input != null);
            this.input = input;
        }
    }

    class Wire
    {
        public int from { get; }
        public int to { get; }
        public string from_param { get; }
        public string to_param { get; }
        public bool is_feedback { get; }
        public Wire(int from, string from_param, int to, string to_param, bool is_feedback = false)
        {
            this.from = from;
            this.from_param = from_param;
            this.to = to;
            this.to_param = to_param;
            this.is_feedback = is_feedback;
        }
    }

    public class FBDGrammarParser
    {
        class GraphData
        {
            public List<IFBDBlock> blocks { get; } = new List<IFBDBlock>();
            public List<Wire> wires { get; } = new List<Wire>();
            public int order { get; set; }
        }

        static void DFS(GraphData graph, FBDInstr block)
        {
            List<Wire> wires = graph.wires;
            List<IFBDBlock> blocks = graph.blocks; 
            Debug.Assert(block != null);
            if (block.is_visited)
                return;

            block.is_visited = true;
            var id = block.id;
            foreach (var wire in wires)
            {
                if (!wire.is_feedback && wire.to == id)
                {
                    var to_block = blocks[wire.from];
                    if (to_block is FBDOcon)
                    {
                        var output = to_block as FBDOcon;
                        var oblock = blocks[output.from_id] as FBDInstr;
                        if (oblock != null)
                        {
                            DFS(graph, oblock);
                        }
                    }
                    else if (to_block is FBDInstr)
                    {
                        DFS(graph, to_block as FBDInstr);
                    }
                }
                else if (wire.is_feedback && wire.from == id)
                {
                    var from_block = blocks[wire.to];
                    if (from_block is FBDIcon)
                    {
                        var input = from_block as FBDIcon;
                        var iblocks = input.to_ids;
                        foreach (var b in iblocks)
                        {
                            DFS(graph, blocks[id] as FBDInstr);
                        }
                        continue;
                    }
                    else if (from_block is FBDInstr)
                    {
                        DFS(graph, from_block as FBDInstr);
                    }
                }
            }

            block.order = graph.order;
            graph.order -= 1;
        }

        static void GetGraphFromSheet(JObject sheet, GraphData graph)
        {
            var blocks = graph.blocks;
            var wires = graph.wires;
            foreach (var block in sheet["Blocks"])
            {
                var klass = (string)block["Class"];
                switch (klass)
                {
                    case "IRef":
                        blocks.Add(new FBDIRefBlock((int)block["ID"], (string)block["Operand"]));
                        break;
                    case "ORef":
                        blocks.Add(new FBDORefBlock((int)block["ID"], (string)block["Operand"]));
                        break;
                    case "ICon":
                        blocks.Add(new FBDIcon((int)block["ID"], (string)block["Name"]));
                        break;
                    case "OCon":
                        blocks.Add(new FBDOcon((int)block["ID"], (string)block["Name"]));
                        break;
                    case "Block":
                        blocks.Add(new FBDBlock((int)block["ID"], (string)block["Type"], (string)block["Operand"]));
                        break;
                    case "AddOnInstruction":
                        var parameters = new List<Tuple<string, string>>();
                        foreach (var parameter in block["Parameters"])
                        {
                            parameters.Add(Tuple.Create((string)parameter["Name"], (string)parameter["Argument"]));
                        }
                        blocks.Add(new FBDAOI((string)block["Name"], (int)block["ID"], (string)block["Operand"], parameters));
                        break;
                    case "Wire":
                        wires.Add(new Wire((int)block["FromID"], (string)block["FromParam"], (int)block["ToID"], (string)block["ToParam"]));
                        break;
                    case "FeedbackWire":
                        wires.Add(new Wire((int)block["FromID"], (string)block["FromParam"], (int)block["ToID"], (string)block["ToParam"], true));
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        static Tuple<IFBDBlock, string> FindSourceInfo(Wire wire, List<IFBDBlock> blocks, Dictionary<string, FBDConnector> connectors)
        {
            int from_id = wire.from;
            var from_param = wire.from_param;

            var from_block = blocks[from_id];
            if (from_block is FBDInstr)
            {
                return Tuple.Create(from_block, from_param);
            }
            else if (from_block is FBDIRefBlock)
            {
                return Tuple.Create(from_block, null as string);
            }
            else if (from_block is FBDIcon)
            {
                var name = (from_block as FBDIcon).name;
                var input_id = connectors[name].input.from_id;
                var input_block = blocks[input_id];
                if (input_block is FBDInstr)
                {
                    return Tuple.Create(input_block, connectors[name].input.from_member);
                }
                else if (input_block is FBDIRefBlock)
                {
                    return Tuple.Create(input_block, null as string);
                }

                Debug.Assert(false);
            }

            Debug.Assert(false);

            return Tuple.Create(null as IFBDBlock, null as string);
        }

        static public ASTNode Parse(JObject config)
        {
            var graph = new GraphData();
            foreach (JObject sheet in config["Sheets"])
            {
                GetGraphFromSheet(sheet, graph);
            }

            //FIXME REMOVE THIS
            var blocks = graph.blocks;
            var wires = graph.wires;

            Debug.Assert(Enumerable.Range(0, blocks.Count).All(i => blocks[i].id == i));

            foreach (var wire in wires)
            {
                int from = wire.from;
                int to = wire.to;
                var from_block = blocks[from] as FBDIcon;
                if (from_block != null)
                {
                    from_block.to_ids.Add(to);
                }

                var to_block = blocks[to] as FBDOcon;
                if (to_block != null)
                {
                    Debug.Assert(to_block.from_id == -1);
                    to_block.from_id = from;
                    to_block.from_member = wire.from_param;
                }
            }

            Debug.Assert(blocks.OfType<FBDOcon>().All(x => x.from_id != -1));
            Debug.Assert(blocks.OfType<FBDIcon>().All(x => x.to_ids.Count >= 1));

            var connectors = new Dictionary<string, FBDConnector>();

            foreach (var block in blocks.OfType<FBDOcon>())
            {
                connectors[block.name] = new FBDConnector(block);
            }

            foreach (var block in blocks.OfType<FBDIcon>())
            {
                connectors[block.name].outputs.Add(block);
            }

            //找到输入信息
            foreach (var wire in wires)
            {
                var output = blocks[wire.to];
                if (!(output is FBDInstr || output is FBDORefBlock))
                {
                    continue;
                }

                var info = FindSourceInfo(wire, blocks, connectors);
                if (output is FBDORefBlock)
                {
                    var block = output as FBDORefBlock;
                    block.from = info.Item1;
                    block.src_member = info.Item2;
                    Debug.Assert(block.from.id != -1);
                } else if (output is FBDInstr)
                {
                    var block = output as FBDInstr;
                    Debug.Assert(wire.to_param != null);
                    block.inputs.Add(Tuple.Create(blocks[info.Item1.id], info.Item2, wire.to_param));
                }
            }

            var instrs = blocks.OfType<FBDInstr>().ToList();
            instrs.ForEach(instr => instr.is_visited = false);
            graph.order = instrs.Count;
            instrs.ForEach(instr => DFS(graph, instr));
            Debug.Assert(graph.order == 0, graph.order.ToString());

            //加载输入
            var inputs = new ASTNodeList();
            foreach (var block in blocks.OfType<FBDIRefBlock>())
            {
                var operand = STASTGenVisitor.ParseExpr(block.operand);
                block.expr = new ASTFBDIRef(operand);
                inputs.AddNode(block.expr);
            }

            //加载功能块
            var instructions = new ASTNodeList();
            foreach (var instr in instrs)
            {
                //对于大部分指令都是只需填充连接，同时给EnableIn对应的值
                //对于某些指令(比如带有输入输出参数的AOI),则需要填充对应的输入输出参数
                //这个信息怎么表述?
                var parameters = new ASTNodeList();
                foreach (var input in instr.inputs)
                {
                    var block = input.Item1;
                    if (block is FBDInstr)
                    {                      
                        parameters.AddNode(new ASTAssignStmt(new ASTName(instr.operand, input.Item3), new ASTName((block as FBDInstr).operand, input.Item2)));
                    } else if (block is FBDIRefBlock)
                    {
                        parameters.AddNode(new ASTAssignStmt(new ASTName(instr.operand, input.Item3), new ASTTempValue((block as FBDIRefBlock).expr.slot)));
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }

                if (instr.inputs.Find(p => p.Item3.Equals("EnableIn")) == null)
                {
                    parameters.AddNode(new ASTAssignStmt(new ASTName(instr.operand, "EnableIn"), new ASTInteger(1)));
                }

                var arguments = new ASTNodeList();
                arguments.AddNode(STASTGenVisitor.ParseExpr(instr.operand));
                if (instr is FBDAOI)
                {
                    var aoi = instr as FBDAOI;
                    foreach (var argument in (instr as FBDAOI).parameters)
                    {
                        arguments.AddNode((STASTGenVisitor.ParseExpr(argument.Item2) as ASTName));
                    }

                    instructions.AddNode(new ASTFBDInstruction(true, aoi.name(), parameters, arguments));
                } else if (instr is FBDBlock)
                {
                    var block = instr as FBDBlock;
                    instructions.AddNode(new ASTFBDInstruction(false, block.type, parameters, arguments));
                }
            }

            //加载输出
            var outputs = new ASTNodeList();
            foreach (var block in blocks.OfType<FBDORefBlock>())
            {
                var operand = STASTGenVisitor.ParseExpr(block.operand) as ASTName;
                Debug.Assert(operand != null);

                var input = block.from;
                if (input is FBDInstr)
                {
                    var from_block = input as FBDInstr;
                    outputs.AddNode(new ASTAssignStmt(STASTGenVisitor.ParseExpr(block.operand) as ASTName, new ASTName(from_block.operand, block.src_member)));
                }
                else if (input is FBDIRefBlock)
                {
                    outputs.AddNode(new ASTAssignStmt(STASTGenVisitor.ParseExpr(block.operand) as ASTName, new ASTTempValue((input as FBDIRefBlock).expr.slot)));
                }
                else
                {
                    Debug.Assert(false);
                }
                
            }

            var res = new ASTNodeList();
            res.AddNode(inputs);
            res.AddNode(instructions);
            res.AddNode(outputs);

            return new ASTFBDRoutine(res);
        }
    }
}