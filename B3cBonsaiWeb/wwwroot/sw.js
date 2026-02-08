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

    // Handling API/Data with Network-First strategy
    if (url.pathname.includes('/api/') || url.pathname.includes('/GetAll')) {
        event.respondWith(
            fetch(event.request)
                .then(response => {
                    const clonedResponse = response.clone();
                    caches.open(CACHE_NAME).then(cache => cache.put(event.request, clonedResponse));
                    return response;
                })
                .catch(() => caches.match(event.request))
        );
        return;
    }

    // Default Cache-First for assets, Network-First for others
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});

self.addEventListener('push', event => {
    const data = event.data ? event.data.json() : { title: 'B3cBonsai', body: 'Bạn có thông báo mới!' };
    const options = {
        body: data.body,
        icon: '/customer/img/favicon/favicon.png',
        badge: '/customer/img/favicon/favicon.png',
        data: { url: data.url || '/' }
    };
    event.waitUntil(self.registration.showNotification(data.title, options));
});

self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(clients.openWindow(event.notification.data.url));
});
