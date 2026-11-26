(function () {
    'use strict';

    const S = window.EmberSite;
    const pathParts = window.location.pathname.split('/').filter(Boolean);
    const roomId = decodeURIComponent(pathParts.pop() || '');

    function setText(id, value) {
        const el = document.getElementById(id);
        if (el) el.textContent = value;
    }

    async function loadRoom() {
        try {
            const data = await S.apiGet('/api/web/rooms/v1/' + encodeURIComponent(roomId));
            if (!data) throw new Error('Room not found');

            const name = data.name || 'Unknown room';
            const owner = data.ownerName || 'Unknown';
            const ownerUrl = '/Profile/' + encodeURIComponent(owner);

            setText('roomTitle', name);
            setText('roomOwnerName', owner);
            setText('roomOwnerNameSide', owner);
            setText('roomIdDisplay', data.roomId || roomId || '—');
            setText('roomCapacity', data.maxPlayers ?? 20);
            setText('roomBadge', S.initials(name));
            document.title = name + ' - Ember Network';

            const description = document.getElementById('roomDescription');
            if (description) description.textContent = data.description || 'No description has been added yet.';

            const ownerLink = document.getElementById('roomOwnerLink');
            const ownerProfileButton = document.getElementById('ownerProfileButton');
            if (ownerLink) ownerLink.href = ownerUrl;
            if (ownerProfileButton) ownerProfileButton.href = ownerUrl;

            const fallback = document.getElementById('roomFallback');
            const image = document.getElementById('roomImage');
            if (fallback) {
                const hue = S.hueFor(name);
                fallback.style.background = 'linear-gradient(135deg,hsl(' + hue + ' 74% 44%),hsl(' + ((hue + 58) % 360) + ' 74% 26%))';
                const initial = fallback.querySelector('span');
                if (initial) initial.textContent = S.initials(name);
            }
            if (image && data.imageName) {
                image.src = S.imageUrl(data.imageName, 1200, 675);
                image.hidden = false;
                image.onload = function () { if (fallback) fallback.style.display = 'none'; };
                image.onerror = function () {
                    image.hidden = true;
                    if (fallback) fallback.style.display = 'grid';
                };
            } else if (image) {
                image.hidden = true;
            }

            const tags = document.getElementById('roomTags');
            if (tags) {
                const list = Array.isArray(data.tags) ? data.tags : [];
                tags.innerHTML = list.length
                    ? list.map(tag => '<span class="tag">' + S.escapeHtml(tag) + '</span>').join('')
                    : '<span class="no-data">No tags set.</span>';
            }

            const players = document.getElementById('roomPlayers');
            if (players) {
                const list = Array.isArray(data.players) ? data.players : [];
                players.innerHTML = list.length
                    ? list.map(player => {
                        const playerName = player.displayName || player.username || 'Player';
                        return '<div class="room-player"><span class="avatar avatar-sm">' + S.escapeHtml(S.initials(playerName)) + '</span>' + S.escapeHtml(playerName) + '</div>';
                    }).join('')
                    : '<p class="no-data">No players online right now.</p>';
            }
        } catch (e) {
            setText('roomTitle', 'Room not found');
            const description = document.getElementById('roomDescription');
            if (description) description.textContent = 'Unable to load room details: ' + e.message;
        }
    }

    function copyRoomLink() {
        S.copyText(window.location.href, 'Room link copied.');
    }

    function joinRoom() {
        S.toast('Open the Ember client to join this room: ' + roomId);
    }

    window.copyRoomLink = copyRoomLink;
    window.joinRoom = joinRoom;

    document.addEventListener('DOMContentLoaded', function () {
        const copy = document.getElementById('copyRoomLinkButton');
        const urlCopy = document.querySelector('[data-copy-url]');
        const play = document.getElementById('playButton');
        if (copy) copy.addEventListener('click', copyRoomLink);
        if (urlCopy) urlCopy.addEventListener('click', copyRoomLink);
        if (play) play.addEventListener('click', joinRoom);
    });

    loadRoom();
})();
