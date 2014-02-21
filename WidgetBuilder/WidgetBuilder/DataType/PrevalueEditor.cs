using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Xml;
using System.Linq;

using System.Collections.Generic;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using umbraco.interfaces;

using System.Reflection;

using System.Web.Script.Serialization;

using WidgetBuilder;

namespace WidgetBuilder
{
    /// <summary>
    /// This class is used to setup the datatype settings. 
    /// On save it will store these values (using the datalayer) in the database
    /// </summary>
    public class Widget_Builder_PrevalueEditor : System.Web.UI.UpdatePanel, IDataPrevalue
    {
        // referenced datatype
        private umbraco.cms.businesslogic.datatype.BaseDataType _datatype;

        private TextBox saveBox;
        private LiteralControl debug = new LiteralControl();

        private JavaScriptSerializer jsonSerializer;
        private Widget_Builder_Options savedOptions;

        public Widget_Builder_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
        {
            _datatype = DataType;
            jsonSerializer = new JavaScriptSerializer();
            savedOptions = Configuration;
        }

        public Control Editor
        {
            get
            {
                return this;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            saveBox = new TextBox();
            saveBox.CssClass = "widgetSaveBox";
            saveBox.TextMode = TextBoxMode.MultiLine;
            ContentTemplateContainer.Controls.Add(saveBox);

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/WidgetBuilder/WidgetBuilder_Prevalue.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_PrevalueEditor), "WidgetBuilderPrevalueCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/WidgetBuilder/WidgetBuilder_Prevalue.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Widget_Builder_PrevalueEditor), "WidgetBuilderPrevalueJS", js, false);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            Widget_Builder_Options renderingOptions;

            //test for postback, decide to use db or saveBox for rendering
            if (Page.IsPostBack)
            {
                //test for saveBox having a value, default if not
                if (saveBox.Text != "")
                {
                    renderingOptions = jsonSerializer.Deserialize<Widget_Builder_Options>(saveBox.Text);
                }
                else
                {
                    renderingOptions = new Widget_Builder_Options();
                }
            }
            else
            {
                renderingOptions = savedOptions;
            }

            renderToolbar(writer);

            renderGrid(writer, renderingOptions);

            renderGlobalOptions(writer, renderingOptions);

            debug.RenderControl(writer);
        }

        protected void renderToolbar(HtmlTextWriter writer)
        {
            LiteralControl toolbar = new LiteralControl();
            toolbar.Text =  "<div class='widgetToolbarWrapper'>";
            toolbar.Text += "<div><a href='#' type='textbox'><img src='/umbraco/plugins/WidgetBuilder/images/textbox.png' alt='' title='Textbox'/><span>Textbox</span></a></div>";
            toolbar.Text += "<div><a href='#' type='textarea'><img src='/umbraco/plugins/WidgetBuilder/images/textarea.png' alt='' title='Textarea'/><span>Textarea</span></a></div>";
            toolbar.Text += "<div><a href='#' type='tinymce'><img src='/umbraco/plugins/WidgetBuilder/images/tinymce.png' alt='' title='TinyMCE'/><span>TinyMCE</span></a></div>";
            toolbar.Text += "<div><a href='#' type='list'><img src='/umbraco/plugins/WidgetBuilder/images/list.png' alt='' title='List'/><span>List</span></a></div>";

            if (Widget_Builder.HasSpreadsheet)
            {
                toolbar.Text += "<div><a href='#' type='spreadsheet'><img src='/umbraco/plugins/WidgetBuilder/images/spreadsheet.png' alt='' title='Spreadsheet'/><span>Spreadsheet</span></a></div>";
            }
            
            toolbar.Text += "<div><a href='#' type='mediapicker'><img src='/umbraco/plugins/WidgetBuilder/images/mediapicker.png' alt='' title='MediaPicker'/><span>Media Picker</span></a></div>";
            toolbar.Text += "<div><a href='#' type='checkradio'><img src='/umbraco/plugins/WidgetBuilder/images/checkradio.png' alt='' title='Checkbox/Radio'/><span>Check/Radio</span></a></div>";
            toolbar.Text += "<div><a href='#' type='dropdown'><img src='/umbraco/plugins/WidgetBuilder/images/dropdown.png' alt='' title='Dropdown'/><span>Dropdown</span></a></div>";
            toolbar.Text += "<div><a href='#' type='contentpicker'><img  src='/umbraco/plugins/WidgetBuilder/images/contentpicker.png' alt='' title='ContentPicker'/><span>Content Picker</span></a></div>";
            
            if (Widget_Builder.HasDamp)
            {
                toolbar.Text += "<div><a href='#' type='damp'><img src='/umbraco/plugins/WidgetBuilder/images/damp.png' alt='' title='DAMP'/><span>DAMP</span></a></div>";
            }

            toolbar.Text += "<div><a href='#' type='map'><img src='/umbraco/plugins/WidgetBuilder/images/map.png' alt='' title='DAMP'/><span>Map</span></a></div>";
            toolbar.Text += "<div><a href='#' type='inlinepicker'><img src='/umbraco/plugins/WidgetBuilder/images/inlinepicker.png' alt='' title='Inline Image Picker'/><span>Inline Image Picker</span></a></div>";

            //toolbar.Text += "<div><a href='#' type='datepicker'><img src='/umbraco/plugins/WidgetBuilder/images/datepicker.png' alt='' title='Date Picker'/><span>Date Picker</span></a></div>";
            
            toolbar.Text += "</div>";
            toolbar.Text += "<div><a class='json' href='#'>Toggle JSON (Advanced Only)</a></div>";

            toolbar.RenderControl(writer);
        }

