$(document).ready(function () {
    $("form[name='registration']").validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                required: true,
                minlength: 6
            }
        },
        messages: {
            Email: {
                required: "Please provide a email address",
                email: "Please enter a valid email address"
            },
            Password: {
                required: "Please provide a password",
                minlength: "Your password must be at least 6 characters long"
            },
            ConfirmPassword: {
                required: "Please provide a password",
                minlength: "Your password must be at least 6 characters long"
            }
        },
        submitHandler: function (form) {
            form.submit();
        }
    });

    $("form[name='login']").validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 6
            }
        },
        messages: {
            Email: {
                required: "Please provide a email address",
                email: "Please enter a valid email address"
            },
            Password: {
                required: "Please provide a password",
                minlength: "Your password must be at least 6 characters long"
            }
        },
        submitHandler: function (form) {
            form.submit();
        }
    });
})

$(".btn-submit-register").click(function () {
    SubmitFormJqueryRegister();
})

$(".btn-submit-login").click(function () {
    $.ajax({
        type: "POST",
        async: false,
        url: "/Account/Login",
        data: { Email: $("#emailLogin").val(), Password: $("#passWordLogin").val() },
        dataType: 'json',
        success: function (data, status) {
            if (data.status == "Ok") {
                location.reload();
            } else {
                $("#loginEmailError").html('');
                $("#loginEmailError").append("<label id='emailLogin-error' class='error' for='emailLogin'>The email or password is incorrect.</label>")
            }
        },
        error: function (data, status) {
            //alert(data.status);
        }
    })
})

function SubmitFormJqueryRegister() {
    $.ajax({
        type: "GET",
        async: false,
        url: "/Account/CheckExistsEmail",
        data: { email: $("#registrationEmail").val() },
        dataType: 'json',
        success: function (data, status) {
            if (data.status == "Ok") {
                $("form[name='registration']").submit();
            } else {
                $("#registrationEmailError").html('');
                $("#registrationEmailError").append("<label id='registrationEmail-error' class='error' for='registrationEmail'>The email address is exists.</label>")
            }

        },
        error: function (data, status) {
            //alert(data.status);
        }
    })
}
