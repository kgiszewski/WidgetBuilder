using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.presentation.Trees;
using umbraco.BusinessLogic;

namespace Trees.Widget_Builder
{
    public class Widget_Builder_Permissions : BaseTree
    {
        public Widget_Builder_Permissions(string application) :
            base(application)
        {
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.Icon = FolderIcon;
            rootNode.OpenIcon = FolderIconOpen;
            rootNode.NodeType = TreeAlias;
            rootNode.NodeID = "init";
        }

        public override void RenderJS(ref System.Text.StringBuilder Javascript)
        {
            Javascript.Append(
                @"
                    function openWidgetBuilderPermissions(userID){
                        parent.right.document.location.href='/umbraco/plugins/WidgetBuilder/permissions.aspx?userID='+userID;
                    }
                ");
        }

        public override void Render(ref XmlTree tree)
        {
            if (User.GetCurrent().IsAdmin())
            {
                //create users tree
                XmlTreeNode xNode;

                foreach (User thisUser in User.getAll())
                {
                    xNode = XmlTreeNode.Create(this);
                    xNode.Text = thisUser.Name;
                    xNode.Icon = "user.gif";
                    xNode.NodeID = thisUser.Id.ToString();
                    xNode.Action = "javascript:openWidgetBuilderPermissions(" + thisUser.Id.ToString() + ")";
                    tree.Add(xNode);
                }
            }
        }
    }
}