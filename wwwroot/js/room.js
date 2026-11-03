async function loadRoom() {
    try {
        const roomId = window.location.pathname.split('/').pop();
        const resp = await fetch(`/api/web/rooms/v1/${encodeURIComponent(roomId)}`);
        const data = await resp.json();
        
        document.getElementById('roomTitle').textContent = data.Name || 'Unknown Room';
        document.getElementById('roomOwnerName').textContent = data.OwnerName || 'Unknown';
        document.getElementById('roomOwner').textContent = 'Owner: ' + (data.OwnerName || 'Unknown');
        document.getElementById('roomIdDisplay').textContent = data.RoomId || data.Id || 'unknown';
        document.getElementById('roomCapacity').textContent = data.Capacity ?? 20;
        document.getElementById('roomDescription').innerHTML = `<p>${data.Description || 'No description available'}</p>`;
        
        const tagsContainer = document.getElementById('roomTags');
        tagsContainer.innerHTML = '';
        (data.Tags || []).forEach(tag => {
            const el = document.createElement('span');
            el.className = 'tag';
            el.textContent = tag;
            tagsContainer.appendChild(el);
        });
    } catch (e) {
        document.getElementById('roomTitle').textContent = 'Room Not Found';
        document.getElementById('roomDescription').innerHTML = '<p>Unable to load room details.</p>';
    }
}

loadRoom();
