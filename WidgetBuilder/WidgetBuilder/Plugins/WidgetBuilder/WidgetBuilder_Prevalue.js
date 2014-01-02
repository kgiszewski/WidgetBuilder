$(function(){

  var $saveBox=$(".widgetSaveBox");
  var $gridWrapperDiv=$('.gridWrapperDiv ');

  //ini
  buildJSON();
  
  //make elements sortable
  //elements
  $('.gridWrapperDiv').sortable({
    handle: ".sortWidgetElement",
    cursor: 'move',
    helper: fixHelper,
    update: function(){buildJSON();},
    start : function(e, ui){ui.placeholder.html('<div>Insert Here</div>')},
    placeholder: 'widgetElementPlaceholder'
  });

  //list table
  $('.widgetListTable tbody').sortable({
    handle: ".sortWidgetListTable",
    cursor: 'move',
    items : 'tr:not(.widgetUnsortable)',
    helper: fixHelper,
    update: function(){buildJSON();},
    start : function(e, ui){ui.placeholder.html('<div>Insert Here</div>')},
    placeholder: 'widgetElementPlaceholder'
  });

  //add element to grid
  $(".widgetToolbarWrapper a").click(function(){
    var $a=$(this);
    
    $gridWrapperDiv.append('<table class="widgetGridTable" type="'+$a.attr('type')+'"/>');
    buildJSON();
    __doPostBack();
  });

  //remove element from grid
  $(".removeWidgetElement").click(function(){
    
    var $button=$(this);
  
    if(confirm('Are you sure you want to remove this?')){
      $button.closest('.widgetGridTable').remove();
      buildJSON();
    }
  });
  
  //warn about element name change
  $(".widgetElementName").live("click", function(){
    var $thisBox=$(this);
    if($thisBox.val()!=''){
      $thisBox.addClass('warn');
      alert("Warning! Changing the element name will result in data loss, please be sure this is correct before entering large amounts of data.");
    }
  });
  
  //respond to element name change
  $(".widgetElementName").keyup(function(){
    var $textBox=$(this);
    
    $textBox.val(makeMachineName($textBox.val()));
    
    buildJSON();
  });
  
  //respond to a checkbox changing
  $("input[type=checkbox]").live("click", function(){
    buildJSON();
  });
  
  //respond to save
  $("form").submit(function(e){
    $(".widgetElementName").each(function(){
      var $thisElementName=$(this);
      $thisElementName.removeClass('error');
      
      if($thisElementName.val()==''){
        $thisElementName.addClass('error');
        e.preventDefault();
      }
    });
  });
  
  //respond to toggle json
  $(".json").click(function(){
    $saveBox.toggle();
  });
  
  //respond to any textbox changes
  $(".widgetGridTable input[type=text]").keyup(function(){
    var $textBox=$(this);
    
    buildJSON();
  });

  $(".widgetGridTable textarea").keyup(function(){
    var $textBox=$(this);
    
    buildJSON();
  });

  //respond to any maxIndent and maxWidget changes
  $(".widgetMaxIndents, .maxWidgets").keyup(function(){
    var $textBox=$(this);
    $textBox.removeClass('error');
    
    if(isNaN($textBox.val())||$textBox.val()==''){
      $textBox.addClass('error');
    } else {
      buildJSON();
    }
  });

  //respond to any custom css or js
  $(".jsInclude, .cssInclude").keyup(function(){
    var $textBox=$(this);
    $textBox.removeClass('error');
    
    buildJSON();
  });
  
  //add list item
  $(".listAdd").live('click', function(){
    var $thisTable=$(this).closest('table');
    
    var $newRow=$(this).closest('tr').clone(true);
    $thisTable.append($newRow);
    $newRow.find("input").val('');
  });
  
  //remove list item
  $(".listRemove").live('click', function(){
    var $thisTable=$(this).closest('table');
    var $oldRow=$(this).closest('tr');
    
    if($thisTable.find('tr').length>2){
      $oldRow.remove();
      buildJSON();
    }
  });
  
  function buildJSON(){
  
    var elements=[];
    
    $(".widgetGridTable").each(function(){
      var $table=$(this);
      var options=new optionsArray();
      
      //elements
      //if not an empty table (aka a new element)
      if($table.find('tr').length){
        switch($table.attr('type')){
          case 'textbox':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;

          case 'textarea':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'tinymce':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            options.add('JSpath', $table.find('.widgetJsPath').val());
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'list':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('maxIndents', $table.find('.widgetMaxIndents').val());
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'spreadsheet':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('classes', $table.find('.widgetClasses').val());
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'mediapicker':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'damp':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('allowedExtensions', $table.find('.widgetFileExtensions').val());
            options.add('selectableMediaNodes', $table.find('.widgetSelectableNodes').val());
            options.add('createableMediaNodes', $table.find('.widgetCreateableNodes').val());
            options.add('defaultMediaNodeID', $table.find('.widgetDefaultNodeID').val());
            options.add('startNodeID', $table.find('.widgetStartNodeID').val());
            options.add('imageCropperAlias', $table.find('.widgetCropperAlias').val());
            options.add('allowMultiple', $table.find('.widgetAllowMultiple input').is(':checked'));
            
            options.add('hideCreate', $table.find('.widgetHideCreate input').is(':checked'));
            options.add('hideEdit', $table.find('.widgetHideEdit input').is(':checked'));
            options.add('hideOpen', $table.find('.widgetHideOpen input').is(':checked'));
            options.add('hidePixlr', $table.find('.widgetHidePixlr input').is(':checked'));
            
            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'dropdown':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('useSelect', $table.find('.widgetUseSelect input').is(':checked'));            
            
            //build the list
            var $listTable=$table.find(".widgetListTable");
            options.add('items', '['+buildList($listTable).join(',')+']', true);            

            elements.push(new element($table.attr('type'), options.get()).value);
            break;

          case 'checkradio':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('isCheckbox', $table.find('.widgetIsCheckbox input').is(':checked'));
            options.add('defaultChecked', $table.find('.widgetDefaultChecked input').is(':checked'));

            //build the list
            var $listTable=$table.find(".widgetListTable");
            options.add('items', '['+buildList($listTable).join(',')+']', true);

            elements.push(new element($table.attr('type'), options.get()).value);
            break;
            
          case 'contentpicker':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('startNodeID', $table.find('.widgetStartNodeID').val());
            options.add('allowedDocTypeIDs', $table.find('.widgetDocTypeIDs').val());
            options.add('allowMultiple', $table.find('.widgetAllowMultiple input').is(':checked'));
            options.add('showAllDocTypes', $table.find('.widgetShowAllDocTypes input').is(':checked'));
            options.add('jsPath', $table.find('.widgetCpJsPath').val());
            options.add('cssPath', $table.find('.widgetCpCssPath').val());
            
            elements.push(element($table.attr('type'), options.get()).value);
            break;
            
        case 'map':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('lat', $table.find('.widgetLat').val());
            options.add('lng', $table.find('.widgetLng').val());
            options.add('zoom', $table.find('.widgetZoom').val());
            options.add('address', $table.find('.widgetAddress').val());
            
            elements.push(element($table.attr('type'), options.get()).value);
            break;
            
        case 'inlinepicker':
            options.add('title', $table.find('.widgetTitle').val());
            options.add('elementName', $table.find('.widgetElementName').val());
            options.add('className', $table.find('.widgetClass').val());
            options.add('description', $table.find('.widgetDescription').val());
            
            options.add('mediaIDs', $table.find('.widgetMediaIDs').val());
            
            elements.push(element($table.attr('type'), options.get()).value);
            break;
        }
      } else {
        //otherwise add an element with empty prevalues
        //elements.push('{"type":"'+$table.attr('type')+'", "prevalues": ""}');
        elements.push(new element($table.attr('type'), "").value);
      }
    });
    
    var json='{';
    //global options
    json+='"maxWidgets":'+$('.maxWidgets').val()+','
    json+='"jsIncludePath":"'+encodeURI($('.jsInclude').val())+'",'
    json+='"cssIncludePath":"'+encodeURI($('.cssInclude').val())+'",'
    
    //piece together the elements
    json+='"elements":['+elements.join(',')+']';
    
    json+='}';

    $saveBox.val(json);
  }
  
  //helpers
  function element(type, optionsString){
    this.value='{"type":"'+type+'", "prevalues": "{'+optionsString+'}"}';
    return this;
  }
  
  function optionsArray(){
    this.options=[];
    
    this.add=function(elementName, data, noEscape){
    
      //data=new String(data).replace(/\n/g, "\\n");
    
      if(noEscape){
        this.options.push("'"+elementName+"':"+(data));
      } else {
        //this.options.push("'"+elementName+"':'"+encodeURI(data)+"'");
        this.options.push("'"+elementName+"':'"+encodeURI(data).replace(/'/g, '%27')+"'");
      }
    }
    
    this.get=function (){
      return this.options.join(',');
    }
    
    return this;
  }
  
  function makeMachineName(string){
    return String(string).replace(/[^\w\s-]/gi, '');
  }
  
  //supposed to help sortable widths 
  function fixHelper(e, ui){
    ui.children().each(function(){
      $(this).width($(this).width());
    });
    return ui;
  };
  
  //build a list
  function buildList($listTable){

    var listItems=[];
    
    $listTable.find('input').each(function(index){
      var $thisInput=$(this);
      var item="";
      
      //alternate
      if(index % 2==0){
        item+="{'value':'"+encodeURI($thisInput.val())+"'";
      } else {
        item+="'display':'"+encodeURI($thisInput.val())+"'}";
      }

      listItems.push(item);
    });
    return listItems;
  }
});