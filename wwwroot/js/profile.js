(function () {
    'use strict';

    const S = window.EmberSite;
    const username = (document.getElementById('profileUsername')?.textContent || '').trim();
    let currentProfileTab = 'feed';

    function setStat(id, value) {
        const el = document.getElementById(id);
        if (el) el.textContent = String(value ?? 0);
    }

    async function loadProfileData() {
        try {
            const data = await S.apiGet('/api/players/v1/' + encodeURIComponent(username));
            if (!data) return;

            const displayName = data.displayName || data.name || data.username || username;
            const handle = data.username || username;

            const title = document.getElementById('profileUsername');
            const handleEl = document.getElementById('usernameDisplay');
            const bio = document.getElementById('profileBio');
            const account = document.getElementById('profileAccountId');

            if (title) title.textContent = displayName;
            if (handleEl) handleEl.textContent = '@' + handle;
            if (bio) bio.textContent = data.bio || 'This player has not added a bio yet.';
            if (account) account.textContent = '#' + (data.accountId ?? '—');

            setStat('roomsCount', data.roomsCount);
            setStat('inventionsCount', data.inventionsCount);
            setStat('friendsCount', data.friendsCount);

            const initial = document.getElementById('profileAvatarInitial');
            if (initial) initial.textContent = S.initials(displayName);

            const image = document.getElementById('profileAvatarImage');
            if (image) {
                if (data.profileImage) {
                    image.src = S.imageUrl(data.profileImage, 180, 180) + '&cropsquare=true';
                    image.hidden = false;
                    image.onerror = function () {
                        image.hidden = true;
                        if (initial) initial.style.display = 'grid';
                    };
                } else {
                    image.hidden = true;
                }
            }

            document.title = displayName + ' - Ember Network';
        } catch (e) {
            const bio = document.getElementById('profileBio');
            if (bio) bio.textContent = 'Unable to load profile details: ' + e.message;
        }
    }

    function feedCard(item) {
        const title = item.title || item.name || 'Untitled post';
        const type = item.type || 'post';
        return '<article class="card">' +
            '<div class="card-media">' +
                S.mediaHtml(title, item.imageName, 640, 360) +
                '<span class="card-badge">' + S.escapeHtml(type) + '</span>' +
            '</div>' +
            '<div class="card-body">' +
                '<div class="news-card-date"><svg class="icon"><use href="#i-calendar"></use></svg> ' + S.escapeHtml(S.formatDate(item.createdAt)) + '</div>' +
                '<div class="card-title" style="margin-top:8px">' + S.escapeHtml(title) + '</div>' +
                '<div class="card-desc">' + S.escapeHtml(item.description || 'No description available.') + '</div>' +
            '</div>' +
        '</article>';
    }

    function roomCard(item) {
        const id = String(item.roomId ?? '');
        const name = item.name || 'Unnamed room';
        return '<a class="card" href="/Room/' + encodeURIComponent(id) + '">' +
            '<div class="card-media">' +
                S.mediaHtml(name, item.imageName, 640, 360) +
                '<span class="card-badge">Room</span>' +
            '</div>' +
            '<div class="card-body">' +
                '<div class="card-title">' + S.escapeHtml(name) + '</div>' +
                '<div class="card-desc">' + S.escapeHtml(item.description || 'No description available.') + '</div>' +
                '<div class="card-meta">' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-users"></use></svg>' + S.escapeHtml(item.maxPlayers ?? 20) + ' players</span>' +
                '</div>' +
            '</div>' +
        '</a>';
    }

    function setMeta(count) {
        const meta = document.getElementById('profileMeta');
        if (!meta) return;
        const label = meta.querySelector('span');
        if (label) label.textContent = count + ' ' + currentProfileTab + ' items';
    }

    async function loadProfileContent(tab) {
        const content = document.getElementById('profileContent');
        if (!content) return;

        content.innerHTML = '<div class="empty-state"><h2>Loading…</h2><p class="no-data">Fetching public activity.</p></div>';

        try {
            const endpoint = tab === 'rooms'
                ? '/api/rooms/v1/user/' + encodeURIComponent(username)
                : '/api/feed/v1/user/' + encodeURIComponent(username);

            const data = await S.apiGet(endpoint);
            const items = Array.isArray(data) ? data : [];
            setMeta(items.length);

            if (!items.length) {
                content.innerHTML = '<div class="empty-state"><h2>Nothing here yet</h2><p class="no-data">No public items in this section.</p></div>';
                return;
            }

            const html = tab === 'rooms' ? items.map(roomCard).join('') : items.map(feedCard).join('');
            content.innerHTML = '<div class="profile-grid">' + html + '</div>';
        } catch (e) {
            content.innerHTML = '<div class="error-state"><h2>Unable to load activity</h2><p class="no-data">' + S.escapeHtml(e.message) + '</p></div>';
        }
    }

    function setProfileTab(tab, button) {
        currentProfileTab = tab;
        document.querySelectorAll('.profile-tab').forEach(b => b.classList.remove('active'));
        if (button) button.classList.add('active');
        loadProfileContent(tab);
    }

    window.setProfileTab = setProfileTab;

    document.addEventListener('DOMContentLoaded', function () {
        const params = new URLSearchParams(window.location.search);
        const requested = params.get('tab');
        if (requested === 'rooms' || requested === 'feed') {
            currentProfileTab = requested;
            const button = document.querySelector('.profile-tab[data-profile-tab="' + requested + '"]');
            document.querySelectorAll('.profile-tab').forEach(b => b.classList.remove('active'));
            if (button) button.classList.add('active');
        }

        document.querySelectorAll('.profile-tab').forEach(button => {
            button.addEventListener('click', () => setProfileTab(button.dataset.profileTab, button));
        });

        loadProfileData();
        loadProfileContent(currentProfileTab);
    });
})();
