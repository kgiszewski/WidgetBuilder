using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WidgetBuilder
{
    public class Widget_Builder_Options
    {
        public List<WidgetElement> elements = new List<WidgetElement>() { };
        public int maxWidgets = 10;
        public string jsIncludePath = "/umbraco/plugins/WidgetBuilder/custom.js";
        public string cssIncludePath = "/umbraco/plugins/WidgetBuilder/custom.css";
    }

    public class WidgetElement
    {
        public string type { get; set; }
        public string prevalues { get; set; }
    }

    public class TextboxOptions
    {
        public string title = "Textbox";
        public string elementName = "";
        public string className = "widgetTextbox";
        public string description = "";
    }

    public class TextareaOptions
    {
        public string title = "Textarea";
        public string elementName = "";
        public string className = "widgetTextarea";
        public string description = "";
    }

    public class TinyMceOptions
    {
        public string title = "TinyMCE";
        public string elementName = "";
        public string className = "widgetTinyMCE";
        public string description = "";
        public string JSpath = "/umbraco/plugins/WidgetBuilder/tinyMceDefault.js";
    }

    public class ListOptions
    {
        public string title = "List";
        public string elementName = "";
        public string className = "widgetList";
        public string description = "";

        public int maxIndents = 1;
    }

    public class SpreadsheetOptions
    {
        public string title = "Spreadsheet";
        public string elementName = "";
        public string className = "widgetSpreadsheet";
        public string description = "";

        public string classes = "spreadsheet";
    }

    public class MediaPickerOptions
    {
        public string title = "Media Picker";
        public string elementName = "";
        public string className = "widgetMediaPicker";
        public string description = "";
    }

    public class CheckRadioOptions
    {
        public string title = "Check/Radio";
        public string elementName = "";
        public string className = "widgetCheckRadio";
        public string description = "";

        public bool isCheckbox = true;
        public bool defaultChecked = false;
        public List<ListItem> items = new List<ListItem>() { new ListItem() };
    }

    public class DropdownOptions
    {
        public string title = "Dropdown";
        public string elementName = "";
        public string className = "widgetDropdown";
        public string description = "";
        public bool useSelect = true;
        
        public List<ListItem> items = new List<ListItem>(){new ListItem()};
    }

    public class ContentPickerOptions
    {
        public string title = "Content Picker";
        public string elementName = "";
        public string className = "widgetContentPicker";
        public string description = "";

        public int startNodeID = -1;
        public string allowedDocTypeIDs = "";
        public bool allowMultiple = true;
        public bool showAllDocTypes = true;
        public string jsPath = "";
        public string cssPath = "";
    }
    
    public class DampOptions
    {
        public string title = "DAMP";
        public string elementName = "";
        public string className = "widgetDamp";
        public string description = "";

        public string allowedExtensions = "";
        public string selectableMediaNodes = "";
        public string createableMediaNodes = "";
        public string startNodeID = "";
        public int defaultMediaNodeID;
        public bool allowMultiple = true;
        public string imageCropperAlias = "";
        public bool hideCreate = false;
        public bool hideEdit = false;
        public bool hideOpen = true;
        public bool hidePixlr = true;
        public bool enableSearch = true;
        public bool enableSearchAutoSuggest = true;
        public string searchMethod = "all";

    }

    public class MapOptions
    {
        public string title = "Map";
        public string elementName = "";
        public string className = "widgetMap";
        public string description = "";

        public string lat = "40.718119";
        public string lng = "-74.004135";
        public int zoom = 16;
        public string address = "Times Square, New York, NY";
    }

    public class InlineOptions
    {
        public string title = "Inline Image Picker";
        public string elementName = "";
        public string className = "widgetInlineImagePicker";
        public string description = "";

        public string mediaIDs = "";
    }

    public class DatePickerOptions
    {
        public string title = "Date Picker";
        public string elementName = "";
        public string className = "widgetDatePicker";
        public string description = "";

        public string JSpath = "/umbraco/plugins/WidgetBuilder/datepicker.js";
    }

    //helper class for lists
    public class ListItem
    {
        public string value {get; set;}
        public string display {get; set;}
    }
}