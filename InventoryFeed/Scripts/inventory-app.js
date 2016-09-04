﻿$(document).ready(function () {

    window.adjustheight = function () {
        var customerForm = $('#customerForm');
        var sendvia = customerForm.find('input[name="sendvia"]:checked').val();
        var addHeight = $("#timediv div").length + $("#filediv div").length - 2;
       
        if (sendvia === 'ftp') {
            $('.testbox').css("height", 970 + addHeight * 40);
        } else {
            $('.testbox').css("height", 810 + addHeight * 40);
        }
    }

    var inventoryApp = {
        time: 0,
        file: 0,
        timeForm: [],
        fileForm: []
    };

 

    $("input").prop('required', true);

    $(".radio").on('click', function () {
        var id = $(this).attr("for");
        var name = $('#' + id).attr("name");
        $('#' + id).prop("checked", true)
 
    });

    $("#submit").on('click', function () {

        
       
        var customerNumber = $("#cnumber").val();
        var required = "";
        var focus = new Array();
        var index=0;
        var time_required = 0;
        var fields_required = 0;
       

        var customerForm = $('#customerForm');

        var controller = customerForm.find('input[name="controller"]').val();
        var method = customerForm.find('input[name="method"]').val();
        var if_id = customerForm.find('input[name="if_id"]').val();

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

 
        var time = $("input[name='time[]']")
              .map(function () { return $(this).val(); }).get();

       
        var tempArray = new Array();
        var a = 0;
        time.forEach(function (entry) {
            if (entry != "") tempArray[a] = entry + ":00";
            else {
                tempArray[a] = "";
                time_required = 1;
            }
            a++;
        });

        time = tempArray.join();


        var field = $("select[name='field[]']")
              .map(function () { return $(this).val(); }).get();

        /*a = 0;
        field.forEach(function (entry) {
            if (entry == "") field_required = 1;
            a++;
        }); */


        if (customerNumber == "") {
            required += "\nCustomer Number is Required";
            focus[index] = "cnumber";
            index++;
        }

        if ( sendvia== "email" && email=="") {
            required += "\nEmail Address is Required";
            focus[index] = "email_address";
            index++;
        }

        if (sendvia == "ftp") {
            if (ftphostname == "") {
                required += "\nFTP hostname is Required";
                focus[index] = "ftphostname";
                index++;
            }

            if (ftpusername == "") {
                required += "\nFTP username is Required";
                focus[index] = "ftpusername";
                index++;
            }

            if (ftppassword == "") {
                required += "\nFTP password is Required";
                focus[index] = "ftppassword";
                index++;
            }
        }

        if (time_required == 1) {
            required += "\nTime is Required";
            focus[index] = "time-0";
            index++;
        }

 
        console.log(focus);
        if (required != "") {
            alert(required);
            $("#" + focus[0]).focus();
            return false;
        }

        var yourJsonObject = {
            customer_no: customerNumber,
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
        
        console.log(yourJsonObject);
        
        var url = "";
        var feed_direct;
        if (controller == "Customer" && method == "Create") {
            url = '/Customer/SendRequest/' + customerNumber;
            feed_direct = "Feed";
             
        }
        else if (controller == "Customer" && method == "Update") {
            url = '/Customer/FeedEdit/' + if_id;
            feed_direct = "../Feed";
        }
        $.post(url,
            $.param(yourJsonObject, true)
        ,
       function (data, status) {

           if (status == "success") {
               window.location.href = feed_direct;

           }
       });  

     

        return false;
    });

    var removeButton = $(".minus");
    removeButton.on('click',function () {
        $('.testbox').css("height", $('.testbox').height() - 40);
        $(this).parent().remove();
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
            var options = $(" <option value=\"PART_NO\">Part Number</option>"+
                       "<option value=\"INVOICED_QTY\">Quantity</option>"+
                       "<option value=\"upc\">UPC Code</option>"+
                        "<option value=\"brand\">Brand Code</option>"
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

        var selected = $('#customerForm').find('input[name="sendvia"]:checked').val();

        console.log(selected);

        var addHeight = $("#timediv div").length + $("#filediv div").length - 2;

        if (selected === 'ftp') {
            $('.testbox').css("height", 970 + addHeight * 40);
        } else {
            $('.testbox').css("height", 810 + addHeight * 40);
        }

        
            $('.testbox').toggleClass('ftp');
          
            $('.ftplabel').toggleClass('labelhide');
            $('.ftpform').toggleClass('labelhide');
           
            $('.email').toggleClass('labelhide');
            

            /*if (selected == "ftp") {
                $('.email').toggle('slow');
            }
            else if (selected == "email") {
            
                $('.ftpform').toggle('slow');
            } */
 
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


    $("a.lnkDelete").on("click", function () {
        
        var id = $(this).attr("id");
        var answer = confirm("You are about to delete this user with ID " + id + " . Continue?");
        var yourJsonObject = {
            if_id: id
        };

        if (answer) {
           $.post('/Customer/FeedDelete/' + id,
               $.param(yourJsonObject, true)
              ,
              function (data, status) {
                  if (status == "success") {
                      window.location.href = 'Feed';
                  } 
              });
        }
           
           // DeleteUser(id);
        return false;
    });

    /*$("a.lnkEdit").on("click", function () {
        var id = $(this).attr("id");
 
        var yourJsonObject = {
            if_id: id
        };

        window.location.href = '/Customer/Update/' + id;
        return false;
    }); */

    window.lnkDelete = function (id) {
        var answer = confirm("You are about to delete this user with ID " + id + " . Continue?");
        var yourJsonObject = {
            if_id: id
        };

        if (answer) {
            $.post('/Customer/FeedDelete/' + id,
                $.param(yourJsonObject, true)
               ,
               function (data, status) {
                   if (status == "success") {
                       window.location.href = 'Feed';
                   }
               });
        }
    }

    window.lnkEdit = function (id) {
        window.location.href = '/Customer/Update/' + id;
        return false;
    }

    $("a#lnkCreate").on("click", function () {
        var dt = $('#example').DataTable();
        dt.table().state.clear();

        window.location.href = '/Customer/Create';
        return false;
    });

});


