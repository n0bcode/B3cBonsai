const CACHE_NAME = 'b3cbonsai-v2';
const ASSETS = [
    '/',
    '/customer/css/style.css',
    '/customer/js/main.js',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/customer/img/logo/logo.png',
    '/customer/img/favicon/favicon.png'
];

// Cache-First for 3D Models
const MODEL_CACHE = 'b3cbonsai-models-v1';

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(ASSETS))
    );
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(keys.map(key => {
                if (key !== CACHE_NAME && key !== MODEL_CACHE) {
                    return caches.delete(key);
                }
            }));
        })
    );
});

self.addEventListener('fetch', event => {
    const url = new URL(event.request.url);

    // Special handling for .glb models
    if (url.pathname.endsWith('.glb')) {
        event.respondWith(
            caches.open(MODEL_CACHE).then(cache => {
                return cache.match(event.request).then(response => {
                    return response || fetch(event.request).then(fetchResponse => {
                        cache.put(event.request, fetchResponse.clone());
                        return fetchResponse;
                    });
                });
            })
        );
        return;
    }

    // Default Cache-First for assets, Network-First for others
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});
