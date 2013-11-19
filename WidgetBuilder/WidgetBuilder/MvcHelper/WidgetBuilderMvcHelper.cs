using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

using Umbraco.Core.Models;
using Umbraco.Web.Models;
using umbraco.BusinessLogic;

namespace WidgetBuilderMvcHelper
{
    public class WidgetHelper
    {
        public enum XmlValue
        {
            InnerXml,
            OuterXml,
            InnerText
        }

        public static string GetWidget(object Xml, string element, XmlValue xmlValue = XmlValue.InnerText)
        {
            XmlDocument xd = new XmlDocument();

            try
            {
                xd.LoadXml(Xml.ToString());

                switch (xmlValue)
                {

                    case XmlValue.InnerText:
                        return xd.SelectSingleNode("//" + element).InnerText;

                    case XmlValue.OuterXml:
                        return xd.SelectSingleNode("//" + element).OuterXml;

                    case XmlValue.InnerXml:
                        return xd.SelectSingleNode("//" + element).InnerXml;

                    default:
                        return xd.SelectSingleNode("//" + element).InnerText;
                }
            }
            catch
            {
                return "";
            }
        }

        public static List<string> GetWidgetAsList(object Xml, string element)
        {
            XmlDocument xd = new XmlDocument();
            List<string> list = new List<string>();

            try
            {
                xd.LoadXml(Xml.ToString());

                foreach (XmlNode node in xd.SelectNodes("//" + element + "/*"))
                {
                    list.Add(node.InnerText);
                }

                return list;
            }
            catch
            {
                return list;
            }
        }

        public static XmlNodeList GetWidgetAsXmlNodeList(object Xml, string xPath)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Xml.ToString());

            return xd.SelectNodes(xPath);
        }

        public static XmlNode GetWidgetAsXmlNode(object Xml, string xPath)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Xml.ToString());

            return xd.SelectSingleNode(xPath);
        }

        public static string GetWidgetImageCropperUrl(object Xml, string cropName)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Xml.ToString());

            XmlNode cropNode;

            cropNode = xd.SelectSingleNode("//crop[@name='" + cropName + "']");

            if (cropNode != null)
            {
                return cropNode.Attributes["url"].Value;
            }
            else
            {
                return "";
            }
        }
    }
}