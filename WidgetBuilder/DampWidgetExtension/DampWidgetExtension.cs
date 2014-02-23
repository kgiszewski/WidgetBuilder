using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigibizAdvancedMediaPicker;

namespace DampWidgetExtension
{
    public class Damp_Data_Editor_Extension : DigibizAdvancedMediaPicker.DAMP_DataEditor
    {
        public string _value
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //Log.Add(LogTypes.Debug, 0, "saveValue=>"+SaveValue);

            ((IMediaPicker)Controls[0]).SaveValue = SaveValue;
            ((IMediaPicker)Controls[0]).AllowedExtensionsValue = AllowedExtensionsValue;
            ((IMediaPicker)Controls[0]).AllowedSelectableMediaTypesValue = AllowedSelectableMediaTypesValue;
            ((IMediaPicker)Controls[0]).AllowedCreateableMediaTypesValue = AllowedCreateableMediaTypesValue;
            ((IMediaPicker)Controls[0]).DefaultMediaTypeValue = DefaultMediaTypeValue;
            ((IMediaPicker)Controls[0]).HideCreateValue = HideCreateValue;
            ((IMediaPicker)Controls[0]).StartNodeIdValue = StartNodeIdValue;
            ((IMediaPicker)Controls[0]).SelectMultipleNodesValue = SelectMultipleNodesValue;
            ((IMediaPicker)Controls[0]).MinimumNodesValue = MinimumNodesValue;
            ((IMediaPicker)Controls[0]).MaximumNodesValue = MaximumNodesValue;
            ((IMediaPicker)Controls[0]).CropPropertyAliasValue = CropPropertyAliasValue;
            ((IMediaPicker)Controls[0]).CropNameValue = CropNameValue;
            ((IMediaPicker)Controls[0]).ThumbnailWidthValue = ThumbnailWidthValue;
            ((IMediaPicker)Controls[0]).ThumbnailHeightValue = ThumbnailHeightValue;
            ((IMediaPicker)Controls[0]).HideEditValue = HideEditValue;
            ((IMediaPicker)Controls[0]).HideOpenValue = HideOpenValue;
            ((IMediaPicker)Controls[0]).HidePixlrValue = HidePixlrValue;
            ((IMediaPicker)Controls[0]).Mandatory = Mandatory;
            ((IMediaPicker)Controls[0]).CheckValidation = CheckValidation;
            ((IMediaPicker)Controls[0]).MinimumNodesText = MinimumNodesText;
            ((IMediaPicker)Controls[0]).MaximumNodesText = MaximumNodesText;
            ((IMediaPicker)Controls[0]).MandatoryText = MandatoryText;
            ((IMediaPicker)Controls[0]).DataTypeDefinitionId = DataTypeDefinitionId;
            ((IMediaPicker)Controls[0]).EnableSearch = EnableSearch;
            ((IMediaPicker)Controls[0]).EnableSearchAutoSuggest = EnableSearchAutoSuggest;
            ((IMediaPicker)Controls[0]).SearchMethod = SearchMethod;

            //Log.Add(LogTypes.Debug, 0, "Allowed Selecteable->"+AllowedSelectableMediaTypesValue);

            //if (FirstLoad || overrideFirstLoad)
            //{
            //The value only needs to be set the first time the page is loaded.
            //If the value is set multiple times the user will lose it's selected items after pressing the save button.
            //We don't use Page.IsPostBack for this because the control could also be loaded the first time on a postback (with Canvas).
            ((IMediaPicker)Controls[0]).Value = _value;

            //FirstLoad = false;
            //}
        }
    }
}