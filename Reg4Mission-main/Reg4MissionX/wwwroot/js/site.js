// wwwroot/js/site.js
// Auth is handled by ASP.NET Identity (cookies).
// This file only handles UI behavior (drawer + small helpers).

(function () {
    "use strict";

    // Run after DOM is ready
    function ready(fn) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", fn);
        } else {
            fn();
        }
    }

    ready(() => {
        // =========================
        // Drawer: open/close
        // =========================
        const menuBtn = document.getElementById("menuBtn");
        const drawer = document.getElementById("drawer");
        const overlay = document.getElementById("drawerOverlay");
        const closeBtn = document.getElementById("drawerCloseBtn");

        function openDrawer() {
            if (!drawer || !overlay || !menuBtn) return;
            drawer.classList.remove("translate-x-full");
            overlay.classList.remove("hidden");
            menuBtn.setAttribute("aria-expanded", "true");
        }

        function closeDrawer() {
            if (!drawer || !overlay || !menuBtn) return;
            drawer.classList.add("translate-x-full");
            overlay.classList.add("hidden");
            menuBtn.setAttribute("aria-expanded", "false");
        }

        if (menuBtn && drawer && overlay && closeBtn) {
            menuBtn.addEventListener("click", openDrawer);
            closeBtn.addEventListener("click", closeDrawer);
            overlay.addEventListener("click", closeDrawer);

            // Close drawer after clicking any navigation link
            drawer.addEventListener("click", (e) => {
                const link = e.target.closest("a");
                if (!link) return;
                closeDrawer();
            });

            // Close drawer on Escape
            document.addEventListener("keydown", (e) => {
                if (e.key === "Escape") closeDrawer();
            });
        }

        // =========================
        // TempData alerts: optional auto-hide
        // Add: data-auto-hide-alert on an alert element to auto-hide it.
        // =========================
        const alerts = document.querySelectorAll("[data-auto-hide-alert]");
        if (alerts.length > 0) {
            setTimeout(() => {
                alerts.forEach(a => a.remove());
            }, 5000);
        }
    });
})();