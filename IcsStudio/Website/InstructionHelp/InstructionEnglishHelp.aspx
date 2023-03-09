<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InstructionEnglishHelp.aspx.cs" Inherits="InstructionHelp_InstructionHelp" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add-On Instruction Help</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Content-Language" content="en-us" />
    <link rel="stylesheet" type="text/css" href="CssAndImage/InstructionHelp.css" />
    <style type="text/css">
        .rllTable{ border-collapse: collapse;margin:0px 0px 0 20px;width: 100px;}
        .rllTable tr>th,.rllTable tr>td{ border: 0.5pt black solid;padding: 5px;font-family: arial, sans-serif;font-size: 15px}
        .rllTable tr{ background-color: rgb(242, 242, 242)}
        .rllTable tr:first-child{background-color: rgb(220,220,220)}
        .label{ display: inline;}
    </style>
</head>
<body>
<form runat="server">
    <h2><span id="lblInstrName"><%=Request.Params["name"] %> <%=Request.Params["revision"] %> <%=Request.Params["extendedText"] %></span></h2>
    <p class="vendor"><span id="AOIVendor"><%=Request.Params["vendor"] %></span></p>
    <p class="message">[Contact the Add-On Instruction developer for questions or problems with this instruction]</p>
    <p class="description"><span id="InstrDescription"><%=Request.Params["description"] %></span></p>

    <p />
    <h3>Available Languages</h3>
    <p>
        <span id="RLLExample">
            <div class="example">
                <img src="CssAndImage/LadderIcon.bmp" /><b> Relay Ladder</b><br />
                <div style="border-top: 1px black solid; width: 20px; position: relative;top: 79px"></div>
                <div style="border-top: 1px black solid; width: 20px; position: relative; top: 62px;left: 120px"></div>
                <table class="rllTable">
                    <tr>
                        <th>
                           <div style="display: inline">
                               <div style="float: left" ><%=Request.Params["name"] %></div>
                           </div>
                        </th>
                    </tr>
                    <tr>
                        <td style="border-bottom: 0px">description</td>
                    </tr>
                    <tr>
                        <td style="border-top: 0px">
                            <div style="width: 21px; height: 18px; border: 1px solid rgb(160, 160, 160); float: right; text-align: center;margin: 5px"><span style="color: black;">...</span></div>
                            <div style="width: 55px">
                                <asp:Label runat="server" ID="lblLLD" CssClass="label">
                                    <div style="float: left">A</div>
                                    <div style="float: right">?</div>
                                    <br/>
                                    <div style="float: left">B</div>
                                    <div style="float: right">?</div>
                                    <br/>
                                </asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </span>
        <span id="FBDExample">
            <div class="example">
                <img src="CssAndImage/FunctionBlockIcon.bmp" /><b> Function Block</b><br />
                <table class="rllTable" style="margin-top:20px">
                    <tr>
                        <th>
                            <div style="display: inline">
                                <div style="float: left" ><%=Request.Params["name"] %></div>
                                <div style="width: 21px; height: 18px; border: 1px solid rgb(160, 160, 160); float: right;text-align: center"><span style="color: black;">...</span></div>
                            </div>
                        </th>
                    </tr>
                    <tr>
                        <td style="border-bottom: 0px">description</td>
                    </tr>
                    <tr>
                        <td style="border-top: 0px">
                            <asp:Label runat="server" ID="lblFBD" CssClass="label">
                                <div style="float: left">A</div>
                                <div style="float: right">?</div>
                                <br/>
                                <div style="float: left">A</div>
                                <div style="float: right">?</div>
                            </asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </span>
        <span id="STXExample">
            <div class="example">
                <img src="CssAndImage/StructuredTextIcon.bmp" /><b> Structured Text</b><br />
                <span id="lblStxCode"><code style='color:black'><span style='color:blue'><%=Request.Params["name"] %></span>(<span style='color:maroon' runat="server" id="spanST"></span>);</code></span>
            </div>
        </span>
    </p>
    <p />

    <h3>Parameters</h3>
    <p>
        <table style="width:100%; ">
            <col style="width:1%; white-space:nowrap; text-align:center;" />
            <col style="width:1%; white-space:nowrap;" />
            <col style="width:1%; white-space:nowrap;" />
            <col style="width:1%; white-space:nowrap;" />
            <col />
            <tr>
                <th>Required</th>
                <th>Name</th>
                <th>Data Type</th>
                <th>Usage</th>
                <th>Description</th>
            </tr>

            <tr>
                <td>x</td>
                <td><%=Request.Params["name"] %></td>
                <td><%=Request.Params["name"] %></td>
                <td>InOut</td>
                <td></td>
            </tr>

            <tr>
                <td></td>
                <td>EnableIn</td>
                <td>BOOL</td>
                <td>Input</td>
                <td></td>
            </tr>

            <tr>
                <td></td>
                <td>EnableOut</td>
                <td>BOOL</td>
                <td>Output</td>
                <td></td>
            </tr>
            <asp:Label runat="server" ID="lblParameters"></asp:Label>
        </table>
    </p>
    <p />

    <h3>Extended Description</h3>
    <p class="comment"><span id="AOIExtendedDesc" runat="server"></span></p>
    <br />
    <p />

    <h3>Signature</h3>
    <p>
        <table style="width:100%; ">
            <col style="width:1%; white-space:nowrap; font-weight:bold;" />
            <col style="width:99%;" />
            <tr>
                <td>ID: </td>
                <td><span id="AOISignatureID" runat="server">&lt;none&gt;</span></td>
            </tr>
            <tr>
                <td>Timestamp: </td>
                <td><span id="AOISignatureTimestamp" runat="server">&lt;none&gt;</span></td>
            </tr>

            <tr>
                <td>Signature History: </td>
            </tr>
            <tr>
                <td colspan="2" style="padding-left:10px;">

                    <table style="width:100%; ">
                        <col style="width:1%; white-space:nowrap;" />
                        <col style="width:1%; white-space:nowrap;" />
                        <col style="width:1%; white-space:nowrap;" />
                        <col />
                        <tr style="font-size: 10px">
                            <th>User</th>
                            <th>ID</th>
                            <th>Timestamp</th>
                            <th>Description</th>
                        </tr>
                        <asp:Label runat="server" ID="lblHistory"></asp:Label>
                    </table>

                </td>
            </tr>
        </table>
    </p>
    <p />

    <h3>Execution</h3>
    <p class="message">[see Add-On Instruction Scan Modes online Help for more information]</p>
    <p>
        <table style="width:100%; ">
            <col style="width:1%; white-space:nowrap;" />
            <col />
            <tr>
                <th>Condition</th>
                <th>Description</th>
            </tr>

            <tr>
                <td>EnableIn is true</td>
                <td></td>
            </tr>
        </table>
    </p>
    <p />

    <h3>Revision&nbsp;<span id="AOIRevision"><%=Request.Params["revision"] %> </span> Notes</h3>
    <p class="comment"><span id="AOIRevisionNote" runat="server"></span></p>
    <p />

</form>
    
</body>
</html>