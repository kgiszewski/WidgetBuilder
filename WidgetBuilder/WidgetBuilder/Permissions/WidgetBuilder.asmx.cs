using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace WidgetBuilder
{
    /// <summary>
    /// Summary description for WidgetBuilder
    /// </summary>
    [WebService(Namespace = "http://fele.com/")]
    [System.Web.Script.Services.ScriptService]

    public class WidgetBuilderService : System.Web.Services.WebService
    {
        //properties
        private JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        private Dictionary<string, string> returnValue = new Dictionary<string, string>();
        public enum statusCodes { SUCCESS, ERROR }

        public WidgetBuilderService()
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, string> HideWidgetFromUser(bool hide, int userID, string widgetGUID)
        {
            Authorize();

            WidgetBuilderUser user = new WidgetBuilderUser(userID);

            try
            {
                user.hideWidgetByUserID (hide, widgetGUID);
                returnValue.Add("status", jsonSerializer.Serialize(statusCodes.SUCCESS.ToString()));
            }
            catch (Exception e)
            {
                returnValue.Add("status", jsonSerializer.Serialize(statusCodes.ERROR.ToString()));
                returnValue.Add("message", jsonSerializer.Serialize(e.Message));
            }

            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, string> HideElementFromUser(bool hide, int userID, string widgetGUID, string element)
        {
            Authorize();

            WidgetBuilderUser user = new WidgetBuilderUser(userID);

            try
            {
                user.hideWidgetElementByUserID(hide, widgetGUID, element);
                returnValue.Add("status", jsonSerializer.Serialize(statusCodes.SUCCESS.ToString()));
            }
            catch (Exception e)
            {
                returnValue.Add("status", jsonSerializer.Serialize(statusCodes.ERROR.ToString()));
                returnValue.Add("message", jsonSerializer.Serialize(e.Message));
            }

            return returnValue;
        }

        internal static void Authorize()
        {
            if (!umbraco.BasePages.BasePage.ValidateUserContextID(umbraco.BasePages.BasePage.umbracoUserContextID))
            {
                throw new Exception("Client authorization failed. User is not logged in");
            }

        }
    }
}
