document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("registerForm");
    const password = document.querySelector('input[name="Password"]');
    const confirmPassword = document.querySelector('input[name="ConfirmPassword"]');
    const dateOfBirth = document.getElementById("dateOfBirth");
    const passwordStrength = document.getElementById("password-strength");
    const addFieldButton = document.getElementById("addField");
    const additionalFieldsContainer = document.getElementById("additionalFieldsContainer");

    confirmPassword.addEventListener("input", function () {
        if (password.value !== confirmPassword.value) {
            confirmPassword.setCustomValidity("Passwords do not match");
        } else {
            confirmPassword.setCustomValidity("");
        }
    });

    password.addEventListener("input", function () {
        const strength = password.value.length >= 8 ? "Strong" : "Weak";
        passwordStrength.textContent = `Password Strength: ${strength}`;
    });

    // Validate Date of Birth before submission
    dateOfBirth.addEventListener("change", function () {
        const selectedDate = new Date(dateOfBirth.value);
        const today = new Date();
        today.setHours(0, 0, 0, 0); 

        if (selectedDate >= today) {
            dateOfBirth.setCustomValidity("Date of Birth must be before today");
            dateOfBirth.reportValidity();
        } else {
            dateOfBirth.setCustomValidity("");
        }
    });

 
});