        protected void renderGrid(HtmlTextWriter writer, Widget_Builder_Options renderingOptions)
        {
            
            HtmlGenericControl gridWrapper=new HtmlGenericControl();
            gridWrapper.TagName = "div";
            gridWrapper.Attributes["class"]="gridWrapperDiv";

            HtmlTable gridTable;
            
            foreach (WidgetElement element in renderingOptions.elements)
            {
                gridTable = new HtmlTable();
                gridTable.Attributes["class"] = "widgetGridTable";
                gridTable.Attributes["type"] = element.type;
                gridWrapper.Controls.Add(gridTable);

                //call each method dynamically based on the element type
                MethodInfo elementMethod = this.GetType().GetMethod(element.type);
                elementMethod.Invoke(this, new object[] { element, gridTable });
            }
            gridWrapper.RenderControl(writer);
        }

        protected void renderGlobalOptions(HtmlTextWriter writer, Widget_Builder_Options renderingOptions)
        {
            HtmlTable globalOptionsTable = new HtmlTable();
            HtmlTableRow tr;
            HtmlTableCell td;
            TextBox textBox;

            tr = new HtmlTableRow();
            globalOptionsTable.Rows.Add(tr);

                //max widgets
                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.InnerText = "Max Widgets Per Property";

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                textBox = new TextBox();
                textBox.CssClass = "maxWidgets";
                textBox.Text = renderingOptions.maxWidgets.ToString();
                td.Controls.Add(textBox);

            tr = new HtmlTableRow();
            globalOptionsTable.Rows.Add(tr);

                //js include
                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.InnerText = "JS Include Path";

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                textBox = new TextBox();
                textBox.CssClass = "jsInclude";
                textBox.Text = renderingOptions.jsIncludePath;
                td.Controls.Add(textBox);

            tr = new HtmlTableRow();
            globalOptionsTable.Rows.Add(tr);

                //css include
                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.InnerText = "CSS Include Path";

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                textBox = new TextBox();
                textBox.CssClass = "cssInclude";
                textBox.Text = renderingOptions.cssIncludePath;
                td.Controls.Add(textBox);


            globalOptionsTable.RenderControl(writer);
        }

        #region elements

        //implement a method that is named by the element type

        public void textbox(WidgetElement element, HtmlTable gridTable)
        {
            //Log.Add(LogTypes.Debug, 0, "here");
            //deserialize the prevalues into options
            TextboxOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<TextboxOptions>(element.prevalues);
            }
            else
            {
                options = new TextboxOptions();
            }

            HtmlTableRow tr=createGridTableRow(gridTable, element.type, "Textbox");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            /////////////////////////

            addButtons(tr);
        }

        public void textarea(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            TextareaOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<TextareaOptions>(element.prevalues);
            }
            else
            {
                options = new TextareaOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Textarea");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            /////////////////////////

            addButtons(tr);
        }

