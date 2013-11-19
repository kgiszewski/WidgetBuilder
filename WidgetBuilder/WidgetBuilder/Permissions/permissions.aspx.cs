using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Web.Script.Serialization;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;

namespace WidgetBuilder
{
    public partial class PermissionsPage : System.Web.UI.Page
    {
        private JavaScriptSerializer jsonSerializer=new JavaScriptSerializer();
        private string userID = HttpContext.Current.Request.QueryString["userID"];

        protected void Page_Load(object sender, EventArgs e)
        {
            Guid propertyEditorGuid = new Guid("c324dcd5-4f2a-482a-94cd-1608a86328d6");
            WidgetBuilderUser user = new WidgetBuilderUser(Convert.ToInt32(userID));
            //Log.Add(LogTypes.Debug, 0, "widget count=>"+ user.widgetPermissions.widgets.Count.ToString());

            HtmlGenericControl dataTypeDiv, hTag, configDiv, permissionTable, tr, th, td;

            //Response.Write()

            foreach (DataTypeDefinition thisDataType in DataTypeDefinition.GetAll().Where(d => d.DataType.Id == propertyEditorGuid))
            {
               dataTypeDiv = new HtmlGenericControl("div");
               mainWrapper.Controls.Add(dataTypeDiv);
               dataTypeDiv.Attributes["class"] = "dataTypeDiv";
               dataTypeDiv.Attributes["userID"] = userID;
               dataTypeDiv.Attributes["guid"] = thisDataType.UniqueId.ToString();
               
               hTag = new HtmlGenericControl("h4");
               dataTypeDiv.Controls.Add(hTag);
               hTag.InnerHtml = "<a href='#'>" + thisDataType.Text+"</a>";

               Widget_Builder_PrevalueEditor prevalueEditor = (Widget_Builder_PrevalueEditor)thisDataType.DataType.PrevalueEditor;
               configDiv = new HtmlGenericControl("div");
               configDiv.Attributes["class"] = "configDiv";
               dataTypeDiv.Controls.Add(configDiv);

               //widget level
               permissionTable = new HtmlGenericControl("table");
               configDiv.Controls.Add(permissionTable);

               tr = new HtmlGenericControl("tr");
               permissionTable.Controls.Add(tr);

               td = new HtmlGenericControl("td");
               tr.Controls.Add(td);
               CheckBox hideWidgetForUser = new CheckBox();
               hideWidgetForUser.Attributes["class"] = "hideWidgetForUser";
               td.Controls.Add(hideWidgetForUser);

               try
               {
                   hideWidgetForUser.Checked = user.WidgetPermissions.widgets[thisDataType.UniqueId.ToString()].hide;
               }
               catch 
               {

               }
    
               th = new HtmlGenericControl("th");
               tr.Controls.Add(th);
               th.InnerHtml = "Hide All?";

               //element level
               permissionTable = new HtmlGenericControl("table");
               configDiv.Controls.Add(permissionTable);

               tr=new HtmlGenericControl("tr");
               permissionTable.Controls.Add(tr);

               th = new HtmlGenericControl("th");
               tr.Controls.Add(th);
               th.InnerHtml = "Hide?";

               th=new HtmlGenericControl("th");
               tr.Controls.Add(th);
               th.InnerHtml="Title";

               th=new HtmlGenericControl("th");
               tr.Controls.Add(th);
               th.InnerHtml="Element";

               th=new HtmlGenericControl("th");
               tr.Controls.Add(th);
               th.InnerHtml="Type";

               th = new HtmlGenericControl("th");
               tr.Controls.Add(th);
               th.InnerHtml = "Description";

               foreach (WidgetElement thisElement in prevalueEditor.Configuration.elements){
                   if (thisElement.prevalues != "")
                   {
                       try
                       {
                           //using textbox options generically since we are just needing the title/element names
                           TextboxOptions options = jsonSerializer.Deserialize<TextboxOptions>(thisElement.prevalues);

                           tr=new HtmlGenericControl("tr");
                           permissionTable.Controls.Add(tr);

                           td = new HtmlGenericControl("td");
                           tr.Controls.Add(td);

                           CheckBox hidePropertyForUser = new CheckBox();
                           hidePropertyForUser.Attributes["class"] = "hidePropertyForUser";
                           td.Controls.Add(hidePropertyForUser);
                           try
                           {
                               hidePropertyForUser.Checked = user.WidgetPermissions.widgets[thisDataType.UniqueId.ToString()].elements[options.elementName].hide;
                           }
                           catch (Exception e2)
                           {}
    
                           td=new HtmlGenericControl("td");
                           tr.Controls.Add(td);
                           td.InnerHtml=HttpUtility.UrlDecode(options.title);

                           td = new HtmlGenericControl("td");
                           tr.Controls.Add(td);
                           td.InnerHtml = HttpUtility.UrlDecode(options.elementName);

                           td = new HtmlGenericControl("td");
                           tr.Controls.Add(td);
                           td.InnerHtml = HttpUtility.UrlDecode(thisElement.type);

                           td = new HtmlGenericControl("td");
                           tr.Controls.Add(td);
                           td.InnerHtml = HttpUtility.UrlDecode(options.description);
                       }
                       catch (Exception e2)
                       {
                           Log.Add(LogTypes.Debug, 0, e2.Message);
                       }
                   }
               }

               tr = new HtmlGenericControl("tr");
               permissionTable.Controls.Add(tr);

               td = new HtmlGenericControl("td");
               tr.Controls.Add(td);
               td.Attributes["colspan"] = "4";
               td.InnerHtml = "GUID: "+thisDataType.UniqueId;
            }
        }
    }
}