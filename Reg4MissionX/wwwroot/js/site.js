(function () {
    const KEY = "r4m_isLoggedIn";

    function isLoggedIn() {
        return localStorage.getItem(KEY) === "true";
    }

    function setLoggedIn(value) {
        localStorage.setItem(KEY, value ? "true" : "false");
    }

    function show(el, shouldShow) {
        if (!el) return;
        el.classList.toggle("d-none", !shouldShow);
    }

    function updateNavbar() {
        const logged = isLoggedIn();

        // Utloggat-läge
        show(document.getElementById("navRegister"), !logged);
        show(document.getElementById("navLogin"), !logged);

        // Inloggat-läge
        show(document.getElementById("navDashboard"), logged);
        show(document.getElementById("navSearch"), logged);
        show(document.getElementById("navProfile"), logged);
        show(document.getElementById("navLogout"), logged);

        // Ska inte synas i app-läge
        show(document.getElementById("navHome"), !logged);
        show(document.getElementById("navPrivacy"), !logged);
    }

    // UI-only "guard"
    function guardProtectedPages() {
        const requires = document.querySelector("[data-requires-auth='true']");
        if (!requires) return;

        if (!isLoggedIn()) {
            window.location.href = "/#login";
        }
    }

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

    function hookLogout() {
        const link = document.getElementById("logoutLink");
        if (!link) return;

        link.addEventListener("click", function (e) {
            e.preventDefault();
            setLoggedIn(false);
            updateNavbar();
            window.location.href = "/";
        });
    }

    // Init
    updateNavbar();
    hookLoginForm();
    hookLogout();
    guardProtectedPages();
})();
