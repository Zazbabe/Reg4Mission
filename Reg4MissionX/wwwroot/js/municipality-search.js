(function () {
    const modalHost = document.getElementById("modalHost");

    function qs(root, selector) {
        return root ? root.querySelector(selector) : null;
    }

    function openModal(modalEl) {
        // Overlay/backdrop (optional if your modal HTML includes one)
        const overlay = qs(modalEl, "[data-modal-overlay]");
        if (overlay) overlay.classList.remove("hidden");

        // Modal panel
        modalEl.classList.remove("hidden");
        modalEl.classList.add("block");

        // Prevent background scroll while modal is open
        document.documentElement.classList.add("overflow-hidden");
    }

    function closeModal(modalEl) {
        const overlay = qs(modalEl, "[data-modal-overlay]");
        if (overlay) overlay.classList.add("hidden");

        modalEl.classList.add("hidden");
        modalEl.classList.remove("block");

        document.documentElement.classList.remove("overflow-hidden");

        // Clear injected modal HTML so we don't keep old DOM around
        modalHost.innerHTML = "";
    }

    async function openProfileModal(candidateId) {
        const res = await fetch(`/Municipality/ProfileModal?id=${candidateId}`, {
            headers: { "X-Requested-With": "fetch" }
        });

        const html = await res.text();
        modalHost.innerHTML = html;

        const modalEl = document.getElementById("candidateModal");
        if (!modalEl) return;

        // Wire up close buttons / overlay click / Escape key
        const closeBtn = qs(modalEl, "[data-modal-close]");
        const overlayBtn = qs(modalEl, "[data-modal-overlay]");

        if (closeBtn) {
            closeBtn.addEventListener("click", () => closeModal(modalEl), { once: true });
        }

        if (overlayBtn) {
            overlayBtn.addEventListener("click", () => closeModal(modalEl), { once: true });
        }

        function onKeyDown(e) {
            if (e.key === "Escape") {
                closeModal(modalEl);
                document.removeEventListener("keydown", onKeyDown);
            }
        }
        document.addEventListener("keydown", onKeyDown);

        // Wire reveal contact form
        const revealForm = qs(modalEl, "#revealForm");
        if (revealForm) {
            revealForm.addEventListener("submit", async (e) => {
                e.preventDefault();

                const formData = new FormData(revealForm);

                const revealRes = await fetch("/Municipality/RevealContact", {
                    method: "POST",
                    body: formData,
                    headers: { "X-Requested-With": "fetch" }
                });

                const contactHtml = await revealRes.text();
                const contactArea = qs(modalEl, "#contactArea");
                if (contactArea) contactArea.innerHTML = contactHtml;
            });
        }

        // Finally show the modal
        openModal(modalEl);
    }

    document.addEventListener("click", (e) => {
        const btn = e.target.closest(".js-view-profile");
        if (!btn) return;

        const id = btn.getAttribute("data-candidate-id");
        if (!id) return;

        openProfileModal(id);
    });
})();