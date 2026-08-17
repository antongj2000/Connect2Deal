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

// ---------- Live badges + toast ----------
(function () {
    if (typeof signalR === 'undefined') return;

    const msgBadge = document.querySelector('[data-badge="messages"]');
    const notifBadge = document.querySelector('[data-badge="notifications"]');

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect()
        .build();

    window.c2dConnection = connection;

    function bump(badge) {
        if (!badge) return;
        const current = parseInt(badge.textContent, 10) || 0;
        badge.textContent = current + 1;
        badge.classList.remove('is-hidden');
    }

    function toast(text) {
        const box = document.createElement('div');
        box.className = 'c2d-toast';
        box.textContent = text;
        document.body.appendChild(box);
        requestAnimationFrame(function () {
            box.classList.add('is-visible');
        });
        setTimeout(function () {
            box.classList.remove('is-visible');
            setTimeout(function () { box.remove(); }, 300);
        }, 4000);
    }

    connection.on("InboxUpdate", function (data) {
        if (data && data.conversationId === window.c2dActiveConversation) return;
        bump(msgBadge);
        toast("New message");
    });

    connection.on("NotificationUpdate", function (data) {
        bump(notifBadge);
        toast(data.message);
    });

    connection.onreconnected(function () {
        connection.invoke("JoinUserChannel").catch(console.error);
        if (window.c2dActiveConversation) {
            connection.invoke("JoinConversation", window.c2dActiveConversation).catch(console.error);
        }
    });

    window.c2dReady = connection.start()
        .then(function () {
            return connection.invoke("JoinUserChannel");
        })
        .catch(console.error);
})();