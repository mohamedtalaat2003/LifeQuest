// Life Quest Dashboard Toggle Script
document.addEventListener("DOMContentLoaded", function () {
    const sidebarToggle = document.getElementById("sidebarToggle");
    const sidebar = document.getElementById("sidebar");

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener("click", function (e) {
            e.preventDefault();
            sidebar.classList.toggle("active");
        });
    }
});
