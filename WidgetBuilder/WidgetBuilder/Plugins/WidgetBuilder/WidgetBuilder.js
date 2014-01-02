$(function(){

  //ini on first load
  buildAll();
  
  $(".widgetStartCollapsed").each(function(){
    collapse($(this));
  });
  
  var $widgetWrapperDiv=$('.widgetWrapperDiv');

  $widgetWrapperDiv.sortable({
    handle: ".widgetSort",
    cursor: 'move',
    helper: fixHelper,
    update: function(){buildXML(this);},
    //start : function(e, ui){ui.placeholder.html('<div>Insert Here</div>')},
    placeholder: 'widgetElementPlaceholder',
    start: function (e, ui) {
        $(this).find('.widgetTinyMCE').each(function () {
            tinyMCE.execCommand('mceRemoveControl', false, $(this).attr('id'));
        });
    },
    stop: function (e, ui) {
        $(this).find('.widgetTinyMCE').each(function () {
            tinyMCE.execCommand('mceAddControl', true, $(this).attr('id'));
            $(this).sortable("refresh");
        });
    }
  });
  
  var $propertyItemContent=$widgetWrapperDiv.closest('.propertyItemContent');
  
  $propertyItemContent.addClass('widgetPropertyItemContent');
  $propertyItemContent.siblings('.propertyItemheader').addClass('widgetPropertyItemheader');

  //respond to input boxes updating
  $(".widgetTextBox, .widgetTextarea").live("keyup", function(){
    buildXML(this);
  });
  
  //respond to lists changing
  $(".TextBoxListSettingsSaveBox").change(function(){
    buildXML(this);
  });
  
  //respond to checkboxes and radios
  $(".widgetControlWrapper input[type=checkbox], .widgetControlWrapper input[type=radio]").live('click', function(){
    buildXML(this);
  });
  
  //add onSave handlers
  $("form").click(function(){
    buildAll();
  });
  
  //collapse/expand widgetControlWrapper
  $(".widgetLabelTitle").live('click', function(){
    var $wcw=$(this).closest(".widgetElementWrapper").find(".widgetControlWrapper");
    $wcw.toggle();
    buildXML(this);
  });
  
  //widget add
  $(".widgetAdd").live("click", function(){
    var $thisWidget=$(this);
    
    var $widgetWrapperDiv=$thisWidget.closest(".widgetWrapperDiv");
    
    //count the number of widgets
    var $widgets=$widgetWrapperDiv.find(".widgetWrapper");
    if($widgets.length<parseInt($widgetWrapperDiv.attr("widgetmaxwidgets"),10)){
           
      //we need to rebuild each widget on entire page (across all tabs), then rebuild the one we are appending to
      buildAll();
      buildXML(this, true);
      __doPostBack();
    } else {
      alert("Max widgets reached.");
    }
  });
  
  //widget remove
  $(".widgetRemove").live("click", function(){
    var $widgetWrapperDiv=$(this).closest(".widgetWrapperDiv");
    
    //count the number of widgets
    var $widgets=$widgetWrapperDiv.find(".widgetWrapper");
    if($widgets.length>1){
      if(confirm('Are you sure you want to remove this?')){
        $(this).closest(".widgetWrapper").remove();
        buildXML($widgetWrapperDiv);
      }
    }
  });
  
  //widget expand/collapse
  $(".widgetCollapse, .widgetFade").live("click", function(){
    var $thisWidget=$(this).closest(".widgetWrapper");
    
    if(!$thisWidget.length){
      $thisWidget=$(this).siblings(".widgetWrapper");
    }
    
    if($thisWidget.hasClass('widgetCollapsed')){
      expand($thisWidget);
    } else {
      collapse($thisWidget);
    }
  });
  
  function collapse($widget){
    $widget.addClass("widgetCollapsed");
    $widget.parent().append("<div class='widgetFade'/>");
    $widget.find(".widgetCollapse").attr('src', '/umbraco/plugins/WidgetBuilder/images/open.png');
  }
  
  function expand($widget){
    $widget.removeClass("widgetCollapsed");
    $widget.parent().find('.widgetFade').remove();
    $widget.find(".widgetCollapse").attr('src', '/umbraco/plugins/WidgetBuilder/images/close.png');
  }

  function buildXML(element, addNew){
    console.log("building...");
 
    var $widgetWrapperDiv=$(element).closest(".widgetWrapperDiv");
    var $saveBox=$widgetWrapperDiv.siblings(".widgetSaveBox");

    var elementArray=[];
    var $widgetDivs=$widgetWrapperDiv.find(".widgetWrapper");
        
    $widgetDivs.each(function(){
      var widgetXML="";
      var $thisWidget=$(this);
      
      $thisWidget.find(".widgetElementWrapper").each(function(){
        var $thisElementWrapper=$(this);
        //widgetXML+='<'+$thisElementWrapper.attr("elementname")+' isCollapsed="'+!$thisElementWrapper.find(".widgetControlWrapper").is(":visible")+'">';
        widgetXML+='<'+$thisElementWrapper.attr("elementname")+'>';
        
        //based on element type
        switch($thisElementWrapper.attr("type")){
          case "textbox":
            widgetXML+=htmlEntities($thisElementWrapper.find(".widgetTextBox").val());
            break;
            
          case "textarea":
          case "tinymce":
            //test for a tinyMCE            
              var $iframe = $thisElementWrapper.find('iframe');

              console.log($iframe);
        
            if($iframe.length>0){
                var html=$iframe.contents().find('body').html();
                widgetXML+="<![CDATA["+html+"]]>";
            } else {
                var value=htmlEntities($thisElementWrapper.find(".widgetTextarea").val());
                //replace line breaks
                value=value.replace(/\n/g, "<br />");
                widgetXML+=value;
            }
            
            break;           
            
          case "mediapicker":
            if(parseInt($thisElementWrapper.find('input').val(),10)){
              widgetXML+=htmlEntities($thisElementWrapper.find('input').val());
            }
            break;
            
          case "spreadsheet":
            widgetXML+=$spreadsheetHiddens=$thisElementWrapper.find("input").val();
            
            break;
            
          case "damp":
            var $dampHidden=$thisElementWrapper.find("input[type=hidden]");

            //removes the final comma
            widgetXML+=$dampHidden.val().slice(0, -1);
            break;
            
          case "list":
            widgetXML+=$thisElementWrapper.find(".TextBoxListSettingsSaveBox").val();
            break;
            
          case "dropdown":
            widgetXML+=$thisElementWrapper.find(".widgetDropdown").val();
            break;
            
          case "checkradio":
            var checkRadioXML="";
            
            var $checkedBoxes=$thisElementWrapper.find("input:checked");
            $checkedBoxes.each(function(){
              var $thisCB=$(this);
              checkRadioXML+="<value>"+escape($thisCB.val())+"</value>";
            });
            
            if($checkedBoxes.length==0){
                checkRadioXML+="<value></value>";
            }
            widgetXML+=checkRadioXML;
            break;
            
          case "contentpicker":
            widgetXML+=$thisElementWrapper.find(".contentPickerSaveBox").val();
            break;
            
          case "map":
            widgetXML+=$thisElementWrapper.find(".uNewsGoogleMapSaveBox").val();
            break;
            
          case "inlinepicker":
            widgetXML+=$thisElementWrapper.find(".InlineImagePickerSaveBox").val();
            break;
        }
        
        widgetXML+="</"+$thisElementWrapper.attr("elementname")+">";
      });
      elementArray.push("<widget isCollapsed=\""+$thisWidget.hasClass('widgetCollapsed')+"\">"+widgetXML+"</widget>");
      
      if($(element).closest('.widgetWrapper')[0]==this && addNew){
        elementArray.push("<widget isCollapsed=\"false\"/>");
      } 
    });
    
    $saveBox.val("<widgets>"+elementArray.join("")+"</widgets>");
    return $saveBox;
  }

  //helpers
  function htmlEntities(string){
    return String(string).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }
  
  //rebuilds all
  function buildAll(){
      console.log("building all...");
    $(".widgetWrapperDiv").each(function(){
        buildXML(this);
    });
  }
  
  //supposed to help sortable widths 
  function fixHelper(e, ui){
    ui.children().each(function(){
      $(this).width($(this).width());
    });
    return ui;
  };
  
  //append an index to spreadsheets
  $('.widgetWrapperDiv').each(function(){
    var counter=0;
    var $thisWidgetWrapperDiv=$(this);
    //console.log($thisWidgetWrapperDiv);
    
    $thisWidgetWrapperDiv.find('[type=spreadsheet]').each(function(){
      var $ul=$(this).find('.controls-list');
      var property=$ul.closest('.widgetWrapperDiv').parent().attr('id').substring(10);
      //console.log(property);
      
      $ul.each(function(){
        var $anchors=$(this).find('a');
        //console.log($anchors);
        $anchors.each(function(){
          var $a=$(this);
          if($a.text()=='Download'){
            $a.attr('href', $a.attr('href')+"&index="+counter);
            //console.log($a.attr('href').replace(/alias=([0-9a-zA-Z])*/, 'alias='+property));
            $a.attr('href', $a.attr('href').replace(/alias=([0-9a-zA-Z])*/, 'alias='+property));
          }
        });
      });
      counter++;
    });
  });
});