$(document).ready(function () {

    var inventoryApp = {
        time: 0,
        file: 0,
        timeForm: [],
        fileForm: []
    };

    /*
    var ids = $('#ulList .vehicle').map(function () {
        return $(this).attr('id');
    }).get();

    console.log(ids);
    */

    $(".radio").on('click', function () {
        var id = $(this).attr("for");
        var name = $('#' + id).attr("name");
        $('#' + id).prop("checked", true)
      
        
        
      
 
    });

    $("#submit").on('click', function () {
       
        var customerNumber = $("#cnumber").val();
        var customerForm = $('#customerForm');

        
        var type = customerForm.find('input[name="type"]:checked').val();

        var sendid = customerForm.find('input[name="sendid"]:checked').val();
        var sendvia = customerForm.find('input[name="sendvia"]:checked').val();
        var email = customerForm.find('input[name="email"]').val();
        var ftphostname = customerForm.find('input[name="ftphostname"]').val();
        var ftpusername = customerForm.find('input[name="ftpusername"]').val();
        var ftppassword = customerForm.find('input[name="ftppassword"]').val();
        var ftpfolder = customerForm.find('input[name="ftpfolder"]').val();
        var buyer = customerForm.find('input[name="buyer"]:checked').val();
        var header = customerForm.find('input[name="header"]:checked').val();

 

      //  var time = customerForm.find('input[name="time[]"]').val();
        var time = $("input[name='time[]']")
              .map(function () { return $(this).val(); }).get();
       

        var field = $("select[name='field[]']")
              .map(function () { return $(this).val(); }).get();

        
        //alert(customerNumber);
      

       
        var yourJsonObject = {
            type: type,
            sendid: sendid,
            sendvia: sendvia,
            email: email,
            ftp: ftphostname + ";" + ftpusername + ";" + ftppassword + ";" + ftpfolder,
            buyer: buyer,
            header: header,
            time: time,
            field: field
        };
        
        
        $.post('/Customer/SendRequest/' + customerNumber,
            $.param(yourJsonObject, true)
       ,
       function (data, status) {

           console.log("Data");
           console.log(data.message);
           console.log("Status");
           console.log(status);
          // console.log("Data: " + data.message + "\nStatus: " + status);
       }); 

       /* $.post('/Customer/SendRequest/' + customerNumber, function (result) {
             if (result.length > 0) 
                 
            } else {
               
            }  
        }); */

        return false;
    });

    $(".add").on('click', function (e) {
        e.preventDefault();
        var toAdd = $(this).data('add');

        // var intId = $("#" + toAdd + "div div").length;
        var intId;
        var index;
        var i = 1;
        while (index !== -1) {
            var intId = inventoryApp[toAdd + 'Form'].length + i;
            var index = inventoryApp[toAdd + 'Form'].indexOf(toAdd + '-' + intId);
            i++;
        }

        var fieldWrapper = $("<div class=\"" + toAdd + "\"/>");
        if (toAdd === 'time') {
            var fName = $("<input type=\"time\" class=\"newfield\" id=\"time-" + intId + "\" name=\"time[]\"  required/>");

        } else {
            var fName = $("<select class=\"newfield\" id=\"field-" + intId + "\" name=\"field[]\" /></select>");
            var options = $(" <option value=\"our_part_number\">Our Part Number</option> \
                              <option value=\"quantity\">Quantity</option> \
                              <option value=\"upc\">UPC Code</option> \
                              <option value=\"brand\">Brand Code</option>"
                          );
            fName.append(options);

        }

        inventoryApp[toAdd + 'Form'].push(toAdd + '-' + intId);

        var removeButton = $("<button type=\"button\" class=\"minus\"><i class=\"fa fa-minus\" aria-hidden=\"true\"></i></button>");

        removeButton.click(function () {
            var index = inventoryApp[toAdd + 'Form'].indexOf(toAdd + '-' + intId);
            inventoryApp[toAdd + 'Form'].splice(index, 1);
            $('.testbox').css("height", $('.testbox').height() - 40);
            $(this).parent().remove();
        });

        fieldWrapper.append(fName);
        fieldWrapper.append(removeButton);

        $("#" + toAdd + "div").append(fieldWrapper);

        /*
        var time_ids = $('#timediv .newfield').map(function () {
            return $(this).attr('id');
        }).get();
       

        console.log(inventoryApp.timeForm);
        console.log(inventoryApp.fileForm);
        console.log('\n');
         */

        $('.testbox').css("height", $('.testbox').height() + 40);

    });

    $('.sendvia').on('click', function () {

        var selected = $("input[type='radio'][name='gender']:checked").val();
        var addHeight = $("#timediv div").length + $("#filediv div").length - 2;

        if (selected === 'FTP') {
            $('.testbox').css("height", 970 + addHeight * 40);
        } else {
            $('.testbox').css("height", 810 + addHeight * 40);
        }

        $('.testbox').toggleClass('ftp');
        $('.email').toggle('slow');
        $('.ftplabel').toggleClass('labelhide');
        $('.ftpform').toggle('slow');
    });


    $('#cnumber').on('blur', function () {
        var customerNumber = $(this).val();
        if (customerNumber != '') {
            $.getJSON('/Customer/Number/' + customerNumber, function (result) {
                if (result.length > 0) {
                    $('#cname').val(result[0].CUSTOMER_NAME);
                } else {
                    $('#cname').val('');
                }
            });
        }
        return false;
    });

    $('#cname').on('blur', function () {
        var customerName = $(this).val();
        if (customerName != '') {
            $.getJSON('/Customer/Name/' + customerName, function (result) {
                if (result.length > 0) {
                    $('#cnumber').val(result[0].CUSTOMER_NO);
                } else {
                    $('#cnumber').val('');
                }
            });
        }
        return false;
    });
});