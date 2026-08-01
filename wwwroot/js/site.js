(function () {
    // ---- sidebar / burger toggle (na svim dashboard stranicama) ----
    const shell = document.getElementById('c2dShell');
    const burger = document.getElementById('c2dBurger');
    const overlay = document.getElementById('c2dOverlay');

    if (shell && burger) {
        if (window.innerWidth <= 900) shell.classList.add('is-collapsed');
        burger.addEventListener('click', () => shell.classList.toggle('is-collapsed'));
        if (overlay) overlay.addEventListener('click', () => shell.classList.add('is-collapsed'));
    }

    // ---- detail modal (Index, Notifications — gdje postoji) ----
    const modal = document.getElementById('c2dModal');
    const modalClose = document.getElementById('c2dModalClose');
    const modalBody = document.getElementById('c2dModalContent');

    if (modal && modalClose && modalBody) {
        window.c2dCloseModal = function () {
            modal.classList.remove('is-open');
            document.body.style.overflow = '';
        };

        modalClose.addEventListener('click', window.c2dCloseModal);
        modal.addEventListener('click', e => { if (e.target === modal) window.c2dCloseModal(); });
        document.addEventListener('keydown', e => {
            if (e.key === 'Escape' && modal.classList.contains('is-open')) window.c2dCloseModal();
        });
    }
})();