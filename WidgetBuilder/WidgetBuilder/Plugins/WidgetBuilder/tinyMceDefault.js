$(function(){

    tinyMCE.init({
        mode:'specific_textareas',
        editor_selector : "widgetTinyMCE",
        theme:'umbraco',
        umbraco_path:'/umbraco',
        width:'100%',
        height:'400',
        theme_umbraco_toolbar_location:'top',
        skin:'umbraco',
        inlinepopups_skin :'umbraco',
        plugins:'contextmenu,umbracomacro,umbracoembed,noneditable,inlinepopups,table,advlink,paste,spellchecker,umbracoimg,umbracocss,umbracopaste,umbracolink,umbracocontextmenu',
        umbraco_advancedMode:true,
        umbraco_maximumDefaultImageWidth:'500',
        language:'en',
        content_css:'',
        theme_umbraco_styles:'Main Heading=h1;Heading 2=h2;Heading 3=h3;Heading 4=h4',
        theme_umbraco_buttons1:'code,separator,undo,redo,cut,copy,pasteword,separator,umbracocss,bold,italic,separator,bullist,numlist,outdent,indent,separator,link,unlink,anchor,separator,image,umbracomacro,table,umbracoembed,separator,charmap,',
        theme_umbraco_buttons2:'',
        theme_umbraco_buttons3:'',
        theme_umbraco_toolbar_align:'left',
        theme_umbraco_disable:'help,visualaid,justifycenter,justifyleft,strikethrough,removeformat,mcespellcheck,underline,subscript,justifyright,justifyfull,inserthorizontalrule,superscript',
        theme_umbraco_path :true,
        extended_valid_elements:'div[*]',
        document_base_url:'/',
        relative_urls:false,
        remove_script_host:true,
        event_elements:'div',
        paste_auto_cleanup_on_paste:true,
        valid_elements:'+a[id|style|rel|rev|charset|hreflang|dir|lang|tabindex|accesskey|type|name|href|target|title|class|onfocus|onblur|onclick|' + 
        'ondblclick|onmousedown|onmouseup|onmouseover|onmousemove|onmouseout|onkeypress|onkeydown|onkeyup],-strong/-b[class|style],-em/-i[class|style],' + 
        '-strike[class|style],-u[class|style],#p[id|style|dir|class|align],-ol[class|reversed|start|style|type],-ul[class|style],-li[class|style],br[class],' + 
        'img[id|dir|lang|longdesc|usemap|style|class|src|onmouseover|onmouseout|border|alt=|title|hspace|vspace|width|height|align|umbracoorgwidth|umbracoorgheight|onresize|onresizestart|onresizeend|rel],' + 
        '-sub[style|class],-sup[style|class],-blockquote[dir|style|class],-table[border=0|cellspacing|cellpadding|width|height|class|align|summary|style|dir|id|lang|bgcolor|background|bordercolor],' + 
        '-tr[id|lang|dir|class|rowspan|width|height|align|valign|style|bgcolor|background|bordercolor],tbody[id|class],' + 
        'thead[id|class],tfoot[id|class],#td[id|lang|dir|class|colspan|rowspan|width|height|align|valign|style|bgcolor|background|bordercolor|scope],' + 
        '-th[id|lang|dir|class|colspan|rowspan|width|height|align|valign|style|scope],caption[id|lang|dir|class|style],-div[id|dir|class|align|style],' + 
        '-span[class|align|style],-pre[class|align|style],address[class|align|style],-h1[id|dir|class|align],-h2[id|dir|class|align],' + 
        '-h3[id|dir|class|align],-h4[id|dir|class|align],-h5[id|dir|class|align],-h6[id|style|dir|class|align],hr[class|style],' + 
        'dd[id|class|title|style|dir|lang],dl[id|class|title|style|dir|lang],dt[id|class|title|style|dir|lang],object[class|id|width|height|codebase|*],' + 
        'param[name|value|_value|class],embed[type|width|height|src|class|*],map[name|class],area[shape|coords|href|alt|target|class],bdo[class],button[class],iframe[*]',
        invalid_elements:'font',
        spellchecker_rpc_url:'GoogleSpellChecker.ashx',
        entity_encoding:'raw',
        //theme_umbraco_pageId:'1057',
        //theme_umbraco_versionId:'1e442626-775e-42a9-a8f8-8e65b2b5e5b9',
        //umbraco_toolbar_id:'umbTinymceMenu3140'
    });
});