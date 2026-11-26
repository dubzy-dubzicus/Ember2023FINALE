// Ember Network — shared site helpers. All API calls stay same-origin.

(function () {
    'use strict';

    function escapeHtml(value) {
        return String(value ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function initials(value) {
        var parts = String(value ?? '').trim().split(/[\s_\-.]+/).filter(Boolean);
        if (!parts.length) return 'E';
        if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
        return (parts[0][0] + parts[1][0]).toUpperCase();
    }

    function hueFor(value) {
        var text = String(value ?? 'ember');
        var hash = 0;
        for (var i = 0; i < text.length; i++) {
            hash = (hash * 31 + text.charCodeAt(i)) % 360;
        }
        return hash;
    }

    function imageUrl(imageName, width, height, cropSquare) {
        if (!imageName) return '';
        var url = '/imageserver/' + encodeURIComponent(imageName) +
            '?width=' + (width || 640) + '&height=' + (height || 360);
        if (cropSquare) url += '&cropsquare=true';
        return url;
    }

    function mediaHtml(label, imageName, width, height) {
        var safeLabel = escapeHtml(label || 'Ember Network');
        var hue = hueFor(label);
        var fallbackStyle = 'display:' + (imageName ? 'none' : 'grid') +
            ';background:linear-gradient(135deg,hsl(' + hue + ' 74% 44%),hsl(' + ((hue + 58) % 360) + ' 74% 26%));';

        var fallback = '<div class="media-fallback" style="' + fallbackStyle + '"><span>' +
            escapeHtml(initials(label)) + '</span></div>';

        if (!imageName) return fallback;

        return '<img src="' + imageUrl(imageName, width, height) + '" alt="' + safeLabel +
            '" loading="lazy" onerror="this.style.display=\'none\';this.nextElementSibling.style.display=\'grid\';">' +
            fallback;
    }

    async function apiGet(path) {
        var url = String(path).startsWith('/') ? path : '/api/' + path;
        var resp = await fetch(url, { headers: { Accept: 'application/json' } });
        var raw = await resp.text();
        var data = null;
        try { data = raw ? JSON.parse(raw) : null; } catch (_) { data = null; }

        if (!resp.ok) {
            var message = data && (data.error || data.message);
            throw new Error(message || ('HTTP ' + resp.status));
        }
        return data;
    }

    function formatDate(value) {
        var date = new Date(value);
        if (isNaN(date.getTime())) return '';
        return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });
    }

    function toast(message) {
        var stack = document.getElementById('toastStack');
        if (!stack) return;
        var el = document.createElement('div');
        el.className = 'toast';
        el.textContent = message;
        stack.appendChild(el);
        window.setTimeout(function () { el.remove(); }, 4200);
    }

    async function copyText(text, successMessage) {
        try {
            if (navigator.clipboard && window.isSecureContext) {
                await navigator.clipboard.writeText(text);
            } else {
                var input = document.createElement('textarea');
                input.value = text;
                input.style.position = 'fixed';
                input.style.opacity = '0';
                document.body.appendChild(input);
                input.select();
                document.execCommand('copy');
                input.remove();
            }
            toast(successMessage || 'Copied to clipboard.');
        } catch (e) {
            toast('Could not copy automatically: ' + text);
        }
    }

    window.EmberAPI = { baseUrl: '/api' };

    window.EmberSite = {
        escapeHtml: escapeHtml,
        initials: initials,
        hueFor: hueFor,
        imageUrl: imageUrl,
        mediaHtml: mediaHtml,
        apiGet: apiGet,
        formatDate: formatDate,
        toast: toast,
        copyText: copyText
    };

    document.addEventListener('DOMContentLoaded', function () {
        var toggle = document.querySelector('.menu-toggle');
        var nav = document.querySelector('.main-nav');

        if (toggle && nav) {
            toggle.addEventListener('click', function () {
                var open = nav.classList.toggle('open');
                toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
            });

            nav.querySelectorAll('a').forEach(function (link) {
                link.addEventListener('click', function () {
                    nav.classList.remove('open');
                    toggle.setAttribute('aria-expanded', 'false');
                });
            });
        }

        document.querySelectorAll('[data-copy]').forEach(function (button) {
            button.addEventListener('click', function () {
                copyText(button.dataset.copy, button.dataset.copyMessage || 'Copied to clipboard.');
            });
        });
    });
})();
