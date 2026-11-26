(function () {
    'use strict';

    const S = window.EmberSite;

    const notes = {
        'Windows PC': 'PC: use the current Ember-patched 2023 client. Confirm EmberPatch.cfg points at the Ember host.',
        'Oculus Quest': 'Quest: ask an admin for the com.ember.dubzydubzicus package. Do not patch global-metadata.dat.',
        'Oculus Rift': 'Rift: run the Windows PC client through your linked headset.',
        'Pico': 'Pico: standalone support depends on the current community package.',
        'iOS': 'iOS: mobile packages are distributed by admins when available.',
        'Android': 'Android: mobile packages are distributed by admins when available.',
        'Nintendo Switch': 'Switch: community support is limited; ask an admin for current status.',
        'Xbox': 'Xbox: community support is limited; ask an admin for current status.',
        'PlayStation': 'PlayStation: community support is limited; ask an admin for current status.'
    };

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('[data-platform]').forEach(function (button) {
            button.addEventListener('click', function () {
                S.toast(notes[button.dataset.platform] || 'Ask an admin for the current client package.');
            });
        });
    });
})();
