using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Web;

namespace WidgetBuilder.Extensions
{
    public static class Extensions
    {
        public static XmlDocument GetWidgetAsXml(IPublishedContent content, string propertyName)
        {
            var xd = new XmlDocument();

            if (content == null)
                return xd;

            var rawXML = content.GetPropertyValue<string>(propertyName);

            if (String.IsNullOrEmpty(rawXML))
                return xd;

            xd.LoadXml(rawXML);

            return xd;
        }

        public static XmlNodeList GetAllWidgets(this IPublishedContent content, string propertyName)
        {
            var xd = GetWidgetAsXml(content, propertyName);

            return xd.SelectNodes("//widget");
        }

        public static XmlNode GetWidget(this IPublishedContent content, string propertyName)
        {
            var xd = GetWidgetAsXml(content, propertyName);

            return xd.SelectSingleNode("//widget");
        }

        public static string GetWidgetPropertyValue(this IPublishedContent content, string propertyName, string elementName)
        {
            var xd = GetWidgetAsXml(content, propertyName);
            
            var node = xd.SelectSingleNode("//widget/" + elementName);

            if (node == null)
                return "";

            return node.InnerText;
        }

        public static List<string> GetWidgetPropertyValues(this IPublishedContent content, string propertyName, string elementName)
        {
            var xd = GetWidgetAsXml(content, propertyName);
            var list = new List<string>();

            var nodes = xd.SelectNodes("//widget/" + elementName);

            if (nodes == null)
                return list;

            foreach (XmlNode node in nodes)
            {
                list.Add(node.InnerText);
            }

            return list;
        }

        public static string GetWidgetValue(this XmlNode widget, string elementName)
        {
            if (widget == null)
                return "";

            var element = widget.SelectSingleNode(elementName);

            if (element == null)
                return "";

            return element.InnerText;
        }

        public static bool GetWidgetValue<T>(this XmlNode widget, string elementName)
        {
            if (widget == null)
                return false;

            var element = widget.SelectSingleNode(elementName);

            if (element == null)
                return false;

            if (element.InnerText == "1" || element.InnerText.ToLower() == "true")
            {
                return true;
            }

            return false;
        }

        public static string GetWidgetPropertyValueRaw(this XmlNode widget, string elementName)
        {
            if (widget == null)
                return "";

            var element = widget.SelectSingleNode(elementName);

            if (element == null)
                return "";

            return element.InnerXml;
        }

        public static bool HasWidgetValue(this XmlNode widget, string elementName)
        {
            if (widget == null)
                return false;

            var element = widget.SelectSingleNode(elementName);

            if (element == null)
                return false;

            return element.InnerText != "";
        }

        public static List<ListItem> ToListItems(this XmlDocument xd)
        {
            var listItems = new List<ListItem>();

            foreach(XmlNode listItem in xd.SelectNodes("list/item"))
            {
                if (!String.IsNullOrEmpty(listItem.InnerText))
                {
                    listItems.Add(new ListItem() { Indent = Convert.ToInt32(listItem.Attributes["indent"].Value), Display = listItem.InnerText });
                }
            }

            return listItems;
        }
    }

    public class ListItem
    {
        public int Indent { get; set; }
        public string Display { get; set; }
    }
}