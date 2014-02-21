using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.BusinessLogic;
using umbraco.editorControls;
using umbraco.cms.businesslogic.datatype;

using Spreadsheet_Uploader;
using TextBoxList;
using ContentPicker_FE;
using uNews.GoogleMap;
using InlineImagePicker;
using DampWidgetExtension;

namespace WidgetBuilder
{
    /// <summary>
    /// This class is used for the actual datatype dataeditor, i.e. the control you will get in the content section of umbraco. 
    /// </summary>
    public class Widget_Builder_DataEditor : System.Web.UI.UpdatePanel, umbraco.interfaces.IDataEditor
    {
        private umbraco.interfaces.IData savedData;
        private Widget_Builder_Options savedOptions;
        private XmlDocument savedXML = new XmlDocument();
        private TextBox saveBox;
        private HtmlGenericControl wrapperDiv = new HtmlGenericControl();
        private WidgetBuilderUser user = new WidgetBuilderUser(umbraco.BusinessLogic.User.GetCurrent().Id);

        private JavaScriptSerializer jsonSerializer;
        private LiteralControl debug = new LiteralControl();

        private int instanceNodeID;
        private DataTypeDefinition dataTypeDefinition;
        private WidgetPermissions widgetPermission=new WidgetPermissions();

        private bool HasAdminButtons=false;

        public Widget_Builder_DataEditor(umbraco.interfaces.IData Data, Widget_Builder_Options Configuration)
        {
            //load the prevalues
            savedOptions = Configuration;

            //ini the savedData object
            savedData = Data;

            jsonSerializer = new JavaScriptSerializer();
        }

        public virtual bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        public bool ShowLabel
        {
            get { return true; }
        }

