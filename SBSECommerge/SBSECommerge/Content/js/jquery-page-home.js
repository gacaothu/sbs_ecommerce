$(".btn-submit-register").click(function () {
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
})