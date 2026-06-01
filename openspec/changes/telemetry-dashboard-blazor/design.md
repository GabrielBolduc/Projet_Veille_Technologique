# Design - Dashboard Temps Réel (Blazor + SignalR)

## 1. Objectif
Ajouter une interface utilisateur interactive au projet .NET existant en utilisant Blazor (Server ou WebAssembly) et ASP.NET Core SignalR. Le dashboard doit consommer les données du `Channel` de télémétrie et les pousser instantanément vers le navigateur pour animer des composants visuels (gauges/cadrans) à 10 Hz sans rafraîchissement de page.

## 2. Spécifications Techniques
* **Framework UI :** Blazor Components (.NET 9/10)
* **Communication Temps Réel :** Hub SignalR (`/telemetryHub`)
* **Flux de Données :** Le `TelemetryProcessor` (BackgroundService) va lire le `Channel` et diffuser instantanément chaque payload à tous les clients connectés via le Hub SignalR.

## 3. Composants Visuels (UI)
* Gauge circulaire pour le **RPM** (avec zone rouge).
* Indicateur numérique/gauge pour la **Vitesse** (km/h).
* Barres de progression pour le **Throttle Position** et l'**Engine Load**.
* Indicateur thermique pour le **Coolant Temperature**.

## 4. Liste des Tâches (Tasks)
- [x] Ajouter le support de SignalR dans `Program.cs` et créer le `TelemetryHub`.
- [x] Injecter `IHubContext<TelemetryHub>` dans `TelemetryProcessor.cs` pour pousser les données du Channel vers SignalR.
- [x] Configurer Blazor dans le projet Web (si ce n'est pas déjà fait).
- [x] Créer la page Razor principale (`Dashboard.razor`) avec la connexion au hub SignalR.
- [x] Designer et intégrer les composants de gauges (HTML/CSS ou librairie légère) connectés aux variables d'état.