$(function(){
  var $dataTypeDivs=$(".dataTypeDiv");
  
  $dataTypeDivs.find('table').hide();
  
  //striping
  $("tr").not(':first').hover(
    function () {
      $(this).addClass('rowHighlight');
    }, 
    function () {
      $(this).removeClass('rowHighlight');
    }
  );
  
  //highlight any titles that have a checkmark
  $('.hideWidgetForUser input:checked, .hidePropertyForUser input:checked').closest('.dataTypeDiv').find('h4').addClass('titleHighlight');

  //toggle table
  $dataTypeDivs.find('h4').click(function(){
    var $thisH=$(this);
    
    $thisH.closest('.dataTypeDiv').find('table').toggle();
  });
  
  $('.hideWidgetForUser input').click(function(){
        
      var $thisCB=$(this);
      var $thisDTD=$thisCB.closest('.dataTypeDiv');
      
      $.ajax({
          type: "POST",
          async: false,
          url: "/umbraco/plugins/WidgetBuilder/WidgetBuilder.asmx/HideWidgetFromUser",
          data: '{"hide": '+$thisCB.is(':checked')+', "userID": '+$thisDTD.attr('userID')+', "widgetGUID":"'+$thisDTD.attr('guid')+'"}',
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (returnValue){
            var response=returnValue.d;
            //console.log(response);
            
            switch(response.status){
              case 'SUCCESS':
                  
                  break;
                  
              case 'EMAIL_INVALID':
                  
                  break;
                  
              case 'CM_ERROR':
                  
                  break;
            }
          }
      });
   });
   
   $('.hidePropertyForUser input').click(function(){
        
      var $thisCB=$(this);
      var $thisDTD=$thisCB.closest('.dataTypeDiv');
      
      $.ajax({
          type: "POST",
          async: false,
          url: "/umbraco/plugins/WidgetBuilder/WidgetBuilder.asmx/HideElementFromUser",
          data: '{"hide": '+$thisCB.is(':checked')+', "userID": '+$thisDTD.attr('userID')+', "widgetGUID":"'+$thisDTD.attr('guid')+'", "element":"'+$($thisCB.closest('tr').find('td')[2]).text()+'"}',
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (returnValue){
            var response=returnValue.d;
            //console.log(response);
            
            switch(response.status){
              case 'SUCCESS':
                  
                  break;
                  
              case 'EMAIL_INVALID':
                  
                  break;
                  
              case 'CM_ERROR':
                  
                  break;
            }
          }
      });
   });
});