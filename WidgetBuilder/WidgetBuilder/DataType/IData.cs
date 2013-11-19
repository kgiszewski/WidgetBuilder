using System;
using System.Data;
using System.Configuration;
using System.Web;

using umbraco.interfaces;

namespace WidgetBuilder
{
    //this class is a token implementation of IData for use with data editors
    public class DataInterface : IData
    {
        private object _value;

        public DataInterface() { }
        public DataInterface(object o)
        {
            Value = o;
        }

        #region IData Members

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void MakeNew(int PropertyId)
        {
            throw new NotImplementedException();
        }

        public int PropertyId
        {
            set { throw new NotImplementedException(); }
        }

        public System.Xml.XmlNode ToXMl(System.Xml.XmlDocument d)
        {
            throw new NotImplementedException();
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        #endregion
    }
}