async function loadRooms() {
    try {
        const resp = await fetch('/api/rooms/v1/all');
        const data = await resp.json();
        const grid = document.getElementById('roomsGrid');
        grid.innerHTML = '';
        
        if (!data || data.length === 0) {
            grid.innerHTML = '<div class="empty-state"><h2>No rooms found</h2><p class="no-data">No rooms are currently available.</p></div>';
            return;
        }
        
        data.forEach(room => {
            const card = document.createElement('div');
            card.className = 'room-card';
            card.innerHTML = `
                <img src="https://via.placeholder.com/400x220/333333/ffffff?text=Room+Image" alt="${room.Name || 'Room'}">
                <div class="room-card-content">
                    <h3>${room.Name || 'Unnamed Room'}</h3>
                    <p>${room.Description || 'No description available'}</p>
                    <div class="room-stats">
                        <span>👥 ${room.Capacity || 20} capacity</span>
                        <span>📍 ${(room.UnitySceneId || 'unknown').substring(0, 8)}</span>
                    </div>
                </div>
            `;
            grid.appendChild(card);
        });
    } catch (e) {
        document.getElementById('roomsGrid').innerHTML = '<div class="empty-state"><h2>Error loading rooms</h2><p class="no-data">Unable to connect to server.</p></div>';
    }
}

loadRooms();
