# Architecture Backend & Dashboard ASP.NET

Cette section documente l'architecture de déploiement du server.Le backend ASP.NET Core est hébergé sur le Raspberry Pi.

---

## 1. Fonctionnement en Boucle Locale (In-Memory / Localhost)

* Collecte : Le script Python communique avec le OBD2, formate le JSON, et effectue une requête HTTP `POST` en locale.

---

## 2. Le Dashboard Blazor Embarqué

Le dashboard développée en Blazor est directement sur le Raspberry Pi. Pour visualiser la télémétrie, il faut un appareil au même réseau que le Pi.

---

## 3. Stratégie d'Accès au Dashboard

Pour ouvrir le dashboard, 2 options sont possible :

1. En Route : Le Raspberry Pi est configuré comme un point d'accès Wi-Fi. Il suffit de s'y connecter avec un téléphone pour voir le dashboard.
2. Maison : Le Pi se connecte automatiquement au Wi-Fi de la maison. Le dashboard devient accessible sur le réseau local.