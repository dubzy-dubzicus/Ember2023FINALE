const username = document.getElementById('profileUsername').textContent;

async function loadProfileData() {
    try {
        const resp = await fetch(`/api/players/v1/${encodeURIComponent(username)}`);
        if (resp.ok) {
            const data = await resp.json();
            document.getElementById('roomsCount').textContent = data.RoomsCount ?? 0;
            document.getElementById('inventionsCount').textContent = data.InventionsCount ?? 0;
            document.getElementById('friendsCount').textContent = data.FriendsCount ?? 0;
        }
    } catch (e) {
        console.error('Failed to load profile stats', e);
    }
}

function setProfileTab(tab, btn) {
    document.querySelectorAll('.profile-tab').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    loadProfileContent(tab);
}

async function loadProfileContent(tab) {
    const content = document.getElementById('profileContent');
    content.innerHTML = '<div class="empty-state"><h2>Loading...</h2><p class="no-data">Fetching data...</p></div>';
    
    try {
        let endpoint = '';
        switch (tab) {
            case 'photos': endpoint = `/api/photos/v1/user/${encodeURIComponent(username)}`; break;
            case 'rooms': endpoint = `/api/rooms/v1/user/${encodeURIComponent(username)}`; break;
            case 'events': endpoint = `/api/events/v1/user/${encodeURIComponent(username)}`; break;
            default: endpoint = `/api/feed/v1/user/${encodeURIComponent(username)}`;
        }
        
        const resp = await fetch(endpoint);
        const data = await resp.json();
        
        content.innerHTML = '';
        
        if (!data || data.length === 0) {
            content.innerHTML = '<div class="empty-state"><h2>No items</h2><p class="no-data">No items found in this section.</p></div>';
            return;
        }
        
        const grid = document.createElement('div');
        grid.className = 'profile-grid';
        content.appendChild(grid);
        
        data.forEach(item => {
            const card = document.createElement('div');
            card.className = 'result-card';
            card.innerHTML = `
                <h3>${item.Name || item.Title || item.Caption || 'Untitled'}</h3>
                <p>${item.Description || 'No description available'}</p>
            `;
            grid.appendChild(card);
        });
    } catch (e) {
        content.innerHTML = '<div class="empty-state"><h2>Error loading content</h2><p class="no-data">Unable to fetch profile data.</p></div>';
    }
}

loadProfileData();
loadProfileContent('feed');
