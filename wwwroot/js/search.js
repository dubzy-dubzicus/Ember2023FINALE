(function () {
    'use strict';

    const S = window.EmberSite;
    const labels = { people: 'people', rooms: 'rooms', inventions: 'inventions' };
    let currentTab = 'people';

    function endpointFor(tab, query) {
        const q = encodeURIComponent(query);
        if (tab === 'rooms') return '/api/rooms/v1/search?q=' + q;
        if (tab === 'inventions') return '/api/inventions/v2/search?q=' + q;
        return '/api/players/v1/search?q=' + q;
    }

    function personCard(item) {
        const name = item.displayName || item.name || item.username || 'Player';
        const username = item.username || name;
        const bio = item.bio || 'No bio has been added yet.';

        return '<a class="card" href="/Profile/' + encodeURIComponent(username) + '">' +
            '<div class="card-media" style="height:150px">' +
                S.mediaHtml(name, item.profileImage, 480, 300) +
                '<span class="card-badge">Player</span>' +
            '</div>' +
            '<div class="card-body">' +
                '<div class="card-title">' + S.escapeHtml(name) + '</div>' +
                '<div class="card-desc">' + S.escapeHtml(bio) + '</div>' +
                '<div class="card-meta">' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-user"></use></svg>@' + S.escapeHtml(username) + '</span>' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-zap"></use></svg>#' + S.escapeHtml(item.accountId ?? '—') + '</span>' +
                '</div>' +
            '</div>' +
        '</a>';
    }

    function roomCard(item) {
        const id = String(item.roomId ?? '');
        const name = item.name || 'Unnamed room';
        return '<a class="card" href="/Room/' + encodeURIComponent(id) + '">' +
            '<div class="card-media">' +
                S.mediaHtml(name, item.imageName, 640, 360) +
                '<span class="card-badge">Room</span>' +
                '<span class="card-rank">#' + S.escapeHtml(id) + '</span>' +
            '</div>' +
            '<div class="card-body">' +
                '<div class="card-title">' + S.escapeHtml(name) + '</div>' +
                '<div class="card-desc">' + S.escapeHtml(item.description || 'No description available.') + '</div>' +
                '<div class="card-meta">' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-users"></use></svg>' + S.escapeHtml(item.maxPlayers ?? 20) + ' players</span>' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-user"></use></svg>' + S.escapeHtml(item.ownerName || 'Unknown') + '</span>' +
                '</div>' +
            '</div>' +
        '</a>';
    }

    function inventionCard(item) {
        const name = item.name || 'Untitled invention';
        return '<article class="card">' +
            '<div class="card-media">' +
                S.mediaHtml(name, item.imageName, 640, 360) +
                '<span class="card-badge">Invention</span>' +
            '</div>' +
            '<div class="card-body">' +
                '<div class="card-title">' + S.escapeHtml(name) + '</div>' +
                '<div class="card-desc">' + S.escapeHtml(item.description || 'No description available.') + '</div>' +
                '<div class="card-meta">' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-calendar"></use></svg>' + S.escapeHtml(S.formatDate(item.createdAt)) + '</span>' +
                    '<span class="card-meta-item"><svg class="icon"><use href="#i-shield"></use></svg>Public</span>' +
                '</div>' +
            '</div>' +
        '</article>';
    }

    function render(data) {
        const grid = document.getElementById('resultsGrid');
        if (!grid) return;

        if (!Array.isArray(data) || !data.length) {
            grid.innerHTML = '<div class="empty-state"><h2>No results found</h2><p class="no-data">Try a different term or category.</p></div>';
            return;
        }

        let html;
        if (currentTab === 'rooms') html = data.map(roomCard).join('');
        else if (currentTab === 'inventions') html = data.map(inventionCard).join('');
        else html = data.map(personCard).join('');
        grid.innerHTML = html;
    }

    function setMeta(query, count) {
        const meta = document.getElementById('resultsMeta');
        if (!meta) return;
        const label = meta.querySelector('span');
        if (label) {
            label.textContent = count + ' ' + labels[currentTab] + ' found for “' + query + '”';
        }
    }

    async function search() {
        const input = document.getElementById('searchInput');
        const grid = document.getElementById('resultsGrid');
        const query = (input?.value || '').trim();

        if (!query) {
            if (grid) {
                grid.innerHTML = '<div class="empty-state"><h2>Ready to search</h2><p class="no-data">Enter a search term above to get started.</p></div>';
            }
            return;
        }

        if (grid) {
            grid.innerHTML = '<div class="empty-state"><h2>Searching…</h2><p class="no-data">Querying live Ember data.</p></div>';
        }

        try {
            const data = await S.apiGet(endpointFor(currentTab, query));
            setMeta(query, Array.isArray(data) ? data.length : 0);
            render(data);

            const url = new URL(window.location.href);
            url.searchParams.set('q', query);
            url.searchParams.set('tab', currentTab);
            window.history.replaceState(null, '', url.toString());
        } catch (e) {
            if (grid) {
                grid.innerHTML = '<div class="error-state"><h2>Search error</h2><p class="no-data">' + S.escapeHtml(e.message) + '</p></div>';
            }
        }
    }

    function setTab(tab, button) {
        currentTab = tab;
        document.querySelectorAll('.search-tab').forEach(b => b.classList.remove('active'));
        if (button) button.classList.add('active');
        search();
    }

    window.setTab = setTab;
    window.search = search;

    document.addEventListener('DOMContentLoaded', function () {
        const params = new URLSearchParams(window.location.search);
        const requestedTab = params.get('tab');
        const query = params.get('q');

        if (['people', 'rooms', 'inventions'].includes(requestedTab)) {
            currentTab = requestedTab;
            const button = document.querySelector('.search-tab[data-tab="' + requestedTab + '"]');
            document.querySelectorAll('.search-tab').forEach(b => b.classList.remove('active'));
            if (button) button.classList.add('active');
        }

        document.querySelectorAll('.search-tab').forEach(button => {
            button.addEventListener('click', () => setTab(button.dataset.tab, button));
        });

        document.querySelectorAll('[data-quick-search]').forEach(button => {
            button.addEventListener('click', () => {
                const input = document.getElementById('searchInput');
                currentTab = button.dataset.tab || 'people';
                document.querySelectorAll('.search-tab').forEach(b => {
                    b.classList.toggle('active', b.dataset.tab === currentTab);
                });
                if (input) input.value = button.dataset.quickSearch;
                search();
            });
        });

        const searchButton = document.getElementById('searchButton');
        if (searchButton) searchButton.addEventListener('click', search);

        const input = document.getElementById('searchInput');
        if (input) {
            input.addEventListener('keydown', e => { if (e.key === 'Enter') search(); });
            if (query) {
                input.value = query;
                search();
            }
        }
    });
})();
