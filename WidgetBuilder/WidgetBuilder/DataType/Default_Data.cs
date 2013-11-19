using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using umbraco.BusinessLogic;

namespace WidgetBuilder
{

    public class Widget_Builder_Default_Data : umbraco.cms.businesslogic.datatype.DefaultData
    {

        public static string defaultXML = "<widgets><widget/></widgets>";

        public Widget_Builder_Default_Data(umbraco.cms.businesslogic.datatype.BaseDataType DataType) : base(DataType) { }
        

        public override System.Xml.XmlNode ToXMl(System.Xml.XmlDocument data)
        {

            XmlDocument xd = new XmlDocument();
            try
            {
                xd.LoadXml(this.Value.ToString());
            }
            catch (Exception e)
            {
                xd.LoadXml(defaultXML);
            }

            return data.ImportNode(xd.DocumentElement, true);
        }
    }
}

