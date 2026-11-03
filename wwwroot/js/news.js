async function loadNews() {
    try {
        const resp = await fetch('/api/news/v1/all');
        const data = await resp.json();
        const grid = document.getElementById('newsGrid');
        grid.innerHTML = '';
        
        if (!data || data.length === 0) {
            grid.innerHTML = '<div class="empty-state"><h2>No news available</h2><p class="no-data">No news articles found.</p></div>';
            return;
        }
        
        data.forEach(article => {
            const card = document.createElement('div');
            card.className = 'news-card';
            card.innerHTML = `
                <img src="${article.ImageUrl || 'https://via.placeholder.com/400x200/333333/ffffff?text=News'}" alt="${article.Title}">
                <div class="news-card-content">
                    <h3>${article.Title}</h3>
                    <p>${article.Summary || article.Content?.substring(0, 150) || ''}...</p>
                    <span class="news-card-date">${new Date(article.CreatedAt || Date.now()).toLocaleDateString()}</span>
                </div>
            `;
            grid.appendChild(card);
        });
    } catch (e) {
        document.getElementById('newsGrid').innerHTML = '<div class="empty-state"><h2>Error loading news</h2><p class="no-data">Unable to connect to server.</p></div>';
    }
}

loadNews();
