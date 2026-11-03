let currentTab = 'people';

function setTab(tab, btn) {
    currentTab = tab;
    document.querySelectorAll('.search-tab').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    search();
}

async function search() {
    const query = document.getElementById('searchInput').value;
    const grid = document.getElementById('resultsGrid');
    
    if (!query) {
        grid.innerHTML = '<div class="empty-state"><h2>Search results</h2><p class="no-data">Enter a search term to find people, rooms, events, and inventions.</p></div>';
        return;
    }
    
    try {
        let endpoint = '';
        switch (currentTab) {
            case 'people': endpoint = `/api/players/v1/search?q=${encodeURIComponent(query)}`; break;
            case 'rooms': endpoint = `/api/rooms/v1/search?q=${encodeURIComponent(query)}`; break;
            case 'inventions': endpoint = `/api/inventions/v2/search?q=${encodeURIComponent(query)}`; break;
            default: endpoint = `/api/search/v1?q=${encodeURIComponent(query)}&type=${currentTab}`;
        }
        
        const resp = await fetch(endpoint);
        const data = await resp.json();
        
        grid.innerHTML = '';
        
        if (!data || data.length === 0) {
            grid.innerHTML = '<div class="empty-state"><h2>No results found</h2><p class="no-data">Try a different search term.</p></div>';
            return;
        }
        
        data.forEach(item => {
            const card = document.createElement('div');
            card.className = 'result-card';
            card.innerHTML = `
                <h3>${item.Name || item.Title || item.Username || 'Untitled'}</h3>
                <p>${item.Description || item.Bio || item.Summary || 'No description available'}</p>
            `;
            grid.appendChild(card);
        });
    } catch (e) {
        grid.innerHTML = '<div class="empty-state"><h2>Search error</h2><p class="no-data">Unable to perform search.</p></div>';
    }
}

document.getElementById('searchInput').addEventListener('keypress', (e) => {
    if (e.key === 'Enter') search();
});
