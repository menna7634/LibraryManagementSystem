$(document).ready(function () {
    // Toggle password visibility
    $('.btn-toggle-password').on('click', function () {
        const passwordField = $(this).siblings('input');
        const passwordFieldType = passwordField.attr('type');

        if (passwordFieldType === 'password') {
            passwordField.attr('type', 'text');
            $(this).text('Hide');
        } else {
            passwordField.attr('type', 'password');
            $(this).text('Show');
        }
    });

    // Form validation before submission
    $('#loginForm').on('submit', function (e) {
        let email = $('input[name="Email"]').val();
        let password = $('input[name="Password"]').val();
        let isValid = true;

        // Email validation
        if (!email) {
            isValid = false;
            $('input[name="Email"]').next('.text-danger').text('Email is required.');
        } else {
            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailPattern.test(email)) {
                isValid = false;
                $('input[name="Email"]').next('.text-danger').text('Invalid email format.');
            }
        }

        // Password validation
        if (!password) {
            isValid = false;
            $('input[name="Password"]').next('.text-danger').text('Password is required.');
        }

        if (!isValid) {
            e.preventDefault();
            $('#errorMessage').text('Please fix the errors above and try again.').show();
        }
    });
});
