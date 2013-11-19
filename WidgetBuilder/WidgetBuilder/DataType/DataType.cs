using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

using umbraco.cms.businesslogic.datatype;
using umbraco.BusinessLogic;

namespace WidgetBuilder
{
    //dev branch, with feature-x
    public class Widget_Builder : umbraco.cms.businesslogic.datatype.BaseDataType, umbraco.interfaces.IDataType
    {
        private umbraco.interfaces.IDataEditor _Editor;
        private umbraco.interfaces.IData _baseData;
        private Widget_Builder_PrevalueEditor _prevalueeditor;

        public static bool HasDamp{
            get {
                //Log.Add(LogTypes.Custom, 0, "Checking for DAMP..."); sup
                var thisType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "Damp_Data_Editor_Extension" 
                        select type).FirstOrDefault();
                return (thisType != null);
            }
        }

        public static bool HasSpreadsheet
        {
            get {
                var thisType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Name == "SpreadsheetData"
                                     select type).FirstOrDefault();
                return (thisType != null);
            }
        }

        public static string GetAssemblyVersion
        {           	
            get {
                return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "WidgetBuilder2").First().GetName().Version.ToString(4);
            }
        }
        
        // Instance of the Datatype
        public override umbraco.interfaces.IDataEditor DataEditor
        {
            get
            {
                if (_Editor == null)
                    _Editor = new Widget_Builder_DataEditor(Data, ((Widget_Builder_PrevalueEditor)PrevalueEditor).Configuration);
                return _Editor;
            }
        }

        //this is what the cache will use when getting the data
        public override umbraco.interfaces.IData Data
        {
            get
            {
                if (_baseData == null)
                    _baseData = new Widget_Builder_Default_Data(this);
                return _baseData;
            }
        }

        /// <summary>
        /// Gets the datatype unique id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get
            {
                return new Guid("c324dcd5-4f2a-482a-94cd-1608a86328d6");
            }
        }

        public int DTDguid
        {
            get
            {
                return this.DataTypeDefinitionId;
            }
        }

        /// <summary>
        /// Gets the datatype unique id.
        /// </summary>
        /// <value>The id.</value>
        public override string DataTypeName
        {
            get
            {
                return "Widget Builder";
            }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueeditor == null)
                {
                    _prevalueeditor = new Widget_Builder_PrevalueEditor(this);
                }
                return _prevalueeditor;
            }
        }
    }
}