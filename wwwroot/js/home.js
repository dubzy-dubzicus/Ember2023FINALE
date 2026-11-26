(function () {
    'use strict';

    const S = window.EmberSite;
    let allRooms = [];

    function roomCard(room) {
        const id = String(room.roomId ?? '');
        const name = room.name || 'Unnamed room';
        const description = room.description || 'No description available.';
        const owner = room.ownerName || 'Unknown creator';
        const capacity = room.maxPlayers ?? 20;

        return '<a class="card room-card" href="/Room/' + encodeURIComponent(id) + '">' +
            '<div class="card-media">' +
                S.mediaHtml(name, room.imageName, 640, 360) +
                '<span class="card-badge">Room</span>' +
                '<span class="card-rank">#' + S.escapeHtml(id) + '</span>' +
            '</div>' +
            '<div class="card-body">' +
                '<div class="card-title">' + S.escapeHtml(name) + '</div>' +
                '<div class="card-desc">' + S.escapeHtml(description) + '</div>' +
                '<div class="card-meta">' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-users"></use></svg>' + S.escapeHtml(capacity) + ' players</span>' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-user"></use></svg>' + S.escapeHtml(owner) + '</span>' +
                '</div>' +
            '</div>' +
        '</a>';
    }

    function renderRooms(list, sort) {
        const grid = document.getElementById('roomsGrid');
        if (!grid) return;

        let items = list.slice();
        if (sort === 'capacity') {
            items.sort((a, b) => (b.maxPlayers ?? 0) - (a.maxPlayers ?? 0));
        } else if (sort === 'name') {
            items.sort((a, b) => String(a.name || '').localeCompare(String(b.name || '')));
        } else if (sort === 'newest') {
            items.sort((a, b) => Number(b.roomId || 0) - Number(a.roomId || 0));
        }

        if (!items.length) {
            grid.innerHTML = '<div class="empty-state"><h2>No rooms found</h2><p class="no-data">No public rooms are available yet.</p></div>';
            return;
        }

        grid.innerHTML = items.slice(0, 12).map(roomCard).join('');
    }

    function updateStats(list) {
        const roomCount = document.getElementById('roomCount');
        const communityRooms = document.getElementById('communityRooms');
        const communityCreators = document.getElementById('communityCreators');
        const communityCapacity = document.getElementById('communityCapacity');

        const creators = new Set(list.map(r => r.ownerName || 'Unknown').filter(Boolean));
        const capacity = list.reduce((sum, r) => sum + (Number(r.maxPlayers) || 0), 0);

        if (roomCount) roomCount.textContent = String(list.length);
        if (communityRooms) communityRooms.textContent = String(list.length);
        if (communityCreators) communityCreators.textContent = String(creators.size);
        if (communityCapacity) communityCapacity.textContent = capacity.toLocaleString();
    }

    function renderCreators(list) {
        const container = document.getElementById('creatorList');
        if (!container) return;

        const counts = new Map();
        list.forEach(room => {
            const owner = room.ownerName || 'Unknown creator';
            counts.set(owner, (counts.get(owner) || 0) + 1);
        });

        const top = Array.from(counts.entries()).sort((a, b) => b[1] - a[1]).slice(0, 5);
        if (!top.length) {
            container.innerHTML = '<div class="empty-state"><h2>No creators yet</h2><p class="no-data">Room owners will appear here.</p></div>';
            return;
        }

        container.innerHTML = top.map(([owner, count]) =>
            '<a class="creator-row" href="/Profile/' + encodeURIComponent(owner) + '">' +
                '<span class="avatar">' + S.escapeHtml(S.initials(owner)) + '</span>' +
                '<span class="creator-row-main"><strong>' + S.escapeHtml(owner) + '</strong><span>Rooms created</span></span>' +
                '<span class="creator-count">' + count + '</span>' +
            '</a>'
        ).join('');
    }

    async function loadRooms() {
        const grid = document.getElementById('roomsGrid');
        try {
            const data = await S.apiGet('/api/rooms/v1/all');
            allRooms = Array.isArray(data) ? data : [];
            renderRooms(allRooms, 'featured');
            updateStats(allRooms);
            renderCreators(allRooms);
        } catch (e) {
            if (grid) {
                grid.innerHTML = '<div class="error-state"><h2>Unable to load rooms</h2><p class="no-data">' + S.escapeHtml(e.message) + '</p></div>';
            }
        }
    }

    async function loadHomeNews() {
        const grid = document.getElementById('newsPreviewGrid');
        if (!grid) return;

        try {
            const data = await S.apiGet('/api/news/v1/all');
            const articles = Array.isArray(data) ? data.slice(0, 3) : [];
            if (!articles.length) {
                grid.innerHTML = '<div class="empty-state"><h2>No news yet</h2><p class="no-data">Announcements will appear here.</p></div>';
                return;
            }

            grid.innerHTML = articles.map(article => {
                const title = article.title || 'Untitled update';
                return '<article class="card">' +
                    '<div class="card-media">' + S.mediaHtml(title, article.imageUrl, 640, 320) + '<span class="card-badge">News</span></div>' +
                    '<div class="card-body">' +
                        '<div class="news-card-date"><svg class="icon"><use href="#i-calendar"></use></svg> ' + S.escapeHtml(S.formatDate(article.createdAt)) + '</div>' +
                        '<div class="card-title" style="margin-top:8px">' + S.escapeHtml(title) + '</div>' +
                        '<div class="card-desc">' + S.escapeHtml(article.summary || article.content || '') + '</div>' +
                    '</div>' +
                '</article>';
            }).join('');
        } catch (e) {
            grid.innerHTML = '<div class="error-state"><h2>Unable to load news</h2><p class="no-data">' + S.escapeHtml(e.message) + '</p></div>';
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('[data-room-sort]').forEach(function (button) {
            button.addEventListener('click', function () {
                document.querySelectorAll('[data-room-sort]').forEach(b => b.classList.remove('active'));
                button.classList.add('active');
                renderRooms(allRooms, button.dataset.roomSort);
            });
        });
    });

    loadRooms();
    loadHomeNews();
})();
