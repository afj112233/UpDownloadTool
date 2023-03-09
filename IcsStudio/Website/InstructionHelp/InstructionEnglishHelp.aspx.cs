using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class InstructionHelp_InstructionHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var parmeters = Request.Params["parameters"];
            string lldHtml = " <div style=\"float: left\">"+Request.Params["name"]+"</div><div style=\"float: right\">?</div> <br/> ";
            string fbdHtml = "";
            string stHtml = Request.Params["name"];
            if (!string.IsNullOrEmpty(parmeters))
            {
                var jArray = JArray.Parse(parmeters);
                StringBuilder parmeterhtml = new StringBuilder();
                foreach (var item in jArray)
                {
                    if (item["Name"].ToString() == "EnableIn" || item["Name"].ToString() == "EnableOut") continue;
                    parmeterhtml.Append("<tr><td></td>");
                    parmeterhtml.Append("<td>" + item["Name"] + "</td>");
                    parmeterhtml.Append("<td>" + item["DataType"] + "</td>");
                    parmeterhtml.Append("<td>" + item["Usage"] + "</td>");
                    parmeterhtml.Append("<td>" + item["Description"] + "</td>");
                    parmeterhtml.Append("</tr>");

                    if (item["Usage"].ToString() == "InOut")
                    {
                        lldHtml = lldHtml + "<div style=\"float: left\">" + item["Name"] +
                                  "</div><div style=\"float: right\">?</div> <br/>";
                        fbdHtml = fbdHtml + "<div style=\"float: left\">" + item["Name"] +
                                  "</div><div style=\"float: right\">?</div> <br/>";
                        stHtml = stHtml +","+ item["Name"];
                    }
                }

                lblParameters.Text = parmeterhtml.ToString();
            }

            lblLLD.Text = lldHtml;
            lblFBD.Text = fbdHtml;
            spanST.InnerHtml = stHtml;


            if (!string.IsNullOrEmpty(Request.Params["extended"]))
                AOIExtendedDesc.InnerHtml = Request.Params["extended"];
            if (!string.IsNullOrEmpty(Request.Params["ID"]))
                AOISignatureID.InnerHtml = Request.Params["ID"];
            if (!string.IsNullOrEmpty(Request.Params["EditedDate"]))
                AOISignatureTimestamp.InnerHtml = Request.Params["EditedDate"];

            var history = Request.Params["signature"];
            if (!string.IsNullOrEmpty(history))
            {
                var jArray = JArray.Parse(history);
                if (jArray.Count == 0)
                {
                    lblHistory.Text =
                        "<tr style=\"font-size: 10px\"><td> &lt;none&gt;</td><td></td><td></td><td></td></tr> ";
                }
                else
                {
                    StringBuilder historyHtml = new StringBuilder();
                    foreach (var item in jArray)
                    {
                        historyHtml.Append("<tr style=\"font-size: 10px\">");
                        historyHtml.Append("<td>" + item["User"] + "</td>");
                        historyHtml.Append("<td>" + item["ID"] + "</td>");
                        historyHtml.Append("<td>" + item["Time"] + "</td>");
                        historyHtml.Append("<td>" + item["Description"] + "</td>");
                        historyHtml.Append("</tr>");
                    }

                    lblHistory.Text = historyHtml.ToString();
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["revisionNote"]))
                AOIRevisionNote.InnerHtml = Request.Params["revisionNote"];
        }
    }
}