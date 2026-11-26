(function () {
    'use strict';

    const S = window.EmberSite;

    async function loadNews() {
        const grid = document.getElementById('newsGrid');
        if (!grid) return;

        try {
            const data = await S.apiGet('/api/news/v1/all');
            const articles = Array.isArray(data) ? data : [];

            if (!articles.length) {
                grid.innerHTML = '<div class="empty-state"><h2>No news available</h2><p class="no-data">No announcements have been posted yet.</p></div>';
                return;
            }

            grid.innerHTML = articles.map(article => {
                const title = article.title || 'Untitled update';
                const summary = article.summary || article.content || 'No summary provided.';
                const date = S.formatDate(article.createdAt);

                return '<article class="card">' +
                    '<div class="card-media">' +
                        S.mediaHtml(title, article.imageUrl, 900, 500) +
                        '<span class="card-badge">Update</span>' +
                    '</div>' +
                    '<div class="card-body">' +
                        '<div class="news-card-date"><svg class="icon"><use href="#i-calendar"></use></svg> ' + S.escapeHtml(date) + '</div>' +
                        '<div class="card-title" style="margin-top:10px">' + S.escapeHtml(title) + '</div>' +
                        '<div class="card-desc" style="-webkit-line-clamp:4">' + S.escapeHtml(summary) + '</div>' +
                        '<div class="card-meta">' +
                            '<span class="card-meta-item"><svg class="icon"><use href="#i-sparkles"></use></svg>Ember Network</span>' +
                        '</div>' +
                    '</div>' +
                '</article>';
            }).join('');
        } catch (e) {
            grid.innerHTML = '<div class="error-state"><h2>Unable to load news</h2><p class="no-data">' + S.escapeHtml(e.message) + '</p></div>';
        }
    }

    loadNews();
})();
