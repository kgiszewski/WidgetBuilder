using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using System.Xml;
using System.Xml.Serialization;

namespace WidgetBuilder
{
    public class WidgetBuilderUser:User 
    {

        private XmlDocument config=new XmlDocument();
        private int userID;
        private WidgetUserPermissions widgetPermissions;

        public WidgetUserPermissions WidgetPermissions
        {
            get
            {
                return widgetPermissions;
            }
        }

        public WidgetBuilderUser(int userID):base(userID){
            
            this.userID = userID;

            config.Load(HttpContext.Current.Server.MapPath(WidgetBuilderCore.ConfigFilename));

            widgetPermissions=getUserWidgetPermissions();
        }

        public void hideWidgetByUserID(bool hide, string widgetGUID)
        {

            XmlNode userNode = getUserPermissionNode();
            XmlNode widgetNode = getUserWidget(userNode, widgetGUID);
            widgetNode.Attributes["hide"].Value = hide.ToString();
            saveWidgetBuilderConfig();
        }

        public void hideWidgetElementByUserID(bool hide, string widgetGUID, string element){

            XmlNode userNode=getUserPermissionNode();
            XmlNode widgetNode = getUserWidget(userNode, widgetGUID);
            XmlNode elementNode = getUserWidgetElement(widgetNode, element);
            elementNode.Attributes["hide"].Value = hide.ToString();
            saveWidgetBuilderConfig();            
        }

        private XmlNode getUsersPermissionNode()
        {
            return config.SelectSingleNode("//users");
        }

        private XmlNode getUserPermissionNode()
        {
            XmlNode userNode=config.SelectSingleNode("//user[@id='"+userID+"']");
            if (userNode == null)
            {
                userNode = config.CreateNode(XmlNodeType.Element, "user", null);
                getUsersPermissionNode().AppendChild(userNode);

                XmlAttribute userIDattribute = config.CreateAttribute("id");
                userNode.Attributes.Append(userIDattribute);

                userIDattribute.Value = userID.ToString();
            }
            return userNode;
        }

        private XmlNode getUserWidget(XmlNode userNode, string widgetGUID)
        {
            XmlNode widgetNode= userNode.SelectSingleNode("widget[@id='" + widgetGUID + "']");
            if (widgetNode == null)
            {
                widgetNode = config.CreateNode(XmlNodeType.Element, "widget", null);
                userNode.AppendChild(widgetNode);

                XmlAttribute widgetGuidAttribute = config.CreateAttribute("id");
                widgetNode.Attributes.Append(widgetGuidAttribute);

                widgetGuidAttribute.Value = widgetGUID;
            }

            XmlAttribute hideAttribute = widgetNode.Attributes["hide"];
            if (hideAttribute == null)
            {
                hideAttribute = config.CreateAttribute("hide");
                widgetNode.Attributes.Append(hideAttribute).Value="False";
            }

            return widgetNode;
        }

        private XmlNode getUserWidgetElement(XmlNode widgetNode, string element)
        {
            XmlNode elementNode = widgetNode.SelectSingleNode("element[@name='" + element + "']");
            if (elementNode == null)
            {
                elementNode = config.CreateNode(XmlNodeType.Element, "element", null);
                widgetNode.AppendChild(elementNode);

                XmlAttribute elementNameAttribute = config.CreateAttribute("name");
                elementNode.Attributes.Append(elementNameAttribute);

                elementNameAttribute.Value = element;
            }

            XmlAttribute hideAttribute = elementNode.Attributes["hide"];
            if (hideAttribute == null)
            {
                hideAttribute = config.CreateAttribute("hide");
                elementNode.Attributes.Append(hideAttribute).Value="False";
            }

            return elementNode;
        }

        private void saveWidgetBuilderConfig()
        {
            config.Save(HttpContext.Current.Server.MapPath(WidgetBuilderCore.ConfigFilename));
        }

        private WidgetUserPermissions getUserWidgetPermissions()
        {

            WidgetUserPermissions permissions = new WidgetUserPermissions();

            XmlNode userNode=getUserPermissionNode();

            //Log.Add(LogTypes.Debug, 0, "user=>"+userNode.InnerXml);

            //add the widgets in
            foreach (XmlNode thisWidget in userNode.SelectNodes("widget"))
            {
                //Log.Add(LogTypes.Debug, 0, "widget");
                WidgetPermissions thisWidgetPermission = new WidgetPermissions();
                thisWidgetPermission.hide = Convert.ToBoolean(thisWidget.Attributes["hide"].Value);

                //add in the elements
                foreach(XmlNode thisElement in thisWidget.SelectNodes("element")){
                    //Log.Add(LogTypes.Debug, 0, "element");
                    thisWidgetPermission.elements.Add(thisElement.Attributes["name"].Value, new WidgetElementPermissions(){hide=Convert.ToBoolean(thisElement.Attributes["hide"].Value)});
                }

                permissions.widgets.Add(thisWidget.Attributes["id"].Value, thisWidgetPermission);
            }

            return permissions;
        }

    }

    public class WidgetUserPermissions
    {
        public Dictionary<string, WidgetPermissions> widgets = new Dictionary<string, WidgetPermissions>() { };
    }

    public class WidgetPermissions{

        public bool hide = false;//default
        public Dictionary<string, WidgetElementPermissions> elements = new Dictionary<string, WidgetElementPermissions>() { };
    }

    public class WidgetElementPermissions
    {
        public bool hide = false;//default
        
    }
}