        public Control Editor { get { return this; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            instanceNodeID=((Widget_Builder_Default_Data)savedData).DataTypeDefinitionId;
            dataTypeDefinition = DataTypeDefinition.GetAll().Where(d =>d.DataType != null && d.DataType.DataTypeDefinitionId == instanceNodeID).First();

            string basicCSS = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/WidgetBuilder/WidgetBuilder.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderCSS", basicCSS, false);            

            saveBox = new TextBox();
            wrapperDiv.Controls.Add(saveBox);
            saveBox.CssClass = "widgetSaveBox";
            saveBox.TextMode = TextBoxMode.MultiLine;
            ContentTemplateContainer.Controls.Add(saveBox);           

            //set widget permissions
            try
            {
                widgetPermission = user.WidgetPermissions.widgets[dataTypeDefinition.UniqueId.ToString()];
            }
            catch (Exception e2)
            { }

            //determine if hidden
            string hideWidgetClass = "";
            if (widgetPermission.hide)
            {
                hideWidgetClass = "widgetHidden";
            }

            wrapperDiv.TagName = "div";
            wrapperDiv.Attributes["class"] = Regex.Replace(this.dataTypeDefinition.Text, @"[^a-zA-z0-9]", "") + " widgetWrapperDiv " + hideWidgetClass;
            wrapperDiv.Attributes["widgetMaxWidgets"] = savedOptions.maxWidgets.ToString();
            ContentTemplateContainer.Controls.Add(wrapperDiv);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void  OnPreRender(EventArgs e)
        {
 	         base.OnPreRender(e);
             buildControls();
        }
        
        protected void buildControls()
        {
            //string jqueryUI = string.Format("<script src=\"{0}\" ></script>", "/umbraco_client/ui/jqueryui.js");
            //ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "jqueryUI", jqueryUI, false);

            string basicJS = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/WidgetBuilder/WidgetBuilder.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderJS", basicJS, false);

            //Include the custom css\js
            string customCSS = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", savedOptions.cssIncludePath);
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderCSScustom" + savedOptions.cssIncludePath, customCSS, false);

            string customJS = string.Format("<script src=\"{0}\" ></script>", savedOptions.jsIncludePath);
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderJS2custom" + savedOptions.jsIncludePath, customJS, false);           

            string data;

            //get the data based on action
            if (Page.IsPostBack&&saveBox.Text!="")
            {
                data = saveBox.Text;
            }
            else
            {
                data = savedData.Value.ToString();
            }

            //load the data into an xml doc
            XmlDocument xd = new XmlDocument();
            
            try
            {
                xd.LoadXml(data);
            }
            catch (Exception e) {
                xd.LoadXml(Widget_Builder_Default_Data.defaultXML);
                //Log.Add(LogTypes.Custom, 0, "xml=>"+data);
                //Log.Add(LogTypes.Custom, 0, e.Message);
            }
            XmlNodeList widgetsXML=xd.SelectNodes("widgets/widget");

            //loop thru each widget
            foreach (XmlNode widgetNode in widgetsXML)
            {

                bool startCollapsed=false;
                try
                {
                     startCollapsed= Convert.ToBoolean(widgetNode.Attributes["isCollapsed"].InnerText);
                }
                catch (Exception e)
                {}
                
                HtmlGenericControl widgetFadeWrapper = new HtmlGenericControl();
                widgetFadeWrapper.TagName = "div";
                widgetFadeWrapper.Attributes["class"] = "widgetFadeWrapper";

                HtmlGenericControl widgetWrapper = new HtmlGenericControl();
                widgetWrapper.TagName = "div";
                widgetWrapper.Attributes["class"] = "widgetWrapper";

                if (startCollapsed)
                {
                    widgetWrapper.Attributes["class"] += " widgetStartCollapsed";
                }

                widgetFadeWrapper.Controls.Add(widgetWrapper);

                //add buttons if more than 1 is allowed
                if (savedOptions.maxWidgets > 1)
                {
                    addButtons(widgetWrapper);
                    HasAdminButtons = true;
                }

                bool hasDamp = Widget_Builder.HasDamp;

                HtmlGenericControl widgetElementsDiv = new HtmlGenericControl("div");
                widgetElementsDiv.Attributes["class"]=(HasAdminButtons)?"WidgetRepeatable":"WidgetNonRepeatable";
                widgetWrapper.Controls.Add(widgetElementsDiv);

                //loop through the schema
                foreach (WidgetElement schemaElement in savedOptions.elements)
                {
                    //call each method dynamically based on the element type
                    if (schemaElement.type == "damp" && !hasDamp) continue;                   
                    
                    MethodInfo elementMethod = this.GetType().GetMethod(schemaElement.type);
                    elementMethod.Invoke(this, new object[] { widgetElementsDiv, schemaElement, widgetNode });
                }

                wrapperDiv.Controls.Add(widgetFadeWrapper);
            }
        }

        protected HtmlGenericControl buildControlWrapper(HtmlGenericControl widgetElementsDiv, string elementName, string title, string description, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //set widget element permissions
            WidgetElementPermissions widgetElementPermission = new WidgetElementPermissions();
            try
            {
                widgetElementPermission = user.WidgetPermissions.widgets[dataTypeDefinition.UniqueId.ToString()].elements[elementName];
            }
            catch (Exception e)
            { }

            //determine if hidden
            string hideWidgetElementClass = "";
            if (widgetElementPermission.hide)
            {
                hideWidgetElementClass = "widgetHidden";
            }

            //create outer wrapper
            HtmlGenericControl elementWrapper = new HtmlGenericControl();
            elementWrapper.TagName = "div";
            elementWrapper.Attributes["class"] = "widgetElementWrapper " + hideWidgetElementClass;
            widgetElementsDiv.Controls.Add(elementWrapper);

            elementWrapper.Attributes["elementName"] = elementName;
            elementWrapper.Attributes["type"] = schemaElement.type;

            //create label wrapper
            HtmlGenericControl labelWrapper = new HtmlGenericControl();
            labelWrapper.TagName = "div";
            labelWrapper.Attributes["class"] = "widgetLabelWrapper";
            
            labelWrapper.InnerHtml = HttpUtility.UrlDecode(title)+"<div><small>"+NewLineToBreak(HttpUtility.UrlDecode(description))+"</small></div>";
            elementWrapper.Controls.Add(labelWrapper);

            //create control wrapper
            HtmlGenericControl controlWrapper = new HtmlGenericControl();
            controlWrapper.TagName = "div";
            controlWrapper.Attributes["class"] = "widgetControlWrapper";
            elementWrapper.Controls.Add(controlWrapper);

            return controlWrapper;
        }

        public static string NewLineToBreak(string input)
        {
            Regex regEx = new Regex(@"[\n|\r]+");
            return regEx.Replace(input, "<br />");
        }

        protected void addButtons(HtmlGenericControl widgetWrapper)
        {
            HtmlGenericControl buttonsDiv = new HtmlGenericControl();
            buttonsDiv.TagName = "div";
            buttonsDiv.Attributes["class"] = "widgetButtons";

            string buttons="";
            buttons += "<img class='widgetSort' src='/umbraco/plugins/WidgetBuilder/images/sort.png' title='Sort Widget' alt='Remove Widget'/>";
            buttons += "<img class='widgetAdd' src='/umbraco/plugins/WidgetBuilder/images/plus.png' title='Add Widget' alt='Add Widget'/>";
            buttons += "<img class='widgetCollapse' src='/umbraco/plugins/WidgetBuilder/images/close.png' title='Expand/Collapse' alt='Expand/Collapse'/>";
            buttons += "<img class='widgetRemove' src='/umbraco/plugins/WidgetBuilder/images/minus.png' title='Remove Widget' alt='Remove Widget'/>";
            buttonsDiv.InnerHtml = buttons;

            widgetWrapper.Controls.Add(buttonsDiv);
        }

        #region elements

        //implements each method named based on element type

        public void textbox(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {

            //deserialize element prevalues
            TextboxOptions prevalues = jsonSerializer.Deserialize<TextboxOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            TextBox textbox = new TextBox();
            textbox.CssClass = "widgetTextBox " + prevalues.className;
            controlWrapper.Controls.Add(textbox);

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                textbox.Text = HttpUtility.HtmlDecode(element.InnerText);
            }
            catch (Exception e) { }
        }

        public void textarea(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            TextareaOptions prevalues = jsonSerializer.Deserialize<TextareaOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            TextBox textbox = new TextBox();
            textbox.TextMode = TextBoxMode.MultiLine;
            textbox.CssClass = "widgetTextarea " + prevalues.className;
            controlWrapper.Controls.Add(textbox);

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                textbox.Text = HttpUtility.HtmlDecode(element.InnerXml.Replace("<br />", "\n"));
            }
            catch (Exception e) {
                Log.Add(LogTypes.Custom, 0, e.Message);            
            }
        }

        public void tinymce(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //tinyMCE library load
            string tinyMCE = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/tinymce3/tinymce3tinymceCompress.aspx?rnd=913ab2c2-c264-4abb-b92d-f3b9702bc32f&amp;module=gzipmodule&amp;themes=umbraco&amp;plugins=contextmenu,umbracomacro,umbracoembed,noneditable,inlinepopups,table,advlink,paste,spellchecker,umbracoimg,umbracocss,umbracopaste,umbracolink,umbracocontextmenu&amp;languages=en");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderTinyMceJS", tinyMCE, false);                      

            //deserialize element prevalues
            TinyMceOptions prevalues = jsonSerializer.Deserialize<TinyMceOptions>(schemaElement.prevalues);

            string tinyMceJS = string.Format("<script src=\"{0}\" ></script>", prevalues.JSpath);
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderTinyMceJS2" + prevalues.JSpath, tinyMceJS, false);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            TextBox textbox = new TextBox();
            textbox.TextMode = TextBoxMode.MultiLine;
            textbox.CssClass = "widgetTinyMCE " + prevalues.className;
            controlWrapper.Controls.Add(textbox);

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                textbox.Text = HttpUtility.HtmlDecode(element.InnerXml.Replace("<![CDATA[", "").Replace("]]>", "").Replace("<br />", "\n"));
            }
            catch (Exception e)
            {
                Log.Add(LogTypes.Custom, 0, e.Message);
            }
        }

