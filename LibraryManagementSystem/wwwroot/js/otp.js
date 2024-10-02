const otpInputs = document.querySelectorAll('.otp-input');
const otpHiddenField = document.getElementById('otpHiddenField');

otpInputs.forEach((input, index) => {
    input.addEventListener('input', function () {
        if (this.value.length === 1 && index < otpInputs.length - 1) {
            otpInputs[index + 1].focus(); 
        }
        let otpValue = '';
        otpInputs.forEach(i => otpValue += i.value);
        otpHiddenField.value = otpValue;
    });
});