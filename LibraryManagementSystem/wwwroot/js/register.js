document.addEventListener("DOMContentLoaded", function () {
    const password = document.getElementById("password");
    const confirmPassword = document.getElementById("confirmPassword");
    const dateOfBirth = document.getElementById("dateOfBirth");
    const passwordStrength = document.getElementById("password-strength");
    const togglePassword = document.getElementById("togglePassword");
    const toggleConfirmPassword = document.getElementById("toggleConfirmPassword");

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

    togglePassword.addEventListener("click", function () {
        const type = password.type === "password" ? "text" : "password";
        password.type = type;
        this.textContent = type === "password" ? "Show" : "Hide";
    });

    toggleConfirmPassword.addEventListener("click", function () {
        const type = confirmPassword.type === "password" ? "text" : "password";
        confirmPassword.type = type;
        this.textContent = type === "password" ? "Show" : "Hide";
    });
});