        public void tinymce(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            TinyMceOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<TinyMceOptions>(element.prevalues);
            }
            else
            {
                options = new TinyMceOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "TinyMCE");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);
            addTextBoxOption(prevalueTable, "Path to TinyMCE Options", options.JSpath, "widgetJsPath");
            /////////////////////////

            addButtons(tr);
        }

        public void list(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            ListOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<ListOptions>(element.prevalues);
            }
            else
            {
                options = new ListOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "List");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);
            addTextBoxOption(prevalueTable, "Max Indents", options.maxIndents.ToString(), "widgetMaxIndents");

            /////////////////////////

            addButtons(tr);
        }

        public void spreadsheet(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            SpreadsheetOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<SpreadsheetOptions>(element.prevalues);
            }
            else
            {
                options = new SpreadsheetOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Spreadsheet");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);
            addTextBoxOption(prevalueTable, "Classes (CSV)", options.classes, "widgetClasses");

            /////////////////////////

            addButtons(tr);
        }

        public void mediapicker(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            MediaPickerOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<MediaPickerOptions>(element.prevalues);
            }
            else
            {
                options = new MediaPickerOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Media Picker");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            /////////////////////////

            addButtons(tr);
        }

        public void damp(WidgetElement element, HtmlTable gridTable)
        {
            if (!Widget_Builder.HasDamp)
            {
                return;
            }

            //deserialize the prevalues into options
            DampOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<DampOptions>(element.prevalues);
            }
            else
            {
                options = new DampOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "DAMP");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);
            addTextBoxOption(prevalueTable, "Extensions (CSV)", options.allowedExtensions, "widgetFileExtensions");
            addTextBoxOption(prevalueTable, "Selectable Media Node ID's (CSV)", options.selectableMediaNodes, "widgetSelectableNodes");
            addTextBoxOption(prevalueTable, "Createable Media Node ID's (CSV)", options.createableMediaNodes, "widgetCreateableNodes");
            addTextBoxOption(prevalueTable, "Default Media Type Node ID", options.defaultMediaNodeID.ToString(), "widgetDefaultNodeID");
            addTextBoxOption(prevalueTable, "Media Tree Start Node ID", options.startNodeID, "widgetStartNodeID");
            addCheckBoxOption(prevalueTable, "Allow Multiple?", options.allowMultiple, "widgetAllowMultiple");

            addCheckBoxOption(prevalueTable, "Hide Create?", options.hideCreate, "widgetHideCreate");
            addCheckBoxOption(prevalueTable, "Hide Edit?", options.hideEdit, "widgetHideEdit");
            addCheckBoxOption(prevalueTable, "Hide Open?", options.hideOpen, "widgetHideOpen");
            addCheckBoxOption(prevalueTable, "Hide Pixlr?", options.hidePixlr, "widgetHidePixlr");
            addTextBoxOption(prevalueTable, "Image Cropper Property Alias", options.imageCropperAlias, "widgetCropperAlias");

            addCheckBoxOption(prevalueTable, "Enable Search?", options.enableSearch, "widgetEnableSearch");
            addCheckBoxOption(prevalueTable, "Enable Search Auto Suggest?", options.enableSearchAutoSuggest, "widgetEnableSearchAutoSuggest");
            addRadioButtonList(prevalueTable, "Search Under Selected Node?", "all,selected", options.searchMethod, "widgetSearchMethod");
            
            /////////////////////////

            addButtons(tr);
        }

        public void dropdown(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            DropdownOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<DropdownOptions>(element.prevalues);
            }
            else
            {
                options = new DropdownOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Dropdown");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);
            addCheckBoxOption(prevalueTable, "Use 'Select'", options.useSelect, "widgetUseSelect");
            addListOption(prevalueTable, "Options", options.items, "widgetListTable");

            /////////////////////////

            addButtons(tr);
        }

        public void checkradio(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            CheckRadioOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<CheckRadioOptions>(element.prevalues);
            }
            else
            {
                options = new CheckRadioOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Check\\Radio");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);
            addCheckBoxOption(prevalueTable, "Render as checkbox?", options.isCheckbox, "widgetIsCheckbox");
            addCheckBoxOption(prevalueTable, "Check by default?", options.defaultChecked, "widgetDefaultChecked");
            addListOption(prevalueTable, "Options", options.items, "widgetListTable");

            /////////////////////////

            addButtons(tr);
        }

        public void contentpicker(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            ContentPickerOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<ContentPickerOptions>(element.prevalues);
            }
            else
            {
                options = new ContentPickerOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Content Picker");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            addTextBoxOption(prevalueTable, "Start Node ID", options.startNodeID.ToString(), "widgetStartNodeID");
            addTextBoxOption(prevalueTable, "Allowed DocTypeIDs (CSV)", options.allowedDocTypeIDs, "widgetDocTypeIDs");
            addCheckBoxOption(prevalueTable, "Allow Multiple?", options.allowMultiple, "widgetAllowMultiple");
            addCheckBoxOption(prevalueTable, "Show All Doc Types?", options.showAllDocTypes, "widgetShowAllDocTypes");

            addTextBoxOption(prevalueTable, "JS Path", options.jsPath, "widgetCpJsPath");
            addTextBoxOption(prevalueTable, "CSS Path", options.cssPath, "widgetCpCssPath");

            /////////////////////////

            addButtons(tr);
        }

        public void map(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            MapOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<MapOptions>(element.prevalues);
            }
            else
            {
                options = new MapOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Map");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            addTextBoxOption(prevalueTable, "Lat", options.lat, "widgetLat");
            addTextBoxOption(prevalueTable, "Lng", options.lng, "widgetLng");
            addTextBoxOption(prevalueTable, "Zoom", options.zoom.ToString(), "widgetZoom");
            addTextBoxOption(prevalueTable, "Address", options.address, "widgetAddress");

            /////////////////////////

            addButtons(tr);
        }

        public void inlinepicker(WidgetElement element, HtmlTable gridTable)
        {
            //deserialize the prevalues into options
            InlineOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<InlineOptions>(element.prevalues);
            }
            else
            {
                options = new InlineOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Inline Image Picker");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            addTextBoxOption(prevalueTable, "Media IDs (CSV)", options.mediaIDs, "widgetMediaIDs");

            /////////////////////////

            addButtons(tr);
        }

        public void datepicker(WidgetElement element, HtmlTable gridTable)
        {
            //Log.Add(LogTypes.Debug, 0, "here");
            //deserialize the prevalues into options
            DatePickerOptions options;
            if (element.prevalues != "")
            {
                options = jsonSerializer.Deserialize<DatePickerOptions>(element.prevalues);
            }
            else
            {
                options = new DatePickerOptions();
            }

            HtmlTableRow tr = createGridTableRow(gridTable, element.type, "Date Picker");

            /////////////////////////
            //prevalues
            HtmlTable prevalueTable = createPrevalueTable(tr);

            addTextBoxOption(prevalueTable, "Title", options.title, "widgetTitle");
            addTextBoxOption(prevalueTable, "Element Name", options.elementName, "widgetElementName");
            addTextBoxOption(prevalueTable, "Class", options.className, "widgetClass");
            addTextBoxOption(prevalueTable, "Description", options.description, "widgetDescription", true);

            addTextBoxOption(prevalueTable, "Path to Date Picker Options", options.JSpath, "widgetJsPath");

            /////////////////////////

            addButtons(tr);
        }

        #endregion

        #region helpers

        protected HtmlTable createPrevalueTable(HtmlTableRow tr)
        {
            HtmlTableCell td = new HtmlTableCell();
            td.Attributes["class"] = "widgetPrevalues";
            tr.Cells.Add(td);

            HtmlTable prevalueTable = new HtmlTable();
            td.Controls.Add(prevalueTable);

            return prevalueTable;
        }

        protected HtmlTableRow createGridTableRow(HtmlTable gridTable, string elementType, string label)
        {
            HtmlTableRow tr = new HtmlTableRow();
            gridTable.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);
            td.InnerHtml = "<img class='gridImage' src='/umbraco/plugins/WidgetBuilder/images/" + elementType + ".png' alt='' title=''/><span>"+label+"</span>";
            return tr;
        }

        protected void addTextBoxOption(HtmlTable prevalueTable, string title, string boxValue, string className, bool allowMultiLine=false, string titleAttribute="test")
        {
            HtmlTableRow tr = new HtmlTableRow();
            prevalueTable.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);
            td.InnerText = title;

            td = new HtmlTableCell();
            tr.Cells.Add(td);

            TextBox textBox = new TextBox();
            textBox.Attributes["title"] = titleAttribute;
            textBox.TextMode = (allowMultiLine)?TextBoxMode.MultiLine:TextBoxMode.SingleLine;
            textBox.CssClass = className;
            textBox.Text = HttpUtility.UrlDecode(boxValue);
            td.Controls.Add(textBox);
        }

        protected void addCheckBoxOption(HtmlTable prevalueTable, string title, bool isChecked, string className)
        {
            HtmlTableRow tr = new HtmlTableRow();
            prevalueTable.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);
            td.InnerText = title;

            td = new HtmlTableCell();
            tr.Cells.Add(td);

            CheckBox checkbox = new CheckBox();
            checkbox.CssClass = className;
            checkbox.Checked = isChecked;
            td.Controls.Add(checkbox);
        }

        protected void addRadioButtonList(HtmlTable prevalueTable, string title, string strCheckBoxList, string checkedRadio, string className)
        {
            HtmlTableRow tr = new HtmlTableRow();
            prevalueTable.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);
            td.InnerText = title;

            td = new HtmlTableCell();
            tr.Cells.Add(td);

            
            foreach(string radioButtonText in strCheckBoxList.Split(',')){
                RadioButton rb = new RadioButton();
                rb.Text = radioButtonText;
                rb.ID = radioButtonText;
                rb.GroupName = className;
                rb.CssClass = className;

                if (radioButtonText == checkedRadio)
                {
                    rb.Checked = true;
                }
                td.Controls.Add(rb);

            }


          
        }


        protected void addListOption(HtmlTable prevalueTable, string title, List<ListItem> list, string className)
        {
            HtmlTableRow tr = new HtmlTableRow();
            prevalueTable.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);
            td.InnerText = title;

            td = new HtmlTableCell();
            tr.Cells.Add(td);

            HtmlTable ListTable = new HtmlTable();
            ListTable.Attributes["class"] = className;

            HtmlTableRow listTR;
            HtmlTableCell listTD;
            TextBox textbox;

            listTR = new HtmlTableRow();
            ListTable.Rows.Add(listTR);
            listTR.Attributes["class"] = "widgetUnsortable";

            listTD = new HtmlTableCell();
            listTR.Cells.Add(listTD);
            listTD.InnerText = "Value";

            listTD = new HtmlTableCell();
            listTR.Cells.Add(listTD);
            listTD.InnerText = "Display";

            foreach (ListItem item in list)
            {
                listTR = new HtmlTableRow();
                ListTable.Rows.Add(listTR);

                listTD = new HtmlTableCell();
                listTR.Cells.Add(listTD);
                textbox = new TextBox();
                textbox.Text = HttpUtility.UrlDecode(item.value);
                listTD.Controls.Add(textbox);

                listTD = new HtmlTableCell();
                listTR.Cells.Add(listTD);
                textbox = new TextBox();
                textbox.Text = HttpUtility.UrlDecode(item.display);
                listTD.Controls.Add(textbox);

                //buttons
                listTD = new HtmlTableCell();
                listTD.Attributes["class"] = "listButtons";
                listTR.Cells.Add(listTD);

                string buttons = "";
                buttons += "<img class='listAdd' src='/umbraco/plugins/WidgetBuilder/images/plus.png' alt='' title='Add'/>";
                buttons += "<img class='listRemove' src='/umbraco/plugins/WidgetBuilder/images/minus.png' alt='' title='Remove'/>";
                buttons += "<img class='sortWidgetListTable' src='/umbraco/plugins/WidgetBuilder/images/sort.png' alt='' title='Sort'/>";
                listTD.InnerHtml = buttons;
            }

            td.Controls.Add(ListTable);
        }
             
        protected void addButtons(HtmlTableRow tr)
        {
            HtmlTableCell td = new HtmlTableCell();
            td.Attributes["class"] = "widgetButtons";
            tr.Cells.Add(td);

            string buttons="";
            buttons += "<img class='removeWidgetElement' src='/umbraco/plugins/WidgetBuilder/images/minus.png' alt='Remove Element' title='Remove Element'/>";
            buttons += "<img class='sortWidgetElement' src='/umbraco/plugins/WidgetBuilder/images/sort.png' alt='Sort Element' title='Sort Element'/>";
            td.InnerHtml = buttons;
        }

        #endregion

        public void Save()
        {
            _datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), DBTypes.Ntext.ToString(), true);

            SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId));
            SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId), SqlHelper.CreateParameter("@value", saveBox.Text));
        }

        public Widget_Builder_Options Configuration
        {
            get
            {
                string dbValue = "";
                try
                {
                    object conf = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
                    dbValue = conf.ToString();
                    //Log.Add(LogTypes.Debug, 0, conf.ToString());
                }
                catch (Exception e)
                {
                }

                if (dbValue.ToString() != "")
                {
                    return jsonSerializer.Deserialize<Widget_Builder_Options>(dbValue.ToString());
                }
                else
                {
                    return new Widget_Builder_Options();
                }
            }
        }

        public static ISqlHelper SqlHelper
        {
            get
            {
                return Application.SqlHelper;
            }
        }
    }
}