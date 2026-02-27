(function () {
    const modalHost = document.getElementById("modalHost");

    async function openProfileModal(candidateId) {
        const res = await fetch(`/Municipality/ProfileModal?id=${candidateId}`, {
            headers: { "X-Requested-With": "fetch" }
        });

        const html = await res.text();
        modalHost.innerHTML = html;

        const modalEl = document.getElementById("candidateModal");
        const modal = new bootstrap.Modal(modalEl);
        modal.show();

        const revealForm = modalEl.querySelector("#revealForm");
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
                const contactArea = modalEl.querySelector("#contactArea");
                if (contactArea) contactArea.innerHTML = contactHtml;
            });
        }

        modalEl.addEventListener("hidden.bs.modal", () => {
            modalHost.innerHTML = "";
        }, { once: true });S
    }

    document.addEventListener("click", (e) => {
        const btn = e.target.closest(".js-view-profile");
        if (!btn) return;

        const id = btn.getAttribute("data-candidate-id");
        if (!id) return;

        openProfileModal(id);
    });
})();