        public void spreadsheet(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            if (!Widget_Builder.HasSpreadsheet)
            {
                return;
            }

            //deserialize element prevalues
            SpreadsheetOptions prevalues = jsonSerializer.Deserialize<SpreadsheetOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            DataInterface dataExtractor = new DataInterface();

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                dataExtractor.Value = element.OuterXml;
            }
            catch (Exception e)
            {
                Log.Add(LogTypes.Custom, 0, "WidgetBuilder=>"+e.Message);
                dataExtractor.Value = "<table class=''><thead></thead><tbody></tbody></table>";
            }

            SpreadsheetDataEditor spreadSheet = new SpreadsheetDataEditor(dataExtractor, HttpUtility.UrlDecode(prevalues.classes) + "|");

            HtmlGenericControl spreadsheetWrapper = new HtmlGenericControl();
            spreadsheetWrapper.Controls.Add(spreadSheet);
            spreadsheetWrapper.TagName = "div";
            spreadsheetWrapper.Attributes["class"] = "widgetSpreadsheet " + prevalues.className;
            controlWrapper.Controls.Add(spreadSheet);
        }

        public void mediapicker(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            MediaPickerOptions prevalues = jsonSerializer.Deserialize<MediaPickerOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            //include media picker JS
            string mediaPickerJS = string.Format("<script src=\"{0}\" ></script>", "/umbraco/controls/Images/ImageViewer.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderMediaPicker", mediaPickerJS, false);

            DataInterface dataExtractor = new DataInterface();

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                dataExtractor.Value = element.InnerText;
            }
            catch (Exception e) {
                dataExtractor.Value = "";
            }
            mediaChooser mediaChooser = new mediaChooser(dataExtractor, true, true);

            HtmlGenericControl mediaWrapper = new HtmlGenericControl();
            mediaWrapper.Controls.Add(mediaChooser);
            mediaWrapper.TagName = "div";
            mediaWrapper.Attributes["class"] = "widgetMediaPicker " + prevalues.className;

            controlWrapper.Controls.Add(mediaChooser);
        }

        public void damp(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            if (!Widget_Builder.HasDamp)
            {
                return;
            }

            //deserialize element prevalues
            DampOptions prevalues = jsonSerializer.Deserialize<DampOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            //include media picker JS
            string dampJS = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/DigibizAdvancedMediaPicker/DAMPScript.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderDamp", dampJS, false);

            Damp_Data_Editor_Extension damp = new Damp_Data_Editor_Extension();

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                //Log.Add(LogTypes.Debug, 0, "sab=>" + saveBox.Text);
                //Log.Add(LogTypes.Debug, 0, "outer=>"+element.OuterXml);
                damp._value = element.InnerText;
            }
            catch (Exception e){
                //Log.Add(LogTypes.Error, 0, e.Message);
            }

            damp.SelectMultipleNodesValue = true;
            damp.HidePixlrValue = prevalues.hidePixlr;
            damp.HideOpenValue = prevalues.hideOpen;
            damp.HideCreateValue = prevalues.hideCreate;
            damp.HideEditValue = prevalues.hideEdit;
            damp.ThumbnailWidthValue = 50;
            damp.MaximumNodesValue = -1;
            damp.MinimumNodesValue = -1;
            damp.AllowedExtensionsValue = prevalues.allowedExtensions;
            damp.AllowedCreateableMediaTypesValue = prevalues.createableMediaNodes;
            damp.AllowedSelectableMediaTypesValue = prevalues.selectableMediaNodes;
            damp.DefaultMediaTypeValue = prevalues.defaultMediaNodeID;
            damp.SelectMultipleNodesValue = prevalues.allowMultiple;

            damp.EnableSearch = prevalues.enableSearch;
            damp.EnableSearchAutoSuggest = prevalues.enableSearchAutoSuggest;
            damp.SearchMethod = prevalues.searchMethod;

            if (prevalues.startNodeID != "")
            {
                damp.StartNodeIdValue = Convert.ToInt32(prevalues.startNodeID);
            }
            damp.CropPropertyAliasValue = prevalues.imageCropperAlias;
            
            HtmlGenericControl dampWrapper = new HtmlGenericControl();
            dampWrapper.Controls.Add(damp);
            dampWrapper.TagName = "div";
            dampWrapper.Attributes["class"] = "widgetDamp " + prevalues.className;

            controlWrapper.Controls.Add(dampWrapper);
        }

        public void list(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            ListOptions prevalues = jsonSerializer.Deserialize<ListOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            DataInterface dataExtractor = new DataInterface();

            TextBoxListOptions listOptions = new TextBoxListOptions();
            listOptions.indentedLimit = prevalues.maxIndents.ToString();
            TextBoxList_DataEditor list = new TextBoxList_DataEditor(dataExtractor, listOptions);

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                dataExtractor.Value = element.InnerXml;
                list.currentData = element.InnerXml;
            }
            catch (Exception e)
            {
                dataExtractor.Value = "<list><item indent='0'/></list>";
            }

            HtmlGenericControl listWrapper = new HtmlGenericControl();
            listWrapper.Controls.Add(list);
            listWrapper.TagName = "div";
            listWrapper.Attributes["class"] = "widgetList " + prevalues.className;

            controlWrapper.Controls.Add(listWrapper);
        }

        public void dropdown(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            DropdownOptions prevalues = jsonSerializer.Deserialize<DropdownOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            string selectedValue = "";
            
            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                selectedValue = HttpUtility.UrlDecode(element.InnerText);
            }
            catch (Exception e)
            {
                
            }

            DropDownList ddl = new DropDownList();
            ddl.CssClass = "widgetDropdown " + prevalues.className;

            if (prevalues.useSelect)
            {
                System.Web.UI.WebControls.ListItem selectOption = new System.Web.UI.WebControls.ListItem("- Select -", "");
                ddl.Items.Add(selectOption);
            }

            foreach(ListItem item in prevalues.items){
                System.Web.UI.WebControls.ListItem listoption = new System.Web.UI.WebControls.ListItem(HttpUtility.UrlDecode(item.display), HttpUtility.UrlDecode(item.value));
                ddl.Items.Add(listoption);
                if (HttpUtility.UrlDecode(item.value) == selectedValue)
                {
                    listoption.Selected = true;
                }
            }

            controlWrapper.Controls.Add(ddl);
        }

        public void checkradio(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            CheckRadioOptions prevalues = jsonSerializer.Deserialize<CheckRadioOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            List<string> selectedValues = new List<string> { };

            //get the value from the XML
            bool anySelected = false;
            try
            {
                XmlNodeList values = widgetNode.SelectNodes (prevalues.elementName+"/value");
                foreach (XmlNode thisValue in values)
                {
                    selectedValues.Add(HttpUtility.HtmlDecode(thisValue.InnerText));
                    anySelected = true;
                }

            }
            catch (Exception e)
            {
                //Log.Add(LogTypes.Debug, 0, e.Message);
            }

            if (prevalues.isCheckbox)
            {
                CheckBoxList cbl = new CheckBoxList();

                cbl.CssClass = "widgetCheckBox " + prevalues.className;

                foreach (ListItem item in prevalues.items)
                {
                    System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(item.display)), HttpUtility.UrlDecode(item.value));
                    if (selectedValues.Any(x => x == HttpUtility.HtmlDecode(item.value)))
                    {
                        listItem.Selected = true;
                    }
                    cbl.Items.Add(listItem);
                }

                //default to the first selected
                if (!anySelected && prevalues.defaultChecked)
                {
                    foreach(System.Web.UI.WebControls.ListItem item in cbl.Items){
                        item.Selected=true;
                    }
                }

                controlWrapper.Controls.Add(cbl);
            }
            else
            {
                RadioButtonList rbl = new RadioButtonList();

                rbl.CssClass = "widgetRadio " + prevalues.className;

                foreach (ListItem item in prevalues.items)
                {
                    System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(item.display)), HttpUtility.UrlDecode(item.value));
                    if (selectedValues.Any(x => x == HttpUtility.HtmlDecode(item.value)))
                    {
                        listItem.Selected = true;
                        anySelected = true;
                    }
                    rbl.Items.Add(listItem);
                }

                //default to the first selected
                if (!anySelected && prevalues.defaultChecked)
                {
                    rbl.Items[0].Selected = true;
                }

                controlWrapper.Controls.Add(rbl);
            }
        }

        public void contentpicker(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            ContentPickerOptions prevalues = jsonSerializer.Deserialize<ContentPickerOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            DataInterface dataExtractor = new DataInterface();
            dataExtractor.Value = "";

            //get the value from the XML

            XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
            ContentPicker_DataEditor contentPicker = new ContentPicker_DataEditor(dataExtractor, new ContentPicker_Options() {startNodeID=prevalues.startNodeID, allowedDocTypeIDs=prevalues.allowedDocTypeIDs, allowMultiple=prevalues.allowMultiple, showAllDocTypes=prevalues.showAllDocTypes, jsPath=prevalues.jsPath, cssPath=prevalues.cssPath});

            try
            {
                contentPicker.currentData = element.InnerXml;
            }
            catch (Exception e)
            {
                contentPicker.currentData = "<contentPicker/>";
            }

            HtmlGenericControl mediaWrapper = new HtmlGenericControl();
            mediaWrapper.Controls.Add(contentPicker);
            mediaWrapper.TagName = "div";
            mediaWrapper.Attributes["class"] = "widgeContentPicker " + prevalues.className;

            controlWrapper.Controls.Add(contentPicker);

        }

        public void map(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            MapOptions prevalues = jsonSerializer.Deserialize<MapOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            DataInterface dataExtractor = new DataInterface();
            dataExtractor.Value = "";

            XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
            uNews.GoogleMap.DataEditor googleMap = new uNews.GoogleMap.DataEditor(dataExtractor, new uNews.GoogleMap.Options() { lat = prevalues.lat,  lng=prevalues.lng, address=prevalues.address, zoom=prevalues.zoom});
            
            try
            {
                googleMap.CurrentData = element.OuterXml;
            }
            catch (Exception e)
            {
                googleMap.CurrentData = uNews.GoogleMap.DefaultData.defaultXML;
            }

            controlWrapper.Controls.Add(googleMap);
        }

        public void inlinepicker(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {
            //deserialize element prevalues
            InlineOptions prevalues = jsonSerializer.Deserialize<InlineOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            DataInterface dataExtractor = new DataInterface();
            dataExtractor.Value = "";

            XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
            //uNews.GoogleMap.DataEditor googleMap = new uNews.GoogleMap.DataEditor(dataExtractor, new uNews.GoogleMap.Options() { lat = prevalues.lat, lng = prevalues.lng, address = prevalues.address, zoom = prevalues.zoom });
            InlineImagePicker.DataEditor inlinePicker =new InlineImagePicker.DataEditor(dataExtractor,new InlineImagePicker.Options(){mediaIDs=prevalues.mediaIDs});
            
            try
            {
                inlinePicker.CurrentData = element.OuterXml;
            }
            catch (Exception e)
            {
                inlinePicker.CurrentData = InlineImagePicker.DefaultData.defaultXML;
            }

            controlWrapper.Controls.Add(inlinePicker);
        }

        public void datepicker(HtmlGenericControl widgetElementsDiv, WidgetElement schemaElement, XmlNode widgetNode)
        {

            //deserialize element prevalues
            DatePickerOptions prevalues = jsonSerializer.Deserialize<DatePickerOptions>(schemaElement.prevalues);

            HtmlGenericControl controlWrapper = buildControlWrapper(widgetElementsDiv, prevalues.elementName, prevalues.title, prevalues.description, schemaElement, widgetNode);

            TextBox textbox = new TextBox();
            textbox.CssClass = "widgetDatePicker " + prevalues.className;
            controlWrapper.Controls.Add(textbox);

            string datePickerJS = string.Format("<script src=\"{0}\" ></script>", prevalues.JSpath);
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_DataEditor), "WidgetBuilderDatePickerJS" + prevalues.JSpath, datePickerJS, false);

            //get the value from the XML
            try
            {
                XmlNode element = widgetNode.SelectSingleNode(prevalues.elementName);
                textbox.Text = HttpUtility.HtmlDecode(element.InnerText);
            }
            catch (Exception e) { }
        }

        #endregion

        public void Save()
        {
            //Log.Add(LogTypes.Custom, 0, "savebox at time of save=>" + saveBox.Text);

            //prevent the user from saving if this property is hidden 
            if (!widgetPermission.hide)
            {
                //Log.Add(LogTypes.Debug, 0, "Allowed to save box=>"+dataTypeDefinition.UniqueId);
                restoreRestrictedValues();

                //save it
                savedData.Value = saveBox.Text;
            }
            else
            {
                Log.Add(LogTypes.Custom, 0, "Denied=>" + dataTypeDefinition.UniqueId);
            }
        }

        private void restoreRestrictedValues()
        {
            //check to see if this user is restricted for an element
            if (widgetPermission.elements.Count > 0)
            {
                foreach (KeyValuePair<string, WidgetElementPermissions> thisElement in widgetPermission.elements)
                {
                    //Log.Add(LogTypes.Debug, 0, "element=>"+thisElement.Key);
                    if (((WidgetElementPermissions)thisElement.Value).hide)
                    {
                        //switch out the current with the original value only for the propery in question
                        try
                        {
                            //get original element data
                            XmlDocument originalData = new XmlDocument();
                            originalData.LoadXml(savedData.Value.ToString());

                            XmlNodeList originalElements = originalData.SelectNodes("//" + thisElement.Key);
                            List<XmlNode> originalElementList = new List<XmlNode>();
                            foreach (XmlNode thisOE in originalElements)
                            {
                                originalElementList.Add(thisOE);
                                //Log.Add(LogTypes.Debug, 0, "oe=>" + thisOE.OuterXml);
                            }

                            //get incoming data
                            XmlDocument newData = new XmlDocument();
                            newData.LoadXml(saveBox.Text);

                            XmlNodeList newElements = newData.SelectNodes("//" + thisElement.Key);
                            List<XmlNode> newElementList = new List<XmlNode>();
                            foreach (XmlNode thisNE in newElements)
                            {
                                newElementList.Add(thisNE);
                                //Log.Add(LogTypes.Debug, 0, "ne=>" + thisNE.OuterXml);
                            }

                            //remove the old
                            int count = 0;
                            foreach (XmlNode thisNE in newElements)
                            {
                                XmlNode importNode = newData.ImportNode(originalElementList[count], true);
                                newElementList[count].ParentNode.AppendChild(importNode);
                                newElementList[count].ParentNode.RemoveChild(newElementList[count]);
                                count++;
                            }

                            //Log.Add(LogTypes.Debug, 0, "final=>" + newData.OuterXml);
                            saveBox.Text = newData.OuterXml;
                        }
                        catch (Exception e)
                        { }
                    }
                }
            }
        }
    }
}