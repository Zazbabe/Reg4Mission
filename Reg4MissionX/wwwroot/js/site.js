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

    function closeOffcanvasIfOpen() {
        const el = document.getElementById("mainOffcanvas");
        if (!el) return;

        if (window.bootstrap?.Offcanvas) {
            const instance = window.bootstrap.Offcanvas.getInstance(el);
            if (instance) instance.hide();
        }
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
    }

    function guardProtectedPages() {
        const path = window.location.pathname.toLowerCase();

        const protectedPaths = ["/dashboard", "/search", "/profile", "/admin"];
        const isProtected = protectedPaths.some(p => path.startsWith(p))
            || document.querySelector("[data-requires-auth='true']");

        if (isProtected && !isLoggedIn()) {
            window.location.replace("/#login");
        }
    }

    function hookLoginForm() {
        const form = document.getElementById("loginForm");
        if (!form) return;

        form.addEventListener("submit", function (e) {
            e.preventDefault();
            setLoggedIn(true);
            updateNavbar();
            closeOffcanvasIfOpen();
            window.location.assign("/Dashboard");
        });
    }

    function hookLogout() {
        const btn =
            document.querySelector("[data-action='logout']") ||
            document.getElementById("navLogout") ||
            document.getElementById("logoutLink");

        if (!btn) return;

        btn.addEventListener("click", function (e) {
            e.preventDefault();

            setLoggedIn(false);
            updateNavbar();
            closeOffcanvasIfOpen();

            setTimeout(() => {
                window.location.assign("/#login");
            }, 50);
        });
    }

    function hookOffcanvasLinkClose() {
        const offcanvas = document.getElementById("mainOffcanvas");
        if (!offcanvas) return;

        offcanvas.addEventListener("click", function (e) {
            const a = e.target.closest("a");
            if (!a) return;

            // Don't interfere with logout (we handle it)
            if (a.id === "navLogout" || a.getAttribute("data-action") === "logout") return;

            closeOffcanvasIfOpen();
        });
    }

    // Init
    updateNavbar();
    guardProtectedPages();
    hookLoginForm();
    hookLogout();
    hookOffcanvasLinkClose();
})();