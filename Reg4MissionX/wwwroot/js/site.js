(function () {
    // UI-only auth flag
    const KEY = "r4m_isLoggedIn";

    function isLoggedIn() {
        return localStorage.getItem(KEY) === "true";
    }

    function setLoggedIn(value) {
        localStorage.setItem(KEY, value ? "true" : "false");
    }

    // Tailwind uses "hidden" instead of Bootstrap "d-none"
    function show(el, shouldShow) {
        if (!el) return;
        el.classList.toggle("hidden", !shouldShow);
    }

    function updateNavbar() {
        const logged = isLoggedIn();

        // Logged out
        show(document.getElementById("navRegister"), !logged);
        show(document.getElementById("navLogin"), !logged);
        show(document.getElementById("navHome"), !logged);

        // Logged in
        show(document.getElementById("navDashboard"), logged);
        show(document.getElementById("navSearch"), logged);
        show(document.getElementById("navProfile"), logged);
        show(document.getElementById("navLogout"), logged);

        // If ever added a navPrivacy id later, this will work:
        show(document.getElementById("navPrivacy"), !logged);
    }

    // UI-only guard: redirect if page requires auth
    function guardProtectedPages() {
        const requires = document.querySelector("[data-requires-auth='true']");
        if (!requires) return;

        if (!isLoggedIn()) {
            window.location.href = "/#login";
        }
    }

    // When logged in, clicking the brand should go to Dashboard (not back to login page)
    function hookBrandLink() {
        const brand = document.getElementById("brandLink");
        if (!brand) return;

        brand.addEventListener("click", (e) => {
            if (!isLoggedIn()) return; // normal behavior when logged out
            e.preventDefault();
            window.location.href = "/Dashboard";
        });
    }

    // If someone clicks "Logga in" while already logged in, send them to Dashboard instead
    function hookLoginNav() {
        const loginLink = document.getElementById("navLogin");
        if (!loginLink) return;

        loginLink.addEventListener("click", (e) => {
            if (!isLoggedIn()) return; // normal: go to /#login
            e.preventDefault();
            window.location.href = "/Dashboard";
        });
    }

    // Login form (UI-only)
    function hookLoginForm() {
        const form = document.getElementById("loginForm");
        if (!form) return;

        form.addEventListener("submit", function (e) {
            e.preventDefault();
            setLoggedIn(true);
            updateNavbar();
            window.location.href = "/Dashboard";
        });
    }

    // Logout (UI-only) - layout uses id="navLogout" + data-action="logout"
    function hookLogout() {
        const logoutBtn = document.getElementById("navLogout") || document.querySelector('[data-action="logout"]');
        if (!logoutBtn) return;

        logoutBtn.addEventListener("click", function (e) {
            e.preventDefault();
            setLoggedIn(false);
            updateNavbar();
            window.location.href = "/"; // back to logged-out Index
        });
    }

    // Init
    updateNavbar();
    hookBrandLink();
    hookLoginNav();
    hookLoginForm();
    hookLogout();
    guardProtectedPages();
})();