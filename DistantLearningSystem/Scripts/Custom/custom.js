function submitForm() {
    console.log('enter submit form!');
    var Name = $("#Name");
    var LastName = $("#LastName");
    var Mail = $("#Email");
    var PassWord = $("#Password");
    var ConfirmPassword = $("#ConfirmPassword");
    var imageUpload = $("#imageUpload");

    console.log(Name.val());
    if (Name.val() == "" ||
        Mail.val() == "" ||
        PassWord.val() == "" ||
        ConfirmPassword.val() == "") {
        document.getElementById("information").innerHTML = 'Заполнены не все поля';
        $("#info").show();
    }

    console.log(PassWord.val());
    console.log(ConfirmPassword.val());

    if (ConfirmPassword.val() != PassWord.val()) {
        document.getElementById("information").innerHTML = 'Пароли не совпадают';
        $("#info").show();
    }
    else {
        $("#main_form").submit();
    }
}

function submitEditForm() {

    var PassWord = $("#Password");
    var ConfirmPassword = $("#ConfirmPassword");

    console.log(Name.val());

    console.log(PassWord.val());
    console.log(ConfirmPassword.val());

    if (ConfirmPassword.val() != PassWord.val()) {
        document.getElementById("information").innerHTML = 'Пароли не совпадают';
        $("#info").show();
    }
    else {
        $("#main_form").submit();
    }
}

function onChange() {
    console.log('here!');
    var val = $('#whoareu').val();
    console.log(val);

    //var querystr = '@Url.Action("AjaxRegistrationFormLoad","User", new { data =' + val + '})';
    //console.log(querystr);
    //$.get( querystr, function(data) {
    //    $('#form').html(data);
    //}); 
    $.ajax({
        url: "/User/AjaxRegistrationFormLoad",
        data: { data: val },
        //datatype: "html",
        //type: "GET",
        success: function (data) {
            console.log(data);
            $("#form").replaceWith(data);
        }
    });
}