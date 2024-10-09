// Dashboard.js
document.addEventListener("DOMContentLoaded", () => {
    const menuItems = document.querySelectorAll(".menu-item");
    const contentSections = document.querySelectorAll(".content-section");

    menuItems.forEach(item => {
        item.addEventListener("click", (event) => {
            event.preventDefault();
            const contentId = item.getAttribute("data-content");

            // Hide all sections
            contentSections.forEach(section => {
                section.classList.remove("active");
            });

            // Show the selected section
            document.getElementById(contentId).classList.add("active");
        });
    });
});
