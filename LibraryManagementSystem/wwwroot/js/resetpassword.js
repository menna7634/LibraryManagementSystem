    function togglePassword(inputId, button) {
        const input = document.getElementById(inputId);
        const isPasswordVisible = input.type === "text";

        input.type = isPasswordVisible ? "password" : "text";

        button.textContent = isPasswordVisible ? "Show" : "Hide";
    }